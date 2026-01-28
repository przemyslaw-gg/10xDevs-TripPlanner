# =======================
# FRONTEND BUILD (Vite)
# =======================
FROM node:20-alpine AS frontend-build
WORKDIR /app

COPY frontend/package*.json ./
RUN npm ci

COPY frontend .
RUN npm run build


# =======================
# BACKEND BUILD
# =======================
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS backend-build
WORKDIR /src

# Kopiuj plik solution
COPY backend/TripPlanner.sln ./

# Kopiuj wszystkie pliki .csproj zachowując strukturę folderów
COPY backend/src/TripPlanner.Domain/TripPlanner.Domain.csproj ./src/TripPlanner.Domain/
COPY backend/src/TripPlanner.Application/TripPlanner.Application.csproj ./src/TripPlanner.Application/
COPY backend/src/TripPlanner.Infrastructure/TripPlanner.Infrastructure.csproj ./src/TripPlanner.Infrastructure/
COPY backend/src/TripPlanner.WebApi/TripPlanner.WebApi.csproj ./src/TripPlanner.WebApi/

# Restore dependencies
RUN dotnet restore TripPlanner.sln

# Kopiuj caly kod zrodlowy
COPY backend/src ./src

# Publish
WORKDIR /src/src/TripPlanner.WebApi
RUN dotnet publish -c Release -o /app/publish --no-restore


# =======================
# FINAL RUNTIME
# =======================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

COPY --from=backend-build /app/publish .
COPY --from=frontend-build /app/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TripPlanner.WebApi.dll"]
