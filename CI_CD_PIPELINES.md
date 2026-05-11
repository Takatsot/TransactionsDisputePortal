# CI/CD Pipelines Documentation

This document explains the continuous integration and deployment pipelines for the API and UI components.

## Table of Contents
- [Overview](#overview)
- [Pipeline Structure](#pipeline-structure)
- [GitHub Actions](#github-actions)
- [Azure DevOps](#azure-devops)
- [GitLab CI/CD](#gitlab-cicd)
- [Configuration](#configuration)
- [Deployment Strategies](#deployment-strategies)

---

## Overview

The project includes CI/CD configurations for three popular platforms:

1. **GitHub Actions** - `.github/workflows/`
2. **Azure DevOps** - `TransactionsDisputePortal/Solution Items/Pipelines/`
3. **GitLab CI/CD** - `.gitlab-ci.yml`

Each pipeline handles:
- ✅ Code compilation/build
- ✅ Unit testing
- ✅ Code coverage reporting
- ✅ Database migration script generation
- ✅ Docker image building
- ✅ Security scanning
- ✅ Deployment to development/test/production

---

## Pipeline Structure

### Azure DevOps Pipeline Organization

The Azure DevOps pipelines are simplified into 3 core files:

```
TransactionsDisputePortal/Solution Items/Pipelines/
├── CI.yml              # Combined build & test for API and UI
├── API Image.yml       # API Docker image deployment
└── UI Image.yml        # UI Docker image deployment
```

#### Pipeline Naming Convention

- **CI.yml** - Triggers on both `develop` and `main` branches, builds both API and UI
- **API Image.yml** - Manual trigger with environment parameter (development/test/production)
- **UI Image.yml** - Manual trigger with environment parameter (development/test/production)

#### Pipeline Flow

```
1. Push to develop/main branch
   ↓
2. CI Pipeline (Auto-triggered)
   - Build API (Debug for develop, Release for main)
   - Build UI
   - Run API Tests
   - Run UI Tests
   - Publish API Artifacts
   - Publish UI Artifacts
   - Generate Database Migration Scripts
   ↓
3. API Image Pipeline (Manual, select environment)
   - Build Docker image
   - Push to registry
   - Deploy to selected environment (dev/test/prod)
   - Scale and verify deployment
   ↓
4. UI Image Pipeline (Manual, select environment)
   - Build Docker image
   - Push to registry
   - Deploy to selected environment (dev/test/prod)
   - Scale and verify deployment
```

---

### GitHub Actions

**Location:** `.github/workflows/`

#### API Pipeline (`api-ci-cd.yml`)
```
├── Build & Test
│   ├── Setup .NET 10.0
│   ├── Restore dependencies
│   ├── Build solution
│   ├── Run unit tests
│   └── Code coverage (Codecov)
│
├── Docker Build
│   ├── Build multi-arch image (amd64, arm64)
│   ├── Push to GitHub Container Registry
│   └── Cache layers
│
├── Security Scan
│   └── Trivy vulnerability scanner
│
├── Deploy to Staging (develop branch)
│   └── Kubernetes deployment
│
└── Deploy to Production (main branch)
    └── Kubernetes deployment (manual approval)
```

#### UI Pipeline (`ui-ci-cd.yml`)
```
├── Build & Test
│   ├── Setup Node.js 20.x
│   ├── Install dependencies (npm ci)
│   ├── Lint code
│   ├── Run tests with coverage
│   └── Build React app
│
├── Docker Build
│   ├── Build multi-arch image
│   └── Push to registry
│
├── Security Scan
│   ├── npm audit
│   └── Snyk scan
│
├── Deploy to Staging
│   └── Kubernetes deployment
│
└── Deploy to Production
    └── Kubernetes deployment
```

**Triggers:**
- Push to `main` or `develop`
- Pull requests to `main`
- Path-based filtering

---

### Azure DevOps

**Location:** `TransactionsDisputePortal/Solution Items/Pipelines/`

Azure DevOps uses 3 streamlined pipelines for build, test, and deployment.

#### CI Pipeline (`CI.yml`)

**Purpose:** Combined build and test pipeline for both API and UI

**Triggers:**
- Branch: `main`, `develop`, `development`, `hotfix`
- Auto-detects branch and configures build (Debug for develop, Release for main)

**Stages:**

1. **Build_API**
   - Restore NuGet packages
   - Build .NET solution
   - Generate EF Core migration scripts

2. **Build_UI**
   - Install Node.js dependencies
   - Run ESLint

3. **Test**
   - Run API unit tests with code coverage
   - Run UI unit tests with code coverage
   - npm security audit

4. **Publish**
   - Publish API artifacts to `development\drop` or `main\drop`
   - Publish UI artifacts to `development\drop-ui` or `main\drop-ui`
   - Include database migration scripts

**Key Features:**
- .NET 10.0 SDK and Node.js 20.x
- Branch-based configuration (Debug/Release)
- Parallel API and UI testing
- Code coverage reporting (Cobertura)
- Artifact separation for API and UI
- EF Core idempotent migration scripts

---

#### API Image Pipeline (`API Image.yml`)

**Purpose:** Build and deploy API Docker image to any environment

**Triggers:**
- Manual trigger only

**Parameters:**
- `Environment`: development | test | production
- `Workload_Type`: api (default)

**Stages:**

1. **Prepare**
   - GitVersion versioning (optional)

2. **Build_Image**
   - Build Docker image with environment-specific configuration
   - Push to container registry
   - Security vulnerability scanning

3. **Deploy_Image**
   - Create deployment backup (production only)
   - Update Kubernetes/OpenShift deployment
   - Verify rollout status
   - Scale to environment-specific replica count
   - Health check validation

**Environment Configuration:**

| Environment | Image Name | Replicas | CPU | Memory | Config |
|------------|-----------|----------|-----|---------|--------|
| Development | dev-transactionsdisputeportal-api | 2 | 100m-500m | 256Mi-1Gi | Debug |
| Test | tst-transactionsdisputeportal-api | 2 | 250m-1000m | 512Mi-2Gi | Release |
| Production | transactionsdisputeportal-api | 3 | 250m-1000m | 512Mi-2Gi | Release |

**Key Features:**
- Environment parameter for flexible deployment
- Auto-configured based on target environment
- Production safety with backup and extended timeout
- Health check validation
- Resource limits per environment

---

#### UI Image Pipeline (`UI Image.yml`)

**Purpose:** Build and deploy UI Docker image to any environment

**Triggers:**
- Manual trigger only

**Parameters:**
- `Environment`: development | test | production
- `Workload_Type`: ui (default)

**Stages:**

1. **Prepare**
   - GitVersion versioning (optional)

2. **Build_Image**
   - Build Docker image with environment-specific API URL
   - Push to container registry
   - Security vulnerability scanning

3. **Deploy_Image**
   - Create deployment backup (production only)
   - Update Kubernetes/OpenShift deployment
   - Verify rollout status
   - Scale to environment-specific replica count

**Environment Configuration:**

| Environment | Image Name | Replicas | CPU | Memory | API URL Variable |
|------------|-----------|----------|-----|---------|-----------------|
| Development | dev-transactionsdisputeportal-ui | 2 | 50m-200m | 64Mi-256Mi | VITE_API_URL_DEV |
| Test | tst-transactionsdisputeportal-ui | 2 | 100m-250m | 128Mi-512Mi | VITE_API_URL_TST |
| Production | transactionsdisputeportal-ui | 3 | 100m-250m | 128Mi-512Mi | VITE_API_URL_PROD |

**Key Features:**
- Environment parameter for flexible deployment
- Build-time API URL injection per environment
- Auto-configured based on target environment
- Production safety with backup
- Lower resource requirements than API

---

### GitLab CI/CD

**Location:** `.gitlab-ci.yml` (root)

**Stages:**
1. `build` - Compile code
2. `test` - Run tests and coverage
3. `docker` - Build Docker images
4. `security` - Security scans
5. `deploy` - Deploy to environments

**Features:**
- Parallel job execution
- Docker-in-Docker (DinD)
- Path-based triggers
- Manual production deployment
- Coverage reporting

---

## Configuration

### Required Secrets/Variables

#### GitHub Actions
```yaml
# Repository Secrets
GITHUB_TOKEN              # Auto-provided
SNYK_TOKEN               # Optional: Snyk security scan
KUBE_CONFIG              # Kubernetes configuration

# Repository Variables
VITE_API_URL             # UI: API endpoint URL
```

#### Azure DevOps
```yaml
# Service Connections
DockerRegistryConnection  # Docker registry
k8s-staging              # Kubernetes staging
k8s-production           # Kubernetes production

# Pipeline Variables
VITE_API_URL             # UI: API endpoint
```

#### GitLab CI/CD
```yaml
# CI/CD Variables
CI_REGISTRY              # Auto-provided
CI_REGISTRY_USER         # Auto-provided
CI_REGISTRY_PASSWORD     # Auto-provided
KUBE_CONTEXT_STAGING     # Kubernetes context
KUBE_CONTEXT_PRODUCTION  # Kubernetes context
```

---

## Deployment Strategies

### Branching Strategy

```
main (production)
  ├── Automatic: Build, test, Docker build
  └── Manual: Deploy to production
  
develop (staging)
  ├── Automatic: Build, test, Docker build
  └── Automatic: Deploy to staging
  
feature/* (development)
  └── Automatic: Build and test only
```

### Environment Flow

```
Developer → Feature Branch → Pull Request → Develop (Staging) → Main (Production)
```

### Deployment Process

#### Staging Deployment
1. **Trigger:** Push to `develop` branch
2. **Automatic:** Builds, tests, and deploys
3. **Validation:** Smoke tests run
4. **Rollback:** Automatic on failure

#### Production Deployment
1. **Trigger:** Push to `main` branch or manual trigger
2. **Manual Approval:** Required (environment protection)
3. **Blue-Green:** Zero-downtime deployment
4. **Rollback:** Manual or automatic on health check failure

### Health Checks

**API Health Check:**
```bash
GET http://api-service:8080/health
```

**UI Health Check:**
```bash
GET http://ui-service:80/health
```

---

## Running Locally

### Test Pipeline Locally (GitHub Actions)

Install [act](https://github.com/nektos/act):
```powershell
# Using Chocolatey
choco install act-cli

# Run workflow
act -W .github/workflows/api-ci-cd.yml
```

### Test Azure Pipeline

```powershell
# Validate pipeline
az pipelines run --name "API-Pipeline" --branch develop --organization <org> --project <project>
```

### Test GitLab Pipeline

```bash
# Validate .gitlab-ci.yml
gitlab-ci-lint .gitlab-ci.yml
```

---

## Docker Image Tags

### Tagging Strategy

```
<registry>/<image>:<tag>

Examples:
ghcr.io/yourorg/transactionsdisputeportal/api:latest
ghcr.io/yourorg/transactionsdisputeportal/api:main-abc123
ghcr.io/yourorg/transactionsdisputeportal/api:v1.2.3
ghcr.io/yourorg/transactionsdisputeportal/ui:develop-xyz789
```

### Tags Generated
- `latest` - Latest build from main branch
- `<branch>-<sha>` - Branch with commit SHA
- `v<version>` - Semantic version tags
- `<pr-number>` - Pull request builds

---

## Monitoring & Notifications

### GitHub Actions
- **Status Badge:** Add to README.md
- **Slack/Teams:** Use actions/slack-notify
- **Email:** Configured in repository settings

### Azure DevOps
- **Dashboard:** Build status dashboard
- **Email:** Automatic on build failure
- **Teams:** Integration with Microsoft Teams

### GitLab CI/CD
- **Pipelines Dashboard:** Built-in
- **Email:** Configured per project
- **Slack:** Integration available

---

## Troubleshooting

### Common Issues

#### Build Fails
```powershell
# Check logs
gh run view <run-id> --log

# Re-run failed jobs
gh run rerun <run-id> --failed
```

#### Docker Build Fails
```bash
# Build locally to debug
docker build -f Dockerfile -t test-api .

# Check build context
docker build --no-cache -f Dockerfile .
```

#### Kubernetes Deployment Fails
```bash
# Check pod status
kubectl get pods -n transactionsdisputeportal

# View pod logs
kubectl logs -f deployment/api -n transactionsdisputeportal

# Describe deployment
kubectl describe deployment api -n transactionsdisputeportal
```

#### Test Coverage Not Uploading
```bash
# Verify coverage format
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Check coverage file exists
ls ./coverage/**/coverage.cobertura.xml
```

---

## Best Practices

### 1. **Fast Feedback**
- Run tests in parallel
- Cache dependencies
- Use Docker layer caching

### 2. **Security**
- Scan dependencies (npm audit, dotnet list package --vulnerable)
- Scan Docker images (Trivy, Snyk)
- Use secret scanning
- Sign commits

### 3. **Quality Gates**
- Minimum code coverage (80%)
- No critical vulnerabilities
- All tests passing
- Linting passes

### 4. **Deployment Safety**
- Always deploy to staging first
- Require manual approval for production
- Implement rollback strategy
- Monitor deployment health

### 5. **Performance**
- Cache npm/nuget packages
- Use multi-stage Docker builds
- Parallel job execution
- Incremental builds

---

## Maintenance

### Regular Tasks

**Weekly:**
- Review failed builds
- Update dependencies
- Check security vulnerabilities

**Monthly:**
- Update CI/CD templates
- Review pipeline performance
- Update base images

**Quarterly:**
- Update .NET SDK version
- Update Node.js version
- Review deployment strategy

---

## Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Pipelines Documentation](https://docs.microsoft.com/en-us/azure/devops/pipelines/)
- [GitLab CI/CD Documentation](https://docs.gitlab.com/ee/ci/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Kubernetes Deployment Strategies](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/)
