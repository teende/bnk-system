# Banking Platform

A modern banking platform developed using microservices architecture and best development practices. This platform provides secure and efficient banking operations with a focus on scalability and maintainability.

## 🚀 Features

- User management and authentication
- Card management and operations
- Admin dashboard and controls
- Secure transactions with end-to-end encryption
- Comprehensive financial operations monitoring
- High-level security with multi-factor authentication
- Transaction history and reporting
- API integration capabilities

## 🛠 Technologies

- Backend Services:
  - .NET Core
  - C#
  - Microservices Architecture
  - Clean Architecture
  - CQRS Pattern
  - MediatR
  - Entity Framework Core
  - SQL Server
  - Redis for caching
  - RabbitMQ for message queuing
  - Serilog for logging
  - Swagger/OpenAPI for API documentation
  - AutoMapper for object mapping
  - FluentValidation for validation
- Database: SQL Server
- Additional tools:
  - Docker for containerization
  - Jenkins for CI/CD
  - xUnit for testing
  - Moq for mocking
  - SonarQube for code quality
  - Git for version control
  - Postman for API testing

## 📋 Requirements

- .NET Core 6.0 or higher
- SQL Server 2019 or higher
- Redis 6.x or higher
- Docker 20.x or higher (optional)
- 4GB RAM minimum
- 10GB free disk space

## 🔧 Installation

1. Clone the repository:
```bash
git clone https://github.com/your-username/banking-platform.git
```

2. Navigate to the project directory:
```bash
cd banking-platform
```

3. Install dependencies:
```bash
dotnet restore
```

4. Configure the project:
   Copy the example configuration file:
   ```bash
   cp appsettings.example.json appsettings.json
   ```
   Then, edit `appsettings.json` to set up your environment. Key configurations to update include:
   - `ConnectionStrings:DefaultConnection`: Your SQL Server connection string.
   - `RedisCacheSettings:ConnectionString`: Your Redis connection string.
   - `RabbitMQSettings:HostName`, `RabbitMQSettings:UserName`, `RabbitMQSettings:Password`: Your RabbitMQ connection details.
   - `JwtSettings:Key`, `JwtSettings:Issuer`, `JwtSettings:Audience`: Settings for JWT authentication.

   Ensure all necessary services (SQL Server, Redis, RabbitMQ) are running and accessible.


## 🚀 Running the Project

### For Development

To run the main banking service for development purposes:
```bash
dotnet run --project Banking.Services.Main
```
This will start the application, and you can typically access it at `https://localhost:5001` or `http://localhost:5000`. Check the console output for the exact URLs.

### For Production

To run the application in a production-like environment:
1. Publish the main service:
   ```bash
   dotnet publish Banking.Services.Main -c Release -o ./publish_output
   ```
   This command compiles and packages the application into the `publish_output` directory.
2. Run the published application:
   ```bash
   dotnet ./publish_output/Banking.Services.Main.dll
   ```
   Ensure your `appsettings.Production.json` (if used) or `appsettings.json` is configured correctly for the production environment.

### Using Docker

If you have Docker and Docker Compose installed, you can run the entire platform using:
```bash
docker-compose up -d
```
This will build the necessary images and start all services defined in the `docker-compose.yml` file.
After the services are up, you can typically access the main API gateway or individual services based on the ports defined in your Docker Compose configuration. Refer to the `docker-compose.yml` for specific port mappings.

## 📖 API Documentation

This project uses Swagger (OpenAPI) to provide interactive API documentation.

### Accessing Swagger UI

Once the application is running (either in development or via Docker), you can access the Swagger UI in your browser. By default, it is often available at:

- `/swagger`
- `/swagger/index.html`

For example, if your application is running locally on port 5001, you would navigate to `https://localhost:5001/swagger`.

The Swagger UI allows you to:
- View all available API endpoints.
- See details for each endpoint, including request parameters, request body schemas, and response schemas.
- Execute API requests directly from the browser to test the endpoints.

This is an invaluable tool for understanding and interacting with the platform's APIs.

## 🔐 Security

The project implements the following security measures:
- JWT-based authentication
- Role-based access control (RBAC)
- HTTPS encryption
- SQL injection prevention
- XSS protection
- Rate limiting
- Input validation
- Secure password hashing
- Regular security audits
- Microservices security with API Gateway
- CORS policy configuration
- Request/Response encryption
- Audit logging

## 🤝 Contributing

We welcome contributions to the project! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Make your changes
4. Run tests (`dotnet test`)
5. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
6. Push to the branch (`git push origin feature/AmazingFeature`)
7. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [.NET Core](https://dotnet.microsoft.com/) - Development platform
- [SQL Server](https://www.microsoft.com/sql-server) - Database
- [Redis](https://redis.io/) - Caching
- [RabbitMQ](https://www.rabbitmq.com/) - Message broker
- [Docker](https://www.docker.com/) - Containerization platform
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
- [MediatR](https://github.com/jbogard/MediatR) - Mediator pattern implementation
- [Serilog](https://serilog.net/) - Logging
- [Swagger/OpenAPI](https://swagger.io/) - API documentation framework
