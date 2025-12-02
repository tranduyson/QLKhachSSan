using System;
using System.Data;
using System.Data.SqlClient;
using HotelManagement.Module;
using DAL;

namespace DAL
{
    public class DatPhongDAL
    {
        public DataTable GetAll()
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_DatPhong_DanhSach", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách: " + ex.Message);
            }
            finally
            {
                DatabaseConnect.CloseDatabase();
            }
        }

        public DataTable GetById(int maDatPhong)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_DatPhong_ChiTiet", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaDatPhong", maDatPhong);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy chi tiết: " + ex.Message);
            }
            finally
            {
                DatabaseConnect.CloseDatabase();
            }
        }

        public string Them(DatPhong obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_DatPhong_Them", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaDat", obj.MaDat);
                cmd.Parameters.AddWithValue("@MaKhach", obj.MaKhach);
                cmd.Parameters.AddWithValue("@MaPhong", (object)obj.MaPhong ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaLoaiPhong", obj.MaLoaiPhong);
                cmd.Parameters.AddWithValue("@NgayNhan", obj.NgayNhan);
                cmd.Parameters.AddWithValue("@NgayTra", obj.NgayTra);
                cmd.Parameters.AddWithValue("@SoKhach", obj.SoKhach);
                cmd.Parameters.AddWithValue("@TrangThai", obj.TrangThai);
                cmd.Parameters.AddWithValue("@NguoiTao", (object)obj.NguoiTao ?? DBNull.Value);
                //cmd.Parameters.AddWithValue("@NgayTao", obj.NgayTao == default(DateTime) ? (object)DBNull.Value : obj.NgayTao);
                cmd.Parameters.AddWithValue("@GhiChu", (object)obj.GhiChu ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                return "Thêm DatPhong thành công";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
            finally
            {
                DatabaseConnect.CloseDatabase();
            }
        }

        public string Sua(DatPhong obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_DatPhong_Sua", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaDatPhong", obj.MaDatPhong);
                cmd.Parameters.AddWithValue("@MaDat", obj.MaDat);
                cmd.Parameters.AddWithValue("@MaKhach", obj.MaKhach);
                cmd.Parameters.AddWithValue("@MaPhong", (object)obj.MaPhong ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaLoaiPhong", obj.MaLoaiPhong);
                cmd.Parameters.AddWithValue("@NgayNhan", obj.NgayNhan);
                cmd.Parameters.AddWithValue("@NgayTra", obj.NgayTra);
                cmd.Parameters.AddWithValue("@SoKhach", obj.SoKhach);
                cmd.Parameters.AddWithValue("@TrangThai", obj.TrangThai);
                cmd.Parameters.AddWithValue("@NguoiTao", (object)obj.NguoiTao ?? DBNull.Value);
                //cmd.Parameters.AddWithValue("@NgayTao", obj.NgayTao == default(DateTime) ? (object)DBNull.Value : obj.NgayTao);
                cmd.Parameters.AddWithValue("@GhiChu", (object)obj.GhiChu ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                return "Sửa DatPhong thành công";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
            finally
            {
                DatabaseConnect.CloseDatabase();
            }
        }

        public string Xoa(int maDatPhong)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_DatPhong_Xoa", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaDatPhong", maDatPhong);
                cmd.ExecuteNonQuery();
                return "Xóa DatPhong thành công";
            }
            catch (Exception ex)
            {
                return "Lỗi: " + ex.Message;
            }
            finally
            {
                DatabaseConnect.CloseDatabase();
            }
        }

        public string CheckIn(int maDatPhong, int? maPhong)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                using var cmd = new SqlCommand("sp_DatPhong_CheckIn", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaDatPhong", maDatPhong);
                cmd.Parameters.AddWithValue("@MaPhong", (object)maPhong ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                return "Check-in thành công";
            }
            catch (Exception ex) { return "Lỗi: " + ex.Message; }
            finally { DatabaseConnect.CloseDatabase(); }
        }

        public string CheckOut(int maDatPhong)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                using var cmd = new SqlCommand("sp_DatPhong_CheckOut", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaDatPhong", maDatPhong);
                cmd.ExecuteNonQuery();
                return "Check-out thành công";
            }
            catch (Exception ex) { return "Lỗi: " + ex.Message; }
            finally { DatabaseConnect.CloseDatabase(); }
        }
    }
}
