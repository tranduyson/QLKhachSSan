using System;

namespace HotelManagement.Module
{
    public class Gia
    {
        public int MaGia { get; set; }
        public int MaLoaiPhong { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public decimal GiaMoiDem { get; set; }
        public decimal? GiaMoiGio { get; set; }
        public string GhiChu { get; set; }

        public Gia()
        {
            MaGia = 0;
            MaLoaiPhong = 0;
            TuNgay = DateTime.Now;
            DenNgay = DateTime.Now;

            GiaMoiDem = 0;
            GiaMoiGio = 0;
            GhiChu = string.Empty;
        }

        public Gia(DateTime tuNgay, DateTime denNgay, decimal giaMoiDem, decimal? giaMoiGio, string ghiChu)

        {
            TuNgay = tuNgay;
            DenNgay = denNgay;
            GiaMoiDem = giaMoiDem;
            GiaMoiGio = giaMoiGio;
            GhiChu = ghiChu;
        }
    }
}
