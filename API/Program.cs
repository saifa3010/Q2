using API.Middleware;
using Application;
using Hangfire;
using Infrastructure;
using Infrastructure.Outbox;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers & OpenAPI ─────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

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

// ── OpenAPI ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

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

app.Run();