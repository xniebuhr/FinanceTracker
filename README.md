# Finance Tracker (In Progress)

A full-stack personal finance tracker built with Angular and ASP.NET Core. This project is currently under active development and aims to provide a secure, user-friendly platform for managing transactions, visualizing financial data, and exploring budgeting trends.

You can see the deployed website at this link: https://app-finance-tracker-6n8mk7w3.azurewebsites.net/
Understand that this is being actively developed, and while all of the backend and testing automation is complete, the frontend is very minimal and functionality to fully communicate with the backend may not be available for a while. Updates will be noted as made to the project

## Project Goals
- **User Authentication**  
  Implement secure login and registration using ASP.NET Identity authentication to support multi-user access and personalized data.

- **Data Visualization**  
  Integrate dynamic graphs and charts to help users understand spending habits, income trends, and financial goals.

- **Automated Testing**
  Multi-layered test coverage to ensure reliability and security
  
- **Live Deployment**  
  Host the application online for public access, removing the need for local setup.

- **Clean Architecture & Repo Hygiene**  
  Maintain a modular, maintainable codebase with clear separation of concerns, semantic HTML, and professional documentation.

## Tech Stack
- **Frontend**: Angular (standalone components, reactive patterns, service-based architecture)
- **Backend**: ASP.NET Core Web API with Entity Framework
- **Database**: Azure SQL Database
- **Dev Tools**: Batch scripts for workflow automation, Git for version control, Terraform for infrastructure management, Renovate for package updates, and Docker for containerization

## Current Features

- Add, view, and delete transactions
- Authentication and user handling
- Clean repo structure with `.gitignore`, README, and unified project layout
- Fully built and scaled backend with security protections in place
- Comprehensive testing and deployment in ci/cd pipeline

## Testing Strategy
Testing is a key part of this project to ensure reliability, security, and maintainability. This strategy follows a layered approach, combining functional, integration, and security testing to align with the best and most secure practices.
### **Frontend**
- Unit testing: Jasmine
- Test runner: Karma
### **Backend**
- Unit / integration testing: xUnit
### **End to End Testing**
- E2E testing: Cypress
- SAST: GitHub CodeQL
- DAST: OWASP ZAP
- Dependency scanning: GitHub Dependabot
- Secret scanning: Trufflehog
- Linting: Super-linter
### **Containerizing**
- Docker & Trivy
### **CI / CD Integration**
  - Automated test runs on every push using GitHub Actions, integrating security scans and dependency checks into the workflow
  - Deployment will be gated on tests and scans

Currently, testing is limited to only the automatic scanners, no unit tests, e2e tests, or integration tests have been written yet. These will be written once all of the main functionality of the web app is in place and no other major changes may make said tests obsolete.

## Status & Roadmap
This project is actively being developed. Core transaction functionality is in place, and authentication is complete. The full ci/cd pipeline is built out, and the next major hurdle is to have a clean and working frontend. After the frontend is complete, tests will be written for all parts and the project will be maintained as I'm available.