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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILoggerService logger)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            // Verificar si ya existe una categoría con el mismo nombre
            var existingCategory = await _categoryRepository.GetByNameAsync(categoryDto.Name);
            if (existingCategory != null)
                throw new ArgumentException($"Ya existe una categoría con el nombre '{categoryDto.Name}'");

            var category = new Category(categoryDto.Name);

            var newCategory = await _categoryRepository.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Categoría creada: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);

            return _mapper.Map<CategoryDto>(newCategory);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (category == null) return false;

            // Verificar si ya existe otra categoría con el mismo nombre
            var existingCategory = await _categoryRepository.GetByNameAsync(categoryDto.Name);
            if (existingCategory != null && existingCategory.Id != categoryDto.Id)
                throw new ArgumentException($"Ya existe una categoría con el nombre '{categoryDto.Name}'");

            category.Update(categoryDto.Name);

            await _categoryRepository.UpdateAsync(category);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Categoría actualizada: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            // Verificar si la categoría tiene libros asociados
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoryWithBooks = categories.FirstOrDefault(c => c.Id == id && c.Books.Any());

            if (categoryWithBooks != null)
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene libros asociados");

            await _categoryRepository.DeleteAsync(category);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Categoría eliminada: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);

            return true;
        }
    }
}
