namespace Company.Product.Api.Endpoints;

internal static class WebhookEndpoints
{
    internal static IEndpointRouteBuilder MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/webhooks/stripe", async (HttpRequest request, IConfiguration config) =>
        {
            if (!request.Headers.TryGetValue("Stripe-Signature", out var signature))
                return Results.BadRequest("Missing Stripe-Signature header.");

            var webhookSecret = config["Stripe:WebhookSecret"] ?? string.Empty;
            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();

            // Validate signature and dispatch event here using Stripe.net SDK:
            // var stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
            // switch (stripeEvent.Type) { ... }

            return Results.Ok();
        })
        .AllowAnonymous()
        .WithTags("Webhooks")
        .WithName("StripeWebhook");

        return app;
    }
}
