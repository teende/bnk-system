# 🏦 Banking Platform

A modern banking platform developed using microservices architecture and best development practices. This platform provides secure and efficient banking operations with a focus on scalability and maintainability.

![Architecture Overview](docs/images/architecture-preview.gif)

---

## 🚀 Features

- ✅ User management and authentication
- 💳 Card management and operations
- 📊 Admin dashboard and controls
- 🔐 Secure transactions with end-to-end encryption
- 📈 Financial operations monitoring
- 🔒 Multi-factor authentication
- 🧾 Transaction history and reporting
- 🔗 API integration ready

---

## 🛠 Technologies

- **Backend**:
  - .NET Core, C#
  - Clean Architecture, CQRS, MediatR
  - Entity Framework Core
  - Redis, RabbitMQ, SQL Server
  - Serilog, Swagger/OpenAPI, AutoMapper
  - FluentValidation
- **Tools**:
  - Docker, Jenkins, xUnit, Moq, SonarQube
  - Git, Postman

---

## 📋 Requirements

- .NET Core 6.0+
- SQL Server 2019+
- Redis 6.x+
- Docker (optional)
- 4GB RAM, 10GB disk space

---

## 🧩 Installation

```bash
git clone https://github.com/your-username/banking-platform.git
cd banking-platform
dotnet restore
```

### 🔧 Configuration

```bash
cp appsettings.example.json appsettings.json
```

Edit `appsettings.json`:

- Database connection strings
- Redis / RabbitMQ credentials
- JWT secret settings

---

## 🚀 Running the Project

### 🛠 Development

```bash
dotnet run --project Banking.Services.Main
```

Visit: `https://localhost:5001/swagger`

### 📦 Production

```bash
dotnet publish Banking.Services.Main -c Release -o ./publish_output
dotnet ./publish_output/Banking.Services.Main.dll
```

### 🐳 Docker

```bash
docker-compose up -d
```

---

## 🎬 UI Preview *(Coming Soon)*

![UI Demo](docs/images/ui-preview.gif)

---

## 🔍 API Explorer

![Swagger UI](docs/images/swagger-demo.gif)

Swagger allows you to:
- View endpoints and models
- Try requests with real data
- Explore the contract-based API

---

## 🛡️ Security Highlights

- JWT authentication
- Role-based access (RBAC)
- HTTPS encryption
- SQL injection & XSS protection
- Rate limiting
- Secure password hashing (BCrypt)
- Audit logging

---

## 🤝 Contributing

1. Fork ➜ `feature/AmazingFeature`
2. Code ➜ `dotnet test`
3. PR ➜ review ➜ merge ✅

---

## 📖 License

Licensed under the [MIT License](LICENSE).

---

## 🙏 Acknowledgments

- [.NET Core](https://dotnet.microsoft.com/)
- [Redis](https://redis.io/)  •  [RabbitMQ](https://www.rabbitmq.com/)
- [Docker](https://www.docker.com/)  •  [Swagger](https://swagger.io/)
- [Serilog](https://serilog.net/)  •  [EF Core](https://docs.microsoft.com/en-us/ef/core/)

