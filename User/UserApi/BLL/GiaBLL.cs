using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class GiaBLL
    {
        private GiaDAL _dal;

        public GiaBLL()
        {
            _dal = new GiaDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        public DataTable GetById(int maGia)
        {
            return _dal.GetById(maGia);
        }

        public string Them(Gia obj)
        {
            if (obj == null)
                return "Thông tin Gia không hợp lệ";

            // Validate MaLoaiPhong
            if (obj.MaLoaiPhong <= 0)
                return "Lỗi: Mã loại phòng không hợp lệ";

            // Validate khoảng thời gian
            if (obj.TuNgay >= obj.DenNgay)
                return "Lỗi: Từ ngày phải nhỏ hơn đến ngày";

            // Validate thời gian không được trong quá khứ
            if (obj.TuNgay.Date < DateTime.Now.Date)
                return "Lỗi: Từ ngày không được trong quá khứ";

            // Validate giá
            if (obj.GiaMoiDem <= 0)
                return "Lỗi: Giá mỗi đêm phải lớn hơn 0";

            if (obj.GiaMoiGio.HasValue && obj.GiaMoiGio.Value <= 0)
                return "Lỗi: Giá mỗi giờ phải lớn hơn 0";

            // Kiểm tra trùng lặp khoảng thời gian
            if (KiemTraTrungKhoangThoiGian(obj))
                return "Lỗi: Đã tồn tại giá cho loại phòng này trong khoảng thời gian trùng lặp";


            return _dal.Them(obj);
        }

        public string Sua(Gia obj)
        {
            if (obj == null)
                return "Thông tin Gia không hợp lệ";

            if (obj.MaGia <= 0)
                return "Lỗi: Mã giá không hợp lệ";

            // Validate MaLoaiPhong
            if (obj.MaLoaiPhong <= 0)
                return "Lỗi: Mã loại phòng không hợp lệ";

            // Validate khoảng thời gian
            if (obj.TuNgay >= obj.DenNgay)
                return "Lỗi: Từ ngày phải nhỏ hơn đến ngày";

            // Validate giá
            if (obj.GiaMoiDem <= 0)
                return "Lỗi: Giá mỗi đêm phải lớn hơn 0";

            if (obj.GiaMoiGio.HasValue && obj.GiaMoiGio.Value <= 0)
                return "Lỗi: Giá mỗi giờ phải lớn hơn 0";

            // Kiểm tra trùng lặp (loại trừ bản ghi hiện tại)
            if (KiemTraTrungKhoangThoiGian(obj, true))
                return "Lỗi: Đã tồn tại giá cho loại phòng này trong khoảng thời gian trùng lặp";


            return _dal.Sua(obj);
        }

        public string Xoa(int maGia)
        {
            if (maGia <= 0)
                return "Lỗi: Mã giá không hợp lệ"; 
            return _dal.Xoa(maGia);
        }

        private bool KiemTraTrungKhoangThoiGian(Gia obj, bool laSua = false)
        {
            try
            {
                DataTable dt = _dal.GetAll();

                foreach (DataRow row in dt.Rows)
                {
                    // Nếu là sửa, bỏ qua bản ghi hiện tại
                    if (laSua && Convert.ToInt32(row["MaGia"]) == obj.MaGia)
                        continue;

                    // Chỉ kiểm tra cùng loại phòng
                    if (Convert.ToInt32(row["MaLoaiPhong"]) != obj.MaLoaiPhong)
                        continue;

                    DateTime tuNgayDb = Convert.ToDateTime(row["TuNgay"]);
                    DateTime denNgayDb = Convert.ToDateTime(row["DenNgay"]);

                    // Kiểm tra trùng lặp:
                    // Trùng nếu: (TuNgay mới < DenNgay cũ) VÀ (DenNgay mới > TuNgay cũ)
                    if (obj.TuNgay < denNgayDb && obj.DenNgay > tuNgayDb)
                    {
                        return true; // Có trùng lặp
                    }
                }

                return false; // Không trùng
            }
            catch
            {
                return false; // Nếu lỗi thì cho phép thêm/sửa
            }
        }
    }
}
