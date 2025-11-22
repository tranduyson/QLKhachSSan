namespace QLKhachSanApi.Models
{
    public class SuDungDichVu
    {
        public int MaSuDung { get; set; }

        public int MaDatPhong { get; set; }

        public int MaDichVu { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }
        public string TenDichVu { get; set; }
    }
}