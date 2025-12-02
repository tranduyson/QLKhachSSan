using System;
using System.Data;
using System.Data.SqlClient;
using DAL;
using HotelManagement.Module;

namespace DAL
{
    public class NguoiDungDAL
    {
        public DataTable GetAll()
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_NguoiDung_DanhSach", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                DatabaseConnect.CloseDatabase();
                return dt;
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                throw new Exception("Lỗi khi lấy danh sách: " + ex.Message);
            }
        }
        public string DoiMatKhau(string tenDangNhap, string matKhauHienTai, string matKhauMoi)
        {
            try
            {
                DatabaseConnect.OpenDatabase();

                using (SqlCommand cmd = new SqlCommand("sp_DoiMatKhau", DatabaseConnect.Conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                    cmd.Parameters.AddWithValue("@MatKhauHienTai", matKhauHienTai);
                    cmd.Parameters.AddWithValue("@MatKhauMoi", matKhauMoi);

                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Không có phản hồi từ server";
                }
            }
            catch (Exception ex)
            {
                return "Lỗi khi đổi mật khẩu: " + ex.Message;
            }
            finally
            {
               
                DatabaseConnect.CloseDatabase();
            }
        }


        public DataTable GetById(int maND)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_NguoiDung_ChiTiet", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaND", maND);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                DatabaseConnect.CloseDatabase();
                return dt;
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                throw new Exception("Lỗi khi lấy chi tiết: " + ex.Message);
            }
        }

        public string Them(NguoiDung obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_NguoiDung_Them", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TenDangNhap", obj.TenDangNhap);
                cmd.Parameters.AddWithValue("@MatKhau", obj.MatKhau);
                cmd.Parameters.AddWithValue("@HoTen", obj.HoTen);
                cmd.Parameters.AddWithValue("@VaiTro", obj.VaiTro);
                cmd.ExecuteNonQuery();
                DatabaseConnect.CloseDatabase();
                return "Thêm NguoiDung thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }

        public string Sua(NguoiDung obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_NguoiDung_Sua", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaND", obj.MaND);
                cmd.Parameters.AddWithValue("@TenDangNhap", obj.TenDangNhap);
                cmd.Parameters.AddWithValue("@MatKhau", obj.MatKhau);
                cmd.Parameters.AddWithValue("@HoTen", obj.HoTen);
                cmd.Parameters.AddWithValue("@VaiTro", obj.VaiTro);
                cmd.ExecuteNonQuery();
                DatabaseConnect.CloseDatabase();
                return "Sửa NguoiDung thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }

        public string Xoa(int maND)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_NguoiDung_Xoa", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaND", maND);
                cmd.ExecuteNonQuery();
                DatabaseConnect.CloseDatabase();
                return "Xóa NguoiDung thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }
    }
}
