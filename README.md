# Book Review App - Aplicación de Reseñas de Libros

## Descripción General

Este proyecto implementa una aplicación de reseñas de libros que permite a los usuarios navegar, buscar y reseñar libros. La aplicación está construida con una arquitectura moderna utilizando .NET 9.0 y PostgreSQL como base de datos.

## Características Principales

- Gestión de libros (búsqueda, filtrado por categoría, detalles)
- Sistema de reseñas (calificación de 1-5 estrellas y comentarios)
- Autenticación y autorización de usuarios
- Gestión de perfil de usuario
- Interfaz responsive y accesible

## Tecnologías Utilizadas

- **.NET 9.0**: Framework principal
- **Entity Framework Core**: ORM para acceso a datos
- **PostgreSQL**: Base de datos relacional
- **JWT**: Autenticación basada en tokens
- **Swagger**: Documentación de API
- **Serilog**: Logging
- **AutoMapper**: Mapeo de objetos
- **BCrypt**: Hashing de contraseñas
- **xUnit**: Framework de pruebas

## Arquitectura

El proyecto sigue los principios de Clean Architecture con una clara separación de responsabilidades:

```
BookReviewApp/
├── src/
│   ├── BookReviewApp.API/              # Capa de presentación (API)
│   ├── BookReviewApp.Application/      # Lógica de aplicación
│   ├── BookReviewApp.Core/             # Interfaces y utilidades
│   ├── BookReviewApp.Domain/           # Entidades y reglas de negocio
│   ├── BookReviewApp.Infrastructure/   # Implementaciones de acceso a datos
│   └── BookReviewApp.Tests/            # Pruebas unitarias e integración
└── BookReviewApp.sln                   # Archivo de solución
```

## Principios SOLID aplicados

- **Principio de Responsabilidad Única (SRP)**: Cada clase tiene una única responsabilidad
- **Principio Abierto/Cerrado (OCP)**: Entidades extensibles sin modificación
- **Principio de Sustitución de Liskov (LSP)**: Implementaciones sustituibles por sus interfaces
- **Principio de Segregación de Interfaces (ISP)**: Interfaces específicas y cohesivas
- **Principio de Inversión de Dependencias (DIP)**: Dependencia en abstracciones, no implementaciones

## Requisitos Previos

- .NET 9.0 SDK
- PostgreSQL 14+
- Visual Studio 2022+ o VS Code

## Configuración y Ejecución

1. Clonar el repositorio:
```bash
git clone https://github.com/jonatanmedina12/Project-.Net-Book-reviews.git
cd BookReviewApp
```

2. Configurar la base de datos:
   - Actualizar la cadena de conexión en `appsettings.json`

3. Aplicar migraciones:
```bash
dotnet ef database update 
```

4. Ejecutar la aplicación:
```bash
cd BookReviewApp.API
dotnet run
```

5. Acceder a la API:
   - API: https://localhost:5001
   - Documentación Swagger: https://localhost:5001/index.html

## Endpoints Principales

### Libros
- `GET /api/Book` - Obtener todos los libros (con filtros)
- `GET /api/Book/{id}` - Obtener libro por ID
- `POST /api/Book` - Crear libro (requiere rol Admin)
- `PUT /api/Book/{id}` - Actualizar libro (requiere rol Admin)
- `DELETE /api/Book/{id}` - Eliminar libro (requiere rol Admin)

### Reseñas
- `GET /api/Review/book/{bookId}` - Obtener reseñas de un libro
- `GET /api/Review/user` - Obtener reseñas del usuario actual
- `POST /api/Review` - Crear reseña (requiere autenticación)
- `PUT /api/Review/{id}` - Actualizar reseña (solo propietario)
- `DELETE /api/Review/{id}` - Eliminar reseña (solo propietario)

### Autenticación
- `POST /api/Auth/register` - Registrar nuevo usuario
- `POST /api/Auth/login` - Iniciar sesión
- `POST /api/Auth/forgot-password` - Solicitar restablecimiento de contraseña
- `POST /api/Auth/reset-password` - Restablecer contraseña

## Estructura de la Base de Datos

La aplicación utiliza PostgreSQL con las siguientes tablas principales:

- `books` - Información de libros
- `reviews` - Reseñas de libros
- `users` - Información de usuarios
- `categories` - Categorías de libros

## Consideraciones de Seguridad

- Autenticación mediante JWT
- Contraseñas hasheadas con BCrypt
- Validación de entradas en todos los endpoints
- Protección contra inyección SQL mediante Entity Framework
- Manejo seguro de excepciones

## JONATAN ALBENIO MEDINA SANDOVAL ###
## JONATANALBENIOMEDINA@OUTLOOK.COM ##