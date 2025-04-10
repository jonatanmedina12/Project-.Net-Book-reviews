# Imagen de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

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

# Publicar la aplicación
WORKDIR "/app/BookReviews.API"
RUN dotnet publish "BookReviews.API.csproj" -c Release -o /app/publish

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Configurar puertos
EXPOSE 80
EXPOSE 443

# Comando de inicio
ENTRYPOINT ["dotnet", "BookReviews.API.dll"]