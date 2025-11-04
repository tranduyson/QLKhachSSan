using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLKhachSanApi.Models
{
    [Table("DatPhong")]
    public class DatPhong
    {
        [Key]
        public int MaDatPhong { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        public int? MaNhanVien { get; set; }

        [Required]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Required]
        public DateTime NgayNhan { get; set; }

        [Required]
        public DateTime NgayTra { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; } = 0;

        [StringLength(50)]
        public string TrangThai { get; set; } = "Đã đặt";

        // Navigation properties
        [ForeignKey("MaKhachHang")]
        public KhachHang KhachHang { get; set; }

        [ForeignKey("MaNhanVien")]
        public NhanVien? NhanVien { get; set; }

        [JsonIgnore]
        public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
        [JsonIgnore]
        public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
        [JsonIgnore]
        public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
    }
}