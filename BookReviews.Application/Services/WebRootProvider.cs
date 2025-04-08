using BookReviews.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace BookReviews.Application.Services
{
    /// <summary>
    /// Implementación alternativa del proveedor de WebRoot para aplicaciones web
    /// </summary>
    public class WebRootProvider : IWebRootProvider
    {
        private readonly IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Constructor para WebRootProvider
        /// </summary>
        /// <param name="hostEnvironment">Entorno de la aplicación</param>
        public WebRootProvider(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        /// <inheritdoc />
        public string GetWebRootPath()
        {
            // En lugar de acceder directamente a WebRootPath (que solo existe en IWebHostEnvironment),
            // determinamos la ruta wwwroot a partir de ContentRootPath
            return Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot");
        }
    }
}