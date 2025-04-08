using BookReviews.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace BookReviews.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas las configuraciones del ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            SeedData(modelBuilder);

        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            // En lugar de usar HasData, puedes usar SQL directo
            modelBuilder.Entity<Category>().ToTable("categories", "public");

            // Después de definir la migración, puedes agregar esto
            // en el método Up() de la migración manualmente
            /*
            migrationBuilder.Sql(@"
                INSERT INTO public.categories (name) VALUES 
                ('Ficción'), ('No Ficción'), ('Ciencia Ficción'), ('Fantasía'), 
                ('Misterio'), ('Historia'), ('Biografía'), ('Autoayuda'), 
                ('Programación'), ('Ciencia');
            ");
            */
        }
    }
}
