FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiuj pliki projektów - ścieżki względem kontekstu (katalog nadrzędny)
COPY ["Walory Backend/WaloryBackend.csproj", "Walory Backend/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastracture/Infrastracture.csproj", "Infrastracture/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# Przywróć zależności
RUN dotnet restore "Walory Backend/WaloryBackend.csproj"
RUN apt-get update && apt-get install -y ca-certificates

# Kopiuj wszystkie pliki źródłowe
COPY ["Walory Backend/", "Walory Backend/"]
COPY ["Application/", "Application/"]
COPY ["Infrastracture/", "Infrastracture/"]
COPY ["Domain/", "Domain/"]

# Ustaw katalog roboczy na projekt główny
WORKDIR "/src/Walory Backend"
RUN dotnet build "WaloryBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WaloryBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WaloryBackend.dll"]
