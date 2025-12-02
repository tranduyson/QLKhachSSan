using System;
using System.Data;
using HotelManagement.Module;
using DAL;

namespace BLL
{
    public class DatPhongBLL
    {
        private DatPhongDAL _dal;

        public DatPhongBLL()
        {
            _dal = new DatPhongDAL();
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        public DataTable GetById(int maDatPhong)
        {
            return _dal.GetById(maDatPhong);
        }

        public string Them(DatPhong obj)
        {
            //if (obj == null)
            //    return "Thông tin DatPhong không hợp lệ";
            //return _dal.Them(obj);
            // Validate cơ bản
            if (obj == null)
                return "Lỗi: Thông tin đặt phòng không hợp lệ";

            // Validate ngày tháng
            if (obj.NgayNhan >= obj.NgayTra)
                return "Lỗi: Ngày nhận phòng phải nhỏ hơn ngày trả phòng";

            // Validate ngày nhận không được trong quá khứ (chỉ cảnh báo cho đặt trước)
            if (obj.NgayNhan.Date < DateTime.Now.Date)
                return "Lỗi: Ngày nhận phòng không được trong quá khứ";

            // Validate số khách
            if (obj.SoKhach <= 0)
                return "Lỗi: Số khách phải lớn hơn 0";

            // Validate mã khách
            if (obj.MaKhach <= 0)
                return "Lỗi: Mã khách hàng không hợp lệ";

            // Validate mã loại phòng
            if (obj.MaLoaiPhong <= 0)
                return "Lỗi: Mã loại phòng không hợp lệ";

            // Validate trạng thái
            if (string.IsNullOrWhiteSpace(obj.TrangThai))
                return "Lỗi: Trạng thái không được để trống";

            // Nếu đã chọn phòng cụ thể, kiểm tra trùng lịch
            if (obj.MaPhong.HasValue && obj.MaPhong.Value > 0)
            {
                if (KiemTraPhongTrungLich(obj))
                    return "Lỗi: Phòng đã được đặt trong khoảng thời gian này";
            }

            return _dal.Them(obj);
        }

        public string Sua(DatPhong obj)
        {
            if (obj == null)
                return "Thông tin DatPhong không hợp lệ";
            if (obj.MaDatPhong <= 0)
                return "Lỗi: Mã đặt phòng không hợp lệ";

            // Validate ngày tháng
            if (obj.NgayNhan >= obj.NgayTra)
                return "Lỗi: Ngày nhận phòng phải nhỏ hơn ngày trả phòng";

            // Validate số khách
            if (obj.SoKhach <= 0)
                return "Lỗi: Số khách phải lớn hơn 0";

            // Validate mã khách
            if (obj.MaKhach <= 0)
                return "Lỗi: Mã khách hàng không hợp lệ";

            // Validate mã loại phòng
            if (obj.MaLoaiPhong <= 0)
                return "Lỗi: Mã loại phòng không hợp lệ";

            // Validate trạng thái
            if (string.IsNullOrWhiteSpace(obj.TrangThai))
                return "Lỗi: Trạng thái không được để trống";

            // Nếu đã chọn phòng, kiểm tra trùng lịch (loại trừ bản ghi hiện tại)
            if (obj.MaPhong.HasValue && obj.MaPhong.Value > 0)
            {
                if (KiemTraPhongTrungLich(obj, true))
                    return "Lỗi: Phòng đã được đặt trong khoảng thời gian này";
            }
            return _dal.Sua(obj);
        }

        public string Xoa(int maDatPhong)
        {
            if (maDatPhong <= 0)
                return "Lỗi: Mã đặt phòng không hợp lệ";

            // Có thể thêm logic kiểm tra: không cho xóa nếu đã CheckIn
            try
            {
                DataTable dt = _dal.GetById(maDatPhong);
                if (dt.Rows.Count > 0)
                {
                    string trangThai = dt.Rows[0]["TrangThai"].ToString();
                    if (trangThai == "CheckedIn" || trangThai == "CheckedOut")
                        return "Lỗi: Không thể xóa đơn đặt phòng đã check-in hoặc check-out";
                }
            }
            catch { }
            return _dal.Xoa(maDatPhong);
        }

        public string CheckIn(int maDatPhong, int? maPhong)
        {
            if (maDatPhong <= 0)
                return "Lỗi: Mã đặt phòng không hợp lệ";

            // Kiểm tra trạng thái
            try
            {
                DataTable dt = _dal.GetById(maDatPhong);
                if (dt.Rows.Count == 0)
                    return "Lỗi: Không tìm thấy đơn đặt phòng";

                string trangThai = dt.Rows[0]["TrangThai"].ToString();
                if (trangThai == "CheckedIn")
                    return "Lỗi: Đơn đặt phòng đã được check-in";
                if (trangThai == "CheckedOut")
                    return "Lỗi: Đơn đặt phòng đã được check-out";
                if (trangThai == "Cancelled")
                    return "Lỗi: Đơn đặt phòng đã bị hủy";

                // Nếu truyền vào mã phòng, kiểm tra phòng có trùng không
                if (maPhong.HasValue && maPhong.Value > 0)
                {
                    var checkObj = new DatPhong
                    {
                        MaDatPhong = maDatPhong,
                        MaPhong = maPhong,
                        NgayNhan = Convert.ToDateTime(dt.Rows[0]["NgayNhan"]),
                        NgayTra = Convert.ToDateTime(dt.Rows[0]["NgayTra"])
                    };

                    if (KiemTraPhongTrungLich(checkObj, true))
                        return "Lỗi: Phòng đã được đặt trong khoảng thời gian này";
                }
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
            return _dal.CheckIn(maDatPhong, maPhong);
        }

        public string CheckOut(int maDatPhong)
        {
            if (maDatPhong <= 0)
                return "Lỗi: Mã đặt phòng không hợp lệ";

            // Kiểm tra trạng thái
            try
            {
                DataTable dt = _dal.GetById(maDatPhong);
                if (dt.Rows.Count == 0)
                    return "Lỗi: Không tìm thấy đơn đặt phòng";

                string trangThai = dt.Rows[0]["TrangThai"].ToString();
                if (trangThai != "CheckedIn")
                    return "Lỗi: Chỉ có thể check-out đơn đã check-in";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
            
            return _dal.CheckOut(maDatPhong);
        }

        private bool KiemTraPhongTrungLich(DatPhong obj, bool laSua = false)
        {
            if (!obj.MaPhong.HasValue || obj.MaPhong.Value <= 0)
                return false;

            try
            {
                DataTable dt = _dal.GetAll();

                foreach (DataRow row in dt.Rows)
                {
                    // Bỏ qua bản ghi hiện tại nếu đang sửa
                    if (laSua && Convert.ToInt32(row["MaDatPhong"]) == obj.MaDatPhong)
                        continue;

                    // Chỉ kiểm tra cùng phòng
                    if (row["MaPhong"] == DBNull.Value)
                        continue;

                    int maPhongDb = Convert.ToInt32(row["MaPhong"]);
                    if (maPhongDb != obj.MaPhong.Value)
                        continue;

                    // Bỏ qua nếu đã hủy hoặc đã check-out
                    string trangThaiDb = row["TrangThai"].ToString();
                    if (trangThaiDb == "Cancelled" || trangThaiDb == "CheckedOut")
                        continue;

                    DateTime ngayNhanDb = Convert.ToDateTime(row["NgayNhan"]);
                    DateTime ngayTraDb = Convert.ToDateTime(row["NgayTra"]);

                    // Kiểm tra trùng lặp thời gian
                    if (obj.NgayNhan < ngayTraDb && obj.NgayTra > ngayNhanDb)
                    {
                        return true; // Có trùng lịch
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
