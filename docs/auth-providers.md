# Authentication Providers

## Overview

The template supports:

- **Native ASP.NET Core Identity** (email/password)
- **Google OAuth 2.0**
- **Microsoft Account**
- **Apple Sign In**

## Configuration

### Environment Variables

```env
# Google
Auth__Google__ClientId=your-google-client-id.apps.googleusercontent.com
Auth__Google__ClientSecret=your-google-client-secret

# Microsoft
Auth__Microsoft__ClientId=your-microsoft-app-id
Auth__Microsoft__ClientSecret=your-microsoft-app-secret

# Apple
Auth__Apple__ClientId=your.apple.service.id
Auth__Apple__TeamId=your-apple-team-id
Auth__Apple__KeyId=your-apple-key-id
Auth__Apple__PrivateKey=-----BEGIN PRIVATE KEY-----
...
-----END PRIVATE KEY-----
```

### Setup Steps

#### Google

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a project or select existing
3. Enable the Google+ API
4. Create OAuth 2.0 Client ID (Web application)
5. Add authorized redirect URIs:
   - `https://localhost:5002/signin-google`
   - `https://yourdomain.com/signin-google`

#### Microsoft

1. Go to [Azure Portal](https://portal.azure.com/)
2. Register a new application (App registrations)
3. Add Web platform with redirect URI:
   - `https://localhost:5002/signin-microsoft`
4. Grant API permissions (e.g., User.Read)
5. Create a client secret

#### Apple

1. Go to [Apple Developer](https://developer.apple.com/)
2. Create a Services ID
3. Configure Sign In with Apple
4. Add authorized return URL:
   - `https://localhost:5002/signin-apple`
5. Create and download a signing key (p8)
6. Use the key contents for `Auth__Apple__PrivateKey`

## Code Flow

### Web (Cookie Auth)

- Users are redirected to provider login
- Provider redirects back to `/signin-{provider}`
- Middleware creates/links `IdentityUser` and signs in via cookies

### API (JWT Bearer)

- Use `/api/v1/auth/login` endpoint (native) or
- Obtain a token via provider-specific OAuth flow (future extension)

## RBAC Integration

- After first login, assign roles via admin UI or seeding
- Policies can require specific roles or claims
- Example: `[Authorize(Roles = "Admin")]` or `[Authorize(Policy = "CanManageBilling")]`

## Security Notes

- Always use HTTPS in production
- Validate redirect URIs to prevent open redirects
- Store client secrets securely (environment variables or Key Vault)
- Consider requiring email confirmation for native accounts
- Enable 2FA for admin accounts
- Review provider-specific security best practices

## Testing Locally

Providers require HTTPS even for local development. Use:

```bash
dotnet dev-certs https --trust
```

Or configure provider with `http://localhost` for testing only (not recommended).

## Troubleshooting

- **Error: redirect_uri_mismatch** — Ensure redirect URI matches exactly what’s configured in the provider console.
- **Error: invalid_client** — Verify client ID and secret.
- **Apple key errors** — Ensure the private key is correctly formatted with line breaks.
- **Missing claims** — Configure scopes in provider console and code.
