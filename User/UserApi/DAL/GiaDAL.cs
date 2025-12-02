using System;
using System.Data;
using System.Data.SqlClient;
using DAL;
using HotelManagement.Module;

namespace DAL
{
    public class GiaDAL
    {
        public DataTable GetAll()
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_Gia_DanhSach", DatabaseConnect.Conn);
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

        public DataTable GetById(int maGia)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_Gia_ChiTiet", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaGia", maGia);
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

        public string Them(Gia obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_Gia_Them", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaLoaiPhong", obj.MaLoaiPhong);
                cmd.Parameters.AddWithValue("@TuNgay", obj.TuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", obj.DenNgay);
                cmd.Parameters.AddWithValue("@GiaMoiDem", obj.GiaMoiDem);
                cmd.Parameters.AddWithValue("@GiaMoiGio", obj.GiaMoiGio);
                cmd.Parameters.AddWithValue("@GhiChu", obj.GhiChu);
                cmd.ExecuteNonQuery();
                DatabaseConnect.CloseDatabase();
                return "Thêm Gia thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }

        public string Sua(Gia obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_Gia_Sua", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaGia", obj.MaGia);
                cmd.Parameters.AddWithValue("@MaLoaiPhong", obj.MaLoaiPhong);
                cmd.Parameters.AddWithValue("@TuNgay", obj.TuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", obj.DenNgay);
                cmd.Parameters.AddWithValue("@GiaMoiDem", obj.GiaMoiDem);
                cmd.Parameters.AddWithValue("@GiaMoiGio", obj.GiaMoiGio);
                cmd.Parameters.AddWithValue("@GhiChu", obj.GhiChu);
                cmd.ExecuteNonQuery();
                DatabaseConnect.CloseDatabase();
                return "Sửa Gia thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }

        public string Xoa(int maGia)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_Gia_Xoa", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaGia", maGia);
                cmd.ExecuteNonQuery();
                DatabaseConnect.CloseDatabase();
                return "Xóa Gia thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }
    }
}
