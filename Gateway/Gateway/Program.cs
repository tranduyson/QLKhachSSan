using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Load Ocelot config
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// 2. CORS – QUAN TRỌNG: phải AllowAnyHeader để Authorization header được qua
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()     // hoặc thay bằng domain cụ thể nếu muốn bảo mật hơn
              .AllowAnyMethod()
              .AllowAnyHeader();    // ← BẮT BUỘC phải có dòng này!
    });
});

// 3. JWT Authentication cho Gateway (để validate token trước khi forward)
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
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ====== THỨ TỰ MIDDLEWARE QUAN TRỌNG NHẤT ======
// 1. CORS PHẢI ĐỨNG TRƯỚC HẾT (đặc biệt khi có Authentication)
app.UseCors("AllowAll");

// 2. Các middleware khác
app.UseHttpsRedirection();

// 3. Authentication & Authorization PHẢI SAU CORS
app.UseAuthentication();
app.UseAuthorization();

// 4. Ocelot phải để CUỐI CÙNG (sau tất cả middleware)
await app.UseOcelot();

app.Run();