using System.Data;
using System.Data.SqlClient;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly DatabaseHelper _db;

        public KhachHangService(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<KhachHang>> GetAllAsync()
        {
            var results = new List<KhachHang>();
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT MaKhachHang, HoTen, SoDienThoai, CCCD, DiaChi, GhiChu FROM KhachHang", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(Map(reader));
            }
            return results;
        }

        public async Task<KhachHang?> GetByIdAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT MaKhachHang, HoTen, SoDienThoai, CCCD, DiaChi, GhiChu FROM KhachHang WHERE MaKhachHang=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                return Map(reader);
            }
            return null;
        }

        public async Task<KhachHang> AddAsync(KhachHang entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(@"INSERT INTO KhachHang (HoTen, SoDienThoai, CCCD, DiaChi, GhiChu)
OUTPUT INSERTED.MaKhachHang VALUES (@HoTen, @SoDienThoai, @CCCD, @DiaChi, @GhiChu)", conn);
            cmd.Parameters.AddWithValue("@HoTen", entity.HoTen);
            cmd.Parameters.AddWithValue("@SoDienThoai", (object?)entity.SoDienThoai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CCCD", (object?)entity.CCCD ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DiaChi", (object?)entity.DiaChi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GhiChu", (object?)entity.GhiChu ?? DBNull.Value);
            await conn.OpenAsync();
            entity.MaKhachHang = (int)await cmd.ExecuteScalarAsync();
            return entity;
        }

        public async Task UpdateAsync(KhachHang entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(@"UPDATE KhachHang SET HoTen=@HoTen, SoDienThoai=@SoDienThoai, CCCD=@CCCD, DiaChi=@DiaChi, GhiChu=@GhiChu WHERE MaKhachHang=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", entity.MaKhachHang);
            cmd.Parameters.AddWithValue("@HoTen", entity.HoTen);
            cmd.Parameters.AddWithValue("@SoDienThoai", (object?)entity.SoDienThoai ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CCCD", (object?)entity.CCCD ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DiaChi", (object?)entity.DiaChi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GhiChu", (object?)entity.GhiChu ?? DBNull.Value);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("DELETE FROM KhachHang WHERE MaKhachHang=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT 1 FROM KhachHang WHERE MaKhachHang=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        private static KhachHang Map(SqlDataReader reader)
        {
            return new KhachHang
            {
                MaKhachHang = reader.GetInt32(reader.GetOrdinal("MaKhachHang")),
                HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                SoDienThoai = reader.IsDBNull(reader.GetOrdinal("SoDienThoai")) ? null : reader.GetString(reader.GetOrdinal("SoDienThoai")),
                CCCD = reader.IsDBNull(reader.GetOrdinal("CCCD")) ? null : reader.GetString(reader.GetOrdinal("CCCD")),
                DiaChi = reader.IsDBNull(reader.GetOrdinal("DiaChi")) ? null : reader.GetString(reader.GetOrdinal("DiaChi")),
                GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu"))
            };
        }
    }
}


