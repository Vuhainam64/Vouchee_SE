# Base stage for running the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Vouchee.API/Vouchee.API.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build "Vouchee.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage for preparing the app for production
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Vouchee.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vouchee.API.dll"]
