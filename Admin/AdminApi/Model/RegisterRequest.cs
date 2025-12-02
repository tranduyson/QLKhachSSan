
using System;

namespace HotelManagement.Module

{
    // Model cho request đăng ký
    public class RegisterRequest
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
    }
}
