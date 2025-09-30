using System.Text;
using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Serilog (dev)
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

// Add EF InMemory
builder.Services.AddDbContext<AuthDbContext>(opt => opt.UseInMemoryDatabase("AuthDb"));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT auth validation (services will validate tokens they receive using same key/issuer)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_shared_key_change_in_prod";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SmartFleet.AuthService";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // dev only
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = "SmartFleet.Client",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();
var app = builder.Build();

//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracing =>
//    {
//        tracing
//        .AddAspNetCoreInstrumentation()
//        .AddConsoleExporter();
//    });


// Seed demo users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    if (!db.Users.Any())
    {
        db.Users.AddRange(new ApplicationUser { Username = "admin", Password = "admin123", Role = "Admin", FullName = "Admin User" },
                         new ApplicationUser { Username = "dispatcher", Password = "dispatch123", Role = "Dispatcher", FullName = "Dispatcher User" },
                         new ApplicationUser { Username = "driver", Password = "driver123", Role = "Driver", FullName = "Driver User" });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();