# Imagen base para compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar global.json para asegurar la versión correcta de .NET
COPY ["global.json", "./"]

# Copiar archivos de proyecto y restaurar dependencias
COPY ["BookReviews.API/BookReviews.API.csproj", "BookReviews.API/"]
COPY ["BookReviews.Application/BookReviews.Application.csproj", "BookReviews.Application/"]
COPY ["BookReviews.Domain/BookReviews.Domain.csproj", "BookReviews.Domain/"]
COPY ["BookReviews.Core/BookReviews.Core.csproj", "BookReviews.Core/"]
COPY ["BookReviews.Infrastructure/BookReviews.Infrastructure.csproj", "BookReviews.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "BookReviews.API/BookReviews.API.csproj"

# Copiar todo el código fuente
COPY . .

# Verificar versión de .NET SDK
RUN dotnet --info

# Publicar la aplicación
RUN dotnet publish "BookReviews.API/BookReviews.API.csproj" -c Release -o /app/publish

# Imagen base para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar los archivos de publicación
COPY --from=build /app/publish .

# Copiar el script de inicio
COPY start.sh .
RUN chmod +x start.sh

# Configuración para Railway
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

# Exponer puerto
EXPOSE $PORT

# Usar el script de inicio en lugar del comando directo
ENTRYPOINT ["./start.sh"]