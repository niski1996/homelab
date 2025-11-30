# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out ./

# Create non-root user for security
RUN adduser --disabled-password --gecos '' --shell /bin/bash --home /home/appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

EXPOSE 5008

ENTRYPOINT ["dotnet", "HomeLabGymApi.dll"]
