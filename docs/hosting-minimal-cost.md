# Minimal Cost Hosting Guide

## Overview

This guide outlines how to host the Company.Product solution using free or low-cost cloud services. Total estimated cost: **$0-25/month** for small-scale production use.

---

## Architecture Options

### Option 1: Serverless + Managed Database (Recommended for MVP)
- **Frontend (Web):** Netlify or Vercel (Free tier)
- **Backend (API):** Railway or Render (Free/$5/month)
- **Database:** Supabase PostgreSQL (Free tier: 500MB)
- **Worker:** Railway background job ($5/month)
- **Blob Storage:** Supabase Storage or Cloudflare R2 (Free tier)
- **Logging:** Seq on Railway or Papertrail (Free tier)

**Total Cost:** $0-10/month

### Option 2: Container Platform (Better for scaling)
- **All Services:** Railway.app ($5-20/month)
- **Database:** Railway PostgreSQL ($5/month)
- **Blob Storage:** Cloudflare R2 (Free tier)

**Total Cost:** $10-25/month

### Option 3: Hybrid (Best balance)
- **Frontend:** Netlify (Free)
- **Backend + Database:** Supabase (Free tier)
- **API:** Railway ($5/month)
- **Worker:** Railway ($5/month)
- **Blob Storage:** Supabase Storage (Free)

**Total Cost:** $10/month

---

## Detailed Setup: Option 3 (Recommended)

### 1. Supabase Setup (Database + Auth + Storage)

#### Create Supabase Project
1. Sign up at https://supabase.com (Free tier: 500MB database, 1GB storage)
2. Create new project
3. Note your connection string from Settings → Database

#### Configure Database
```bash
# Connection string format
postgresql://postgres:[YOUR-PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres

# Add to .env
CONNECTION_STRINGS__Default=postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres
CONNECTION_STRINGS__ReadOnly=postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres
```

#### Run Migrations
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Apply migrations to Supabase
dotnet ef database update \
  --project src/Company.Product.Infrastructure \
  --startup-project src/Company.Product.Api \
  --connection "postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres"
```

#### Enable Supabase Storage (for Blob Service)
1. Go to Storage in Supabase dashboard
2. Create bucket: `company-product-blobs`
3. Set bucket to public or private based on needs
4. Get service role key from Settings → API

```env
# Add to .env
SUPABASE_URL=https://[PROJECT-REF].supabase.co
SUPABASE_ANON_KEY=[anon-key]
SUPABASE_SERVICE_ROLE_KEY=[service-role-key]
```

**Note:** You'll need to implement `SupabaseBlobService` similar to `AzureBlobService` using Supabase Storage SDK.

---

### 2. Railway Setup (API + Worker)

#### Install Railway CLI
```bash
npm install -g @railway/cli
railway login
```

#### Deploy API
```bash
# Initialize Railway project
cd src/Company.Product.Api
railway init

# Add environment variables
railway variables set ASPNETCORE_ENVIRONMENT=Production
railway variables set CONNECTION_STRINGS__Default="[supabase-connection-string]"
railway variables set Jwt__Key="[your-jwt-key-32-chars-min]"
railway variables set Jwt__Issuer="Company.Product"
railway variables set Jwt__Audience="Company.Product"

# Deploy
railway up
```

#### Deploy Worker
```bash
cd src/Company.Product.Worker
railway init

# Add same environment variables as API
railway variables set ASPNETCORE_ENVIRONMENT=Production
railway variables set CONNECTION_STRINGS__Default="[supabase-connection-string]"

# Deploy
railway up
```

#### Configure Custom Domain (Optional)
```bash
# In Railway dashboard
# Settings → Domains → Generate Domain or add custom domain
# Example: api.yourapp.com
```

---

### 3. Netlify Setup (Web Frontend)

#### Prepare Web Project for Static Hosting

**Option A: Blazor WebAssembly (if converting)**
```bash
# Convert to Blazor WASM (requires refactoring)
# Not recommended for this Razor Pages app
```

**Option B: Deploy as Container to Netlify Functions + Railway**
Since this is a Razor Pages app (not static), you have two choices:

1. **Deploy Web to Railway** (alongside API)
   ```bash
   cd src/Company.Product.Web
   railway init
   railway up
   ```

2. **Convert to Static Site** (requires significant refactoring)
   - Convert Razor Pages to static HTML/JS
   - Use API for all dynamic content
   - Not recommended for this architecture

**Recommended:** Deploy Web to Railway ($5/month) for full Razor Pages support.

---

### 4. Cloudflare R2 Setup (Blob Storage Alternative)

If you prefer Cloudflare R2 over Supabase Storage:

1. Sign up at https://cloudflare.com
2. Go to R2 Object Storage
3. Create bucket: `company-product-blobs`
4. Generate API token with R2 permissions

```env
# Add to .env
R2_ACCOUNT_ID=[account-id]
R2_ACCESS_KEY_ID=[access-key]
R2_SECRET_ACCESS_KEY=[secret-key]
R2_BUCKET_NAME=company-product-blobs
```

**Note:** Requires implementing `CloudflareR2BlobService` using AWS S3 SDK (R2 is S3-compatible).

---

## Cost Breakdown

### Free Tier Limits

**Supabase (Free):**
- 500MB database
- 1GB file storage
- 2GB bandwidth
- 50,000 monthly active users
- Unlimited API requests

**Railway (Free Trial):**
- $5 credit/month
- After trial: $5/service/month

**Netlify (Free):**
- 100GB bandwidth
- 300 build minutes/month
- Automatic HTTPS

**Cloudflare R2 (Free):**
- 10GB storage
- 1 million Class A operations/month
- 10 million Class B operations/month

### Estimated Monthly Costs

| Service | Free Tier | Paid Tier | Recommended |
|---------|-----------|-----------|-------------|
| Supabase DB | ✅ Free (500MB) | $25/month (8GB) | Free for MVP |
| Railway API | ❌ $5/month | $5-20/month | $5/month |
| Railway Worker | ❌ $5/month | $5-20/month | $5/month |
| Railway Web | ❌ $5/month | $5-20/month | $5/month |
| Supabase Storage | ✅ Free (1GB) | Included in DB plan | Free |
| Seq Logging | ✅ Free (Papertrail) | $7/month | Free |

**Total: $15/month** (API + Worker + Web on Railway, everything else free)

---

## Environment Variables

Create `.env.production` with all required variables:

```env
# Environment
ASPNETCORE_ENVIRONMENT=Production
APP_BASE_URL=https://yourapp.com
API_BASE_URL=https://api-yourapp.railway.app
WEB_BASE_URL=https://web-yourapp.railway.app

# Database (Supabase)
CONNECTION_STRINGS__Default=postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres
CONNECTION_STRINGS__ReadOnly=postgresql://postgres:[PASSWORD]@db.[PROJECT-REF].supabase.co:5432/postgres

# JWT
Jwt__Issuer=Company.Product
Jwt__Audience=Company.Product
Jwt__Key=[generate-secure-32-char-key]

# OAuth (Optional)
Auth__Google__ClientId=[google-client-id]
Auth__Google__ClientSecret=[google-secret]
Auth__Microsoft__ClientId=[microsoft-client-id]
Auth__Microsoft__ClientSecret=[microsoft-secret]

# Stripe (Optional)
STRIPE__PUBLISHABLE_KEY=pk_test_...
STRIPE__SECRET_KEY=sk_test_...
STRIPE__WEBHOOK_SECRET=whsec_...
STRIPE__PRICE_ID_BASIC=price_...
STRIPE__PRICE_ID_PRO=price_...

# Supabase Storage
SUPABASE_URL=https://[PROJECT-REF].supabase.co
SUPABASE_ANON_KEY=[anon-key]
SUPABASE_SERVICE_ROLE_KEY=[service-role-key]

# Logging (Papertrail)
Seq__ServerUrl=https://logs.papertrailapp.com/api/events
```

---

## Deployment Checklist

### Pre-Deployment
- [ ] Create Supabase project and note connection string
- [ ] Run database migrations to Supabase
- [ ] Create Railway projects (API, Worker, Web)
- [ ] Configure environment variables in Railway
- [ ] Set up OAuth providers (Google, Microsoft)
- [ ] Configure Stripe webhooks (if using payments)

### Deployment
- [ ] Deploy API to Railway: `railway up`
- [ ] Deploy Worker to Railway: `railway up`
- [ ] Deploy Web to Railway: `railway up`
- [ ] Verify health endpoints
- [ ] Test authentication flow
- [ ] Test CRUD operations
- [ ] Test blob upload/download

### Post-Deployment
- [ ] Configure custom domains (optional)
- [ ] Set up monitoring (Railway metrics)
- [ ] Configure backups (Supabase automatic backups)
- [ ] Set up alerts for errors
- [ ] Document deployment process

---

## Monitoring & Logging

### Railway Metrics
- Built-in metrics dashboard
- CPU, memory, network usage
- Request logs

### Supabase Monitoring
- Database metrics in dashboard
- Query performance
- Connection pooling stats

### Papertrail (Free Logging)
```bash
# Add Papertrail to Railway
railway variables set Seq__ServerUrl="https://logs.papertrailapp.com/api/events"
```

---

## Scaling Considerations

### When to Upgrade

**Database (Supabase):**
- Upgrade to $25/month when approaching 500MB
- Consider read replicas for high traffic

**API/Worker (Railway):**
- Scale vertically: Increase memory/CPU ($5-20/month per service)
- Scale horizontally: Add more instances (Railway auto-scaling)

**Storage:**
- Cloudflare R2 is more cost-effective for large files
- Supabase Storage is simpler for small files

---

## Alternative Platforms

### Other Low-Cost Options

**Render.com:**
- Similar to Railway
- Free tier for static sites
- $7/month for web services
- $7/month for PostgreSQL

**Fly.io:**
- Free tier: 3 shared-cpu VMs
- Good for .NET apps
- Global edge deployment

**Azure App Service (Free Tier):**
- F1 tier: Free
- 1GB RAM, 1GB storage
- Good for .NET apps
- Limited to 60 CPU minutes/day

**DigitalOcean App Platform:**
- $5/month for basic app
- $15/month for managed PostgreSQL
- Simple deployment

---

## Security Considerations

### Production Checklist
- [ ] Use HTTPS only (Railway provides automatic HTTPS)
- [ ] Rotate JWT keys regularly
- [ ] Use strong database passwords
- [ ] Enable Supabase Row Level Security (RLS)
- [ ] Configure CORS properly
- [ ] Use environment variables for secrets (never commit)
- [ ] Enable rate limiting
- [ ] Set up Web Application Firewall (Cloudflare free tier)

---

## Backup Strategy

### Supabase Backups
- Automatic daily backups (retained 7 days on free tier)
- Manual backups via pg_dump:
  ```bash
  pg_dump -h db.[PROJECT-REF].supabase.co \
    -U postgres -d postgres > backup.sql
  ```

### Railway Backups
- No automatic backups for app code
- Use Git for version control
- Database backups via Supabase

---

## Troubleshooting

### Common Issues

**Connection Timeout:**
- Check Supabase connection string
- Verify IP allowlist (Supabase allows all by default)
- Check Railway environment variables

**Migration Failures:**
- Ensure connection string is correct
- Check database permissions
- Run migrations locally first

**Authentication Issues:**
- Verify OAuth redirect URIs
- Check JWT configuration
- Ensure HTTPS is enabled

**Blob Upload Failures:**
- Check Supabase Storage permissions
- Verify bucket exists
- Check file size limits

---

## Next Steps

1. **Set up Supabase project** and run migrations
2. **Deploy API to Railway** with environment variables
3. **Deploy Worker to Railway** for background jobs
4. **Deploy Web to Railway** for Razor Pages
5. **Configure OAuth providers** for social login
6. **Set up monitoring** with Railway metrics
7. **Test thoroughly** before going live

---

## Cost Optimization Tips

1. **Use Supabase free tier** until you hit limits
2. **Combine services** on Railway to reduce costs
3. **Use Cloudflare CDN** (free) for static assets
4. **Enable caching** to reduce database queries
5. **Optimize images** before uploading to blob storage
6. **Use connection pooling** to reduce database connections
7. **Monitor usage** regularly to avoid surprises

---

## Support Resources

- **Railway:** https://docs.railway.app
- **Supabase:** https://supabase.com/docs
- **Netlify:** https://docs.netlify.com
- **Cloudflare R2:** https://developers.cloudflare.com/r2
- **.NET Deployment:** https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy

---

## Conclusion

This setup provides a production-ready hosting solution for **$15-25/month** that can handle thousands of users. As your application grows, you can scale individual components independently without major architectural changes.

For the absolute minimal cost ($0-5/month), consider:
- Using Supabase free tier for everything
- Deploying only the API to Railway ($5/month)
- Using Supabase Auth instead of ASP.NET Identity
- Serving static files from Netlify

The architecture is flexible enough to migrate to enterprise platforms (Azure, AWS, GCP) when needed.
