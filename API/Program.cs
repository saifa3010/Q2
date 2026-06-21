using API.Middleware;
using Application;
using Hangfire;
using Infrastructure;
using Infrastructure.Outbox;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers & Swagger ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Q2 Invoice Management API",
        Version = "v1",
        Description = "POC demonstrating DDD, Outbox Pattern, and Keycloak IDP integration"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste a Keycloak access token (without the 'Bearer ' prefix)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── Application (MediatR handlers) ───────────────────────────────────────
builder.Services.AddApplication();

// ── Infrastructure (DbContext + Repositories + UoW + Hangfire) ───────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Authentication ────────────────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ── Global error handling (must be first middleware) ──────────────────────
app.UseGlobalExceptionHandler();

// ── Swagger ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Q2 Invoice API v1");
        options.RoutePrefix = "swagger"; // → https://localhost:44399/swagger
    });
}

// ── Hangfire dashboard ────────────────────────────────────────────────────
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [] // TODO: add auth filter before production
});

// ── Recurring jobs ────────────────────────────────────────────────────────
var jobManager = app.Services.GetRequiredService<IRecurringJobManager>();
jobManager.RegisterRecurringJobs();

// ── Middleware pipeline ───────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Redirect root to Swagger UI so / doesn't 404 during development.
if (app.Environment.IsDevelopment())
    app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();