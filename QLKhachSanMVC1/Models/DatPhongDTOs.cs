namespace HotelManagement.Models
{
    public class DatPhongRequest
    {
        public int MaKhachHang { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public string? TrangThai { get; set; }
        public List<ChiTietDatPhongRequest> ChiTietDatPhongs { get; set; } = new();
        public List<SuDungDichVuRequest>? SuDungDichVus { get; set; }
    }

    public class ChiTietDatPhongRequest
    {
        public int MaPhong { get; set; }
        public decimal DonGia { get; set; }
        public int SoDem { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class SuDungDichVuRequest
    {
        public int MaDichVu { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}