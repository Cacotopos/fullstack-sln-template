using Company.Product.Application.Extensions;
using Company.Product.Infrastructure.Extensions;
using Company.Product.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

var externalAuth = builder.Services.AddAuthentication();

if (builder.Configuration["Auth:Google:ClientId"] is { Length: > 0 } googleId)
{
    externalAuth.AddGoogle(options =>
    {
        options.ClientId = googleId;
        options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"]!;
    });
}

if (builder.Configuration["Auth:Microsoft:ClientId"] is { Length: > 0 } msId)
{
    externalAuth.AddMicrosoftAccount(options =>
    {
        options.ClientId = msId;
        options.ClientSecret = builder.Configuration["Auth:Microsoft:ClientSecret"]!;
    });
}

// Apple auth setup commented out until needed
// if (builder.Configuration["Auth:Apple:ClientId"] is { Length: > 0 } appleId)
// {
//     externalAuth.AddApple(options =>
//     {
//         options.ClientId = appleId;
//         options.TeamId = builder.Configuration["Auth:Apple:TeamId"]!;
//         options.KeyId = builder.Configuration["Auth:Apple:KeyId"]!;
//         options.UsePrivateKey(keyId => /* TODO: Implement private key loading */);
//     });
// }

builder.Services.AddAuthorization();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var supportedCultures = new[] { "en", "en-AU", "en-US", "en-GB", "fr", "de", "es", "ja", "zh" };
app.UseRequestLocalization(options =>
{
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
    options.ApplyCurrentCultureToResponseHeaders = true;
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

app.Run();
