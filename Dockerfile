# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY CourseWork.sln ./CourseWork/

COPY CourseWork/CourseWork.csproj ./CourseWork/CourseWork
COPY InventoryManagement.Models/InventoryManagement.Models.csproj ./CourseWork/InventoryManagement.Models/
COPY InventoryManagement.Data/InventoryManagement.Data.csproj ./CourseWork/InventoryManagement.Data/
COPY InventoryManagement.Services/InventoryManagement.Services.csproj ./CourseWork/InventoryManagement.Services/
COPY InventoryManagement.Tests/InventoryManagement.Tests.csproj ./CourseWork/InventoryManagement.Tests/

# Restore dependencies
RUN dotnet restore ./CourseWork/CourseWork.sln

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /src/CourseWork/CourseWork
RUN dotnet build CourseWork.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
WORKDIR /src/CourseWork/CourseWork
RUN dotnet publish CourseWork.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CourseWork.dll"]