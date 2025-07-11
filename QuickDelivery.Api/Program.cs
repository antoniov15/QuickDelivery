﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuickDelivery.Api.Middleware; // IMPORTANT: Import pentru middleware
using QuickDelivery.Core.Interfaces.Repositories;
using QuickDelivery.Core.Interfaces.Services;
using QuickDelivery.Core.Options;
using QuickDelivery.Core.Exceptions;
using QuickDelivery.Database;
using QuickDelivery.Database.Extensions;
using QuickDelivery.Infrastructure.Repositories;
using QuickDelivery.Infrastructure.Services;
using System.Text;
using QuickDelivery.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Entity Framework Configuration
var connectionString = builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"];
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero
    };
});

// JWT Options Configuration
builder.Services.Configure<JwtOptions>(options =>
{
    options.Secret = secretKey!;
    options.Issuer = issuer!;
    options.Audience = audience!;
    options.ExpiryMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("DelivererOnly", policy => policy.RequireRole("Deliverer"));
    options.AddPolicy("PartnerOnly", policy => policy.RequireRole("Partner"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QuickDelivery API",
        Version = "v1",
        Description = "API for QuickDelivery - Urban Delivery Platform with Exception Handling",
        Contact = new OpenApiContact
        {
            Name = "QuickDelivery Team",
            Email = "support@quickdelivery.com"
        }
    });

    // JWT Bearer configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// AutoMapper (add this package if you want to use it)
// builder.Services.AddAutoMapper(typeof(Program));

// Register custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<RoleBasedDataFilter>();
builder.Services.AddScoped<ISecureUserService, SecureUserService>();
//builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserService>(provider => provider.GetService<ISecureUserService>());
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuickDelivery API v1");
        c.RoutePrefix = string.Empty; // Swagger UI available at root
    });
}

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // apply migrations
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migration completed successfully.");

        // Seed many-to-many relationships
        await context.SeedManyToManyRelationshipsAsync(logger);
        logger.LogInformation("Data seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseHttpsRedirection();

// Exception handling middleware - FIXED
//app.UseExceptionHandlingMiddleware(); // Folosește extension method-ul

// Sau alternativ:
// app.UseMiddleware<ExceptionHandlingMiddleware>(); // Folosește direct middleware-ul

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();