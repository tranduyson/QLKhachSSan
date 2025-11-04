using System.Data;
using System.Data.SqlClient;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public class ThanhToanService : IThanhToanService
    {
        private readonly DatabaseHelper _db;

        public ThanhToanService(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ThanhToan>> GetAllWithDatPhongAsync()
        {
            var results = new List<ThanhToan>();
            using var conn = _db.GetConnection();
            var sql = @"SELECT tt.MaThanhToan, tt.MaDatPhong, tt.NgayThanhToan, tt.SoTien, tt.PhuongThuc, tt.GhiChu, tt.TrangThai,
                               dp.MaDatPhong AS DP_MaDatPhong, dp.MaKhachHang, dp.MaNhanVien, dp.NgayDat, dp.NgayNhan, dp.NgayTra, dp.TongTien, dp.TrangThai AS DP_TrangThai
                        FROM ThanhToan tt
                        JOIN DatPhong dp ON dp.MaDatPhong = tt.MaDatPhong";
            using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapThanhToanWithDatPhong(reader));
            }
            return results;
        }

        public async Task<ThanhToan?> GetByIdWithDatPhongAsync(int id)
        {
            using var conn = _db.GetConnection();
            var sql = @"SELECT tt.MaThanhToan, tt.MaDatPhong, tt.NgayThanhToan, tt.SoTien, tt.PhuongThuc, tt.GhiChu, tt.TrangThai,
                               dp.MaDatPhong AS DP_MaDatPhong, dp.MaKhachHang, dp.MaNhanVien, dp.NgayDat, dp.NgayNhan, dp.NgayTra, dp.TongTien, dp.TrangThai AS DP_TrangThai
                        FROM ThanhToan tt
                        JOIN DatPhong dp ON dp.MaDatPhong = tt.MaDatPhong
                        WHERE tt.MaThanhToan=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                return MapThanhToanWithDatPhong(reader);
            }
            return null;
        }

        public async Task<IEnumerable<ThanhToan>> GetByDatPhongAsync(int maDatPhong)
        {
            var results = new List<ThanhToan>();
            using var conn = _db.GetConnection();
            var sql = @"SELECT tt.MaThanhToan, tt.MaDatPhong, tt.NgayThanhToan, tt.SoTien, tt.PhuongThuc, tt.GhiChu, tt.TrangThai,
                               dp.MaDatPhong AS DP_MaDatPhong, dp.MaKhachHang, dp.MaNhanVien, dp.NgayDat, dp.NgayNhan, dp.NgayTra, dp.TongTien, dp.TrangThai AS DP_TrangThai
                        FROM ThanhToan tt
                        JOIN DatPhong dp ON dp.MaDatPhong = tt.MaDatPhong
                        WHERE tt.MaDatPhong=@ma";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@ma", SqlDbType.Int) { Value = maDatPhong });
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapThanhToanWithDatPhong(reader));
            }
            return results;
        }

        public async Task<ThanhToan> AddAsync(ThanhToan entity)
        {
            using var conn = _db.GetConnection();
            var sql = @"INSERT INTO ThanhToan (MaDatPhong, NgayThanhToan, SoTien, PhuongThuc, GhiChu, TrangThai)
                        OUTPUT INSERTED.MaThanhToan
                        VALUES (@MaDatPhong, @NgayThanhToan, @SoTien, @PhuongThuc, @GhiChu, @TrangThai)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaDatPhong", entity.MaDatPhong);
            cmd.Parameters.AddWithValue("@NgayThanhToan", entity.NgayThanhToan);
            cmd.Parameters.AddWithValue("@SoTien", entity.SoTien);
            cmd.Parameters.AddWithValue("@PhuongThuc", (object?)entity.PhuongThuc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GhiChu", (object?)entity.GhiChu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TrangThai", entity.TrangThai);
            await conn.OpenAsync();
            entity.MaThanhToan = (int)await cmd.ExecuteScalarAsync();
            return entity;
        }

        public async Task UpdateAsync(ThanhToan entity)
        {
            using var conn = _db.GetConnection();
            var sql = @"UPDATE ThanhToan SET MaDatPhong=@MaDatPhong, NgayThanhToan=@NgayThanhToan, SoTien=@SoTien, PhuongThuc=@PhuongThuc, GhiChu=@GhiChu, TrangThai=@TrangThai WHERE MaThanhToan=@Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", entity.MaThanhToan);
            cmd.Parameters.AddWithValue("@MaDatPhong", entity.MaDatPhong);
            cmd.Parameters.AddWithValue("@NgayThanhToan", entity.NgayThanhToan);
            cmd.Parameters.AddWithValue("@SoTien", entity.SoTien);
            cmd.Parameters.AddWithValue("@PhuongThuc", (object?)entity.PhuongThuc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GhiChu", (object?)entity.GhiChu ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TrangThai", entity.TrangThai);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.GetConnection();
            var sql = "DELETE FROM ThanhToan WHERE MaThanhToan=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = _db.GetConnection();
            var sql = "SELECT 1 FROM ThanhToan WHERE MaThanhToan=@id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        private static ThanhToan MapThanhToanWithDatPhong(SqlDataReader reader)
        {
            return new ThanhToan
            {
                MaThanhToan = reader.GetInt32(reader.GetOrdinal("MaThanhToan")),
                MaDatPhong = reader.GetInt32(reader.GetOrdinal("MaDatPhong")),
                NgayThanhToan = reader.GetDateTime(reader.GetOrdinal("NgayThanhToan")),
                SoTien = reader.GetDecimal(reader.GetOrdinal("SoTien")),
                PhuongThuc = reader.IsDBNull(reader.GetOrdinal("PhuongThuc")) ? null : reader.GetString(reader.GetOrdinal("PhuongThuc")),
                GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu")),
                TrangThai = reader.GetBoolean(reader.GetOrdinal("TrangThai")),
                DatPhong = new DatPhong
                {
                    MaDatPhong = reader.GetInt32(reader.GetOrdinal("DP_MaDatPhong")),
                    MaKhachHang = reader.GetInt32(reader.GetOrdinal("MaKhachHang")),
                    MaNhanVien = reader.IsDBNull(reader.GetOrdinal("MaNhanVien")) ? null : reader.GetInt32(reader.GetOrdinal("MaNhanVien")),
                    NgayDat = reader.GetDateTime(reader.GetOrdinal("NgayDat")),
                    NgayNhan = reader.GetDateTime(reader.GetOrdinal("NgayNhan")),
                    NgayTra = reader.GetDateTime(reader.GetOrdinal("NgayTra")),
                    TongTien = reader.GetDecimal(reader.GetOrdinal("TongTien")),
                    TrangThai = reader.GetString(reader.GetOrdinal("DP_TrangThai"))
                }
            };
        }
    }
}


