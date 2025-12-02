using System;

namespace HotelManagement.Module

{
    public class Phong
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public int MaLoaiPhong { get; set; }
        public string TinhTrang { get; set; }

        // Constructor mặc định
        public Phong()
        {
            MaPhong = 0;
            SoPhong = string.Empty;
            MaLoaiPhong = 0;
            TinhTrang = string.Empty;
        }

        // Constructor có tham số
        public Phong(string soPhong, string tinhTrang)
        {
            SoPhong = soPhong;
            TinhTrang = tinhTrang;
        }
    }
}
