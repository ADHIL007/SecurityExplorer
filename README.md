# Security Explorer

> A plugin-based security testing framework for ASP.NET Core applications.

Security Explorer is an extensible security testing framework that integrates directly into ASP.NET Core applications, similar to how Swagger integrates API documentation.

Its goal is to provide developers with a centralized dashboard for discovering application endpoints, executing automated security test suites, and identifying common security vulnerabilities during development.

The framework is designed around a modular plugin architecture, allowing built-in and third-party security analyzers to be added without modifying the core engine.

---

## Features

- Plugin-based architecture
- Automatic plugin discovery
- ASP.NET Core middleware integration
- Interactive dashboard (`/securityexplorer`)
- Extensible security test pipeline
- Dependency Injection support
- Modular architecture following SOLID principles
- Easy integration into existing ASP.NET Core applications

---

## Planned Features

- Endpoint discovery
- Authentication testing
- Authorization testing
- SQL Injection detection
- Cross-Site Scripting (XSS)
- CSRF testing
- Security header analysis
- JWT validation
- CORS configuration analysis
- Sensitive information exposure detection
- OWASP Top 10 security checks
- HTML dashboard
- PDF & JSON report generation
- Plugin SDK
- CLI support
- CI/CD integration
- Visual Studio integration

---

## Architecture

```
Application
        │
        ▼
SecurityExplorer.AspNetCore
        │
        ▼
SecurityExplorer.Core
        │
        ▼
Plugin Loader
        │
        ▼
Security Plugins
        │
        ▼
Security Reports
```

---

## Project Structure

```
SecurityExplorer
│
├── src/
│   ├── SecurityExplorer.AspNetCore
│   ├── SecurityExplorer.Core
│   ├── SecurityExplorer.UI
│   └── SecurityExplorer.Abstractions
│
├── tests/
│
├── samples/
│
└── docs/
```

---

## Installation

> Coming soon

```bash
dotnet add package SecurityExplorer.AspNetCore
```

---

## Usage

```csharp
builder.Services.AddSecurityExplorer();

var app = builder.Build();

app.UseSecurityExplorer();
```

Navigate to

```
/securityexplorer
```

---

## Plugin Development

Plugins simply implement the `ISecurityTest` interface.

```csharp
public class SqlInjectionTest : ISecurityTest
{
    public string Name => "SQL Injection";

    public async Task<TestResult> ExecuteAsync(TestContext context)
    {
        // Security test implementation
    }
}
```

The framework automatically discovers and registers plugins during application startup.

---

## Roadmap

- [x] Solution Architecture
- [x] Core Engine
- [x] Dependency Injection
- [x] Plugin Discovery
- [ ] Endpoint Discovery
- [ ] Dashboard UI
- [ ] Authentication Suite
- [ ] SQL Injection Suite
- [ ] Report Generator
- [ ] Plugin SDK
- [ ] CLI
- [ ] Documentation

---

## Compatibility

| Platform | Status |
|----------|--------|
| .NET 9 | ✅ Supported |
| ASP.NET Core 9 | ✅ Supported |
| .NET 8 | 🚧 Planned |
| ASP.NET Core 8 | 🚧 Planned |
| Linux | 🚧 Planned |
| Windows | 🚧 Planned |
| macOS | 🚧 Planned |

---

## License

This project is licensed under the LICENSE-APACHE-2.0.

See the LICENSE file for details.
