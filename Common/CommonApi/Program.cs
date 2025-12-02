using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Add services to the container
// =======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
var key = Encoding.UTF8.GetBytes("ThisIsA256BitSecretKeyForJWT1234567890");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer",
            ValidAudience = "yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role // <- để phân quyền theo role
        };
    });

// Role-based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("LeTanOnly", policy => policy.RequireRole("LeTan"));
    options.AddPolicy("KeToanOnly", policy => policy.RequireRole("KeToan"));
    options.AddPolicy("KhachOnly", policy => policy.RequireRole("Khach"));

    options.AddPolicy("AdminOrLeTan", policy => policy.RequireRole("Admin", "LeTan"));
    options.AddPolicy("AdminOrKeToan", policy => policy.RequireRole("Admin", "KeToan"));
    options.AddPolicy("AdminOrLeTanOrKeToan", policy => policy.RequireRole("Admin", "LeTan", "KeToan"));
});

var app = builder.Build();

// =======================
// Configure the HTTP request pipeline
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
