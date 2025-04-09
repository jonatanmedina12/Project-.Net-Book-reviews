FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BookReviews.API/BookReviews.API.csproj", "BookReviews.API/"]
COPY ["BookReviews.Application/BookReviews.Application.csproj", "BookReviews.Application/"]
COPY ["BookReviews.Core/BookReviews.Core.csproj", "BookReviews.Core/"]
COPY ["BookReviews.Domain/BookReviews.Domain.csproj", "BookReviews.Domain/"]
COPY ["BookReviews.Infrastructure/BookReviews.Infrastructure.csproj", "BookReviews.Infrastructure/"]
RUN dotnet restore "BookReviews.API/BookReviews.API.csproj"
COPY . .
WORKDIR "/src/BookReviews.API"
RUN dotnet build "BookReviews.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookReviews.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookReviews.API.dll"]