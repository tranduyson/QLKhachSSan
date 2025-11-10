using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Repositories
{
    public class DichVuAdoRepository : IRepository<DichVu>
    {
        private readonly DatabaseHelper _dbHelper;

        public DichVuAdoRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<IEnumerable<DichVu>> GetAllAsync()
        {
            var list = new List<DichVu>();
            using (var conn = _dbHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaDichVu, TenDichVu, DonGia, DonViTinh, GhiChu FROM DichVu", conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(Map(reader));
                    }
                }
            }
            return list;
        }

        public async Task<DichVu?> GetByIdAsync(int id)
        {
            using (var conn = _dbHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaDichVu, TenDichVu, DonGia, DonViTinh, GhiChu FROM DichVu WHERE MaDichVu = @Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return Map(reader);
                    }
                }
            }
            return null;
        }

        public Task<IEnumerable<DichVu>> FindAsync(System.Linq.Expressions.Expression<Func<DichVu, bool>> predicate)
        {
            throw new NotSupportedException("FindAsync with Expression is not supported in ADO repository. Use specific query methods instead.");
        }

        public async Task<DichVu> AddAsync(DichVu entity)
        {
            using (var conn = _dbHelper.GetConnection())
            using (var cmd = new SqlCommand(@"INSERT INTO DichVu (TenDichVu, DonGia, DonViTinh, GhiChu)
OUTPUT INSERTED.MaDichVu, INSERTED.TenDichVu, INSERTED.DonGia, INSERTED.DonViTinh, INSERTED.GhiChu
VALUES (@TenDichVu, @DonGia, @DonViTinh, @GhiChu)", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@TenDichVu", SqlDbType.NVarChar, 100) { Value = (object)entity.TenDichVu ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = entity.DonGia });
                cmd.Parameters.Add(new SqlParameter("@DonViTinh", SqlDbType.NVarChar, 50) { Value = (object?)entity.DonViTinh ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@GhiChu", SqlDbType.NVarChar, 200) { Value = (object?)entity.GhiChu ?? DBNull.Value });

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return Map(reader);
                    }
                }
            }
            return entity;
        }

        public async Task UpdateAsync(DichVu entity)
        {
            using (var conn = _dbHelper.GetConnection())
            using (var cmd = new SqlCommand(@"UPDATE DichVu
SET TenDichVu = @TenDichVu,
    DonGia = @DonGia,
    DonViTinh = @DonViTinh,
    GhiChu = @GhiChu
WHERE MaDichVu = @MaDichVu", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDichVu", SqlDbType.Int) { Value = entity.MaDichVu });
                cmd.Parameters.Add(new SqlParameter("@TenDichVu", SqlDbType.NVarChar, 100) { Value = (object)entity.TenDichVu ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@DonGia", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = entity.DonGia });
                cmd.Parameters.Add(new SqlParameter("@DonViTinh", SqlDbType.NVarChar, 50) { Value = (object?)entity.DonViTinh ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@GhiChu", SqlDbType.NVarChar, 200) { Value = (object?)entity.GhiChu ?? DBNull.Value });

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var conn = _dbHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM DichVu WHERE MaDichVu = @Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var conn = _dbHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT 1 FROM DichVu WHERE MaDichVu = @Id", conn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return result != null;
            }
        }

        private static DichVu Map(SqlDataReader reader)
        {
            return new DichVu
            {
                MaDichVu = reader.GetInt32(reader.GetOrdinal("MaDichVu")),
                TenDichVu = reader.GetString(reader.GetOrdinal("TenDichVu")),
                DonGia = reader.GetDecimal(reader.GetOrdinal("DonGia")),
                DonViTinh = reader.IsDBNull(reader.GetOrdinal("DonViTinh")) ? null : reader.GetString(reader.GetOrdinal("DonViTinh")),
                GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu"))
            };
        }
    }
}


