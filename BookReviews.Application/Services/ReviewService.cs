using AutoMapper;
using BookReviews.Application.DTOs;
using BookReviews.Core.Interfaces;
using BookReviews.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetBookReviewsAsync(int bookId)
        {
            var reviews = await _reviewRepository.GetReviewsByBookAsync(bookId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId)
        {
            var reviews = await _reviewRepository.GetReviewsByUserAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> CreateReviewAsync(ReviewDto reviewDto, int userId)
        {
            // Validar que el libro existe
            if (await _bookRepository.GetByIdAsync(reviewDto.BookId) == null)
                throw new ArgumentException("El libro especificado no existe");

            // Validar que el usuario existe
            if (await _userRepository.GetByIdAsync(userId) == null)
                throw new ArgumentException("El usuario especificado no existe");

            // Validar que el usuario no haya dejado ya una reseña para este libro
            if (await _reviewRepository.HasUserReviewedBookAsync(userId, reviewDto.BookId))
                throw new ArgumentException("Ya has dejado una reseña para este libro");

            var review = new Review(
                reviewDto.BookId,
                userId,
                reviewDto.Rating,
                reviewDto.Comment);

            var newReview = await _reviewRepository.AddAsync(review);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ReviewDto>(newReview);
        }

        public async Task<bool> UpdateReviewAsync(ReviewDto reviewDto, int userId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewDto.Id);
            if (review == null) return false;

            // Verificar que el usuario es el dueño de la reseña
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("No tienes permiso para editar esta reseña");

            review.Update(
                reviewDto.Rating,
                reviewDto.Comment);

            await _reviewRepository.UpdateAsync(review);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id, int userId)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null) return false;

            // Verificar que el usuario es el dueño de la reseña
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("No tienes permiso para eliminar esta reseña");

            await _reviewRepository.DeleteAsync(review);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
