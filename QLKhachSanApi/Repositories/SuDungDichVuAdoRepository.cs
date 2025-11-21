using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class SuDungDichVuAdoRepository : IRepository<SuDungDichVu>
    {
        private readonly DatabaseHelper _db;
        public SuDungDichVuAdoRepository(DatabaseHelper db) { _db = db; }

        public async Task<IEnumerable<SuDungDichVu>> GetAllAsync()
        {
            var list = new List<SuDungDichVu>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaSuDung, MaDatPhong, MaDichVu, SoLuong, DonGia, ThanhTien FROM SuDungDichVu", conn))
            {
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        public async Task<SuDungDichVu?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaSuDung, MaDatPhong, MaDichVu, SoLuong, DonGia, ThanhTien FROM SuDungDichVu WHERE MaSuDung=@Id", conn))
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

        public Task<IEnumerable<SuDungDichVu>> FindAsync(System.Linq.Expressions.Expression<Func<SuDungDichVu, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<SuDungDichVu> AddAsync(SuDungDichVu e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO SuDungDichVu(MaDatPhong, MaDichVu, SoLuong, DonGia)
OUTPUT INSERTED.MaSuDung, INSERTED.MaDatPhong, INSERTED.MaDichVu, INSERTED.SoLuong, INSERTED.DonGia, INSERTED.ThanhTien
VALUES(@MaDatPhong, @MaDichVu, @SoLuong, @DonGia)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaDichVu", SqlDbType.Int) { Value = e.MaDichVu });
                cmd.Parameters.Add(new SqlParameter("@SoLuong", SqlDbType.Int) { Value = e.SoLuong });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DonGia });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task<SuDungDichVu> AddAsync(SuDungDichVu e, SqlConnection conn, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO SuDungDichVu(MaDatPhong, MaDichVu, SoLuong, DonGia)
OUTPUT INSERTED.MaSuDung, INSERTED.MaDatPhong, INSERTED.MaDichVu, INSERTED.SoLuong, INSERTED.DonGia, INSERTED.ThanhTien
VALUES(@MaDatPhong, @MaDichVu, @SoLuong, @DonGia)", conn, tx))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaDichVu", SqlDbType.Int) { Value = e.MaDichVu });
                cmd.Parameters.Add(new SqlParameter("@SoLuong", SqlDbType.Int) { Value = e.SoLuong });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DonGia });
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task<IEnumerable<SuDungDichVu>> GetByDatPhongAsync(int maDatPhong)
        {
            var list = new List<SuDungDichVu>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaSuDung, MaDatPhong, MaDichVu, SoLuong, DonGia, ThanhTien FROM SuDungDichVu WHERE MaDatPhong=@MaDatPhong", conn))
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

        public async Task UpdateAsync(SuDungDichVu e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE SuDungDichVu SET MaDatPhong=@MaDatPhong, MaDichVu=@MaDichVu, SoLuong=@SoLuong, DonGia=@DonGia WHERE MaSuDung=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaSuDung });
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaDichVu", SqlDbType.Int) { Value = e.MaDichVu });
                cmd.Parameters.Add(new SqlParameter("@SoLuong", SqlDbType.Int) { Value = e.SoLuong });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DonGia });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM SuDungDichVu WHERE MaSuDung=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM SuDungDichVu WHERE MaSuDung=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                return (await cmd.ExecuteScalarAsync()) != null;
            }
        }

        private static SuDungDichVu Map(SqlDataReader r)
        {
            return new SuDungDichVu
            {
                MaSuDung = r.GetInt32(r.GetOrdinal("MaSuDung")),
                MaDatPhong = r.GetInt32(r.GetOrdinal("MaDatPhong")),
                MaDichVu = r.GetInt32(r.GetOrdinal("MaDichVu")),
                SoLuong = r.GetInt32(r.GetOrdinal("SoLuong")),
                DonGia = r.GetDecimal(r.GetOrdinal("DonGia")),
                ThanhTien = r.GetDecimal(r.GetOrdinal("ThanhTien"))
            };
        }
    }
}
