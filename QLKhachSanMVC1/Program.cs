using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// CORS: allow frontend local origins during development. Prefer explicit FRONTEND_ORIGIN env var.
var frontendOrigin = builder.Configuration["FRONTEND_ORIGIN"];
var allowedOrigins = !string.IsNullOrWhiteSpace(frontendOrigin)
    ? new[] { frontendOrigin }
    : new[] {
        "https://localhost:5001", // common ASP.NET Core dev HTTPS port for MVC app
        "http://localhost:5000",
        "https://localhost:7158",
        "https://localhost:7160"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Require authentication for most pages: if AuthUser cookie is missing, redirect to login.
app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    // Allow static assets, Account pages (login), and API endpoints to be accessed without the cookie
    if (path.StartsWithSegments("/css") || path.StartsWithSegments("/js") || path.StartsWithSegments("/lib") ||
        path.StartsWithSegments("/favicon.ico") || path.StartsWithSegments("/_framework") ||
        path.StartsWithSegments("/Account") || path.StartsWithSegments("/api"))
    {
        await next();
        return;
    }

    if (!context.Request.Cookies.ContainsKey("AuthUser"))
    {
        var returnUrl = context.Request.Path + context.Request.QueryString;
        var loginUrl = "/Account/Login" + (string.IsNullOrEmpty(returnUrl) ? "" : "?returnUrl=" + System.Net.WebUtility.UrlEncode(returnUrl));
        context.Response.Redirect(loginUrl);
        return;
    }

    await next();
});

// Apply CORS policy for local development before authorization
app.UseCors("AllowLocalDev");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

