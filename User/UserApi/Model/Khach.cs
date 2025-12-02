using System;

namespace HotelManagement.Module
{
    public class Khach
    {
        public int MaKhach { get; set; }
        public string HoTen { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string CMND { get; set; }
        public string DiaChi { get; set; }

        // Constructor mặc định
        public Khach()
        {
            MaKhach = 0;
            HoTen = string.Empty;
            DienThoai = string.Empty;
            Email = string.Empty;
            CMND = string.Empty;
            DiaChi = string.Empty;
        }

        // Constructor có tham số
        public Khach(string hoTen, string dienThoai, string email, string cMND, string diaChi)
        {
            HoTen = hoTen;
            DienThoai = dienThoai;
            Email = email;
            CMND = cMND;
            DiaChi = diaChi;
        }
    }
}
