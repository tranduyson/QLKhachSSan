using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class KhachHangAdoRepository : IRepository<KhachHang>
    {
        private readonly DatabaseHelper _db;

        public KhachHangAdoRepository(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<KhachHang>> GetAllAsync()
        {
            var list = new List<KhachHang>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaKhachHang, HoTen, SoDienThoai, CCCD, DiaChi, GhiChu FROM KhachHang", conn))
            {
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        public async Task<KhachHang?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaKhachHang, HoTen, SoDienThoai, CCCD, DiaChi, GhiChu FROM KhachHang WHERE MaKhachHang=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return null;
        }

        public Task<IEnumerable<KhachHang>> FindAsync(System.Linq.Expressions.Expression<Func<KhachHang, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<KhachHang> AddAsync(KhachHang e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO KhachHang(HoTen, SoDienThoai, CCCD, DiaChi, GhiChu)
OUTPUT INSERTED.MaKhachHang, INSERTED.HoTen, INSERTED.SoDienThoai, INSERTED.CCCD, INSERTED.DiaChi, INSERTED.GhiChu
VALUES (@HoTen,@SoDienThoai,@CCCD,@DiaChi,@GhiChu)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = e.HoTen });
                cmd.Parameters.Add(new SqlParameter("@SoDienThoai", SqlDbType.NVarChar, 20) { Value = (object?)e.SoDienThoai ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CCCD", SqlDbType.NVarChar, 20) { Value = (object?)e.CCCD ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@DiaChi", SqlDbType.NVarChar, 200) { Value = (object?)e.DiaChi ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@GhiChu", SqlDbType.NVarChar, 500) { Value = (object?)e.GhiChu ?? DBNull.Value });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task UpdateAsync(KhachHang e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE KhachHang SET HoTen=@HoTen, SoDienThoai=@SoDienThoai, CCCD=@CCCD, DiaChi=@DiaChi, GhiChu=@GhiChu WHERE MaKhachHang=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaKhachHang });
                cmd.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = e.HoTen });
                cmd.Parameters.Add(new SqlParameter("@SoDienThoai", SqlDbType.NVarChar, 20) { Value = (object?)e.SoDienThoai ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CCCD", SqlDbType.NVarChar, 20) { Value = (object?)e.CCCD ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@DiaChi", SqlDbType.NVarChar, 200) { Value = (object?)e.DiaChi ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@GhiChu", SqlDbType.NVarChar, 500) { Value = (object?)e.GhiChu ?? DBNull.Value });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM KhachHang WHERE MaKhachHang=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM KhachHang WHERE MaKhachHang=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                var o = await cmd.ExecuteScalarAsync();
                return o != null;
            }
        }

        private static KhachHang Map(SqlDataReader r)
        {
            return new KhachHang
            {
                MaKhachHang = r.GetInt32(r.GetOrdinal("MaKhachHang")),
                HoTen = r.GetString(r.GetOrdinal("HoTen")),
                SoDienThoai = r.IsDBNull(r.GetOrdinal("SoDienThoai")) ? null : r.GetString(r.GetOrdinal("SoDienThoai")),
                CCCD = r.IsDBNull(r.GetOrdinal("CCCD")) ? null : r.GetString(r.GetOrdinal("CCCD")),
                DiaChi = r.IsDBNull(r.GetOrdinal("DiaChi")) ? null : r.GetString(r.GetOrdinal("DiaChi")),
                GhiChu = r.IsDBNull(r.GetOrdinal("GhiChu")) ? null : r.GetString(r.GetOrdinal("GhiChu"))
            };
        }
    }
}