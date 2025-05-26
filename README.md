# Banking Platform

## Description
Modern banking platform built with .NET 8 and microservices architecture.

## Technologies
- .NET 8
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Docker
- RabbitMQ
- Redis
- gRPC
- MediatR
- CQRS
- Event Sourcing
- DDD

## Project Structure
```
banking-platform/
├── src/
│   ├── BankingPlatform.API/           # API Gateway
│   ├── BankingPlatform.Auth/          # Authentication Service
│   ├── BankingPlatform.Accounts/      # Account Management Service
│   ├── BankingPlatform.Transactions/  # Transaction Service
│   ├── BankingPlatform.Notifications/ # Notification Service
│   └── BankingPlatform.Common/        # Common Components
├── tests/
│   ├── BankingPlatform.API.Tests/
│   ├── BankingPlatform.Auth.Tests/
│   ├── BankingPlatform.Accounts.Tests/
│   ├── BankingPlatform.Transactions.Tests/
│   └── BankingPlatform.Notifications.Tests/
└── docs/                              # Documentation
```

## Requirements
- .NET 8 SDK
- Docker
- Docker Compose
- PostgreSQL 15+

## Installation and Setup
1. Clone the repository:
```bash
git clone https://github.com/your-username/banking-platform.git
cd banking-platform
```

2. Run with Docker Compose:
```bash
docker-compose up -d
```

## Configuration
Main settings are located in `appsettings.json` files of each service.

## API Documentation
API documentation is available at `/swagger` after application startup.

## Commit Convention
[Commit Convention](https://www.conventionalcommits.org/en/v1.0.0/)

## License
This project is licensed under the MIT License - see the LICENSE file for details. 