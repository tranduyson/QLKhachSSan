using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLKhachSanApi.Models
{
    [Table("Phong")]
    public class Phong
    {
        [Key]
        public int MaPhong { get; set; }
        public int Id
        {
            get
            {
                return MaPhong;
            }
        }


        [Required]
        [StringLength(10)]
        public string SoPhong { get; set; }

        [Required]
        public int MaLoaiPhong { get; set; }

        [Required]
        [StringLength(50)]
        public string TinhTrang { get; set; } = "Trống";

        // Navigation properties
        [ForeignKey("MaLoaiPhong")]
        public LoaiPhong LoaiPhong { get; set; }

        public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
    }
}