using System;

namespace HotelManagement.Module
{
    public class HoaDon
    {
        public int MaHD { get; set; }
        public string SoHD { get; set; }
        public int MaKhach { get; set; }
        public int MaND { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal? TongTien { get; set; }
        public string HinhThucThanhToan { get; set; }
        public decimal? SoTienDaTra { get; set; }
        public decimal? SoTienConNo { get; set; }

        public HoaDon()
        {
            MaHD = 0;
            SoHD = string.Empty;
            MaKhach = 0;
            MaND = 0;
            NgayLap = DateTime.Now;
            TongTien = 0;
            HinhThucThanhToan = string.Empty;
            SoTienDaTra = 0;
            SoTienConNo = 0;
        }

        public HoaDon(string soHD, DateTime ngayLap, decimal? tongTien, string hinhThucThanhToan, decimal? soTienDaTra, decimal? soTienConNo)
        {
            SoHD = soHD;
            NgayLap = ngayLap;
            TongTien = tongTien;
            HinhThucThanhToan = hinhThucThanhToan;
            SoTienDaTra = soTienDaTra;
            SoTienConNo = soTienConNo;
        }
    }
}
