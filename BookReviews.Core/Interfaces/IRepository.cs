
using System.Linq.Expressions;


namespace BookReviews.Core.Interfaces
{
    // Interfaz genérica donde T debe ser una clase (referencia, no tipo de valor)
    public interface IRepository<T> where T : class
    {
        // Obtiene una entidad por su ID de forma asíncrona
        Task<T> GetByIdAsync(int id);

        // Obtiene todas las entidades como una lista de solo lectura de forma asíncrona
        Task<IReadOnlyList<T>> ListAllAsync();

        // Obtiene entidades que cumplen con una condición específica (predicado)
        // Expression<Func<T, bool>> permite pasar una expresión lambda que se traducirá a SQL
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate);

        // Agrega una nueva entidad y devuelve la entidad persistida (ID generado)
        Task<T> AddAsync(T entity);

        // Actualiza una entidad existente
        Task UpdateAsync(T entity);

        // Elimina una entidad
        Task DeleteAsync(T entity);
    }
}
