using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class LoaiPhongAdoRepository : IRepository<LoaiPhong>
    {
        private readonly DatabaseHelper _db;
        public LoaiPhongAdoRepository(DatabaseHelper db) { _db = db; }

        public async Task<IEnumerable<LoaiPhong>> GetAllAsync()
        {
            var list = new List<LoaiPhong>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaLoaiPhong, TenLoaiPhong, GiaMoiDem, MoTa, SoGiuong, DienTich FROM LoaiPhong", conn))
            {
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        public async Task<LoaiPhong?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaLoaiPhong, TenLoaiPhong, GiaMoiDem, MoTa, SoGiuong, DienTich FROM LoaiPhong WHERE MaLoaiPhong=@Id", conn))
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

        public Task<IEnumerable<LoaiPhong>> FindAsync(System.Linq.Expressions.Expression<Func<LoaiPhong, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<LoaiPhong> AddAsync(LoaiPhong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO LoaiPhong(TenLoaiPhong, GiaMoiDem, MoTa, SoGiuong, DienTich)
OUTPUT INSERTED.MaLoaiPhong, INSERTED.TenLoaiPhong, INSERTED.GiaMoiDem, INSERTED.MoTa, INSERTED.SoGiuong, INSERTED.DienTich
VALUES (@Ten,@Gia,@MoTa,@SoGiuong,@DienTich)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Ten", SqlDbType.NVarChar, 100) { Value = e.TenLoaiPhong });
                var pGia = new SqlParameter("@Gia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.GiaMoiDem };
                cmd.Parameters.Add(pGia);
                cmd.Parameters.Add(new SqlParameter("@MoTa", SqlDbType.NVarChar, 500) { Value = (object?)e.MoTa ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@SoGiuong", SqlDbType.Int) { Value = e.SoGiuong });
                var pDt = new SqlParameter("@DienTich", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DienTich };
                cmd.Parameters.Add(pDt);
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task UpdateAsync(LoaiPhong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE LoaiPhong SET TenLoaiPhong=@Ten, GiaMoiDem=@Gia, MoTa=@MoTa, SoGiuong=@SoGiuong, DienTich=@DienTich WHERE MaLoaiPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaLoaiPhong });
                cmd.Parameters.Add(new SqlParameter("@Ten", SqlDbType.NVarChar, 100) { Value = e.TenLoaiPhong });
                var pGia = new SqlParameter("@Gia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.GiaMoiDem };
                cmd.Parameters.Add(pGia);
                cmd.Parameters.Add(new SqlParameter("@MoTa", SqlDbType.NVarChar, 500) { Value = (object?)e.MoTa ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@SoGiuong", SqlDbType.Int) { Value = e.SoGiuong });
                var pDt = new SqlParameter("@DienTich", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DienTich };
                cmd.Parameters.Add(pDt);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM LoaiPhong WHERE MaLoaiPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM LoaiPhong WHERE MaLoaiPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                return (await cmd.ExecuteScalarAsync()) != null;
            }
        }

        private static LoaiPhong Map(SqlDataReader r)
        {
            return new LoaiPhong
            {
                MaLoaiPhong = r.GetInt32(r.GetOrdinal("MaLoaiPhong")),
                TenLoaiPhong = r.GetString(r.GetOrdinal("TenLoaiPhong")),
                GiaMoiDem = r.GetDecimal(r.GetOrdinal("GiaMoiDem")),
                MoTa = r.IsDBNull(r.GetOrdinal("MoTa")) ? null : r.GetString(r.GetOrdinal("MoTa")),
                SoGiuong = r.GetInt32(r.GetOrdinal("SoGiuong")),
                DienTich = r.GetDecimal(r.GetOrdinal("DienTich"))
            };
        }
    }
}