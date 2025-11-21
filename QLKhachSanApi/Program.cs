using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ADO.NET configuration
builder.Services.AddSingleton<QLKhachSanApi.DAL.DatabaseHelper>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.DichVu>, QLKhachSanApi.Repositories.DichVuAdoRepository>();
// Register ADO repositories for entities that have ADO implementations
builder.Services.AddScoped<QLKhachSanApi.Repositories.IPhongRepository, QLKhachSanApi.Repositories.PhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.Phong>, QLKhachSanApi.Repositories.PhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.KhachHang>, QLKhachSanApi.Repositories.KhachHangAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.LoaiPhong>, QLKhachSanApi.Repositories.LoaiPhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.INhanVienRepository, QLKhachSanApi.Repositories.NhanVienAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.NhanVien>, QLKhachSanApi.Repositories.NhanVienAdoRepository>();

// New ADO repositories for booking/payment
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.DatPhong>, QLKhachSanApi.Repositories.DatPhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.ChiTietDatPhong>, QLKhachSanApi.Repositories.ChiTietDatPhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.SuDungDichVu>, QLKhachSanApi.Repositories.SuDungDichVuAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.IRepository<QLKhachSanApi.Models.ThanhToan>, QLKhachSanApi.Repositories.ThanhToanAdoRepository>();
// Register concrete ADO classes so controllers can request them directly
builder.Services.AddScoped<QLKhachSanApi.Repositories.DatPhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.ChiTietDatPhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.SuDungDichVuAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.ThanhToanAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.PhongAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.KhachHangAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.NhanVienAdoRepository>();
builder.Services.AddScoped<QLKhachSanApi.Repositories.DichVuAdoRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


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
