using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class ChiTietDatPhongAdoRepository : IRepository<ChiTietDatPhong>
    {
        private readonly DatabaseHelper _db;
        public ChiTietDatPhongAdoRepository(DatabaseHelper db) { _db = db; }

        public async Task<IEnumerable<ChiTietDatPhong>> GetAllAsync()
        {
            var list = new List<ChiTietDatPhong>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT 
                    ct.MaCT, 
                    ct.MaDatPhong, 
                    ct.MaPhong, 
                    p.SoPhong,
                    p.MaLoaiPhong,
                    lp.TenLoaiPhong,
                    ct.DonGia, 
                    ct.SoDem, 
                    ct.ThanhTien 
                FROM ChiTietDatPhong ct
                LEFT JOIN Phong p ON ct.MaPhong = p.MaPhong
                LEFT JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong", conn))
            {
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        public async Task<ChiTietDatPhong?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT 
                    ct.MaCT, 
                    ct.MaDatPhong, 
                    ct.MaPhong, 
                    p.SoPhong,
                    p.MaLoaiPhong,
                    lp.TenLoaiPhong,
                    ct.DonGia, 
                    ct.SoDem, 
                    ct.ThanhTien 
                FROM ChiTietDatPhong ct
                LEFT JOIN Phong p ON ct.MaPhong = p.MaPhong
                LEFT JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
                WHERE ct.MaCT=@Id", conn))
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

        public Task<IEnumerable<ChiTietDatPhong>> FindAsync(System.Linq.Expressions.Expression<Func<ChiTietDatPhong, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<ChiTietDatPhong> AddAsync(ChiTietDatPhong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO ChiTietDatPhong(MaDatPhong, MaPhong, DonGia, SoDem)
                VALUES(@MaDatPhong, @MaPhong, @DonGia, @SoDem);
                
                SELECT 
                    ct.MaCT, 
                    ct.MaDatPhong, 
                    ct.MaPhong, 
                    p.SoPhong,
                    p.MaLoaiPhong,
                    lp.TenLoaiPhong,
                    ct.DonGia, 
                    ct.SoDem, 
                    ct.ThanhTien 
                FROM ChiTietDatPhong ct
                LEFT JOIN Phong p ON ct.MaPhong = p.MaPhong
                LEFT JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
                WHERE ct.MaCT = SCOPE_IDENTITY()", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.Int) { Value = e.MaPhong });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DonGia });
                cmd.Parameters.Add(new SqlParameter("@SoDem", SqlDbType.Int) { Value = e.SoDem });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task<ChiTietDatPhong> AddAsync(ChiTietDatPhong e, SqlConnection conn, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand(@"
                INSERT INTO ChiTietDatPhong(MaDatPhong, MaPhong, DonGia, SoDem)
                VALUES(@MaDatPhong, @MaPhong, @DonGia, @SoDem);
                
                SELECT 
                    ct.MaCT, 
                    ct.MaDatPhong, 
                    ct.MaPhong, 
                    p.SoPhong,
                    p.MaLoaiPhong,
                    lp.TenLoaiPhong,
                    ct.DonGia, 
                    ct.SoDem, 
                    ct.ThanhTien 
                FROM ChiTietDatPhong ct
                LEFT JOIN Phong p ON ct.MaPhong = p.MaPhong
                LEFT JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
                WHERE ct.MaCT = SCOPE_IDENTITY()", conn, tx))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.Int) { Value = e.MaPhong });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DonGia });
                cmd.Parameters.Add(new SqlParameter("@SoDem", SqlDbType.Int) { Value = e.SoDem });
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }
            return e;
        }

        public async Task UpdateAsync(ChiTietDatPhong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE ChiTietDatPhong 
                SET MaDatPhong=@MaDatPhong, 
                    MaPhong=@MaPhong, 
                    DonGia=@DonGia, 
                    SoDem=@SoDem 
                WHERE MaCT=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaCT });
                cmd.Parameters.Add(new SqlParameter("@MaDatPhong", SqlDbType.Int) { Value = e.MaDatPhong });
                cmd.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.Int) { Value = e.MaPhong });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = e.DonGia });
                cmd.Parameters.Add(new SqlParameter("@SoDem", SqlDbType.Int) { Value = e.SoDem });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM ChiTietDatPhong WHERE MaCT=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM ChiTietDatPhong WHERE MaCT=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                return (await cmd.ExecuteScalarAsync()) != null;
            }
        }

        public async Task<IEnumerable<ChiTietDatPhong>> GetByDatPhongAsync(int maDatPhong)
        {
            var list = new List<ChiTietDatPhong>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT 
                    ct.MaCT, 
                    ct.MaDatPhong, 
                    ct.MaPhong, 
                    p.SoPhong,
                    p.MaLoaiPhong,
                    lp.TenLoaiPhong,
                    ct.DonGia, 
                    ct.SoDem, 
                    ct.ThanhTien 
                FROM ChiTietDatPhong ct
                LEFT JOIN Phong p ON ct.MaPhong = p.MaPhong
                LEFT JOIN LoaiPhong lp ON p.MaLoaiPhong = lp.MaLoaiPhong
                WHERE ct.MaDatPhong=@MaDatPhong", conn))
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

        private static ChiTietDatPhong Map(SqlDataReader r)
        {
            return new ChiTietDatPhong
            {
                MaCT = r.GetInt32(r.GetOrdinal("MaCT")),
                MaDatPhong = r.GetInt32(r.GetOrdinal("MaDatPhong")),
                MaPhong = r.GetInt32(r.GetOrdinal("MaPhong")),
                SoPhong = r.IsDBNull(r.GetOrdinal("SoPhong")) ? null : r.GetString(r.GetOrdinal("SoPhong")),
                MaLoaiPhong = r.IsDBNull(r.GetOrdinal("MaLoaiPhong")) ? 0 : r.GetInt32(r.GetOrdinal("MaLoaiPhong")),
                TenLoaiPhong = r.IsDBNull(r.GetOrdinal("TenLoaiPhong")) ? null : r.GetString(r.GetOrdinal("TenLoaiPhong")),
                DonGia = r.GetDecimal(r.GetOrdinal("DonGia")),
                SoDem = r.GetInt32(r.GetOrdinal("SoDem")),
                ThanhTien = r.GetDecimal(r.GetOrdinal("ThanhTien"))
            };
        }
    }
}