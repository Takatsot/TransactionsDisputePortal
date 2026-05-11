# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["TransactionsDisputePortal/TransactionsDisputePortal.sln", "TransactionsDisputePortal/"]
COPY ["TransactionsDisputePortal/TransactionsDisputePortal.Api/TransactionsDisputePortal.Api.csproj", "TransactionsDisputePortal/TransactionsDisputePortal.Api/"]
COPY ["TransactionsDisputePortal/TransactionsDisputePortal.Application/TransactionsDisputePortal.Application.csproj", "TransactionsDisputePortal/TransactionsDisputePortal.Application/"]
COPY ["TransactionsDisputePortal/TransactionsDisputePortal.Domain/TransactionsDisputePortal.Domain.csproj", "TransactionsDisputePortal/TransactionsDisputePortal.Domain/"]
COPY ["TransactionsDisputePortal/TransactionsDisputePortal.Infrastructure/TransactionsDisputePortal.Infrastructure.csproj", "TransactionsDisputePortal/TransactionsDisputePortal.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "TransactionsDisputePortal/TransactionsDisputePortal.Api/TransactionsDisputePortal.Api.csproj"

# Copy everything else
COPY TransactionsDisputePortal/ TransactionsDisputePortal/

# Build the application
WORKDIR /src/TransactionsDisputePortal/TransactionsDisputePortal.Api
RUN dotnet build "TransactionsDisputePortal.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TransactionsDisputePortal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create uploads directory with proper permissions
RUN mkdir -p /app/uploads && chmod 777 /app/uploads

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app

EXPOSE 8080

COPY --from=publish /app/publish .

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TransactionsDisputePortal.Api.dll"]
