using BookReviews.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookReviews.Infrastructure.Data
{
    /// <summary>
    /// Contexto de base de datos para la aplicación
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor del contexto de base de datos
        /// </summary>
        /// <param name="options">Opciones de configuración</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// DbSet para la entidad Book
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// DbSet para la entidad Review
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        /// <summary>
        /// DbSet para la entidad User
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// DbSet para la entidad Category
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Configura el modelo de base de datos
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas las configuraciones del ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Aplicar datos semilla
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Configura los datos semilla para la base de datos
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo</param>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Configuración de esquema para PostgreSQL
            modelBuilder.Entity<Category>().ToTable("categories", "public");
            modelBuilder.Entity<Book>().ToTable("books", "public");
            modelBuilder.Entity<Review>().ToTable("reviews", "public");
            modelBuilder.Entity<User>().ToTable("users", "public");

            // El uso de HasData requiere que los setters sean públicos o protected en la entidad
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ficción" },
                new Category { Id = 2, Name = "No Ficción" },
                new Category { Id = 3, Name = "Ciencia Ficción" },
                new Category { Id = 4, Name = "Fantasía" },
                new Category { Id = 5, Name = "Misterio" },
                new Category { Id = 6, Name = "Historia" },
                new Category { Id = 7, Name = "Biografía" },
                new Category { Id = 8, Name = "Autoayuda" },
                new Category { Id = 9, Name = "Programación" },
                new Category { Id = 10, Name = "Ciencia" }
            );

            // Alternativa: Usar Sql directo para la siembra de datos si prefieres mantener setters privados
            /*
            // Este método permite mantener los setters privados en las entidades
            modelBuilder.Entity<Category>().ToTable("categories", "public");
            */
        }
    }
}