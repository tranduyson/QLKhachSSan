namespace QLKhachSanApi.Models
{
    public class ThanhToan
    {
        public int MaThanhToan { get; set; }

        public int MaDatPhong { get; set; }

        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        public decimal SoTien { get; set; }

        public string? PhuongThuc { get; set; }

        public string? GhiChu { get; set; }

        public bool TrangThai { get; set; }

        // Populated by ADO controllers when needed
        // Ignore during JSON (de)serialization to avoid client sending nested DatPhong objects
        // which cause binding errors when they are sent as ids or other shapes.
        [System.Text.Json.Serialization.JsonIgnore]
        public DatPhong? DatPhong { get; set; }
    }
}
