using System;

namespace HotelManagement.Module
{
    public class HoaDonChiTiet
    {
        public int MaCTHD { get; set; }
        public int MaHD { get; set; }
        public int? MaDatPhong { get; set; }
        public int? MaDV { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal? ThanhTien { get; set; }

        public HoaDonChiTiet()
        {
            MaCTHD = 0;
            MaHD = 0;
            MaDatPhong = 0;
            MaDV = 0;
            SoLuong = 0;
            DonGia = 0;
            ThanhTien = 0;
        }

        public HoaDonChiTiet(int soLuong, decimal donGia, decimal? thanhTien)
        {
            SoLuong = soLuong;
            DonGia = donGia;
            ThanhTien = thanhTien;
        }
    }
}
