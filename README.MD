# 🍔 RestaurantApp - Food Ordering System

A RESTful web service that implements the core functionality of a food ordering system. It allows users to register, log in, browse restaurants, and place orders. Restaurant owners can manage their incoming orders.

---

## ✨ Core Features

* **User Management:** Registration and login with `User` and `Restaurant` roles.
* **Restaurant Browsing:** List restaurants and their menus.
* **Order Management:** Customers can place orders, and restaurants can update the order status.
* **Real-time Notifications:** Customers receive instant updates on their order status changes via SignalR.
* **Automatic Database Management:** EF Core migrations are applied automatically on startup.
* **Seed Data:** The application seeds the database with a sample restaurant and a manager user on its first run.

---

## 🛠️ Tech Stack

* **Backend:** C#
* **Framework:** ASP.NET Core 8 Web API
* **Database:** Entity Framework Core 8 with SQLite
* **Authentication:** JWT (JSON Web Tokens)
* **Real-time Communication:** SignalR
* **Testing:** xUnit
* **API Documentation:** Swagger (OpenAPI)
* **Containerization:** Docker

---

## 🚀 Getting Started

You can run the project in two ways.

### 1. Using the .NET CLI

**Prerequisites:**
* .NET 8 SDK

**Steps:**
1.  Clone the repository.
2.  Navigate to the `RestaurantApp` project folder in your terminal.
3.  Run the following command:
    ```bash
    dotnet run
    ```
4.  The application will start, and the database (**`RestaurantApp.db`**) will be created automatically.

The API will be available at `https://localhost:7048`, and the Swagger UI can be accessed at `https://localhost:7048/swagger`.

### 2. Using Docker

**Prerequisites:**
* Docker Desktop

**Steps:**
1.  Clone the repository.
2.  Navigate to the solution's root directory in your terminal.
3.  Build the Docker image:
    ```bash
    docker build -t restaurant-app .
    ```
4.  Run the container:
    ```bash
    docker run -p 8080:8080 -p 8081:8081 restaurant-app
    ```

The API will be available at `http://localhost:8080`, and the Swagger UI can be accessed at `http://localhost:8080/swagger`.

---

## 🧪 Testing

The project includes unit tests for the service layer logic. You can run the tests from the solution's root directory:
```bash
dotnet test