# Documentation Index

Complete documentation for the Company.Product full-stack template.

---

## 📋 Quick Links

- **[Architecture](ARCHITECTURE.md)** - System design, layers, and principles
- **[Deployment](deployment.md)** - Complete deployment guide for all environments
- **[Hosting (Minimal Cost)](hosting-minimal-cost.md)** - $10-25/month hosting setup

---

## 🏗️ Architecture & Design

### [ARCHITECTURE.md](ARCHITECTURE.md)
Complete system architecture including:
- Clean architecture layers (Domain, Application, Infrastructure)
- Project structure and dependencies
- Mobile reuse strategy
- Integration and webhook patterns
- Design principles and boundaries

**Key Topics:**
- Domain-driven design
- Dependency rules
- Layer responsibilities
- Mobile client support
- Webhook integration patterns

---

## 🚀 Deployment & Hosting

### [deployment.md](deployment.md)
Comprehensive deployment guide covering:
- Local development setup
- Development environment
- Pre-production environment
- Production deployment
- Common tasks and troubleshooting

**Environments Covered:**
- Local (Docker Compose)
- Development (Docker or manual)
- Pre-Production (staging)
- Production (with rollback procedures)

### [hosting-minimal-cost.md](hosting-minimal-cost.md)
Budget-friendly hosting options ($10-25/month):
- Supabase (PostgreSQL + Storage)
- Railway (API, Worker, Web)
- Netlify/Vercel alternatives
- Step-by-step setup guides
- Cost breakdown and optimization tips

**Recommended Stack:**
- Database: Supabase (Free tier)
- API/Worker/Web: Railway ($15/month)
- Blob Storage: Supabase Storage (Free)
- Total: ~$15/month

---

## 🔐 Authentication & Security

### [auth-providers.md](auth-providers.md)
OAuth provider configuration:
- Google OAuth setup
- Microsoft OAuth setup
- Apple Sign-In setup
- JWT configuration
- Security best practices

**Providers Supported:**
- Google
- Microsoft
- Apple (documented, implementation ready)

---

## 💾 Data & Storage

### [blob-storage.md](blob-storage.md)
File storage configuration:
- Azure Blob Storage setup
- Local file system fallback
- IBlobService interface
- Upload/download patterns
- Security considerations

**Storage Options:**
- Azure Blob Storage (production)
- Local file system (development)
- Supabase Storage (minimal cost option)

---

## 💳 Payments & Billing

### [stripe-payments.md](stripe-payments.md)
Stripe integration design (ready to implement):
- Subscription tiers (Free, Basic, Pro)
- Domain model (Subscription entity)
- Webhook events handling
- Checkout and billing portal
- Database schema

**Status:** Fully designed, implementation pending

---

## 📊 Logging & Monitoring

### [logging.md](logging.md)
Structured logging with Serilog:
- Serilog configuration
- Seq integration
- Log levels and filtering
- Correlation IDs
- Best practices

**Stack:**
- Serilog for structured logging
- Seq for log aggregation (local/dev)
- Papertrail for production (free tier)

---

## 🌍 Internationalization

### [i18n.md](i18n.md)
Localization and internationalization:
- Resource files setup
- Culture configuration
- Localization in Razor Pages
- Localization in API
- Supported cultures

**Features:**
- Multi-language support
- Culture-specific formatting
- Resource file organization
- Accept-Language header support

---

## 📚 Additional Resources

### Project Structure

```
Company.Product/
├── src/
│   ├── Company.Product.Domain/          # Business entities
│   ├── Company.Product.Contracts/       # DTOs and shared models
│   ├── Company.Product.Application/     # Business logic
│   ├── Company.Product.Infrastructure/  # Data access & external services
│   ├── Company.Product.Api/             # REST API
│   ├── Company.Product.Web/             # Razor Pages web app
│   ├── Company.Product.Web.Components/  # Shared UI components
│   ├── Company.Product.Worker/          # Background service
│   └── Company.Product.Tests/           # Test suite
├── docs/                                # This documentation
├── .env.example                         # Environment template
├── docker-compose.yml                   # Local development
└── Company.Product.sln                  # Solution file
```

### Technology Stack

**Backend:**
- .NET 10.0
- ASP.NET Core (API & Web)
- Entity Framework Core
- PostgreSQL
- Serilog

**Frontend:**
- Razor Pages
- Progressive enhancement
- Responsive design

**Testing:**
- xUnit v3
- Moq
- FluentAssertions

**Infrastructure:**
- Docker & Docker Compose
- Azure Blob Storage
- Seq (logging)

---

## 🎯 Getting Started

### For New Developers

1. Read [ARCHITECTURE.md](ARCHITECTURE.md) to understand the system design
2. Follow [deployment.md](deployment.md#local-development) for local setup
3. Review this README for available features and documentation

### For Deployment

1. Choose your hosting strategy:
   - **Budget:** [hosting-minimal-cost.md](hosting-minimal-cost.md) ($10-25/month)
   - **Enterprise:** [deployment.md](deployment.md#production-environment)
2. Configure authentication: [auth-providers.md](auth-providers.md)
3. Set up logging: [logging.md](logging.md)
4. Configure storage: [blob-storage.md](blob-storage.md)

### For Feature Development

1. Review [ARCHITECTURE.md](ARCHITECTURE.md) for layer responsibilities
2. Follow clean architecture principles
3. Add tests for new features
4. Update relevant documentation

---

## 🔧 Common Tasks

### Local Development
```bash
# Start services
docker compose up -d

# Run migrations
dotnet ef database update --project src/Company.Product.Infrastructure

# Run API
cd src/Company.Product.Api && dotnet run

# Run Web
cd src/Company.Product.Web && dotnet run
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test src/Company.Product.Tests
```

### Building
```bash
# Build entire solution
dotnet build

# Build for release
dotnet build -c Release
```

---

## 📖 Documentation Standards

When adding new documentation:

1. **Keep it concise** - Focus on essential information
2. **Use examples** - Show code snippets and commands
3. **Link related docs** - Cross-reference other documentation
4. **Update this index** - Add new docs to the appropriate section
5. **Follow structure** - Use consistent formatting and headings

---

## 🤝 Contributing

When contributing to the template:

1. Follow the architecture principles in [ARCHITECTURE.md](ARCHITECTURE.md)
2. Maintain clean architecture boundaries
3. Add tests for new features
4. Update documentation
5. Ensure build succeeds with 0 warnings

---

## 📞 Support

For questions or issues:

1. Check relevant documentation first
2. Consult [ARCHITECTURE.md](ARCHITECTURE.md) for design decisions
3. Check deployment guides for environment-specific issues
4. Review this README for available features

---

## 📅 Last Updated

This documentation index was last updated: March 29, 2026

For the most current information, always refer to the individual documentation files.
