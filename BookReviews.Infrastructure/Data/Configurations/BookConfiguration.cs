using BookReviews.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace BookReviews.Infrastructure.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Define que la entidad Book se mapea a la tabla "books" en el esquema "public" de la base de datos
            builder.ToTable("books", "public");

            // Establece la propiedad Id como clave primaria de la entidad
            builder.HasKey(b => b.Id);

            // Configura Id como una columna de identidad (auto-incrementable)
            builder.Property(b => b.Id).UseIdentityColumn();

            // Configura Title como un campo requerido con longitud máxima de 200 caracteres
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Configura Author como un campo requerido con longitud máxima de 150 caracteres
            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(150);

            // Configura Summary como un campo opcional con longitud máxima de 2000 caracteres
            builder.Property(b => b.Summary)
                .HasMaxLength(2000);

            // Establece una relación uno-a-muchos entre Book y Category:
            // - Un libro pertenece a una categoría (HasOne)
            // - Una categoría puede tener muchos libros (WithMany)
            // - La relación se establece mediante la clave foránea CategoryId
            // - Al eliminar una categoría, se restringe la eliminación si existen libros asociados
            builder.HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Crea índices para mejorar el rendimiento en consultas por título
            builder.HasIndex(b => b.Title);

            // Crea índices para mejorar el rendimiento en consultas por autor
            builder.HasIndex(b => b.Author);
        }
    }
}
