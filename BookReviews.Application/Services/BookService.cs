using AutoMapper;
using BookReviews.Application.DTOs;
using BookReviews.Application.Interfaces;
using BookReviews.Core.Interfaces;
using BookReviews.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetBooksAsync(string searchTerm = null, int? categoryId = null)
        {
            var books = await _bookRepository.SearchBooksAsync(searchTerm, categoryId);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetBookWithReviewsAsync(id);
            if (book == null) return null;

            var bookDto = _mapper.Map<BookDto>(book);
            bookDto.AverageRating = book.GetAverageRating();
            bookDto.ReviewCount = book.Reviews.Count;

            return bookDto;
        }

        public async Task<BookDto> CreateBookAsync(BookDto bookDto)
        {
            // Validar que la categoría existe
            if (!await CategoryExistsAsync(bookDto.CategoryId))
                throw new ArgumentException("La categoría especificada no existe");

            var book = new Book(
                bookDto.Title,
                bookDto.Author,
                bookDto.Summary,
                bookDto.CategoryId);

            var newBook = await _bookRepository.AddAsync(book);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookDto>(newBook);
        }

        public async Task<bool> UpdateBookAsync(BookDto bookDto)
        {
            var book = await _bookRepository.GetByIdAsync(bookDto.Id);
            if (book == null) return false;

            // Validar que la categoría existe
            if (!await CategoryExistsAsync(bookDto.CategoryId))
                throw new ArgumentException("La categoría especificada no existe");

            book.UpdateDetails(
                bookDto.Title,
                bookDto.Author,
                bookDto.Summary,
                bookDto.CategoryId);

            await _bookRepository.UpdateAsync(book);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return false;

            await _bookRepository.DeleteAsync(book);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _categoryRepository.GetByIdAsync(categoryId) != null;
        }
    }
}
