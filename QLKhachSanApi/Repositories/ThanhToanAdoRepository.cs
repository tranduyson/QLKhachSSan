using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class ThanhToanAdoRepository : IRepository<ThanhToan>
    {
        private readonly DatabaseHelper _db;
        public ThanhToanAdoRepository(DatabaseHelper db) { _db = db; }

        public async Task<IEnumerable<ThanhToan>> GetAllAsync()
        {
            var list = new List<ThanhToan>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaThanhToan, MaDatPhong, NgayThanhToan, SoTien, PhuongThuc, GhiChu, TrangThai FROM ThanhToan", conn))
            {
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        public async Task<ThanhToan?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaThanhToan, MaDatPhong, NgayThanhToan, SoTien, PhuongThuc, GhiChu, TrangThai FROM ThanhToan WHERE MaThanhToan=@Id", conn))
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

        public Task<IEnumerable<ThanhToan>> FindAsync(System.Linq.Expressions.Expression<Func<ThanhToan, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<ThanhToan> AddAsync(ThanhToan e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO ThanhToan(MaDatPhong, NgayThanhToan, SoTien, PhuongThuc, GhiChu, TrangThai)
OUTPUT INSERTED.MaThanhToan, INSERTED.MaDatPhong, INSERTED.NgayThanhToan, INSERTED.SoTien, INSERTED.PhuongThuc, INSERTED.GhiChu, INSERTED.TrangThai
VALUES(@MaDatPhong, @NgayThanhToan, @SoTien, @PhuongThuc, @GhiChu, @TrangThai)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@NgayThanhToan", SqlDbType.DateTime) { Value = e.NgayThanhToan });
                cmd.Parameters.Add(new SqlParameter("@SoTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.SoTien });
                cmd.Parameters.Add(new SqlParameter("@PhuongThuc", SqlDbType.NVarChar, 50) { Value = (object?)e.PhuongThuc ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@GhiChu", SqlDbType.NVarChar, 200) { Value = (object?)e.GhiChu ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Bit) { Value = e.TrangThai });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task UpdateAsync(ThanhToan e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE ThanhToan SET MaDatPhong=@MaDatPhong, NgayThanhToan=@NgayThanhToan, SoTien=@SoTien, PhuongThuc=@PhuongThuc, GhiChu=@GhiChu, TrangThai=@TrangThai WHERE MaThanhToan=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaThanhToan });
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@NgayThanhToan", SqlDbType.DateTime) { Value = e.NgayThanhToan });
                cmd.Parameters.Add(new SqlParameter("@SoTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.SoTien });
                cmd.Parameters.Add(new SqlParameter("@PhuongThuc", SqlDbType.NVarChar, 50) { Value = (object?)e.PhuongThuc ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@GhiChu", SqlDbType.NVarChar, 200) { Value = (object?)e.GhiChu ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Bit) { Value = e.TrangThai });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM ThanhToan WHERE MaThanhToan=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM ThanhToan WHERE MaThanhToan=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                return (await cmd.ExecuteScalarAsync()) != null;
            }
        }

        public async Task<IEnumerable<ThanhToan>> GetByDatPhongAsync(int maDatPhong)
        {
            var list = new List<ThanhToan>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaThanhToan, MaDatPhong, NgayThanhToan, SoTien, PhuongThuc, GhiChu, TrangThai FROM ThanhToan WHERE MaDatPhong=@MaDatPhong", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = maDatPhong });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        private static ThanhToan Map(SqlDataReader r)
        {
            return new ThanhToan
            {
                MaThanhToan = r.GetInt32(r.GetOrdinal("MaThanhToan")),
                MaDatPhong = r.GetInt32(r.GetOrdinal("MaDatPhong")),
                NgayThanhToan = r.GetDateTime(r.GetOrdinal("NgayThanhToan")),
                SoTien = r.GetDecimal(r.GetOrdinal("SoTien")),
                PhuongThuc = r.IsDBNull(r.GetOrdinal("PhuongThuc")) ? null : r.GetString(r.GetOrdinal("PhuongThuc")),
                GhiChu = r.IsDBNull(r.GetOrdinal("GhiChu")) ? null : r.GetString(r.GetOrdinal("GhiChu")),
                TrangThai = r.GetBoolean(r.GetOrdinal("TrangThai"))
            };
        }
    }
}
