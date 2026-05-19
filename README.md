# Web App Template

This repository is a starter template for rapidly building responsive full-stack web applications with a shared backend architecture, reusable business logic, REST APIs, and deployment paths for local, test/preproduction, and production environments.

## Goals

- Fast bootstrap for new applications.
- Responsive web UI that works on smartphone, tablet, and laptop.
- HTML-first baseline that still functions with JavaScript disabled.
- OIDC/OAuth authentication with RBAC.
- REST API for integrations and future clients.
- EF Core code-first migrations with PostgreSQL.
- Clean modular boundaries so business logic can be reused by future iOS and Android apps.
- Hosting flexibility for Netlify, Supabase, Docker, and other environments.

## Recommended stack

- Backend: ASP.NET Core.
- Data access: Entity Framework Core.
- Database: PostgreSQL.
- Authentication: OIDC/OAuth with role-based authorization.
- Web app: componentized, responsive frontend with progressive enhancement.
- Integration surface: REST + webhooks.
- Deployment: local Docker Compose, test/preprod environment, production environment.

## Repository layout

```text
/
├─ README.md
├─ ARCHITECTURE.md
├─ SKILLS.md
├─ .env.example
├─ docker-compose.yml
├─ docker-compose.override.yml
├─ docker-compose.test.yml
├─ /docs
├─ /deploy
├─ /scripts
└─ /src
   ├─ Company.Product.Domain
   ├─ Company.Product.Application
   ├─ Company.Product.Infrastructure
   ├─ Company.Product.Contracts
   ├─ Company.Product.Api
   ├─ Company.Product.Web
   ├─ Company.Product.Web.Components
   ├─ Company.Product.Worker
   └─ Company.Product.Tests
```

## Architecture summary

The template should be structured as a layered system:

- `Domain`: pure business entities, value objects, invariants, and domain services.
- `Application`: use cases, orchestration, interfaces, and authorization-aware workflows.
- `Infrastructure`: EF Core, PostgreSQL, OIDC integration, external services, and persistence details.
- `Contracts`: API DTOs, webhook payloads, shared request/response models, and integration contracts.
- `Api`: versioned REST endpoints, webhook endpoints, health checks, and integration entry points.
- `Web`: responsive HTML-first front end with progressive enhancement.
- `Web.Components`: reusable UI components and design primitives.
- `Worker`: background jobs, scheduled tasks, outbox processing, and async integration handling.

This separation is important because it keeps the business logic reusable for future mobile applications while allowing the web app and API to evolve independently.

## Frontend approach

The web layer should be responsive and accessible by default. Use HTML-first rendering for core flows so the site still works without JavaScript, then progressively enhance with JavaScript only where it improves the experience.

Recommended front-end principles:

- Use semantic HTML.
- Keep forms server-compatible.
- Ensure navigation and critical actions work without JS.
- Componentize the UI so screens can be recomposed quickly.
- Test desktop, tablet, and mobile layouts.
- Add automated tests for accessibility, rendering, and core user flows.

## Authentication and RBAC

Use OIDC/OAuth for user authentication and define app roles and policies explicitly in code.

Recommended roles:

- `Admin`
- `Editor`
- `User`
- `ServiceIntegration`

Recommended auth rules:

- Separate interactive user auth from machine/integration auth.
- Keep policy definitions in a single place.
- Seed development roles and a bootstrap admin path.
- Support external identity providers.
- Make authorization decisions testable through application services.

## API and integrations

The API is a first-class integration surface and should be versioned from day one.

Recommended API principles:

- Use `/api/v1/...` route versioning.
- Keep request and response contracts in `Contracts`.
- Support idempotency for important operations.
- Add webhook receiver endpoints with signature validation.
- Publish OpenAPI/Swagger docs.
- Separate read models from write models where useful.
- Keep integration endpoints stable and documented.

## Database strategy

Use PostgreSQL and EF Core code-first migrations.

Recommended rules:

- Keep EF Core types out of the `Domain` layer.
- Put all persistence configuration in `Infrastructure`.
- Use migrations as the source of truth for schema change.
- Keep seed data deterministic.
- Validate migration scripts in CI.
- Apply migrations explicitly during deployment.

## Local development

Local development should run entirely from the repository with minimal setup.

Suggested local stack:

- API
- Web app
- PostgreSQL
- Optional worker

Recommended local workflow:

1. Copy `.env.example` to `.env`.
2. Start the local stack with Docker Compose.
3. Run database migrations.
4. Seed data.
5. Start the web and API services.
6. Run automated tests.

## Test and preproduction

Use a separate environment for test and preproduction that mirrors production as closely as possible.

Recommended practices:

- Separate configuration and secrets from local development.
- Use a dedicated PostgreSQL instance.
- Run migrations before app startup or as a release step.
- Use the same build artifact promoted through environments.
- Run smoke tests after deployment.

## Production

Production should be predictable and reversible.

Recommended practices:

- Build once, deploy many.
- Use immutable artifacts.
- Keep secrets outside the repo.
- Add structured logging, metrics, and health checks.
- Use rollback-friendly deployment methods.
- Keep application startup fast and deterministic.

## Hosting strategies

### Option 1: Netlify for web, separate API

This is a practical low-friction setup for many projects.

- Host the frontend on Netlify.
- Host the API in containers or on a server platform.
- Use Supabase or another managed Postgres provider.
- Keep the API as the integration and business logic entry point.

Netlify is a sensible choice for the frontend because it supports static site deployment, custom domains, SSL, and simple CI-driven publishing.

### Option 2: Fully containerized deployment

This is the most portable and usually the cleanest long-term option.

- Package the API, web app, and worker as containers.
- Use Docker Compose for local and test environments.
- Deploy to a container host in production.
- Use managed PostgreSQL separately.

### Option 3: Supabase-backed database and auth

Supabase is a useful low-cost starting point for Postgres and auth.

- Use Supabase Postgres for hosted data storage.
- Use Supabase Auth if it fits the identity model.
- Keep your application architecture independent of Supabase-specific features.
- Use it as infrastructure, not as the core business logic layer.

## Mobile reuse plan

The architecture should assume that future iOS and Android apps will reuse backend logic and contracts.

Recommended approach:

- Keep domain logic in reusable .NET libraries.
- Keep UI code separate from business rules.
- Share DTOs and validation contracts where practical.
- Keep APIs stable and versioned.
- Expose mobile-friendly endpoints from the same backend.

Do not couple mobile planning to the web UI component model. Reuse the business logic and contracts, not the browser-specific presentation layer.

## Sample starter feature set

The first template implementation should include:

- Landing page.
- Login and logout.
- Role-protected dashboard.
- One sample CRUD entity.
- One sample webhook receiver.
- Health check endpoint.
- Background worker example.
- Seed data and admin bootstrap.
- At least one integration test and one UI test.

## Commands

Add project-specific commands here once the solution is scaffolded, including local startup, migration, seed, and test commands.

## Documentation files

- `ARCHITECTURE.md`: canonical system design and dependency rules.
- `SKILLS.md`: instructions for future coding agents.
- `.env.example`: required environment variables.
- `docs/`: deployment, integration, and operational notes.
