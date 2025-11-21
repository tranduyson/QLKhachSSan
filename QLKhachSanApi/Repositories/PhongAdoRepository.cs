using System;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class PhongAdoRepository : IPhongRepository
    {
        private readonly DatabaseHelper _db;
        public PhongAdoRepository(DatabaseHelper db) { _db = db; }

        public async Task<IEnumerable<Phong>> GetAllAsync()
        {
            var list = new List<Phong>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaPhong, SoPhong, MaLoaiPhong, TinhTrang FROM Phong", conn))
            {
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync()) list.Add(Map(r));
                }
            }
            return list;
        }

        public async Task<Phong?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaPhong, SoPhong, MaLoaiPhong, TinhTrang FROM Phong WHERE MaPhong=@Id", conn))
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

            public async Task<IEnumerable<Phong>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
            {
                var list = new List<Phong>();
                using (var conn = _db.GetConnection())
                using (var cmd = new SqlCommand(@"SELECT p.MaPhong, p.SoPhong, p.MaLoaiPhong, p.TinhTrang
    FROM Phong p
    WHERE p.TinhTrang = N'Trống'
      AND NOT EXISTS (
        SELECT 1 FROM ChiTietDatPhong ct
        JOIN DatPhong dp ON ct.MaDatPhong = dp.MaDatPhong
        WHERE ct.MaPhong = p.MaPhong
          AND dp.TrangThai <> N'Hủy'
          AND dp.NgayNhan <= @CheckOut
          AND dp.NgayTra >= @CheckIn
    )", conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@CheckIn", SqlDbType.DateTime) { Value = checkIn });
                    cmd.Parameters.Add(new SqlParameter("@CheckOut", SqlDbType.DateTime) { Value = checkOut });
                    await conn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        while (await r.ReadAsync()) list.Add(Map(r));
                    }
                }
                return list;
            }

        public Task<IEnumerable<Phong>> FindAsync(System.Linq.Expressions.Expression<Func<Phong, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<Phong> AddAsync(Phong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO Phong(SoPhong, MaLoaiPhong, TinhTrang)
OUTPUT INSERTED.MaPhong, INSERTED.SoPhong, INSERTED.MaLoaiPhong, INSERTED.TinhTrang
VALUES (@SoPhong, @MaLoaiPhong, @TinhTrang)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@SoPhong", SqlDbType.NVarChar, 10) { Value = e.SoPhong });
                cmd.Parameters.Add(new SqlParameter("@MaLoaiPhong", SqlDbType.Int) { Value = e.MaLoaiPhong });
                cmd.Parameters.Add(new SqlParameter("@TinhTrang", SqlDbType.NVarChar, 50) { Value = e.TinhTrang });
                await conn.OpenAsync();
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync()) return Map(r);
                }
            }

            return e;
        }

        public async Task UpdateAsync(Phong e)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE Phong SET SoPhong=@SoPhong, MaLoaiPhong=@MaLoaiPhong, TinhTrang=@TinhTrang WHERE MaPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = e.MaPhong });
                cmd.Parameters.Add(new SqlParameter("@SoPhong", SqlDbType.NVarChar, 10) { Value = e.SoPhong });
                cmd.Parameters.Add(new SqlParameter("@MaLoaiPhong", SqlDbType.Int) { Value = e.MaLoaiPhong });
                cmd.Parameters.Add(new SqlParameter("@TinhTrang", SqlDbType.NVarChar, 50) { Value = e.TinhTrang });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Phong WHERE MaPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM Phong WHERE MaPhong=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                return (await cmd.ExecuteScalarAsync()) != null;
            }
        }

        private static Phong Map(SqlDataReader r)
        {
            return new Phong
            {
                MaPhong = r.GetInt32(r.GetOrdinal("MaPhong")),
                SoPhong = r.GetString(r.GetOrdinal("SoPhong")),
                MaLoaiPhong = r.GetInt32(r.GetOrdinal("MaLoaiPhong")),
                TinhTrang = r.GetString(r.GetOrdinal("TinhTrang"))
            };
        }

    }
}