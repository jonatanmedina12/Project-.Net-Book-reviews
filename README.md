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


# Guía de configuración de BookReviews API

## Estructura de configuración mejorada

La configuración del proyecto ahora está organizada de la siguiente manera:

1. **Archivos appsettings.json**: Contienen la configuración base por defecto
2. **Variables de entorno del sistema**: Tienen prioridad sobre los archivos appsettings
3. **Archivo .env**: Se carga en las variables de entorno, facilitando el desarrollo local

## Orden de prioridad de configuración

La aplicación busca los valores de configuración en el siguiente orden:

1. Variables de entorno directamente presentes en el sistema
2. Variables definidas en el archivo `.env` (si existe)
3. Valores en `appsettings.{Environment}.json` (según el entorno activo)
4. Valores en `appsettings.json`

## Configuración para desarrollo local

### Creación del archivo .env

1. Copia el archivo `.env.example` a `.env` en la raíz del proyecto:
   ```
   cp .env.example .env
   ```

2. Edita el archivo `.env` para configurar tus propios valores:
   ```
   # Configuración de base de datos
   DEFAULT_CONNECTION=Host=localhost;Database=bookreviewsdb;Username=tu_usuario;Password=tu_password;
   
   # Otras configuraciones
   ...
   ```

### Configuración de la base de datos PostgreSQL local

1. Asegúrate de tener PostgreSQL instalado y ejecutándose
2. Crea la base de datos:
   ```
   createdb bookreviewsdb
   ```
3. Actualiza las credenciales en tu archivo `.env` o en `appsettings.Development.json`

### Ejecución de migraciones

```bash
dotnet ef database update
```

## Configuración para entornos de despliegue

Para Railway, Heroku u otros servicios de despliegue, configura las variables de entorno directamente en la plataforma:

- `DEFAULT_CONNECTION`: Cadena de conexión principal
- `JWT_SECRET`: Clave secreta para firmar tokens JWT 
- `JWT_EXPIRY_MINUTES`: Tiempo de expiración de tokens
- `LOGGING_ENABLED`: Activar/desactivar logging detallado

## Notas adicionales

- Las variables definidas en entornos de despliegue tendrán siempre prioridad sobre los archivos de configuración
- Se recomienda no comprometer el archivo `.env` en el control de versiones (ya está en .gitignore)
- Nunca almacenes contraseñas o secretos en archivos de configuración incluidos en el repositorio