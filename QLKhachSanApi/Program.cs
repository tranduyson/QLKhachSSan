using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<QLKhachSanApi.DAL.HotelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelDb")));

// Register repositories
builder.Services.AddScoped(typeof(QLKhachSanApi.Repositories.IRepository<>), typeof(QLKhachSanApi.Repositories.Repository<>));
// ADO.NET helpers and overrides
builder.Services.AddSingleton<QLKhachSanApi.DAL.DatabaseHelper>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.DichVu>, QLKhachSanApi.Repositories.DichVuAdoRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()      // hoặc dùng .WithOrigins("https://localhost:44300") nếu muốn chỉ định
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Thêm dòng này TRƯỚC UseAuthorization
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
