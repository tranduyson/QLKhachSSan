using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("ThisIsA256BitSecretKeyForJWT1234567890")),
            RoleClaimType = ClaimTypes.Role
        };
    });

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

builder.Services.AddControllers();
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
