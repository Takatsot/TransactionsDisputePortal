# Rancher Desktop Deployment Guide

This guide explains how to deploy the Transactions Dispute Portal on Rancher Desktop.

## Prerequisites

1. **Rancher Desktop** installed and running
   - Download from: https://rancherdesktop.io/
   - Enable Kubernetes in settings
   - Set container runtime to `containerd` or `dockerd`

2. **kubectl** configured (comes with Rancher Desktop)
   ```powershell
   kubectl version --client
   ```

3. **Docker Compose** (included with Rancher Desktop)

## Deployment Options

### Option 1: Docker Compose (Recommended for Development)

#### Step 1: Build Images
```powershell
# Navigate to project root
cd C:\project\TransactionsDisputePortal

# Build all images
docker-compose build

# Or build individually
docker build -t transactionsdisputeportal-api:latest -f Dockerfile .
docker build -t transactionsdisputeportal-ui:latest -f TransactionsDisputePortal.UI/Dockerfile ./TransactionsDisputePortal.UI
```

#### Step 2: Start Services
```powershell
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Check status
docker-compose ps
```

#### Step 3: Access Application
- **UI**: http://localhost:3000
- **API**: http://localhost:5000
- **API Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433
  - Username: `sa`
  - Password: `YourStrong@Passw0rd`

#### Step 4: Stop Services
```powershell
# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: deletes database)
docker-compose down -v
```

---

### Option 2: Kubernetes Deployment (Recommended for Production-like)

#### Step 1: Build and Load Images
```powershell
# Build images
docker-compose build

# For Rancher Desktop, images are automatically available in Kubernetes
# Verify images
docker images | Select-String "transactionsdisputeportal"
```

#### Step 2: Deploy to Kubernetes
```powershell
# Apply Kubernetes manifests
kubectl apply -f k8s-deployment.yaml

# Check deployment status
kubectl get all -n transactionsdisputeportal

# Watch pod status
kubectl get pods -n transactionsdisputeportal -w
```

#### Step 3: Access Application
```powershell
# Get service URLs
kubectl get svc -n transactionsdisputeportal

# Port forward UI (access at http://localhost:3000)
kubectl port-forward -n transactionsdisputeportal svc/ui-service 3000:80

# Port forward API (access at http://localhost:5000)
kubectl port-forward -n transactionsdisputeportal svc/api-service 5000:8080

# Port forward SQL Server (access at localhost:1433)
kubectl port-forward -n transactionsdisputeportal svc/sqlserver-service 1433:1433
```

#### Step 4: View Logs
```powershell
# API logs
kubectl logs -n transactionsdisputeportal -l app=api -f

# UI logs
kubectl logs -n transactionsdisputeportal -l app=ui -f

# SQL Server logs
kubectl logs -n transactionsdisputeportal -l app=sqlserver -f
```

#### Step 5: Scale Application
```powershell
# Scale API replicas
kubectl scale deployment api -n transactionsdisputeportal --replicas=3

# Scale UI replicas
kubectl scale deployment ui -n transactionsdisputeportal --replicas=3
```

#### Step 6: Clean Up
```powershell
# Delete all resources
kubectl delete namespace transactionsdisputeportal
```

---

## Configuration

### Environment Variables

#### API Container
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` or `Production`
- `ASPNETCORE_URLS`: HTTP endpoint (default: `http://+:8080`)
- `ConnectionStrings__DefaultConnection`: Database connection string
- `JwtSettings__SecretKey`: JWT signing key (min 32 characters)
- `JwtSettings__Issuer`: JWT issuer
- `JwtSettings__Audience`: JWT audience
- `JwtSettings__ExpiryHours`: Token expiration (default: 24)

#### UI Container
- `VITE_API_URL`: API base URL (default: `http://localhost:5000`)

### Persistent Storage

#### Docker Compose
- `sqlserver-data`: SQL Server database files
- `api-uploads`: Uploaded files (dispute evidence)

#### Kubernetes
- SQL Server uses a `StatefulSet` with `PersistentVolumeClaim`
- API uses `emptyDir` for uploads (ephemeral)
- For persistent uploads, create a `PersistentVolumeClaim` and mount it

---

## Troubleshooting

### Database Not Accessible
```powershell
# Check SQL Server container logs
docker-compose logs sqlserver
# OR
kubectl logs -n transactionsdisputeportal -l app=sqlserver

# Test connection from API container
docker-compose exec api bash -c "curl -v http://sqlserver:1433"
# OR
kubectl exec -it -n transactionsdisputeportal deployment/api -- curl -v http://sqlserver-service:1433
```

### API Not Starting
```powershell
# Check API logs
docker-compose logs api
# OR
kubectl logs -n transactionsdisputeportal -l app=api

# Common issues:
# 1. Database not ready - wait for SQL Server health check
# 2. Connection string incorrect - check environment variables
# 3. Port already in use - change port in docker-compose.yml
```

### UI Cannot Connect to API
```powershell
# Check UI logs
docker-compose logs ui
# OR
kubectl logs -n transactionsdisputeportal -l app=ui

# Verify VITE_API_URL is correct
# For Docker Compose: http://localhost:5000
# For Kubernetes: Use port-forward or Ingress URL
```

### Health Checks Failing
```powershell
# Test health endpoint manually
curl http://localhost:5000/health

# If 404, ensure HealthChecksConfiguration.cs is configured
# Check API logs for startup errors
```

### Image Pull Errors (Kubernetes)
```powershell
# Ensure images are built and available
docker images | Select-String "transactionsdisputeportal"

# Set imagePullPolicy to IfNotPresent in k8s-deployment.yaml
# Already configured by default
```

---

## Performance Tuning

### Resource Limits

#### Docker Compose
Edit `docker-compose.yml`:
```yaml
api:
  deploy:
    resources:
      limits:
        cpus: '1.0'
        memory: 1G
      reservations:
        cpus: '0.5'
        memory: 512M
```

#### Kubernetes
Already configured in `k8s-deployment.yaml`:
- **API**: 512Mi-1Gi RAM, 250m-500m CPU
- **UI**: 64Mi-128Mi RAM, 100m-200m CPU
- **SQL Server**: 2Gi-4Gi RAM, 1-2 CPU

### Connection Pooling
SQL Server connection string already includes:
- `MultipleActiveResultSets=True`
- Default pool size: 100
- To customize, add: `;Max Pool Size=200;Min Pool Size=10`

---

## Security Considerations

### Production Deployment Checklist

- [ ] Change default SQL Server password
- [ ] Use strong JWT secret key (generate random 64-character string)
- [ ] Enable HTTPS (add TLS certificates)
- [ ] Implement rate limiting
- [ ] Add authentication to SQL Server port (don't expose publicly)
- [ ] Use Kubernetes Secrets for sensitive data
- [ ] Enable CORS properly in API
- [ ] Scan images for vulnerabilities
- [ ] Implement network policies
- [ ] Enable audit logging

### Generate Secure Secrets
```powershell
# Generate JWT secret (PowerShell)
-join ((65..90) + (97..122) + (48..57) + (33,35,36,37,38,42,43,45,61) | Get-Random -Count 64 | ForEach-Object {[char]$_})

# Generate SQL password
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 32 | ForEach-Object {[char]$_}) + "@1"
```

---

## Monitoring

### Docker Compose
```powershell
# Resource usage
docker stats

# Container health
docker-compose ps

# Logs
docker-compose logs -f --tail=100
```

### Kubernetes
```powershell
# Resource usage
kubectl top pods -n transactionsdisputeportal
kubectl top nodes

# Events
kubectl get events -n transactionsdisputeportal --sort-by='.lastTimestamp'

# Describe pod for detailed info
kubectl describe pod -n transactionsdisputeportal <pod-name>
```

---

## Backup and Restore

### SQL Server Backup (Docker Compose)
```powershell
# Backup database
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "BACKUP DATABASE TransactionsDisputePortalDb TO DISK = '/var/opt/mssql/backup/db.bak' WITH FORMAT"

# Copy backup to host
docker cp transactionsdisputeportal-db:/var/opt/mssql/backup/db.bak ./db-backup.bak
```

### SQL Server Restore
```powershell
# Copy backup to container
docker cp ./db-backup.bak transactionsdisputeportal-db:/var/opt/mssql/backup/db.bak

# Restore database
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "RESTORE DATABASE TransactionsDisputePortalDb FROM DISK = '/var/opt/mssql/backup/db.bak' WITH REPLACE"
```

---

## Next Steps

1. Configure domain name (edit `/etc/hosts` or DNS)
2. Set up SSL/TLS certificates
3. Implement CI/CD pipeline
4. Configure monitoring (Prometheus/Grafana)
5. Set up log aggregation (ELK/Loki)
6. Implement backup automation

## Support

For issues or questions:
- Check logs first
- Review this documentation
- Check Docker/Kubernetes documentation
- Review application logs in `/app/logs` (if configured)
