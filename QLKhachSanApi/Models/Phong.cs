namespace QLKhachSanApi.Models
{
    public class Phong
    {
        public int MaPhong { get; set; }

        public int Id => MaPhong;

        public string SoPhong { get; set; }

        public int MaLoaiPhong { get; set; }

        public string TinhTrang { get; set; } = "Trống";
    }

    public class PhongCreate
    {
        public string SoPhong { get; set; }

        public int MaLoaiPhong { get; set; }

        public string TinhTrang { get; set; } = "Trống";
    }
}