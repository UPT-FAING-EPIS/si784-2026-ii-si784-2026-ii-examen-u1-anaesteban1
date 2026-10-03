FROM node:24-alpine AS frontend-build
WORKDIR /frontend
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/LostAndFound.Api.csproj backend/
RUN dotnet restore backend/LostAndFound.Api.csproj

COPY backend/ backend/
COPY --from=frontend-build /frontend/dist backend/wwwroot
RUN dotnet publish backend/LostAndFound.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "LostAndFound.Api.dll"]
