using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLKhachSanApi.Models
{

    [Table("ThanhToan")]
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }

        [Required]
        public int MaDatPhong { get; set; }

        [Required]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [StringLength(50)]
        public string? PhuongThuc { get; set; }

        [StringLength(200)]
        public string? GhiChu { get; set; }
        public bool TrangThai { get; set; }

        // Navigation property
        [ForeignKey("MaDatPhong")]
        public DatPhong DatPhong { get; set; }
    }

}
