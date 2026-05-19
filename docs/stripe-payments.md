# Stripe Subscription / Payment Model

## Purpose

This document describes the subscription and payment model for the application using Stripe. It serves as the design contract for implementing billing features on top of this template.

## Subscription Tiers

| Plan   | Stripe Price ID env key       | Features                              |
|--------|-------------------------------|---------------------------------------|
| Free   | _(no Stripe price)_           | Limited access, no payment required   |
| Basic  | `STRIPE__PRICE_ID_BASIC`      | Standard feature set, monthly billing |
| Pro    | `STRIPE__PRICE_ID_PRO`        | Full access, monthly or annual        |

## Domain Model

### `Subscription` entity (`Domain`)

```
Subscription
  Id                  Guid
  UserId              string          (FK to Identity user)
  StripeCustomerId    string          (cus_xxx)
  StripeSubscriptionId string         (sub_xxx)
  Plan                SubscriptionPlan enum  (Free | Basic | Pro)
  Status              SubscriptionStatus enum (Active | PastDue | Canceled | Trialing)
  CurrentPeriodStart  DateTimeOffset
  CurrentPeriodEnd    DateTimeOffset
  CanceledAt          DateTimeOffset?
  CreatedAt           DateTimeOffset
  UpdatedAt           DateTimeOffset
```

### `SubscriptionPlan` enum

```
Free = 0
Basic = 1
Pro = 2
```

### `SubscriptionStatus` enum

```
Trialing = 0
Active = 1
PastDue = 2
Canceled = 3
Incomplete = 4
```

## Application Interfaces

```
ISubscriptionService
  Task<Subscription?> GetForUserAsync(string userId)
  Task<string> CreateCheckoutSessionAsync(string userId, SubscriptionPlan plan)
  Task<string> CreateBillingPortalSessionAsync(string userId, string returnUrl)
  Task HandleWebhookAsync(string json, string stripeSignature)

IStripeClient (infrastructure interface)
  Task<Customer> GetOrCreateCustomerAsync(string userId, string email)
  Task<Session> CreateCheckoutSessionAsync(string customerId, string priceId, string successUrl, string cancelUrl)
  Task<Session> CreateBillingPortalSessionAsync(string customerId, string returnUrl)
  Task<Event> ParseWebhookEventAsync(string json, string signature, string secret)
```

## Stripe Webhook Events to Handle

| Event                                  | Action                                          |
|----------------------------------------|-------------------------------------------------|
| `customer.subscription.created`        | Create or activate Subscription record          |
| `customer.subscription.updated`        | Update plan, status, period dates               |
| `customer.subscription.deleted`        | Mark Subscription as Canceled                   |
| `invoice.payment_succeeded`            | Extend period, clear PastDue status             |
| `invoice.payment_failed`               | Set status to PastDue, notify user              |
| `checkout.session.completed`           | Link StripeCustomerId / StripeSubscriptionId    |

## API Endpoints

```
POST /api/v1/billing/checkout          Create Stripe Checkout session (authenticated)
POST /api/v1/billing/portal            Create Stripe Billing Portal session (authenticated)
GET  /api/v1/billing/subscription      Get current user subscription status (authenticated)
POST /api/v1/webhooks/stripe           Receive Stripe webhook events (public, signature verified)
```

## Web Pages

```
/billing/plans           Plan selection page with Stripe Checkout redirect
/billing/success         Post-checkout success landing
/billing/cancel          Post-checkout cancel landing
/billing/manage          Redirect to Stripe Billing Portal
```

## Infrastructure

- NuGet package: `Stripe.net` (latest stable)
- Stripe secret key loaded from `STRIPE__SECRET_KEY` env var
- Webhook secret loaded from `STRIPE__WEBHOOK_SECRET` env var
- Publishable key exposed to frontend via `STRIPE__PUBLISHABLE_KEY`
- `StripeClient` implements `IStripeClient` and is registered as scoped
- Webhook endpoint reads raw body and validates `Stripe-Signature` header before parsing

## EF Core Migration Notes

Add `Subscriptions` table with the fields above. Index on `UserId` and `StripeCustomerId`.

```sql
CREATE TABLE "Subscriptions" (
    "Id"                    uuid          NOT NULL PRIMARY KEY,
    "UserId"                varchar(450)  NOT NULL,
    "StripeCustomerId"      varchar(255)  NOT NULL,
    "StripeSubscriptionId"  varchar(255),
    "Plan"                  int           NOT NULL DEFAULT 0,
    "Status"                int           NOT NULL DEFAULT 0,
    "CurrentPeriodStart"    timestamptz   NOT NULL,
    "CurrentPeriodEnd"      timestamptz   NOT NULL,
    "CanceledAt"            timestamptz,
    "CreatedAt"             timestamptz   NOT NULL,
    "UpdatedAt"             timestamptz   NOT NULL
);
CREATE INDEX IX_Subscriptions_UserId ON "Subscriptions" ("UserId");
CREATE UNIQUE INDEX UX_Subscriptions_StripeCustomerId ON "Subscriptions" ("StripeCustomerId");
```

## Environment Variables Required

```
STRIPE__PUBLISHABLE_KEY=pk_test_...
STRIPE__SECRET_KEY=sk_test_...
STRIPE__WEBHOOK_SECRET=whsec_...
STRIPE__PRICE_ID_BASIC=price_...
STRIPE__PRICE_ID_PRO=price_...
```

## Local Development

Use the [Stripe CLI](https://stripe.com/docs/stripe-cli) to forward webhook events locally:

```bash
stripe listen --forward-to https://localhost:5001/api/v1/webhooks/stripe
```

The CLI will output a webhook signing secret to use in `.env` for `STRIPE__WEBHOOK_SECRET`.

## Security Rules

- Never log or store raw Stripe secret keys.
- Always validate `Stripe-Signature` before processing webhook payloads.
- Always read raw request body (before model binding) for signature verification.
- Webhook handler must be idempotent — Stripe may deliver the same event more than once.
- Use `Authorization` policy `SubscriptionRequired` to gate Pro/Basic-only features.
