# eShop Backend API 🛒

A robust and scalable E-commerce Backend RESTful API built with **ASP.NET Core 8**. This project implements a complete shopping experience with a focus on clean architecture, security, and performance.

## 🚀 Features

### 👤 User Module
* **Authentication & Authorization:** Secure access using **JWT Tokens** and Identity Framework.
* **Profile Management:** Users can update their info (Address, Phone) and change passwords securely.
* **Shopping Cart:** Full cart functionality (Add, Remove, Update quantities).
* **Order System:** Transaction-based ordering system to ensure data integrity.
* **Review System:** Users can rate and review products they have purchased.

### 🛡️ Admin Dashboard
* **Inventory Management:** Complete CRUD operations for products and categories.
* **Order Tracking:** Ability to update order status (Pending, Shipped, Delivered, Cancelled).
* **Real-time Statistics:** Dashboard stats showing Total Revenue, Order counts, and Top Selling products.
* **User Management:** Ability to manage user accounts.

## 🛠️ Tech Stack
* **Framework:** .NET 8 (Web API)
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **Security:** JWT Authentication & ASP.NET Core Identity
* **Mapping:** AutoMapper
* **Documentation:** Swagger UI (OpenAPI)
* **Patterns:** Service Pattern, Repository-like logic, Middleware, Global Exception Handling.

## 📂 Architecture
The project follows a modular structure to ensure separation of concerns:
- **Controllers:** Handle HTTP requests and responses.
- **Services:** Contain the core business logic.
- **Data (DbContext):** Handles database connectivity and migrations.
- **DTOs:** Data Transfer Objects for secure and optimized data flow.
- **Middlewares:** Global exception handling and custom pipelines.

## 🔧 How to Run
1. Clone the repository.
2. Update the connection string in `appsettings.json`.
3. Run `Update-Database` in the Package Manager Console.
4. Press `F5` to run the project and explore the Swagger documentation.
