using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public interface INhanVienRepository : IRepository<NhanVien>
    {
        Task<NhanVien?> FindByCredentialsAsync(string email, string password);
    }

    public class NhanVienAdoRepository : INhanVienRepository
    {
        private readonly DatabaseHelper _db;
        public NhanVienAdoRepository(DatabaseHelper db) { _db = db; }

        public async Task<IEnumerable<NhanVien>> GetAllAsync()
        {
            var list = new List<NhanVien>();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaNhanVien, TenDangNhap, MatKhau, HoTen, ChucVu, SoDienThoai, TrangThai FROM NhanVien", conn))
            {
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync()) list.Add(Map(rdr));
                }
            }
            return list;
        }

        public async Task<NhanVien?> GetByIdAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaNhanVien, TenDangNhap, MatKhau, HoTen, ChucVu, SoDienThoai, TrangThai FROM NhanVien WHERE MaNhanVien=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync()) return Map(rdr);
                }
            }
            return null;
        }

        public Task<IEnumerable<NhanVien>> FindAsync(System.Linq.Expressions.Expression<Func<NhanVien, bool>> predicate)
        {
            throw new NotSupportedException();
        }

        public async Task<NhanVien> AddAsync(NhanVien entity)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO NhanVien(TenDangNhap, MatKhau, HoTen, ChucVu, SoDienThoai, TrangThai)
OUTPUT INSERTED.MaNhanVien, INSERTED.TenDangNhap, INSERTED.MatKhau, INSERTED.HoTen, INSERTED.ChucVu, INSERTED.SoDienThoai, INSERTED.TrangThai
VALUES(@TenDangNhap, @MatKhau, @HoTen, @ChucVu, @SoDienThoai, @TrangThai)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@TenDangNhap", SqlDbType.NVarChar, 50) { Value = entity.TenDangNhap });
                cmd.Parameters.Add(new SqlParameter("@MatKhau", SqlDbType.NVarChar, 100) { Value = entity.MatKhau });
                cmd.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = entity.HoTen });
                cmd.Parameters.Add(new SqlParameter("@ChucVu", SqlDbType.NVarChar, 50) { Value = (object?)entity.ChucVu ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@SoDienThoai", SqlDbType.NVarChar, 20) { Value = (object?)entity.SoDienThoai ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = entity.TrangThai });
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync()) return Map(rdr);
                }
            }
            return entity;
        }

        public async Task UpdateAsync(NhanVien entity)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE NhanVien SET TenDangNhap=@TenDangNhap, MatKhau=@MatKhau, HoTen=@HoTen, ChucVu=@ChucVu, SoDienThoai=@SoDienThoai, TrangThai=@TrangThai WHERE MaNhanVien=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = entity.MaNhanVien });
                cmd.Parameters.Add(new SqlParameter("@TenDangNhap", SqlDbType.NVarChar, 50) { Value = entity.TenDangNhap });
                cmd.Parameters.Add(new SqlParameter("@MatKhau", SqlDbType.NVarChar, 100) { Value = entity.MatKhau });
                cmd.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = entity.HoTen });
                cmd.Parameters.Add(new SqlParameter("@ChucVu", SqlDbType.NVarChar, 50) { Value = (object?)entity.ChucVu ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@SoDienThoai", SqlDbType.NVarChar, 20) { Value = (object?)entity.SoDienThoai ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = entity.TrangThai });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM NhanVien WHERE MaNhanVien=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM NhanVien WHERE MaNhanVien=@Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                var r = await cmd.ExecuteScalarAsync();
                return r != null;
            }
        }

        public async Task<NhanVien?> FindByCredentialsAsync(string email, string password)
        {
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(@"SELECT TOP 1 MaNhanVien, TenDangNhap, MatKhau, HoTen, ChucVu, SoDienThoai, TrangThai
FROM NhanVien WHERE TenDangNhap=@Email AND MatKhau=@Password AND TrangThai=1", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 50) { Value = email });
                cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = password });
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync()) return Map(rdr);
                }
            }
            return null;
        }

        private static NhanVien Map(SqlDataReader rdr)
        {
            return new NhanVien
            {
                MaNhanVien = rdr.GetInt32(rdr.GetOrdinal("MaNhanVien")),
                TenDangNhap = rdr.GetString(rdr.GetOrdinal("TenDangNhap")),
                MatKhau = rdr.GetString(rdr.GetOrdinal("MatKhau")),
                HoTen = rdr.GetString(rdr.GetOrdinal("HoTen")),
                ChucVu = rdr.IsDBNull(rdr.GetOrdinal("ChucVu")) ? null : rdr.GetString(rdr.GetOrdinal("ChucVu")),
                SoDienThoai = rdr.IsDBNull(rdr.GetOrdinal("SoDienThoai")) ? null : rdr.GetString(rdr.GetOrdinal("SoDienThoai")),
                TrangThai = rdr.GetInt32(rdr.GetOrdinal("TrangThai"))
            };
        }
    }
}


