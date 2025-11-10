using Microsoft.EntityFrameworkCore;
using QLKhachSanApi.Models;

namespace QLKhachSanApi.DAL
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<LoaiPhong> LoaiPhongs { get; set; }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<DatPhong> DatPhongs { get; set; }
        public DbSet<ChiTietDatPhong> ChiTietDatPhongs { get; set; }
        public DbSet<SuDungDichVu> SuDungDichVus { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure computed columns
            modelBuilder.Entity<ChiTietDatPhong>()
                .Property(p => p.ThanhTien)
                .HasComputedColumnSql("[DonGia] * [SoDem]");

            modelBuilder.Entity<SuDungDichVu>()
                .Property(p => p.ThanhTien)
                .HasComputedColumnSql("[SoLuong] * [DonGia]");

            // Configure relationships
            modelBuilder.Entity<Phong>()
                .HasOne(p => p.LoaiPhong)
                .WithMany(lp => lp.Phongs)
                .HasForeignKey(p => p.MaLoaiPhong);

            modelBuilder.Entity<DatPhong>()
                .HasOne(dp => dp.KhachHang)
                .WithMany(kh => kh.DatPhongs)
                .HasForeignKey(dp => dp.MaKhachHang);

            modelBuilder.Entity<DatPhong>()
                .HasOne(dp => dp.NhanVien)
                .WithMany(nv => nv.DatPhongs)
                .HasForeignKey(dp => dp.MaNhanVien);

            modelBuilder.Entity<ChiTietDatPhong>()
                .HasOne(ct => ct.DatPhong)
                .WithMany(dp => dp.ChiTietDatPhongs)
                .HasForeignKey(ct => ct.MaDatPhong);

            modelBuilder.Entity<ChiTietDatPhong>()
                .HasOne(ct => ct.Phong)
                .WithMany(p => p.ChiTietDatPhongs)
                .HasForeignKey(ct => ct.MaPhong);

            modelBuilder.Entity<SuDungDichVu>()
                .HasOne(sd => sd.DatPhong)
                .WithMany(dp => dp.SuDungDichVus)
                .HasForeignKey(sd => sd.MaDatPhong);

            modelBuilder.Entity<SuDungDichVu>()
                .HasOne(sd => sd.DichVu)
                .WithMany(dv => dv.SuDungDichVus)
                .HasForeignKey(sd => sd.MaDichVu);

            modelBuilder.Entity<ThanhToan>()
                .HasOne(tt => tt.DatPhong)
                .WithMany(dp => dp.ThanhToans)
                .HasForeignKey(tt => tt.MaDatPhong);
        }
    }
}