# Use the official ASP.NET runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files
COPY Vouchee.API/Vouchee.API.csproj Vouchee.API/
COPY Vouchee.Business/Vouchee.Business.csproj Vouchee.Business/
COPY Vouchee.Data/Vouchee.Data.csproj Vouchee.Data/

# Restore dependencies
RUN dotnet restore "Vouchee.API/Vouchee.API.csproj"

# Copy the remaining source code
COPY . .

# Build the application
WORKDIR /src/Vouchee.API
RUN dotnet build "Vouchee.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Vouchee.API.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vouchee.API.dll"]
