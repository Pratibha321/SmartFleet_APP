using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace;
using Serilog;
using System.Text;
using TelemetryService.Data;
using TelemetryService.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var key = builder.Configuration["Jwt:Key"] ?? "your_super_secret_key_12345";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "SmartFleet.AuthService";
var audience = builder.Configuration["Jwt:Audience"] ?? "SmartFleet.Client";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });
builder.Services.AddAuthorization();

// ======== Swagger with JWT Bearer support ========
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName ?? "SmartFleet API", Version = "v1" });

    // JWT input (Authorize button)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer' [space] and then your token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { securityScheme, new string[] { } }
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// EF InMemory
builder.Services.AddDbContext<TelemetryDbContext>(o => o.UseInMemoryDatabase("TelemetryDb"));

// OpenTelemetry
//builder.Services.AddOpenTelemetryTracing(b => b.AddAspNetCoreInstrumentation().AddConsoleExporter());

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter();
    });
var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.MapGrpcService<TelemetryGrpcService>();
app.MapControllers();
app.Run();
