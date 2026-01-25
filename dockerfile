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
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /src

COPY backend/*.csproj backend/
RUN dotnet restore backend/TripPlanner.WebApi.csproj

COPY backend/src backend
WORKDIR /src/backend
RUN dotnet publish TripPlanner.WebApi/TripPlanner.WebApi.csproj -c Release -o /app/publish


# =======================
# FINAL RUNTIME
# =======================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=backend-build /app/publish .
COPY --from=frontend-build /app/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TripPlanner.WebApi.dll"]