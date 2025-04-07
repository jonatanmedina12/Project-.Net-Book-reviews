namespace BookReviews.Core.Interfaces
{
    // Interfaz que hereda de IDisposable para garantizar la liberación adecuada de recursos
    public interface IUnitOfWork : IDisposable
    {
        // Método para confirmar todas las operaciones pendientes en una transacción
        // Retorna el número de entidades afectadas y acepta un token de cancelación opcional
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}