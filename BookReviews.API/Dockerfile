FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar todo el código fuente
COPY . .

# Restaurar dependencias y publicar
RUN dotnet restore BookReviews.API/BookReviews.API.csproj
RUN dotnet publish BookReviews.API/BookReviews.API.csproj -c Release -o out

# Imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Puerto expuesto
EXPOSE 80
EXPOSE 443

# Comando de inicio
ENTRYPOINT ["dotnet", "BookReviews.API.dll"]