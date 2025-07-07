# Task Management Application

This is a simple Task Management application built with an Angular frontend and a .NET Web API backend. It allows users to authenticate, manage tasks (add, view, edit, delete, mark as completed), and features basic filtering, sorting, and responsiveness.

---

## Table of Contents

1.  [Project Overview](#1-project-overview)
2.  [Features](#2-features)
3.  [Technologies Used](#3-technologies-used)
4.  [Prerequisites](#4-prerequisites)
5.  [Backend Setup (.NET Web API)](#5-backend-setup-net-web-api)
    * [5.1 Clone the Repository](#51-clone-the-repository)
    * [5.2 Configure Database Connection](#52-configure-database-connection)
    * [5.3 Database Migrations & Seeding](#53-database-migrations--seeding)
    * [5.4 Run the Backend](#54-run-the-backend)
    * [5.5 Backend API Endpoints (Swagger UI)](#55-backend-api-endpoints-swagger-ui)
6.  [Frontend Setup (Angular)](#6-frontend-setup-angular)
    * [6.1 Node.js Version Management](#61-nodejs-version-management)
    * [6.2 Install Angular CLI](#62-install-angular-cli)
    * [6.3 Install Dependencies](#63-install-dependencies)
    * [6.4 Configure Frontend Environment Variables (API URL)](#64-configure-frontend-environment-variables-api-url)
    * [6.5 Run the Frontend](#65-run-the-frontend)

---

## 1. Project Overview

This project demonstrates a full-stack task management solution. The backend provides secure RESTful APIs for task CRUD operations and user authentication, while the Angular frontend offers an intuitive and responsive user interface.

## 2. Features

**Backend:**
* RESTful APIs for Task CRUD (Create, Read, Update, Delete) operations.
* MSSQL Express database using Entity Framework Core (Code-First approach).
* Basic username/password authentication for API endpoints.
* Centralized custom exception handling.
* Secure handling of sensitive data (like default user credentials) using .NET User Secrets.

**Frontend:**
* User-friendly interface to display, add, edit, delete, and mark tasks as completed.
* Simple username/password authentication.
* Side-by-side view for task list and add/update form.
* Responsive design for desktop and mobile devices.
* Task filtering by search term and completion status.
* Task sorting by creation date, title, and completion status (ascending/descending).
* User feedback messages for operations.
* Environment-based configuration for API URLs to avoid hardcoding.

## 3. Technologies Used

**Backend:**
* .NET 8.0
* ASP.NET Core Web API
* Entity Framework Core 8.0.x
* Microsoft SQL Server Express
* C#

**Frontend:**
* Angular v17
* Angular CLI 17.3.17
* Node.js 20.6.1
* TypeScript
* HTML5, CSS3

## 4. Prerequisites

Before you begin, ensure you have the following installed on your system:

* **Visual Studio 2022:** - Select "ASP.NET and web development" workload during installation.
* **SQL Server Express:** Download and install SQL Server Express.
* **SQL Server Management Studio (SSMS):** Download and install SSMS to manage your SQL Server databases.
* **Node.js & npm:** Highly recommended to use a Node Version Manager (NVM) to easily switch Node.js versions.
    * **For Windows:** [nvm-windows](https://github.com/coreybutler/nvm-windows/releases)
    * **For macOS/Linux:** [nvm.sh](https://nvm.sh/)
    * Ensure you have Node.js **v18 or v20** installed and active.
* **Angular CLI 17.3.17:** (See Frontend Setup for installation instructions).
* **Git:** For cloning the repository.

---

## 5. Backend Setup (.NET Web API)

### 5.1 Clone the Repository
First, clone this repository to your local machine:
git clone <repository_url>
cd TaskManagementApp # Or whatever your main project folder is named

Navigate into the backend project folder:
cd Backend/TaskManagementApi

### 5.2 Configure Database Connection
The database connection string is configured in appsettings.json.
    1. Open Backend/TaskManagementApi/appsettings.json.
    2. Locate the ConnectionStrings section:
                {
                  "ConnectionStrings": {
                    "DefaultConnection": "Server=server_name;Database=DB_name;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
                  },
                  // ... rest of config
                }
    3. Adjust the Server value if your SQL Server Express instance name is different from localhost\SQLEXPRESS (e.g., (localdb)\MSSQLLocalDB). The Database name is TaskDB. Trusted_Connection=True uses Windows Authentication.

### 5.3 Database Migrations & Seeding
This project uses Entity Framework Core's Code-First approach. The database schema and a default user (admin) will be created and seeded automatically.
1. Open the Solution in Visual Studio 2022.
        Open TaskManagementApi.sln.
2. Ensure NuGet Packages are Restored.
        Right-click on the TaskManagementApi project in Solution Explorer -> "Manage NuGet Packages..." -> "Restore" (if needed).
3. Configure Default User Credentials (Securely):
        The backend seeds a default admin user if the database is empty. Its credentials are NOT stored in source control.
        Open your terminal/command prompt and navigate to the Backend/TaskManagementApi directory (where your .csproj file is).
        Initialize .NET User Secrets (if not already initialized):\
                dotnet user-secrets init
        Set the default username and password for testing:
                dotnet user-secrets set "SeedUser:Username" "YourUserName"
                dotnet user-secrets set "SeedUser:Password" "YourPassword"
        You can verify by running dotnet user-secrets list.
4. Perform Database Migrations:
        Open the Package Manager Console in Visual Studio (Tools > NuGet Package Manager > Package Manager Console).
        Ensure TaskManagementApi is selected as the "Default project".
        Run "Database Update": This command will create the database (if it doesn't exist), apply all migrations, and run the seeding logic to add the admin user.
        Verify Seeding: Open SSMS, refresh your "Databases" node, navigate to the DB -> Tables -> dbo.Users. Right-click and "Select Top 1000 Rows" to confirm the admin user is present.

### 5.4 Run the Backend
In Visual Studio, set TaskManagementApi as the startup project. Press F5 to run the application in debug mode. A console window will open, and Swagger UI will launch in your browser.

### 5.5 Backend API Endpoints (Swagger UI)
Example URL: https://localhost:7000/swagger.
Authentication:
        To test the API endpoints, click the "Authorize" button (or the lock icon next to the endpoint).
        Select "Basic Auth".
        Enter the SeedUser:Username (admin) and SeedUser:Password (SecureP@ssw0rd!) that you configured in user secrets.
        Click "Authorize" and then "Close".
        You can now "Try it out" and "Execute" the API calls (GET, POST, PUT, DELETE for /api/Tasks).

### 6. Frontend Setup (Angular)

### 6.1 Node.js Version Management
Ensure you are using a Node.js version compatible with Angular v17 (Node.js v18 or v20). Use a Node Version Manager (like nvm or nvm-windows) if you need to switch versions.

### 6.2 Install Angular CLI
Navigate to the Frontend/TaskManagementApp directory (or wherever your angular.json is located).
Install Angular CLI 17. It's recommended to use npx to ensure the correct version without global conflicts:
    cd Frontend/TaskManagementApp # If not already there
nvm use 20 # (If using nvm, to ensure Node 20 is active)
Install Angular CLI 17 locally for this project:
    npm install --save-dev @angular/cli@17

### 6.3 Install Dependencies
From the Frontend/TaskManagementApp directory, install all project dependencies:
    npm install

### 6.4 Configure Frontend Environment Variables (API URL)
To avoid hardcoding the backend API URL and its port, Angular uses environment files.
    Open Frontend/TaskManagementApp/src/environments/environment.ts:
    This file is used when you run ng serve (development).
        export const environment = {
          production: false,
          apiUrl: 'https://localhost:7000/api' // Your backend API base URL
        };

Ensure https://localhost:7000 matches the base URL and port your .NET backend is running on (check Visual Studio's launch settings or the Swagger UI URL).

### 6.5 Run the Frontend
Ensure your Backend API is running (as described in 5.4 Run the Backend).
From your Frontend/TaskManagementApp directory, run the Angular development server:
    ng serve

This will compile your Angular application and open it in your default web browser.
