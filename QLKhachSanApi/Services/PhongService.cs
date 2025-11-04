using System.Data;
using System.Data.SqlClient;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public class PhongService : IPhongService
    {
        private readonly DatabaseHelper _db;

        public PhongService(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Phong>> GetAllWithLoaiPhongAsync()
        {
            var results = new List<Phong>();
            using var conn = _db.GetConnection();
            var sql = @"SELECT p.MaPhong, p.SoPhong, p.MaLoaiPhong, p.TinhTrang,
                               lp.MaLoaiPhong AS LP_MaLoaiPhong, lp.TenLoaiPhong, lp.GiaMoiDem, lp.MoTa, lp.SoGiuong, lp.DienTich
                        FROM Phong p
                        JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong";
            using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapPhongWithLoai(reader));
            }
            return results;
        }

        public async Task<Phong?> GetByIdWithLoaiPhongAsync(int id)
        {
            using var conn = _db.GetConnection();
            var sql = @"SELECT p.MaPhong, p.SoPhong, p.MaLoaiPhong, p.TinhTrang,
                               lp.MaLoaiPhong AS LP_MaLoaiPhong, lp.TenLoaiPhong, lp.GiaMoiDem, lp.MoTa, lp.SoGiuong, lp.DienTich
                        FROM Phong p
                        JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
                        WHERE p.MaPhong=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                return MapPhongWithLoai(reader);
            }
            return null;
        }

        public async Task<IEnumerable<Phong>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var results = new List<Phong>();
            using var conn = _db.GetConnection();
            var sql = @"
                ;WITH Booked AS (
                    SELECT DISTINCT ct.MaPhong
                    FROM ChiTietDatPhong ct
                    JOIN DatPhong dp ON dp.MaDatPhong = ct.MaDatPhong
                    WHERE dp.TrangThai <> N'Hủy'
                      AND (dp.NgayNhan <= @checkOut AND dp.NgayTra >= @checkIn)
                )
                SELECT p.MaPhong, p.SoPhong, p.MaLoaiPhong, p.TinhTrang,
                       lp.MaLoaiPhong AS LP_MaLoaiPhong, lp.TenLoaiPhong, lp.GiaMoiDem, lp.MoTa, lp.SoGiuong, lp.DienTich
                FROM Phong p
                JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
                WHERE p.TinhTrang = N'Trống' AND p.MaPhong NOT IN (SELECT MaPhong FROM Booked)
            ";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@checkIn", SqlDbType.DateTime2) { Value = checkIn });
            cmd.Parameters.Add(new SqlParameter("@checkOut", SqlDbType.DateTime2) { Value = checkOut });
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapPhongWithLoai(reader));
            }
            return results;
        }

        public async Task<Phong> AddAsync(Phong entity)
        {
            using var conn = _db.GetConnection();
            var sql = @"INSERT INTO Phong (SoPhong, MaLoaiPhong, TinhTrang)
                        OUTPUT INSERTED.MaPhong
                        VALUES (@SoPhong, @MaLoaiPhong, @TinhTrang)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SoPhong", entity.SoPhong);
            cmd.Parameters.AddWithValue("@MaLoaiPhong", entity.MaLoaiPhong);
            cmd.Parameters.AddWithValue("@TinhTrang", entity.TinhTrang);
            await conn.OpenAsync();
            entity.MaPhong = (int)await cmd.ExecuteScalarAsync();
            return entity;
        }

        public async Task UpdateAsync(Phong entity)
        {
            using var conn = _db.GetConnection();
            var sql = @"UPDATE Phong SET SoPhong=@SoPhong, MaLoaiPhong=@MaLoaiPhong, TinhTrang=@TinhTrang WHERE MaPhong=@Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", entity.MaPhong);
            cmd.Parameters.AddWithValue("@SoPhong", entity.SoPhong);
            cmd.Parameters.AddWithValue("@MaLoaiPhong", entity.MaLoaiPhong);
            cmd.Parameters.AddWithValue("@TinhTrang", entity.TinhTrang);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.GetConnection();
            var sql = "DELETE FROM Phong WHERE MaPhong=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = _db.GetConnection();
            var sql = "SELECT 1 FROM Phong WHERE MaPhong=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        private static Phong MapPhongWithLoai(SqlDataReader reader)
        {
            return new Phong
            {
                MaPhong = reader.GetInt32(reader.GetOrdinal("MaPhong")),
                SoPhong = reader.GetString(reader.GetOrdinal("SoPhong")),
                MaLoaiPhong = reader.GetInt32(reader.GetOrdinal("MaLoaiPhong")),
                TinhTrang = reader.GetString(reader.GetOrdinal("TinhTrang")),
                LoaiPhong = new LoaiPhong
                {
                    MaLoaiPhong = reader.GetInt32(reader.GetOrdinal("LP_MaLoaiPhong")),
                    TenLoaiPhong = reader.GetString(reader.GetOrdinal("TenLoaiPhong")),
                    GiaMoiDem = reader.GetDecimal(reader.GetOrdinal("GiaMoiDem")),
                    MoTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? null : reader.GetString(reader.GetOrdinal("MoTa")),
                    SoGiuong = reader.GetInt32(reader.GetOrdinal("SoGiuong")),
                    DienTich = reader.GetDecimal(reader.GetOrdinal("DienTich"))
                }
            };
        }
    }
}


