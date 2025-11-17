using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLKhachSanApi.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        public int MaNhanVien { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(100)]
        public string MatKhau { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(50)]
        public string? ChucVu { get; set; }

        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        public int TrangThai { get; set; }

        // Navigation property
        //[JsonIgnore]
        public ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
    }
}