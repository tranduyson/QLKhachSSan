using System.Data;
using System.Data.SqlClient;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public class DichVuService : IDichVuService
    {
        private readonly DatabaseHelper _db;

        public DichVuService(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<DichVu>> GetAllAsync()
        {
            var results = new List<DichVu>();
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT MaDichVu, TenDichVu, DonGia, MoTa FROM DichVu", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(Map(reader));
            }
            return results;
        }

        public async Task<DichVu?> GetByIdAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT MaDichVu, TenDichVu, DonGia, MoTa FROM DichVu WHERE MaDichVu=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                return Map(reader);
            }
            return null;
        }

        public async Task<DichVu> AddAsync(DichVu entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(@"INSERT INTO DichVu (TenDichVu, DonGia, MoTa)
OUTPUT INSERTED.MaDichVu VALUES (@TenDichVu, @DonGia, @MoTa)", conn);
            cmd.Parameters.AddWithValue("@TenDichVu", entity.TenDichVu);
            cmd.Parameters.AddWithValue("@DonGia", entity.DonGia);
            cmd.Parameters.AddWithValue("@MoTa", (object?)entity.MoTa ?? DBNull.Value);
            await conn.OpenAsync();
            entity.MaDichVu = (int)await cmd.ExecuteScalarAsync();
            return entity;
        }

        public async Task UpdateAsync(DichVu entity)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(@"UPDATE DichVu SET TenDichVu=@TenDichVu, DonGia=@DonGia, MoTa=@MoTa WHERE MaDichVu=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", entity.MaDichVu);
            cmd.Parameters.AddWithValue("@TenDichVu", entity.TenDichVu);
            cmd.Parameters.AddWithValue("@DonGia", entity.DonGia);
            cmd.Parameters.AddWithValue("@MoTa", (object?)entity.MoTa ?? DBNull.Value);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("DELETE FROM DichVu WHERE MaDichVu=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT 1 FROM DichVu WHERE MaDichVu=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        private static DichVu Map(SqlDataReader reader)
        {
            return new DichVu
            {
                MaDichVu = reader.GetInt32(reader.GetOrdinal("MaDichVu")),
                TenDichVu = reader.GetString(reader.GetOrdinal("TenDichVu")),
                DonGia = reader.GetDecimal(reader.GetOrdinal("DonGia")),
                MoTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? null : reader.GetString(reader.GetOrdinal("MoTa"))
            };
        }
    }
}


