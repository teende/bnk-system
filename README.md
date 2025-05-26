# Banking Platform

Modern banking platform built with .NET Core following Clean Architecture principles.

## Project Structure

```
banking/
├── src/
│   ├── Banking.Common/           # Common utilities and shared code
│   ├── Banking.Gateway/         # API Gateway
│   └── Banking.Services.Main/   # Main banking service
├── tests/                       # Test projects
└── docs/                        # Documentation
```

## Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or later
- SQL Server 2019 or later

### Installation

1. Clone the repository
```bash
git clone https://github.com/your-username/banking-platform.git
```

2. Restore dependencies
```bash
dotnet restore
```

3. Build the solution
```bash
dotnet build
```

4. Run the application
```bash
dotnet run --project src/Banking.Services.Main/Banking.Services.Main.Api
```

## Development

### Branching Strategy

- `main` - Production branch
- `develop` - Development branch
- `feature/*` - Feature branches
- `bugfix/*` - Bug fix branches
- `release/*` - Release preparation branches

### Commit Convention

Commits should follow the format:
```
BNK-{number}: {type} {description}
```

Types:
- feat: New feature
- fix: Bug fix
- docs: Documentation changes
- style: Code style changes
- refactor: Code refactoring
- test: Adding or modifying tests
- chore: Maintenance tasks

## License

This project is licensed under the MIT License - see the LICENSE file for details. 