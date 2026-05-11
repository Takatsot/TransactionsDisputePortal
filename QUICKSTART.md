# Quick Start - Rancher Desktop

## Step 1: Start Rancher Desktop
1. Open Rancher Desktop
2. Ensure Kubernetes is enabled
3. Wait for it to be ready (green checkmark)

## Step 2: Build and Deploy with Docker Compose

```powershell
# Navigate to project directory
cd C:\project\TransactionsDisputePortal

# Build all images (first time only or after code changes)
docker-compose build

# Start all services
docker-compose up -d

# Watch logs
docker-compose logs -f
```

## Step 3: Wait for Services to Start
- Database takes ~30 seconds to initialize
- API takes ~60 seconds to start and run migrations
- UI takes ~10 seconds

## Step 4: Access the Application

**Application URLs:**
- UI: http://localhost:3000
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

**Test Credentials:**
- Email: `testuser@email.com`
- Password: `Password123!`

OR

- Email: `testuser2@email.com`
- Password: `Password123!`

## Step 5: Verify Everything is Running

```powershell
# Check container status
docker-compose ps

# Should show:
# - transactionsdisputeportal-db (healthy)
# - transactionsdisputeportal-api (healthy)
# - transactionsdisputeportal-ui (healthy)
```

## Common Commands

```powershell
# Stop all services
docker-compose down

# Stop and remove all data (fresh start)
docker-compose down -v

# Rebuild after code changes
docker-compose build api
docker-compose up -d api

# View specific service logs
docker-compose logs -f api
docker-compose logs -f ui
docker-compose logs -f sqlserver

# Restart a service
docker-compose restart api

# Access SQL Server
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

## Troubleshooting

### "Port already in use"
```powershell
# Find what's using the port
netstat -ano | findstr :5000

# Kill the process (replace PID with actual number)
Stop-Process -Id <PID> -Force

# Or change ports in docker-compose.yml
```

### "Database not responding"
```powershell
# Check SQL Server logs
docker-compose logs sqlserver

# Restart SQL Server
docker-compose restart sqlserver

# Wait for health check
docker-compose ps
```

### "API not starting"
```powershell
# View detailed logs
docker-compose logs api

# Check if database is ready
docker-compose ps sqlserver

# Force rebuild
docker-compose build --no-cache api
docker-compose up -d api
```

### "UI shows connection error"
- Ensure API is running: `docker-compose ps api`
- Check API logs: `docker-compose logs api`
- Verify API URL in browser: http://localhost:5000/health
- Clear browser cache and reload

## Next Steps
- See [RANCHER_DESKTOP_DEPLOYMENT.md](RANCHER_DESKTOP_DEPLOYMENT.md) for detailed documentation
- Configure production settings
- Set up monitoring
- Enable HTTPS
