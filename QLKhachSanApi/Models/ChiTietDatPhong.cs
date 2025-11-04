using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLKhachSanApi.Models
{
    [Table("ChiTietDatPhong")]
    public class ChiTietDatPhong
    {
        [Key]
        public int MaCT { get; set; }

        [Required]
        public int MaDatPhong { get; set; }

        [Required]
        public int MaPhong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Required]
        public int SoDem { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        // Navigation properties
        [ForeignKey("MaDatPhong")]
        public DatPhong DatPhong { get; set; }

        [ForeignKey("MaPhong")]
        public Phong Phong { get; set; }
    }
}