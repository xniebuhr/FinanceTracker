# Finance Tracker (In Progress)

A full-stack personal finance tracker built with Angular and ASP.NET Core. This project is currently under active development and aims to provide a secure, user-friendly platform for managing transactions, visualizing financial data, and exploring budgeting trends.

## Project Goals

- **User Authentication**  
  Implement secure login and registration using ASP.NET Identity authentication to support multi-user access and personalized data.

- **Data Visualization**  
  Integrate dynamic graphs and charts to help users understand spending habits, income trends, and financial goals.

- **Automated Testing**
  Multi-layered test coverage to ensure reliability and security
  
- **Live Deployment**  
  Host the application online for public access, removing the need for local setup. (Deployment TBD)

- **Clean Architecture & Repo Hygiene**  
  Maintain a modular, maintainable codebase with clear separation of concerns, semantic HTML, and professional documentation.

## Tech Stack

- **Frontend**: Angular (standalone components, reactive patterns, service-based architecture)
- **Backend**: ASP.NET Core Web API with Entity Framework
- **Database**: Currently migrating to Azure SQL Database
- **Dev Tools**: Batch scripts for workflow automation, Git for version control

## Current Features

- Add, view, and delete transactions
- Reactive UI updates using Subjects
- Form reset and refresh logic for predictable state
- Clean repo structure with `.gitignore`, README, and unified project layout

## Testing Strategy (planned)

Testing will be a key part of this project to ensure reliability, security, and maintainability. This strategy follows a layered approach, combining functional, integration, and security testing to align with the best and most secure practices.
### **Frontend**
- Unit testing: Jasmine
- Test runner: Karma
### **Backend**
- Unit / integration testing: xUnit
### **End to End Testing**
- E2E testing: Cypress
- SAST: GitHub CodeQL
- DAST: OWASP ZAP
- Dependency scanning: GitHub Dependabot or OWASP Dependency-Check
### **Infra / Security***
- Terraform & Trivy 
### **CI / CD Integration**
  - Automated test runs on every push using GitHub Actions, integrating security scans and dependency checks into the workflow
  - Deployment will be gated on tests and scans

## Status & Roadmap

This project is actively being developed. Core transaction functionality is in place, and authentication is basically complete. Testing is currently in progress and clean UI, graphing, and deployment will follow.

## Setup - Not currently available

Local setup is currently disabled while authentication and database migration are in progress. The application will be publicly accessible soon with proper authentication and access controls in place.
