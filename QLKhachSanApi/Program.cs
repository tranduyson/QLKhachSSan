using QLKhachSanApi.DAL;
using QLKhachSanApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<DatabaseHelper>();
builder.Services.AddScoped<ILoaiPhongService, LoaiPhongService>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<IDichVuService, DichVuService>();
builder.Services.AddScoped<IPhongService, PhongService>();
builder.Services.AddScoped<IThanhToanService, ThanhToanService>();
builder.Services.AddScoped<IDatPhongService, DatPhongService>();

// Remove EF generic repository registration; now using ADO services

builder.Services.AddControllers();
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

app.Run();
