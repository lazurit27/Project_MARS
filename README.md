# 🚗 Project MARS (Automotive Service & Multi-Tenant Management System)

Project MARS is a robust, production-ready backend service designed for automotive management, maintenance tracking, and multi-tenant administration. Built following modern software architecture principles, it features secure JWT-based authentication, role-based access control, and a clean separation of concerns.

---

## 🛠️ Tech Stack & Architecture

* **Framework:** ASP.NET Core 8.0 Web API
* **Architecture Style:** Clean Architecture
* **Database & ORM:** SQL Server, Entity Framework Core
* **Authentication & Identity:** ASP.NET Core Identity, JWT Bearer Tokens (Access & Refresh Tokens)
* **Documentation:** Swagger / OpenAPI with JWT security definitions
* **Version Control:** Git & GitHub

---

## 📂 Project Structure

The solution follows **Clean Architecture** patterns, divided into layers:
* **`Project_MARS.WebApi`**: Presentation layer containing controllers, middleware (request logging, exception handling), and dependency injection setups.
* **`Project_MARS.Application`**: Core business logic, DTOs, interfaces, and service implementations.
* **`Project_MARS.Infrastructure`**: Database contexts, Entity Framework configurations, repository patterns, identity setups, and security token generators.

---

## 🚀 Key Features

* **Secure Authentication & Authorization:** 
  * User registration and login with secure password hashing via ASP.NET Core Identity.
  * JWT Access Token generation with customizable lifetimes and Refresh Token rotation flows.
  * Role-based and claim-based authorization (`[Authorize]`, custom roles like Admin/User).
* **Automotive Management:** Track vehicle specifications (VIN, brand, model, year), user ownership associations, and service schedules.
* **API Standardization & Validation:** DTO-based data transfer utilizing DataAnnotations validation attributes.
* **Robust Logging & Middleware:** Custom request logging middleware to monitor incoming HTTP requests and track server responses.

---

## ⚙️ Getting Started

### Prerequisites
* .NET 8.0 SDK or later
* SQL Server (LocalDB or Express)

### Installation & Running

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/your-username/Project_MARS.git](https://github.com/your-username/Project_MARS.git)
   cd Project_MARS
