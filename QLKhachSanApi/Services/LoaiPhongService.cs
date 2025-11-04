using System.Data;
using System.Data.SqlClient;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public class LoaiPhongService : ILoaiPhongService
    {
        private readonly DatabaseHelper _db;

        public LoaiPhongService(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LoaiPhong>> GetAllAsync()
        {
            var results = new List<LoaiPhong>();
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT MaLoaiPhong, TenLoaiPhong, GiaMoiDem, MoTa, SoGiuong, DienTich FROM LoaiPhong", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapLoaiPhong(reader));
            }
            return results;
        }

        public async Task<LoaiPhong?> GetByIdAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT MaLoaiPhong, TenLoaiPhong, GiaMoiDem, MoTa, SoGiuong, DienTich FROM LoaiPhong WHERE MaLoaiPhong=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                return MapLoaiPhong(reader);
            }
            return null;
        }

        public async Task<LoaiPhong> AddAsync(LoaiPhong entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(@"INSERT INTO LoaiPhong (TenLoaiPhong, GiaMoiDem, MoTa, SoGiuong, DienTich)
OUTPUT INSERTED.MaLoaiPhong VALUES (@TenLoaiPhong, @GiaMoiDem, @MoTa, @SoGiuong, @DienTich)", conn);
            cmd.Parameters.AddWithValue("@TenLoaiPhong", entity.TenLoaiPhong);
            cmd.Parameters.AddWithValue("@GiaMoiDem", entity.GiaMoiDem);
            cmd.Parameters.AddWithValue("@MoTa", (object?)entity.MoTa ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SoGiuong", entity.SoGiuong);
            cmd.Parameters.AddWithValue("@DienTich", entity.DienTich);
            await conn.OpenAsync();
            entity.MaLoaiPhong = (int)await cmd.ExecuteScalarAsync();
            return entity;
        }

        public async Task UpdateAsync(LoaiPhong entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(@"UPDATE LoaiPhong SET TenLoaiPhong=@TenLoaiPhong, GiaMoiDem=@GiaMoiDem, MoTa=@MoTa, SoGiuong=@SoGiuong, DienTich=@DienTich WHERE MaLoaiPhong=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", entity.MaLoaiPhong);
            cmd.Parameters.AddWithValue("@TenLoaiPhong", entity.TenLoaiPhong);
            cmd.Parameters.AddWithValue("@GiaMoiDem", entity.GiaMoiDem);
            cmd.Parameters.AddWithValue("@MoTa", (object?)entity.MoTa ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SoGiuong", entity.SoGiuong);
            cmd.Parameters.AddWithValue("@DienTich", entity.DienTich);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("DELETE FROM LoaiPhong WHERE MaLoaiPhong=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT 1 FROM LoaiPhong WHERE MaLoaiPhong=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        private static LoaiPhong MapLoaiPhong(SqlDataReader reader)
        {
            return new LoaiPhong
            {
                MaLoaiPhong = reader.GetInt32(reader.GetOrdinal("MaLoaiPhong")),
                TenLoaiPhong = reader.GetString(reader.GetOrdinal("TenLoaiPhong")),
                GiaMoiDem = reader.GetDecimal(reader.GetOrdinal("GiaMoiDem")),
                MoTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? null : reader.GetString(reader.GetOrdinal("MoTa")),
                SoGiuong = reader.GetInt32(reader.GetOrdinal("SoGiuong")),
                DienTich = reader.GetDecimal(reader.GetOrdinal("DienTich"))
            };
        }
    }
}


