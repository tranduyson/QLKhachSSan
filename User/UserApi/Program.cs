using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// JWT Authentication
var key = Encoding.UTF8.GetBytes("ThisIsA256BitSecretKeyForJWT1234567890");
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
        ValidIssuer = "yourIssuer",
        ValidAudience = "yourAudience",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role // <- quan trọng để phân quyền theo role
    };
});

// Authorization - định nghĩa policy cho từng role
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
