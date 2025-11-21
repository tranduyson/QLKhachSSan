namespace QLKhachSanApi.Models
{
    public class DatPhong
    {
        public int MaDatPhong { get; set; }

        public int MaKhachHang { get; set; }

        public int? MaNhanVien { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        public DateTime NgayNhan { get; set; }

        public DateTime NgayTra { get; set; }

        public decimal TongTien { get; set; } = 0;

        public string TrangThai { get; set; } = "Đã đặt";

        // Related objects (populated by ADO repositories)
        public KhachHang? KhachHang { get; set; }
        public NhanVien? NhanVien { get; set; }

        // Related collections (populated by ADO repositories)
        public List<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
        public List<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
        public List<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
    }
}