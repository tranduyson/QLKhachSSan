using System;
using System.Data;
using System.Data.SqlClient;
using DAL;
using HotelManagement.Module;

namespace DAL
{using HotelManagement.Module;

    public class HoaDonChiTietDAL
    {
        public DataTable GetAll()
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_HoaDonChiTiet_DanhSach", DatabaseConnect.Conn);
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

        public DataTable GetById(int maCTHD)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_HoaDonChiTiet_ChiTiet", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaCTHD", maCTHD);
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

        public string Them(HoaDonChiTiet obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();

                using (SqlCommand cmd = new SqlCommand("sp_HoaDonChiTiet_Them", DatabaseConnect.Conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaHD", obj.MaHD);
                    cmd.Parameters.AddWithValue("@MaDatPhong", obj.MaDatPhong ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaDV", obj.MaDV ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SoLuong", obj.SoLuong);
                    cmd.Parameters.AddWithValue("@DonGia", obj.DonGia);

                    cmd.ExecuteNonQuery();
                }

                DatabaseConnect.CloseDatabase();
                return "Thêm HoaDonChiTiet thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }



        public string Sua(HoaDonChiTiet obj)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                using (SqlCommand cmd = new SqlCommand("sp_HoaDonChiTiet_Sua", DatabaseConnect.Conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaCTHD", obj.MaCTHD);
                    cmd.Parameters.AddWithValue("@MaHD", obj.MaHD);

                    cmd.Parameters.AddWithValue("@MaDatPhong",
                        obj.MaDatPhong.HasValue ? (object)obj.MaDatPhong.Value : DBNull.Value);

                    cmd.Parameters.AddWithValue("@MaDV",
                        obj.MaDV.HasValue ? (object)obj.MaDV.Value : DBNull.Value);

                    cmd.Parameters.AddWithValue("@SoLuong", obj.SoLuong);
                    cmd.Parameters.AddWithValue("@DonGia", obj.DonGia);

                    cmd.ExecuteNonQuery();
                }

                DatabaseConnect.CloseDatabase();
                return "Sửa HoaDonChiTiet thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }



        public string Xoa(int maCTHD)
        {
            try
            {
                DatabaseConnect.OpenDatabase();
                SqlCommand cmd = new SqlCommand("sp_HoaDonChiTiet_Xoa", DatabaseConnect.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaCTHD", maCTHD);
                cmd.ExecuteNonQuery();
                DatabaseConnect.CloseDatabase();
                return "Xóa HoaDonChiTiet thành công";
            }
            catch (Exception ex)
            {
                DatabaseConnect.CloseDatabase();
                return "Lỗi: " + ex.Message;
            }
        }
    }
}
