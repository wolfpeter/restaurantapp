RestaurantApp - Food Ordering System
This project is a RESTful web service that implements the core functionality of a food ordering system. It allows users to register, log in, browse restaurants, and place orders. Restaurant owners can manage their incoming orders.

Tech Stack
The project is built on a modern technology stack:

Backend: C#

Framework: ASP.NET Core 8 Web API

Database: Entity Framework Core 8 with SQLite

Authentication: JWT (JSON Web Tokens)

Real-time Communication: SignalR

Testing: xUnit

API Documentation: Swagger (OpenAPI)

Containerization: Docker

Core Features
User Management: Registration and login with User and Restaurant roles.

Restaurant Browsing: List restaurants and their menus.

Order Management: Customers can place orders, and restaurants can update the order status.

Real-time Notifications: Customers receive instant updates on their order status changes via SignalR.

Automatic Database Management: EF Core migrations are applied automatically on startup.

Seed Data: The application seeds the database with a sample restaurant and a manager user on its first run.

How to Run the Application
You can run the project in two ways.

1. Using the .NET CLI
Prerequisites:

.NET 8 SDK

Steps:

Clone the repository.

Navigate to the RestaurantApp project folder in your terminal.

Run the following command:

Bash

dotnet run
The application will start, and the database (RestaurantApp.db) will be created automatically in the project folder.

The API will be available at https://localhost:7048, and the Swagger UI can be accessed at https://localhost:7048/swagger.

2. Using Docker
Prerequisites:

Docker Desktop

Steps:

Clone the repository.

Navigate to the solution's root directory in your terminal.

Build the Docker image:

Bash

docker build -t restaurant-app .
Run the container:

Bash

docker run -p 8080:8080 -p 8081:8081 restaurant-app
The API will be available at http://localhost:8080, and the Swagger UI can be accessed at http://localhost:8080/swagger.

Testing
The project includes unit tests for the service layer logic. You can run the tests from the solution's root directory:

Bash

dotnet test