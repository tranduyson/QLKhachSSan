using System.Data;
using System.Data.SqlClient;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public class DatPhongService : IDatPhongService
    {
        private readonly DatabaseHelper _db;

        public DatPhongService(DatabaseHelper db)
        {
            _db = db;
        }

        public async Task<IEnumerable<DatPhong>> GetAllExpandedAsync()
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();

            var datPhongs = new List<DatPhong>();
            var datPhongById = new Dictionary<int, DatPhong>();

            // Load DatPhong base
            using (var cmd = new SqlCommand("SELECT MaDatPhong, MaKhachHang, MaNhanVien, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai FROM DatPhong", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var dp = new DatPhong
                    {
                        MaDatPhong = reader.GetInt32(0),
                        MaKhachHang = reader.GetInt32(1),
                        MaNhanVien = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                        NgayDat = reader.GetDateTime(3),
                        NgayNhan = reader.GetDateTime(4),
                        NgayTra = reader.GetDateTime(5),
                        TongTien = reader.GetDecimal(6),
                        TrangThai = reader.GetString(7)
                    };
                    datPhongs.Add(dp);
                    datPhongById[dp.MaDatPhong] = dp;
                }
            }

            if (datPhongs.Count == 0) return datPhongs;

            // Load KhachHang & NhanVien
            var idsCsv = string.Join(",", datPhongs.Select(x => x.MaDatPhong));

            // ChiTietDatPhong + Phong + LoaiPhong
            using (var cmd = new SqlCommand(@"SELECT ct.MaCT, ct.MaDatPhong, ct.MaPhong, ct.DonGia, ct.SoDem, ct.ThanhTien,
                                                    p.MaPhong, p.SoPhong, p.MaLoaiPhong, p.TinhTrang,
                                                    lp.MaLoaiPhong, lp.TenLoaiPhong, lp.GiaMoiDem, lp.MoTa, lp.SoGiuong, lp.DienTich
                                             FROM ChiTietDatPhong ct
                                             JOIN Phong p ON p.MaPhong = ct.MaPhong
                                             JOIN LoaiPhong lp ON lp.MaLoaiPhong = p.MaLoaiPhong
                                             WHERE ct.MaDatPhong IN (" + idsCsv + ")", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var maDatPhong = reader.GetInt32(reader.GetOrdinal("MaDatPhong"));
                    if (!datPhongById.TryGetValue(maDatPhong, out var dp)) continue;

                    var ct = new ChiTietDatPhong
                    {
                        MaCT = reader.GetInt32(reader.GetOrdinal("MaCT")),
                        MaDatPhong = maDatPhong,
                        MaPhong = reader.GetInt32(reader.GetOrdinal("MaPhong")),
                        DonGia = reader.GetDecimal(reader.GetOrdinal("DonGia")),
                        SoDem = reader.GetInt32(reader.GetOrdinal("SoDem")),
                        ThanhTien = reader.GetDecimal(reader.GetOrdinal("ThanhTien")),
                        Phong = new Phong
                        {
                            MaPhong = reader.GetInt32(reader.GetOrdinal("MaPhong")),
                            SoPhong = reader.GetString(reader.GetOrdinal("SoPhong")),
                            MaLoaiPhong = reader.GetInt32(reader.GetOrdinal("MaLoaiPhong")),
                            TinhTrang = reader.GetString(reader.GetOrdinal("TinhTrang")),
                            LoaiPhong = new LoaiPhong
                            {
                                MaLoaiPhong = reader.GetInt32(reader.GetOrdinal("MaLoaiPhong")),
                                TenLoaiPhong = reader.GetString(reader.GetOrdinal("TenLoaiPhong")),
                                GiaMoiDem = reader.GetDecimal(reader.GetOrdinal("GiaMoiDem")),
                                MoTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? null : reader.GetString(reader.GetOrdinal("MoTa")),
                                SoGiuong = reader.GetInt32(reader.GetOrdinal("SoGiuong")),
                                DienTich = reader.GetDecimal(reader.GetOrdinal("DienTich"))
                            }
                        }
                    };
                    dp.ChiTietDatPhongs.Add(ct);
                }
            }

            // SuDungDichVu + DichVu
            using (var cmd = new SqlCommand(@"SELECT sd.MaSuDung, sd.MaDatPhong, sd.MaDichVu, sd.SoLuong, sd.DonGia, sd.ThanhTien,
                                                    d.MaDichVu, d.TenDichVu, d.DonGia AS DonGiaDichVu, d.MoTa
                                             FROM SuDungDichVu sd
                                             JOIN DichVu d ON d.MaDichVu = sd.MaDichVu
                                             WHERE sd.MaDatPhong IN (" + idsCsv + ")", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var maDatPhong = reader.GetInt32(reader.GetOrdinal("MaDatPhong"));
                    if (!datPhongById.TryGetValue(maDatPhong, out var dp)) continue;
                    var sd = new SuDungDichVu
                    {
                        MaSuDung = reader.GetInt32(reader.GetOrdinal("MaSuDung")),
                        MaDatPhong = maDatPhong,
                        MaDichVu = reader.GetInt32(reader.GetOrdinal("MaDichVu")),
                        SoLuong = reader.GetInt32(reader.GetOrdinal("SoLuong")),
                        DonGia = reader.GetDecimal(reader.GetOrdinal("DonGia")),
                        ThanhTien = reader.GetDecimal(reader.GetOrdinal("ThanhTien")),
                        DichVu = new DichVu
                        {
                            MaDichVu = reader.GetInt32(reader.GetOrdinal("MaDichVu")),
                            TenDichVu = reader.GetString(reader.GetOrdinal("TenDichVu")),
                            DonGia = reader.GetDecimal(reader.GetOrdinal("DonGiaDichVu")),
                            MoTa = reader.IsDBNull(reader.GetOrdinal("MoTa")) ? null : reader.GetString(reader.GetOrdinal("MoTa"))
                        }
                    };
                    dp.SuDungDichVus.Add(sd);
                }
            }

            // KhachHang & NhanVien basic references
            // For brevity, not loading full nested objects here since controllers did not serialize them directly.

            return datPhongs;
        }

        public async Task<DatPhong?> GetByIdExpandedAsync(int id)
        {
            var all = await GetAllExpandedAsync();
            return all.FirstOrDefault(x => x.MaDatPhong == id);
        }

        public async Task<DatPhong> CreateAsync(DatPhongRequest request)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                int maDatPhong;
                using (var cmd = new SqlCommand(@"INSERT INTO DatPhong (MaKhachHang, MaNhanVien, NgayDat, NgayNhan, NgayTra, TongTien, TrangThai)
                                                 OUTPUT INSERTED.MaDatPhong
                                                 VALUES (@MaKhachHang, @MaNhanVien, @NgayDat, @NgayNhan, @NgayTra, 0, @TrangThai)", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@MaKhachHang", request.MaKhachHang);
                    cmd.Parameters.AddWithValue("@MaNhanVien", (object?)request.MaNhanVien ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayDat", request.NgayDat);
                    cmd.Parameters.AddWithValue("@NgayNhan", request.NgayNhan);
                    cmd.Parameters.AddWithValue("@NgayTra", request.NgayTra);
                    cmd.Parameters.AddWithValue("@TrangThai", (object?)request.TrangThai ?? "Đã đặt");
                    maDatPhong = (int)await cmd.ExecuteScalarAsync();
                }

                foreach (var ct in request.ChiTietDatPhongs)
                {
                    using (var cmd = new SqlCommand(@"INSERT INTO ChiTietDatPhong (MaDatPhong, MaPhong, DonGia, SoDem)
                                                     VALUES (@MaDatPhong, @MaPhong, @DonGia, @SoDem)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@MaDatPhong", maDatPhong);
                        cmd.Parameters.AddWithValue("@MaPhong", ct.MaPhong);
                        cmd.Parameters.AddWithValue("@DonGia", ct.DonGia);
                        cmd.Parameters.AddWithValue("@SoDem", ct.SoDem);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new SqlCommand("UPDATE Phong SET TinhTrang=N'Đang ở' WHERE MaPhong=@MaPhong", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@MaPhong", ct.MaPhong);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                if (request.SuDungDichVus != null)
                {
                    foreach (var sd in request.SuDungDichVus)
                    {
                        using var cmd = new SqlCommand(@"INSERT INTO SuDungDichVu (MaDatPhong, MaDichVu, SoLuong, DonGia)
                                                         VALUES (@MaDatPhong, @MaDichVu, @SoLuong, @DonGia)", conn, tx);
                        cmd.Parameters.AddWithValue("@MaDatPhong", maDatPhong);
                        cmd.Parameters.AddWithValue("@MaDichVu", sd.MaDichVu);
                        cmd.Parameters.AddWithValue("@SoLuong", sd.SoLuong);
                        cmd.Parameters.AddWithValue("@DonGia", sd.DonGia);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await tx.CommitAsync();

                return new DatPhong
                {
                    MaDatPhong = maDatPhong,
                    MaKhachHang = request.MaKhachHang,
                    MaNhanVien = request.MaNhanVien,
                    NgayDat = request.NgayDat,
                    NgayNhan = request.NgayNhan,
                    NgayTra = request.NgayTra,
                    TrangThai = request.TrangThai ?? "Đã đặt"
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task CheckInAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("UPDATE DatPhong SET TrangThai=N'Đã nhận' WHERE MaDatPhong=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task CheckOutAsync(int id)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            try
            {
                using (var cmd = new SqlCommand("UPDATE DatPhong SET TrangThai=N'Đã trả' WHERE MaDatPhong=@id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                // Reset rooms to available
                using (var cmd = new SqlCommand("SELECT MaPhong FROM ChiTietDatPhong WHERE MaDatPhong=@id", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var rooms = new List<int>();
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        rooms.Add(reader.GetInt32(0));
                    }
                    reader.Close();

                    foreach (var roomId in rooms)
                    {
                        using var up = new SqlCommand("UPDATE Phong SET TinhTrang=N'Trống' WHERE MaPhong=@p", conn, tx);
                        up.Parameters.AddWithValue("@p", roomId);
                        await up.ExecuteNonQueryAsync();
                    }
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(DatPhong datPhong)
        {
            using var conn = _db.GetConnection();
            var sql = @"UPDATE DatPhong SET MaKhachHang=@MaKhachHang, MaNhanVien=@MaNhanVien, NgayDat=@NgayDat, NgayNhan=@NgayNhan, NgayTra=@NgayTra, TongTien=@TongTien, TrangThai=@TrangThai WHERE MaDatPhong=@Id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", datPhong.MaDatPhong);
            cmd.Parameters.AddWithValue("@MaKhachHang", datPhong.MaKhachHang);
            cmd.Parameters.AddWithValue("@MaNhanVien", (object?)datPhong.MaNhanVien ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NgayDat", datPhong.NgayDat);
            cmd.Parameters.AddWithValue("@NgayNhan", datPhong.NgayNhan);
            cmd.Parameters.AddWithValue("@NgayTra", datPhong.NgayTra);
            cmd.Parameters.AddWithValue("@TongTien", datPhong.TongTien);
            cmd.Parameters.AddWithValue("@TrangThai", datPhong.TrangThai);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                using (var cmd = new SqlCommand("DELETE FROM SuDungDichVu WHERE MaDatPhong=@id", conn, tx))
                { cmd.Parameters.AddWithValue("@id", id); await cmd.ExecuteNonQueryAsync(); }
                using (var cmd = new SqlCommand("DELETE FROM ChiTietDatPhong WHERE MaDatPhong=@id", conn, tx))
                { cmd.Parameters.AddWithValue("@id", id); await cmd.ExecuteNonQueryAsync(); }
                using (var cmd = new SqlCommand("DELETE FROM ThanhToan WHERE MaDatPhong=@id", conn, tx))
                { cmd.Parameters.AddWithValue("@id", id); await cmd.ExecuteNonQueryAsync(); }
                using (var cmd = new SqlCommand("DELETE FROM DatPhong WHERE MaDatPhong=@id", conn, tx))
                { cmd.Parameters.AddWithValue("@id", id); await cmd.ExecuteNonQueryAsync(); }
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand("SELECT 1 FROM DatPhong WHERE MaDatPhong=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
    }
}


