using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLKhachSanApi.Models
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        [StringLength(20)]
        public string? CCCD { get; set; }

        [StringLength(200)]
        public string? DiaChi { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        // Navigation property
        [JsonIgnore]
        public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
    }
}