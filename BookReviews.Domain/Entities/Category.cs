using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Domain.Entities
{
    /// <summary>
    /// Entidad Category que representa una categoría de libros
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Identificador único de la categoría
        /// </summary>
        public int Id { get; set; } // Cambiado a public para EF Core

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        public string Name { get; set; } // Cambiado a public para EF Core

        /// <summary>
        /// Colección de libros asociados a esta categoría
        /// </summary>
        public ICollection<Book> Books { get; private set; } = new List<Book>();

        /// <summary>
        /// Constructor sin parámetros requerido por EF Core
        /// </summary>
        public Category() { } // Cambiado a protected para EF Core

        /// <summary>
        /// Constructor principal para crear una nueva categoría
        /// </summary>
        /// <param name="name">Nombre de la categoría</param>
        public Category(string name)
        {
            ValidateName(name);
            Name = name;
        }

        /// <summary>
        /// Actualiza el nombre de la categoría
        /// </summary>
        /// <param name="name">Nuevo nombre</param>
        public void Update(string name)
        {
            ValidateName(name);
            Name = name;
        }

        /// <summary>
        /// Valida que el nombre cumpla con las reglas de negocio
        /// </summary>
        /// <param name="name">Nombre a validar</param>
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre de la categoría no puede estar vacío", nameof(name));
        }
    }
}