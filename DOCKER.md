# Transactions Dispute Portal - Docker Deployment

## Quick Start

### Prerequisites
- Docker Desktop installed and running
- At least 4GB RAM available for Docker

### Running with Docker Compose

1. **Clone the repository**
   ```bash
   cd c:\project\TransactionsDisputePortal
   ```

2. **Start all services**
   ```bash
   docker-compose up -d
   ```

   This will start:
   - SQL Server (port 1433)
   - .NET API (port 5000)
   - React UI (port 3000)

3. **Check service health**
   ```bash
   docker-compose ps
   ```

4. **View logs**
   ```bash
   # All services
   docker-compose logs -f

   # Specific service
   docker-compose logs -f api
   docker-compose logs -f ui
   docker-compose logs -f sqlserver
   ```

5. **Access the application**
   - UI: http://localhost:3000
   - API: http://localhost:5000
   - API Health: http://localhost:5000/health
   - Swagger: http://localhost:5000/swagger

### Stopping Services

```bash
# Stop services
docker-compose down

# Stop and remove volumes (database data will be lost)
docker-compose down -v
```

## Building Individual Images

### API
```bash
docker build -t transactionsdisputeportal-api:latest -f Dockerfile .
```

### UI
```bash
cd TransactionsDisputePortal.UI
docker build -t transactionsdisputeportal-ui:latest .
```

## Environment Variables

### API
- `ASPNETCORE_ENVIRONMENT`: Development | Staging | Production
- `ConnectionStrings__DefaultConnection`: SQL Server connection string
- `JwtSettings__Secret`: JWT signing key (min 32 chars)
- `JwtSettings__ExpiryHours`: Token expiration (default: 24)

### UI
- `VITE_API_URL`: API base URL (default: http://localhost:5000)

## Troubleshooting

### Database not starting
```bash
# Check SQL Server logs
docker-compose logs sqlserver

# Ensure port 1433 is not in use
netstat -an | findstr :1433
```

### API cannot connect to database
```bash
# Wait for SQL Server to be healthy
docker-compose ps

# Check API logs
docker-compose logs api
```

### UI cannot connect to API
- Ensure CORS is properly configured in API
- Check browser console for CORS errors
- Verify API is running: http://localhost:5000/health

## Production Deployment

For production, update:

1. **Environment**: Change `ASPNETCORE_ENVIRONMENT=Production`
2. **Secrets**: Use Docker secrets or environment variables
3. **Database**: Use managed SQL Server (Azure SQL, AWS RDS)
4. **File Storage**: Use blob storage (Azure Blob, S3)
5. **CORS**: Restrict to your domain only
6. **HTTPS**: Add reverse proxy (nginx, Traefik) with SSL certificates

### Example Production docker-compose.yml
```yaml
version: '3.8'

services:
  api:
    image: yourregistry.azurecr.io/transactionsdisputeportal-api:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
      - JwtSettings__Secret=${JWT_SECRET}
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - /etc/ssl/certs:/app/certs:ro
    restart: unless-stopped

  ui:
    image: yourregistry.azurecr.io/transactionsdisputeportal-ui:latest
    environment:
      - VITE_API_URL=https://api.yourdomain.com
    ports:
      - "80:80"
    restart: unless-stopped
```

## Health Checks

All services include health checks:
- **SQL Server**: Checks server connectivity
- **API**: `/health` endpoint (database + file storage)
- **UI**: nginx health endpoint

## Volumes

- `sqlserver-data`: Persists database files
- `./uploads`: Persists uploaded dispute attachments

## Networks

All services communicate via `transactionsdisputeportal-network` bridge network.
