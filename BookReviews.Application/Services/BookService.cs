using AutoMapper;
using BookReviews.Application.DTOs;
using BookReviews.Application.Helpers;
using BookReviews.Application.Interfaces;
using BookReviews.Core.Interfaces;
using BookReviews.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;


namespace BookReviews.Application.Services
{
    /// <summary>
    /// Servicio para manejar operaciones relacionadas con libros
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IWebRootProvider _webRootProvider; // A
        /// <summary>
        /// Constructor del servicio de libros
        /// </summary>
    
        public BookService(
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHostEnvironment hostEnvironment,
            IWebRootProvider webRootProvider) // Agregamos el parámetro
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _webRootProvider = webRootProvider ?? throw new ArgumentNullException(nameof(webRootProvider));
        }

        /// <summary>
        /// Obtiene la lista de libros con filtros opcionales
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda opcional</param>
        /// <param name="categoryId">ID de categoría opcional</param>
        /// <returns>Colección de DTOs de libros</returns>
        public async Task<IEnumerable<BookDto>> GetBooksAsync(string searchTerm = null, int? categoryId = null)
        {
            var books = await _bookRepository.SearchBooksAsync(searchTerm, categoryId);
            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);

            // Añadir nombre de categoría y URL de imagen a cada DTO
            foreach (var bookDto in bookDtos)
            {
                var category = await _categoryRepository.GetByIdAsync(bookDto.CategoryId);
                bookDto.CategoryName = category?.Name;

                // Asegurar que la URL de la imagen se establece correctamente
                if (!string.IsNullOrEmpty(bookDto.CoverImageUrl) && !bookDto.CoverImageUrl.StartsWith("http"))
                {
                    bookDto.CoverImageUrl = bookDto.CoverImageUrl;
                }
            }

            return bookDtos;
        }
        /// <summary>
        /// Obtiene un libro por su ID
        /// </summary>
        /// <param name="id">ID del libro</param>
        /// <returns>DTO del libro con detalles adicionales</returns>
        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetBookWithReviewsAsync(id);
            if (book == null) return null;

            var bookDto = _mapper.Map<BookDto>(book);
            bookDto.AverageRating = book.GetAverageRating();
            bookDto.ReviewCount = book.Reviews.Count;

            // Obtener nombre de categoría
            var category = await _categoryRepository.GetByIdAsync(book.CategoryId);
            bookDto.CategoryName = category?.Name;

            // Asegurar que el campo CoverImageUrl se establece correctamente desde CoverImagePath
            if (!string.IsNullOrEmpty(book.CoverImagePath))
            {
                bookDto.CoverImageUrl = book.CoverImagePath;
            }

            return bookDto;
        }

        /// <summary>
        /// Crea un nuevo libro
        /// </summary>
        /// <param name="bookDto">DTO con detalles del libro</param>
        /// <returns>DTO del libro creado</returns>
        public async Task<BookDto> CreateBookAsync(BookDto bookDto)
        {
            // Validar que la categoría existe
            if (!await CategoryExistsAsync(bookDto.CategoryId))
                throw new ArgumentException("La categoría especificada no existe");

            // Manejar la subida de imagen
            string coverImagePath = null;

            // Verificar si hay una imagen base64
            if (!string.IsNullOrEmpty(bookDto.CoverImageUrl) &&
                bookDto.CoverImageUrl.StartsWith("data:image"))
            {
                // Convertir base64 a IFormFile
                var coverImage = ImageHelper.Base64ToIFormFile(
                    bookDto.CoverImageUrl,
                    $"{Guid.NewGuid()}_cover.png"
                );

                coverImagePath = await SaveCoverImageAsync(coverImage);
            }
            // Si es un archivo de imagen tradicional
            else if (bookDto.CoverImage != null)
            {
                coverImagePath = await SaveCoverImageAsync(bookDto.CoverImage);
            }

            var book = new Book(
                bookDto.Title,
                bookDto.Author,
                bookDto.Summary,
                bookDto.Isbn,
                bookDto.CategoryId,
                null, // Reviews
                bookDto.Language,
                bookDto.PublishedYear,
                bookDto.Publisher,
                bookDto.Pages);

            // Si hay una imagen, actualizarla
            if (!string.IsNullOrEmpty(coverImagePath))
            {
                book.UpdateCoverImage(coverImagePath);
            }

            var newBook = await _bookRepository.AddAsync(book);
            await _unitOfWork.CompleteAsync();

            var result = _mapper.Map<BookDto>(newBook);
            result.CoverImageUrl = coverImagePath;
            return result;
        }

        /// <summary>
        /// Actualiza los detalles de un libro existente
        /// </summary>
        /// <param name="bookDto">DTO con los nuevos detalles del libro</param>
        /// <returns>Indica si la actualización fue exitosa</returns>
        public async Task<bool> UpdateBookAsync(BookDto bookDto)
        {
            var book = await _bookRepository.GetByIdAsync(bookDto.Id);
            if (book == null) return false;

            // Validar que la categoría existe
            if (!await CategoryExistsAsync(bookDto.CategoryId))
                throw new ArgumentException("La categoría especificada no existe");

            // Manejar la subida de imagen
            string coverImagePath = book.CoverImagePath;

            // Verificar si hay una imagen base64
            if (!string.IsNullOrEmpty(bookDto.CoverImageUrl) &&
                bookDto.CoverImageUrl.StartsWith("data:image"))
            {
                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(coverImagePath))
                {
                    DeleteCoverImage(coverImagePath);
                }

                // Convertir base64 a IFormFile
                var coverImage = ImageHelper.Base64ToIFormFile(
                    bookDto.CoverImageUrl,
                    $"{Guid.NewGuid()}_cover.png"
                );

                coverImagePath = await SaveCoverImageAsync(coverImage);
            }
            // Si es un archivo de imagen tradicional
            else if (bookDto.CoverImage != null)
            {
                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(coverImagePath))
                {
                    DeleteCoverImage(coverImagePath);
                }

                // Guardar nueva imagen
                coverImagePath = await SaveCoverImageAsync(bookDto.CoverImage);
            }

            // Actualizar detalles del libro utilizando el método UpdateDetails
            book.UpdateDetails(
                bookDto.Title,
                bookDto.Author,
                bookDto.Summary,
                bookDto.CategoryId,
                bookDto.Isbn,
                bookDto.Language,
                bookDto.Pages,
                bookDto.PublishedYear,
                bookDto.Publisher,
                coverImagePath);

            await _bookRepository.UpdateAsync(book);
            await _unitOfWork.CompleteAsync();

            return true;
        }


        /// <summary>
        /// Elimina un libro por su ID
        /// </summary>
        /// <param name="id">ID del libro a eliminar</param>
        /// <returns>Indica si la eliminación fue exitosa</returns>
        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return false;

            // Eliminar imagen de portada si existe
            if (!string.IsNullOrEmpty(book.CoverImagePath))
            {
                DeleteCoverImage(book.CoverImagePath);
            }

            await _bookRepository.DeleteAsync(book);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        /// <summary>
        /// Guarda la imagen de portada de un libro
        /// </summary>
        /// <param name="coverImage">Archivo de imagen</param>
        /// <returns>Ruta de la imagen guardada</returns>
        private async Task<string> SaveCoverImageAsync(IFormFile coverImage)
        {
            // Validar que la imagen no sea nula
            if (coverImage == null || coverImage.Length == 0)
            {
                throw new ArgumentException("La imagen de portada no es válida.");
            }

            // Obtenemos la ruta a través del proveedor
            string webRootPath = _webRootProvider.GetWebRootPath();

            // CAMBIO IMPORTANTE: Guardar directamente en wwwroot sin subdirectorio uploads
            string uploadFolder = Path.Combine(
                webRootPath,
                "book-covers"
            );

            // Asegurar que el directorio existe
            Directory.CreateDirectory(uploadFolder);

            // Generar nombre de archivo único
            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(coverImage.FileName)}";
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Guardar imagen
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await coverImage.CopyToAsync(fileStream);
            }

            // CAMBIO IMPORTANTE: Devolver ruta relativa sin el prefijo "uploads/"
            return $"book-covers/{uniqueFileName}";
        }
        /// <summary>
        /// Método auxiliar para obtener la ruta raíz web
        /// </summary>
        /// <returns>Ruta raíz web</returns>
        private string GetWebRootPath()
        {
            // Esta implementación dependerá de tu configuración específica
            // Podrías necesitar inyectar un servicio que proporcione esta ruta
            throw new NotImplementedException("Necesita implementar la obtención de la ruta raíz web");
        }

        /// <summary>
        /// Elimina la imagen de portada de un libro
        /// </summary>
        /// <param name="imagePath">Ruta de la imagen</param>
        private void DeleteCoverImage(string imagePath)
        {
            // Validar que la ruta no esté vacía
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            try
            {
                // Ruta física completa de la imagen
                string fullPath = Path.Combine(
                    _webRootProvider.GetWebRootPath(),
                    imagePath.TrimStart('/', '\\')
                );

                // Eliminar archivo si existe
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                // Registrar el error (se recomienda usar un sistema de logging)
                // Puedes inyectar un ILogger si lo necesitas
                Console.WriteLine($"Error al eliminar la imagen: {ex.Message}");
                // O usar tu sistema de logging preferido
                // _logger.LogError($"Error al eliminar la imagen: {ex.Message}");
            }
        }


        /// <summary>
        /// Verifica si una categoría existe
        /// </summary>
        /// <param name="categoryId">ID de la categoría</param>
        /// <returns>Indica si la categoría existe</returns>
        private async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _categoryRepository.GetByIdAsync(categoryId) != null;
        }
    }
}