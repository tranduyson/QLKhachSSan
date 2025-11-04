using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QLKhachSanApi.Models
{
    [Table("LoaiPhong")]
    public class LoaiPhong
    {
        [Key]
        public int MaLoaiPhong { get; set; }

        [Required]
        [StringLength(100)]
        public string TenLoaiPhong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaMoiDem { get; set; }

        [StringLength(500)]
        public string? MoTa { get; set; }
        public int SoGiuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DienTich  { get; set; }

        // Navigation property
        [JsonIgnore]
        public ICollection<Phong> Phongs { get; set; } = new List<Phong>();
    }
}