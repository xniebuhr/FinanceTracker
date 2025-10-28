# CI/CD Pipeline (Placeholder)

This file documents the planned GitHub Actions pipeline.  
When ready, it will be saved as `.github/workflows/ci.yml`.

## On Commit to `dev`
- Run backend unit + integration tests (xUnit with EF InMemory)
- Run frontend unit tests (Jasmine/Karma)
- Run static analysis (CodeQL)
- Run dependency scanning (Dependabot)

## On Pull Request to `main`
- Build Docker image of backend
- Run containerized integration tests (xUnit with SQL Server/Postgres in Docker)
- Run Cypress end-to-end tests against the containerized app
- Run Trivy scan on the Docker image
- Run OWASP ZAP DAST scan
- Validate Terraform configuration

## Notes
- EF migrations will be applied in both commit-time (in-memory DB) and PR-time (containerized DB)
- This file is just a placeholder and does not execute until converted to a `.yml`

This is what the real file might look like, generated courtesy of Copilot becuase I'm not 100% what it should look like yet. Read this in raw formatting otherwise it looks like absolute garbage

name: CI Pipeline

on:
  push:
    branches: [ "dev" ]
  pull_request:
    branches: [ "main" ]

jobs:
  # Fast checks on commit to dev
  commit-tests:
    if: github.ref == 'refs/heads/dev'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Run EF migrations (in-memory/SQLite)
        run: dotnet ef database update --context AppDbContext --environment Testing

      - name: Run backend tests
        run: dotnet test --no-build --verbosity normal

      - name: Run frontend tests
        run: npm install && npm test

      - name: CodeQL Analysis
        uses: github/codeql-action/init@v3
        with:
          languages: csharp, javascript
      - uses: github/codeql-action/analyze@v3

  # Heavier checks on PR to main
  pr-tests:
    if: github.event_name == 'pull_request'
    runs-on: ubuntu-latest
    services:
      db:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          SA_PASSWORD: "Your_password123"
          ACCEPT_EULA: "Y"
        ports:
          - 1433:1433
    steps:
      - uses: actions/checkout@v4

      - name: Build Docker image
        run: docker build -t project-backend .

      - name: Run containerized integration tests
        run: dotnet test --filter Category=Integration

      - name: Run Cypress E2E tests
        run: npm run cypress:run

      - name: Trivy scan
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: project-backend:latest

      - name: OWASP ZAP scan
        run: echo "TODO: Add ZAP scan step"

      - name: Terraform validate
        run: terraform validate
