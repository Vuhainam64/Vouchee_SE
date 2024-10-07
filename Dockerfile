# Base stage for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and set working directory
COPY ["Vouchee.API/Vouchee.API.csproj", "Vouchee.Business/Vouchee.Business.csproj", "Vouchee.Data/Vouchee.Data.csproj", "./"]

# Restore dependencies for all projects
RUN dotnet restore "Vouchee.API/Vouchee.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "Vouchee.API/Vouchee.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage for preparing the app for production
FROM build AS publish
RUN dotnet publish "Vouchee.API/Vouchee.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vouchee.API.dll"]
