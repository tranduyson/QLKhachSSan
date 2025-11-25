namespace QLKhachSanApi.Models
{
    public class ChiTietDatPhong
    {
        public int MaCT { get; set; }

        public int MaDatPhong { get; set; }

        public int MaPhong { get; set; }

        public string SoPhong { get; set; } 

        public int MaLoaiPhong { get; set; } 

        public string TenLoaiPhong { get; set; } 

        public decimal DonGia { get; set; }

        public int SoDem { get; set; }

        public decimal ThanhTien { get; set; }
    }
}