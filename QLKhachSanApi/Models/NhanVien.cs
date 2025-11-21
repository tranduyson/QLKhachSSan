namespace QLKhachSanApi.Models
{
    public class NhanVien
    {
        public int MaNhanVien { get; set; }

        public string TenDangNhap { get; set; }

        public string MatKhau { get; set; }

        public string HoTen { get; set; }

        public string? ChucVu { get; set; }

        public string? SoDienThoai { get; set; }

        public int TrangThai { get; set; }
    }
}