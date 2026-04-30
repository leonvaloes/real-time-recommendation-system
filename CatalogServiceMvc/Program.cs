using System.Text;
using CatalogServiceMvc.Data;
using CatalogServiceMvc.Repositories;
using CatalogServiceMvc.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<PostgresContext>();
builder.Services.AddScoped<IProductRepository, PostgresProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? builder.Configuration["JWT_ISSUER"]
            ?? "UserAuth.CleanArch.API";
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? builder.Configuration["JWT_AUDIENCE"]
            ?? "UserAuth.CleanArch.Client";
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? builder.Configuration["JWT_SECRET"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT_SECRET must be configured.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadCatalog", policy => policy.RequireClaim("permission", "catalog:read"));
    options.AddPolicy("CanWriteCatalog", policy => policy.RequireClaim("permission", "catalog:write"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
