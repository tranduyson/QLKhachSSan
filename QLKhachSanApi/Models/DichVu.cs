using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLKhachSanApi.Models
{
    [Table("DichVu")]
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDichVu { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [StringLength(50)]
        public string? DonViTinh { get; set; }

        [StringLength(200)]
        public string? GhiChu { get; set; }

        // Navigation property
        public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
    }
}