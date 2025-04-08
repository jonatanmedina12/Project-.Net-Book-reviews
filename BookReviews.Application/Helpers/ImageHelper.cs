using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BookReviews.Application.Helpers
{
    /// <summary>
    /// Ayudante para manejar conversiones de imágenes
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Convierte una cadena base64 en un IFormFile
        /// </summary>
        /// <param name="base64String">Cadena base64 de la imagen</param>
        /// <param name="fileName">Nombre de archivo para la imagen</param>
        /// <returns>IFormFile representando la imagen</returns>
        public static IFormFile Base64ToIFormFile(string base64String, string fileName = "image.png")
        {
            // Validar entrada
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentNullException(nameof(base64String));

            // Eliminar el prefijo de datos base64 si existe
            if (base64String.Contains(","))
            {
                base64String = base64String.Split(',')[1];
            }

            // Decodificar base64
            byte[] fileBytes = Convert.FromBase64String(base64String);

            // Usar un MemoryStream que persista
            var stream = new MemoryStream(fileBytes);

            // Implementación simple compatible con IFormFile
            return new FormFileImplementation(stream, 0, fileBytes.Length, "file", fileName)
            {
                ContentType = GetContentType(fileName)
            };
        }

        /// <summary>
        /// Determina el tipo de contenido basado en la extensión del archivo
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <returns>Tipo de contenido MIME</returns>
        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }

    /// <summary>
    /// Implementación personalizada de IFormFile para compatibilidad con .NET 9
    /// </summary>
    public class FormFileImplementation : IFormFile
    {
        private readonly Stream _baseStream;
        private readonly long _baseStreamOffset;

        /// <summary>
        /// Constructor para FormFileImplementation
        /// </summary>
        public FormFileImplementation(Stream baseStream, long baseStreamOffset, long length, string name, string fileName)
        {
            _baseStream = baseStream;
            _baseStreamOffset = baseStreamOffset;
            Length = length;
            Name = name;
            FileName = fileName;
            Headers = new HeaderDictionary();
        }

        /// <inheritdoc />
        public string ContentType { get; set; }

        /// <inheritdoc />
        public string ContentDisposition => $"form-data; name=\"{Name}\"; filename=\"{FileName}\"";

        /// <inheritdoc />
        public IHeaderDictionary Headers { get; }

        /// <inheritdoc />
        public long Length { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string FileName { get; }

        /// <inheritdoc />
        public void CopyTo(Stream target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            using var readStream = OpenReadStream();
            readStream.CopyTo(target);
        }

        /// <inheritdoc />
        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            using var readStream = OpenReadStream();
            await readStream.CopyToAsync(target, cancellationToken);
        }

        /// <inheritdoc />
        public Stream OpenReadStream()
        {
            _baseStream.Position = _baseStreamOffset;
            return new StreamLimiter(_baseStream, Length);
        }

        // Clase auxiliar para limitar el stream a una longitud específica
        private class StreamLimiter : Stream
        {
            private readonly Stream _innerStream;
            private readonly long _length;
            private long _position;

            public StreamLimiter(Stream innerStream, long length)
            {
                _innerStream = innerStream;
                _length = length;
                _position = 0;
            }

            public override bool CanRead => _innerStream.CanRead;
            public override bool CanSeek => _innerStream.CanSeek;
            public override bool CanWrite => false;
            public override long Length => _length;
            public override long Position
            {
                get => _position;
                set
                {
                    if (value < 0 || value > _length)
                        throw new ArgumentOutOfRangeException(nameof(value));

                    _position = value;
                    _innerStream.Seek(_position, SeekOrigin.Begin);
                }
            }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_position >= _length)
                    return 0;

                int bytesToRead = (int)Math.Min(count, _length - _position);
                int bytesRead = _innerStream.Read(buffer, offset, bytesToRead);
                _position += bytesRead;
                return bytesRead;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                long newPosition;

                switch (origin)
                {
                    case SeekOrigin.Begin:
                        newPosition = offset;
                        break;
                    case SeekOrigin.Current:
                        newPosition = _position + offset;
                        break;
                    case SeekOrigin.End:
                        newPosition = _length + offset;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(origin));
                }

                if (newPosition < 0 || newPosition > _length)
                    throw new IOException("Cannot seek outside of stream bounds");

                _position = newPosition;
                _innerStream.Seek(_position, SeekOrigin.Begin);
                return _position;
            }

            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // No cerramos el stream interno para que pueda ser reutilizado
                }
                base.Dispose(disposing);
            }
        }
    }
}