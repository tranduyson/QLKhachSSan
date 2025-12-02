using System;

namespace HotelManagement.Module

{
    public class NguoiDung
    {
        public int MaND { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; }

        // Constructor mặc định
        public NguoiDung()
        {
            MaND = 0;
            TenDangNhap = string.Empty;
            MatKhau = string.Empty;
            HoTen = string.Empty;
            VaiTro = string.Empty;
        }

        // Constructor có tham số
        public NguoiDung(string tenDangNhap, string hoTen, string vaiTro)
        {
            TenDangNhap = tenDangNhap;
            HoTen = hoTen;
            VaiTro = vaiTro;
        }
    }
}
