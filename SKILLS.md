# Skills for Future Coding Agents

## Mission

This repository is a reusable template for quickly launching new web applications. Your job is to preserve the template boundaries while implementing product-specific features in a clean, testable way.

## Primary objectives

- Keep the solution easy to clone for future projects.
- Preserve architecture boundaries.
- Favor testability and automation.
- Maintain responsive and accessible web behavior.
- Keep the backend reusable for future mobile clients.
- Support local, test/preprod, and production environments.

## Operating rules

- Treat `Domain` as framework-free.
- Keep EF Core and PostgreSQL details in `Infrastructure`.
- Keep business workflows in `Application`.
- Keep contracts stable and versioned.
- Keep web pages functional without JavaScript for core tasks.
- Prefer explicit interfaces for external systems.
- Use the worker for asynchronous and background processing.
- Add tests alongside implementation.

## Commands and workflows

Add the exact project commands here once the solution exists.

Minimum workflows to document:

- Restore/build.
- Run web app locally.
- Run API locally.
- Run database migrations.
- Seed development data.
- Run unit tests.
- Run integration tests.
- Run UI tests.
- Prepare release artifacts.

## How to add a feature

1. Define the business intent.
2. Add or update domain objects.
3. Add application use cases and interfaces.
4. Add persistence mappings if needed.
5. Add API contracts and endpoints.
6. Add web UI or component support.
7. Add tests for the feature.
8. Document any new configuration.

## How to add a migration

1. Update domain and infrastructure models.
2. Create the EF Core migration.
3. Review the generated schema changes.
4. Run the migration locally.
5. Validate the upgrade and rollback path if relevant.
6. Commit the migration with the model change.

## How to add a UI component

- Keep it accessible.
- Make it responsive by default.
- Ensure a no-JS fallback exists if the component supports a core workflow.
- Prefer reusable primitives over one-off markup.
- Add visual and interaction tests where appropriate.

## How to add an integration or webhook

- Define the contract in `Contracts`.
- Add validation and signature checks.
- Make handlers idempotent where possible.
- Log enough context to trace failures.
- Add retries or outbox processing when delivery matters.
- Add tests for both valid and invalid payloads.

## How to support a mobile client

- Reuse domain and application logic.
- Expose stable API contracts.
- Keep auth and authorization consistent.
- Do not move browser-specific UI logic into shared libraries.
- Add client-friendly endpoints if needed, but keep the backend canonical.

## What to avoid

- Putting business rules in controllers or page handlers.
- Coupling domain logic to EF Core entities.
- Hard-coding a single auth provider into business logic.
- Making the frontend dependent on JavaScript for basic navigation or form submission.
- Copying implementation details between layers instead of using interfaces.

## Template quality bar

- Clear boundaries.
- Fast local startup.
- Reliable tests.
- Repeatable migrations.
- Responsive frontend.
- Stable API contracts.
- Good documentation.
- Easy future extension.
