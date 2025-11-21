using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class DatPhongAdoRepository : IRepository<DatPhong>
    {
        private readonly DatabaseHelper _db;
        public DatPhongAdoRepository(DatabaseHelper db) { _db = db; }

        public async Task<IEnumerable<DatPhong>> GetAllAsync()
        {
            var list = new List<DatPhong>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaDatPhong, MaKhachHang, MaNhanVien, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai FROM DatPhong", conn))
            {
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        public async Task<DatPhong?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaDatPhong, MaKhachHang, MaNhanVien, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai FROM DatPhong WHERE MaDatPhong=@Id", conn))
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

        public Task<IEnumerable<DatPhong>> FindAsync(System.Linq.Expressions.Expression<Func<DatPhong, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<DatPhong> AddAsync(DatPhong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO DatPhong(MaKhachHang, MaNhanVien, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai)
OUTPUT INSERTED.MaDatPhong, INSERTED.MaKhachHang, INSERTED.MaNhanVien, INSERTED.NgayDat, INSERTED.NgayNhan, INSERTED.NgayTra, INSERTED.TongTien, INSERTED.TrangThai
VALUES(@MaKhachHang, @MaNhanVien, @NgayDat, @NgayNhan, @NgayTra, @TongTien, @TrangThai)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaKhachHang", SqlDbType.Int) { Value = e.MaKhachHang });
                cmd.Parameters.Add(new SqlParameter("@MaNhanVien", SqlDbType.Int) { Value = (object?)e.MaNhanVien ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@NgayDat", SqlDbType.DateTime) { Value = e.NgayDat });
                cmd.Parameters.Add(new SqlParameter("@NgayNhan", SqlDbType.DateTime) { Value = e.NgayNhan });
                cmd.Parameters.Add(new SqlParameter("@NgayTra", SqlDbType.DateTime) { Value = e.NgayTra });
                cmd.Parameters.Add(new SqlParameter("@TongTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.TongTien });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = e.TrangThai });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task<DatPhong> AddAsync(DatPhong e, SqlConnection conn, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO DatPhong(MaKhachHang, MaNhanVien, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai)
OUTPUT INSERTED.MaDatPhong, INSERTED.MaKhachHang, INSERTED.MaNhanVien, INSERTED.NgayDat, INSERTED.NgayNhan, INSERTED.NgayTra, INSERTED.TongTien, INSERTED.TrangThai
VALUES(@MaKhachHang, @MaNhanVien, @NgayDat, @NgayNhan, @NgayTra, @TongTien, @TrangThai)", conn, tx))
            {
                cmd.Parameters.Add(new SqlParameter("@MaKhachHang", SqlDbType.Int) { Value = e.MaKhachHang });
                cmd.Parameters.Add(new SqlParameter("@MaNhanVien", SqlDbType.Int) { Value = (object?)e.MaNhanVien ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@NgayDat", SqlDbType.DateTime) { Value = e.NgayDat });
                cmd.Parameters.Add(new SqlParameter("@NgayNhan", SqlDbType.DateTime) { Value = e.NgayNhan });
                cmd.Parameters.Add(new SqlParameter("@NgayTra", SqlDbType.DateTime) { Value = e.NgayTra });
                cmd.Parameters.Add(new SqlParameter("@TongTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.TongTien });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = e.TrangThai });
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task UpdateAsync(DatPhong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE DatPhong SET MaKhachHang=@MaKhachHang, MaNhanVien=@MaNhanVien, NgayDat=@NgayDat, NgayNhan=@NgayNhan, NgayTra=@NgayTra, TongTien=@TongTien, TrangThai=@TrangThai WHERE MaDatPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaKhachHang", SqlDbType.Int) { Value = e.MaKhachHang });
                cmd.Parameters.Add(new SqlParameter("@MaNhanVien", SqlDbType.Int) { Value = (object?)e.MaNhanVien ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@NgayDat", SqlDbType.DateTime) { Value = e.NgayDat });
                cmd.Parameters.Add(new SqlParameter("@NgayNhan", SqlDbType.DateTime) { Value = e.NgayNhan });
                cmd.Parameters.Add(new SqlParameter("@NgayTra", SqlDbType.DateTime) { Value = e.NgayTra });
                cmd.Parameters.Add(new SqlParameter("@TongTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.TongTien });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = e.TrangThai });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(DatPhong e, SqlConnection conn, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand(@"UPDATE DatPhong SET MaKhachHang=@MaKhachHang, MaNhanVien=@MaNhanVien, NgayDat=@NgayDat, NgayNhan=@NgayNhan, NgayTra=@NgayTra, TongTien=@TongTien, TrangThai=@TrangThai WHERE MaDatPhong=@Id", conn, tx))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaKhachHang", SqlDbType.Int) { Value = e.MaKhachHang });
                cmd.Parameters.Add(new SqlParameter("@MaNhanVien", SqlDbType.Int) { Value = (object?)e.MaNhanVien ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@NgayDat", SqlDbType.DateTime) { Value = e.NgayDat });
                cmd.Parameters.Add(new SqlParameter("@NgayNhan", SqlDbType.DateTime) { Value = e.NgayNhan });
                cmd.Parameters.Add(new SqlParameter("@NgayTra", SqlDbType.DateTime) { Value = e.NgayTra });
                cmd.Parameters.Add(new SqlParameter("@TongTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.TongTien });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.NVarChar, 50) { Value = e.TrangThai });
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                using (var tx = conn.BeginTransaction())
                {
                    // Delete child records first to satisfy FK constraints
                    using (var cmd = new SqlCommand("DELETE FROM ThanhToan WHERE MaDatPhong=@Id", conn, tx))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new SqlCommand("DELETE FROM SuDungDichVu WHERE MaDatPhong=@Id", conn, tx))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new SqlCommand("DELETE FROM ChiTietDatPhong WHERE MaDatPhong=@Id", conn, tx))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new SqlCommand("DELETE FROM DatPhong WHERE MaDatPhong=@Id", conn, tx))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                        await cmd.ExecuteNonQueryAsync();
                    }

                    tx.Commit();
                }
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM DatPhong WHERE MaDatPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                return (await cmd.ExecuteScalarAsync()) != null;
            }
        }

        private static DatPhong Map(SqlDataReader r)
        {
            return new DatPhong
            {
                MaDatPhong = r.GetInt32(r.GetOrdinal("MaDatPhong")),
                MaKhachHang = r.GetInt32(r.GetOrdinal("MaKhachHang")),
                MaNhanVien = r.IsDBNull(r.GetOrdinal("MaNhanVien")) ? null : r.GetInt32(r.GetOrdinal("MaNhanVien")),
                NgayDat = r.GetDateTime(r.GetOrdinal("NgayDat")),
                NgayNhan = r.GetDateTime(r.GetOrdinal("NgayNhan")),
                NgayTra = r.GetDateTime(r.GetOrdinal("NgayTra")),
                TongTien = r.GetDecimal(r.GetOrdinal("TongTien")),
                TrangThai = r.GetString(r.GetOrdinal("TrangThai"))
            };
        }
    }
}
