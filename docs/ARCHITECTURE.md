# Architecture

## Purpose

This repository is a reusable template for multiple future applications. The architecture is designed to reduce setup time, enforce good boundaries, and keep the backend business logic reusable for future mobile clients.

## Principles

- Prefer explicit boundaries over convenience.
- Keep domain logic framework-independent.
- Keep infrastructure replaceable.
- Keep the web experience responsive and accessible.
- Keep the API stable and versioned.
- Keep integrations observable and testable.
- Keep the solution easy to clone into new products.

## System layers

### Domain

Contains business entities, value objects, invariants, domain events, and domain services. It must not depend on ASP.NET Core, EF Core, or external services.

### Application

Contains use cases, orchestration, validation, authorization-aware workflows, and interfaces for external dependencies. It coordinates the domain without owning persistence or transport details.

### Infrastructure

Contains EF Core, PostgreSQL persistence, auth integration, external API clients, webhook transport details, file storage, email delivery, and other adapters.

### Contracts

Contains shared DTOs, API request and response types, webhook payloads, and integration contracts. These types should be suitable for use by web clients, mobile clients, and other systems.

### Api

Contains versioned REST endpoints, webhook endpoints, health checks, and integration-facing endpoints. It should be thin and delegate behavior to application services.

### Web

Contains the HTML-first web experience with progressive enhancement. It should function without JavaScript for core flows and still provide a polished responsive interface when JS is available.

### Web.Components

Contains reusable UI components, layout primitives, form elements, and style-system building blocks.

### Worker

Contains background jobs, scheduled tasks, event processing, and outbox handlers.

---

## Mobile Reuse Strategy

The template supports future Android and iOS clients by reusing backend business logic and contracts.

### Reusable Components

- **Domain logic** - Keep in framework-independent .NET libraries
- **Application services** - Orchestration and validation logic
- **API contracts** - Shared contracts project for DTOs
- **Authentication/authorization** - Reusable across platforms
- **External service abstractions** - IBlobService, IEmailService, etc.

### Mobile Integration Points

- REST API for all data operations
- JWT authentication for mobile clients
- Webhook handling for push notifications
- Shared validation rules via contracts
- Consistent error handling and responses

Mobile apps can be added later without changing core business rules or domain logic.

---

## Integration & Webhooks

Integration is a core requirement for this template.

### Requirements

- Versioned REST API
- Webhook receiver endpoints
- Signature verification for security
- Idempotent handlers where possible
- Reliable async processing through worker or outbox pattern

### Recommended Patterns

- **Accept inbound webhooks** in the API layer
- **Hand off processing** to application services
- **Persist event records** before processing for reliability
- **Log correlation IDs** for traceability
- **Use retries and dead-letter handling** for failed deliveries
- **Verify signatures** before processing payloads
- **Return 200 OK quickly** and process asynchronously

## Dependency rules

- `Domain` depends on nothing else in the solution.
- `Application` depends on `Domain` and contracts/interfaces only.
- `Infrastructure` depends on `Application` and `Domain`.
- `Api` depends on `Application`, `Infrastructure`, and `Contracts`.
- `Web` depends on `Application`, `Contracts`, and UI libraries only.
- `Worker` depends on `Application` and `Infrastructure`.

No layer should bypass these rules just for speed.

## Authentication model

Authentication uses OIDC/OAuth. The application should support external identity providers and should not hard-code one provider into business logic.

Authorization should be role-based and policy-based.

Recommended roles:

- `Admin`
- `Editor`
- `User`
- `ServiceIntegration`

Recommended policy categories:

- Admin-only operations.
- User-owned resource access.
- Integration scopes for machine-to-machine access.
- Environment-specific bootstrap policies for development and testing.

## Web model

The web app must remain usable with JavaScript disabled. Core navigation, forms, and content should work through server-rendered HTML or an equivalent no-JS path.

Progressive enhancement should be used for richer interactions such as instant validation, async updates, and component interactivity.

Responsive layout targets:

- Smartphone.
- Tablet.
- Laptop and desktop.

## API model

The API should be versioned and contract-driven.

Recommended conventions:

- Prefix routes with `/api/v1`.
- Keep read and write models explicit.
- Use standard HTTP verbs and status codes.
- Add webhook endpoints with signature verification.
- Add idempotency where external retries are expected.
- Publish OpenAPI documentation.

## Persistence model

Use PostgreSQL with EF Core code-first migrations.

Rules:

- Keep persistence concerns out of domain logic.
- Keep entity configuration in Infrastructure.
- Review migration SQL during release preparation.
- Seed data deterministically.
- Use explicit migration execution in deployment pipelines.

## Mobile reuse model

The architecture should make it possible to add iOS and Android apps later without rewriting the business domain.

Reuse priorities:

- Domain logic.
- Use-case orchestration.
- Validation rules.
- API contracts.
- Integration adapters that can be abstracted behind interfaces.

Do not attempt to reuse browser UI components on mobile. Reuse business logic and contracts instead.

## Integration model

Integration is a first-class concern.

Required patterns:

- REST API for external systems.
- Webhooks for event-driven integration.
- Outbound integration processing through worker jobs.
- Reliable delivery patterns where needed, such as retries and outbox handling.
- Structured logging and traceability for each integration path.

## Environment model

### Local

Local development should be runnable from the repo with minimal external dependencies. Docker Compose is the preferred default for database and support services.

### Test / preproduction

Should mirror production as closely as practical. Use separate secrets, databases, and configuration values.

### Production

Use immutable artifacts, controlled migrations, and health-checked deployments. Keep deployment and rollback documented and repeatable.

## Hosting model

The template should support multiple hosting options without redesign.

### Netlify frontend

Useful when the frontend is hosted separately and you want simple CI-driven deployment, custom domains, and SSL.

### Containerized backend

Useful for portability and consistency across environments.

### Supabase database/auth

Useful as a managed starting point for PostgreSQL and auth-related infrastructure while keeping the application architecture provider-neutral.

## Observability

Add these early:

- Structured logs.
- Health checks.
- Request correlation IDs.
- Basic metrics where practical.
- Error handling that surfaces useful operational detail without leaking secrets.

## Template-first implementation order

1. Create the solution structure.
2. Add the domain model.
3. Add application use cases.
4. Add infrastructure and EF Core migrations.
5. Add auth and RBAC.
6. Add API endpoints.
7. Add the HTML-first web layer.
8. Add tests.
9. Add deployment and hosting support.
10. Add mobile-reuse documentation and abstractions.

## Design constraints

- Do not let product-specific code leak into the template foundation.
- Do not place business rules in controllers or page handlers.
- Do not tie auth to a specific frontend implementation.
- Do not make the web UI the only client the architecture can support.
- Do not use the database as the source of business logic.
