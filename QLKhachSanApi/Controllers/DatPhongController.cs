using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatPhongController : ControllerBase
    {
        private readonly DatPhongAdoRepository _repository;
        private readonly ChiTietDatPhongAdoRepository _chiTietRepository;
        private readonly SuDungDichVuAdoRepository _suDungDichVuRepository;
        private readonly DichVuAdoRepository _dichVuRepository;
        private readonly PhongAdoRepository _phongRepository;
        private readonly ThanhToanAdoRepository _thanhToanRepository;
        private readonly KhachHangAdoRepository _khachHangRepository;
        private readonly NhanVienAdoRepository _nhanVienRepository;
        private readonly DatabaseHelper _dbHelper;

        public DatPhongController(
            DatPhongAdoRepository repository,
            ChiTietDatPhongAdoRepository chiTietRepository,
            SuDungDichVuAdoRepository suDungDichVuRepository,
            DichVuAdoRepository dichVuRepository,
            PhongAdoRepository phongRepository,
            ThanhToanAdoRepository thanhToanRepository,
            KhachHangAdoRepository khachHangRepository,
            NhanVienAdoRepository nhanVienRepository,
            DatabaseHelper dbHelper)
        {
            _repository = repository;
            _chiTietRepository = chiTietRepository;
            _suDungDichVuRepository = suDungDichVuRepository;
            _dichVuRepository = dichVuRepository;
            _phongRepository = phongRepository;
            _thanhToanRepository = thanhToanRepository;
            _khachHangRepository = khachHangRepository;
            _nhanVienRepository = nhanVienRepository;
            _dbHelper = dbHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var datPhongs = (await _repository.GetAllAsync()).ToList();

            foreach (var dp in datPhongs)
            {
                // Load dữ liệu chi tiết
                dp.ChiTietDatPhongs = (await _chiTietRepository.GetByDatPhongAsync(dp.MaDatPhong)).ToList();
                dp.SuDungDichVus = (await _suDungDichVuRepository.GetByDatPhongAsync(dp.MaDatPhong)).ToList();
                dp.ThanhToans = (await _thanhToanRepository.GetByDatPhongAsync(dp.MaDatPhong)).ToList();
                dp.KhachHang = await _khachHangRepository.GetByIdAsync(dp.MaKhachHang);
                dp.NhanVien = dp.MaNhanVien.HasValue ? await _nhanVienRepository.GetByIdAsync(dp.MaNhanVien.Value) : null;

                // 👉 Tính lại TongTien dựa vào ThanhTien
                var tongTienChiTiet = dp.ChiTietDatPhongs.Sum(x => x.ThanhTien);
                var tongTienDichVu = dp.SuDungDichVus.Sum(x => x.ThanhTien);

                dp.TongTien = tongTienChiTiet + tongTienDichVu;
            }

            return Ok(datPhongs);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var datPhong = await _repository.GetByIdAsync(id);
            if (datPhong == null)
                return NotFound();

            datPhong.ChiTietDatPhongs = (await _chiTietRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
            datPhong.SuDungDichVus = (await _suDungDichVuRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
            datPhong.ThanhToans = (await _thanhToanRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
            datPhong.KhachHang = await _khachHangRepository.GetByIdAsync(datPhong.MaKhachHang);
            datPhong.NhanVien = datPhong.MaNhanVien.HasValue ? await _nhanVienRepository.GetByIdAsync(datPhong.MaNhanVien.Value) : null;

            return Ok(datPhong);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DatPhongRequest request)
        {
            // Validate MaKhachHang exists
            var khachHang = await _khachHangRepository.GetByIdAsync(request.MaKhachHang);
            if (khachHang == null)
                return BadRequest($"Khách hàng ID {request.MaKhachHang} không tồn tại");

            // Validate rooms exist
            foreach (var chiTiet in request.ChiTietDatPhongs)
            {
                var phong = await _phongRepository.GetByIdAsync(chiTiet.MaPhong);
                if (phong == null)
                    return BadRequest($"Phòng ID {chiTiet.MaPhong} không tồn tại");
            }

            // Validate services exist if provided
            if (request.SuDungDichVus != null && request.SuDungDichVus.Count > 0)
            {
                foreach (var dichVu in request.SuDungDichVus)
                {
                    var service = await _dichVuRepository.GetByIdAsync(dichVu.MaDichVu);
                    if (service == null)
                        return BadRequest($"Dịch vụ ID {dichVu.MaDichVu} không tồn tại");
                }
            }

            using var conn = _dbHelper.GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            try
            {
                var datPhong = new DatPhong
                {
                    MaKhachHang = request.MaKhachHang,
                    MaNhanVien = request.MaNhanVien,
                    NgayDat = request.NgayDat,
                    NgayNhan = request.NgayNhan,
                    NgayTra = request.NgayTra,
                    TrangThai = request.TrangThai ?? "Đã đặt"
                };

                var createdDatPhong = await _repository.AddAsync(datPhong, conn, tx);

                // Add ChiTietDatPhong and update room status
                foreach (var chiTiet in request.ChiTietDatPhongs)
                {
                    var chiTietDatPhong = new ChiTietDatPhong
                    {
                        MaDatPhong = createdDatPhong.MaDatPhong,
                        MaPhong = chiTiet.MaPhong,
                        DonGia = chiTiet.DonGia,
                        SoDem = chiTiet.SoDem
                    };
                    await _chiTietRepository.AddAsync(chiTietDatPhong, conn, tx);

                    // Update room status via SQL in same transaction
                    using (var cmd = new SqlCommand("UPDATE Phong SET TinhTrang=@TinhTrang WHERE MaPhong=@Id", conn, tx))
                    {
                        cmd.Parameters.Add(new SqlParameter("@TinhTrang", SqlDbType.NVarChar, 50) { Value = "Đang ở" });
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = chiTiet.MaPhong });
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Add SuDungDichVu if any
                if (request.SuDungDichVus != null)
                {
                    foreach (var dichVu in request.SuDungDichVus)
                    {
                        var suDungDichVu = new SuDungDichVu
                        {
                            MaDatPhong = createdDatPhong.MaDatPhong,
                            MaDichVu = dichVu.MaDichVu,
                            SoLuong = dichVu.SoLuong,
                            DonGia = dichVu.DonGia
                        };
                        await _suDungDichVuRepository.AddAsync(suDungDichVu, conn, tx);
                    }
                }

                tx.Commit();

                return CreatedAtAction(nameof(GetById), new { id = createdDatPhong.MaDatPhong }, createdDatPhong);
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        [HttpPut("{id}/check-in")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var datPhong = await _repository.GetByIdAsync(id);
            if (datPhong == null)
                return NotFound(new { message = $"Booking with ID {id} not found" });

            if (datPhong.TrangThai == "Đã nhận")
                return BadRequest(new { message = "Booking has already been checked in" });

            if (datPhong.TrangThai == "Đã trả")
                return BadRequest(new { message = "Cannot check in a booking that has already been checked out" });

            datPhong.TrangThai = "Đã nhận";
            datPhong.NgayNhan = DateTime.Now;
            await _repository.UpdateAsync(datPhong);

            // Load related data for response
            datPhong.ChiTietDatPhongs = (await _chiTietRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
            datPhong.SuDungDichVus = (await _suDungDichVuRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
            datPhong.ThanhToans = (await _thanhToanRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
            datPhong.KhachHang = await _khachHangRepository.GetByIdAsync(datPhong.MaKhachHang);
            datPhong.NhanVien = datPhong.MaNhanVien.HasValue ? await _nhanVienRepository.GetByIdAsync(datPhong.MaNhanVien.Value) : null;

            return Ok(new { message = "Check-in successful", data = datPhong });
        }

        [HttpPut("{id}/check-out")]
        public async Task<IActionResult> CheckOut(int id)
        {
            using var conn = _dbHelper.GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            try
            {
                var datPhong = await _repository.GetByIdAsync(id);
                if (datPhong == null)
                    return NotFound(new { message = $"Booking with ID {id} not found" });

                if (datPhong.TrangThai == "Đã trả")
                    return BadRequest(new { message = "Booking has already been checked out" });

                if (datPhong.TrangThai == "Hủy")
                    return BadRequest(new { message = "Cannot check out a cancelled booking" });

                datPhong.TrangThai = "Đã trả";
                datPhong.NgayTra = DateTime.Now;

                await _repository.UpdateAsync(datPhong, conn, tx);

                var chiTietPhongs = (await _chiTietRepository.GetByDatPhongAsync(id)).ToList();
                foreach (var chiTiet in chiTietPhongs)
                {
                    using (var cmd = new SqlCommand("UPDATE Phong SET TinhTrang=@TinhTrang WHERE MaPhong=@Id", conn, tx))
                    {
                        cmd.Parameters.Add(new SqlParameter("@TinhTrang", SqlDbType.NVarChar, 50) { Value = "Trống" });
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = chiTiet.MaPhong });
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                tx.Commit();

                // Load related data for response
                datPhong.ChiTietDatPhongs = (await _chiTietRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
                datPhong.SuDungDichVus = (await _suDungDichVuRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
                datPhong.ThanhToans = (await _thanhToanRepository.GetByDatPhongAsync(datPhong.MaDatPhong)).ToList();
                datPhong.KhachHang = await _khachHangRepository.GetByIdAsync(datPhong.MaKhachHang);
                datPhong.NhanVien = datPhong.MaNhanVien.HasValue ? await _nhanVienRepository.GetByIdAsync(datPhong.MaNhanVien.Value) : null;

                return Ok(new { message = "Check-out successful", data = datPhong });
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return StatusCode(500, new { message = "An error occurred during check-out", error = ex.Message });
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DatPhong datPhong)
        {
            if (id != datPhong.MaDatPhong)
                return BadRequest();

            await _repository.UpdateAsync(datPhong);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }

    public class DatPhongRequest
    {
        public int MaKhachHang { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public string? TrangThai { get; set; }
        public List<ChiTietDatPhongRequest> ChiTietDatPhongs { get; set; } = new();
        public List<SuDungDichVuRequest>? SuDungDichVus { get; set; }
    }

    public class ChiTietDatPhongRequest
    {
        public int MaPhong { get; set; }
        public decimal DonGia { get; set; }
        public int SoDem { get; set; }
    }

    public class SuDungDichVuRequest
    {
        public int MaDichVu { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}