# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install dependencies for health checks and debugging
RUN apt-get update && apt-get install -y \
    curl \
    postgresql-client \
    && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Create a non-root user
RUN addgroup --gid 1001 --system aspnet && \
    adduser --uid 1001 --system --ingroup aspnet aspnet

# Set ownership and switch to non-root user
RUN chown -R aspnet:aspnet /app
USER aspnet

EXPOSE 8080

ENTRYPOINT ["dotnet", "backend.dll"]
