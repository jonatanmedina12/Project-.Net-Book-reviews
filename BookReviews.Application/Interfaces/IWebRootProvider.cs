using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Interfaces
{
    /// <summary>
    /// Interfaz para obtener la ruta WebRoot
    /// </summary>
    public interface IWebRootProvider
    {
        /// <summary>
        /// Obtiene la ruta física del directorio WebRoot
        /// </summary>
        /// <returns>Ruta del directorio WebRoot</returns>
        string GetWebRootPath();
    }
}
