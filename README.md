### TodoApp
## Overview
TodoApp is a task management web application built using ASP.NET 8. It provides functionalities for creating, updating, and managing tasks, users, and categories. The application supports two types of users:
Owner (Admin): Can create and manage categories and tasks, assign tasks to users, view users, delete tasks, and send system invitations via email (SMTP).
Guest User: Can view their assigned tasks and mark them as completed.
The application features task filtering, error handling, logging, and an intuitive interface for efficient task management.

## Project Structure
TodoApp.Api.sln (Solution)
│
├─ docker-compose.yml
├─ Dockerfile
│
├─ TodoApp.Api
├─ TodoApp.Application
├─ TodoApp.Core
├─ TodoApp.Domain
├─ TodoApp.Infrastructure
└─ TodoApp.Tests
TodoApp.Core: Contains shared information across projects to avoid circular references.
TodoApp.Api: The API project handling HTTP requests and controllers.
TodoApp.Application, Domain, Infrastructure: Layered architecture for application logic, entities, and data access.
TodoApp.Tests: Unit and integration tests.


## Prerequisites
Visual Studio 2022 (or later)
.NET 8 SDK
SQL Server (for local development)
Docker Desktop (for containerized deployment)


## Getting Started
# Option 1: Running Locally (without Docker)

1. Ensure SQL Server is installed and running.
2. Open TodoApp.Api/appsettings.Development.json and comment out the Docker connection string
"ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=TodoAppDb;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
3. Uncomment the local connection string:
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TodoAppDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
4.Start SQL Server Management Studio (or any database engine tool) and connect to server: localhost.
5.Open Visual Studio and run the TodoApp.Api project.
6.The application will launch and Swagger UI will be available in the browser.


# Option 2: Running with Docker

1.Ensure Docker Desktop is installed and running.
2.Open TodoApp.Api/appsettings.Development.json, comment the local connection string and enable the Docker connection string.
3.Open a terminal at the project root directory and run:
docker-compose up --build
4.Docker will build the images and start the containers.
5.Once running, you can access the API at:
http://localhost:5000
6. A Postman collection is provided in the repository for testing all API endpoints.

## Default Users

Owner (Admin):
Email: owner@example.com
Password: P@ssw0rd
Guest Users: Can create an account to view and complete their tasks.


## Features
Task creation, assignment, update, and deletion.
Category management and filtering.
User management for Owners.
Email invitations using SMTP.
Error handling and logging.
Fully layered architecture for maintainability.


## Notes
TodoApp.Core contains shared objects used across all layers to prevent circular dependencies.
The application supports filtering, task completion, and proper logging for debugging.


## Detailed Postman import + usage steps (step-by-step)
Follow these steps to import and use the collection and environment in Postman.

# A. Import the Collection
Open Postman.
Click File → Import (or the blue Import button top-left).
Drag & drop TodoApp_API.postman_collection.json into the import area, or click Upload Files and select it.
Postman will show the collection under Collections.

# B. Import the Environment
Click the gear icon (Manage Environments) or go to Environments → Import.
Upload TodoApp.postman_environment.json.
You should now see TodoApp Environment in the environment dropdown.

# C. Select Environment
From the top-right environment selector (eye icon), select TodoApp Environment.
If needed, edit the environment (Manage Environments) and set baseUrl to your running server:
Local (default): http://localhost:5000/api
Docker: same default if you mapped ports as described in README
Ensure token is empty (it will be filled automatically by the Login test script).

# D. Set Authorization (recommended)
You can set collection-level Authorization so all requests automatically use the token:
Click the collection name TodoApp API.
Click the three dots → Edit → Authorization tab.
Choose Type: Bearer Token and in Token put {{token}}.
Save.
