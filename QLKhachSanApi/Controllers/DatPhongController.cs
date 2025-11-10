using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatPhongController : ControllerBase
    {
        private readonly IRepository<DatPhong> _repository;
        private readonly IRepository<ChiTietDatPhong> _chiTietRepository;
        private readonly IRepository<SuDungDichVu> _dichVuRepository;
        private readonly HotelDbContext _context;

        public DatPhongController(
            IRepository<DatPhong> repository,
            IRepository<ChiTietDatPhong> chiTietRepository,
            IRepository<SuDungDichVu> dichVuRepository,
            HotelDbContext context)
        {
            _repository = repository;
            _chiTietRepository = chiTietRepository;
            _dichVuRepository = dichVuRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var datPhongs = await _context.DatPhongs
                .Include(dp => dp.KhachHang)
                .Include(dp => dp.NhanVien)
                .Include(dp => dp.ChiTietDatPhongs).ThenInclude(ct => ct.Phong).ThenInclude(p => p.LoaiPhong)
                .Include(dp => dp.SuDungDichVus).ThenInclude(sd => sd.DichVu)
                .ToListAsync();
            return Ok(datPhongs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var datPhong = await _context.DatPhongs
                .Include(dp => dp.KhachHang)
                .Include(dp => dp.NhanVien)
                .Include(dp => dp.ChiTietDatPhongs).ThenInclude(ct => ct.Phong).ThenInclude(p => p.LoaiPhong)
                .Include(dp => dp.SuDungDichVus).ThenInclude(sd => sd.DichVu)
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == id);

            if (datPhong == null)
                return NotFound();
            return Ok(datPhong);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DatPhongRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create DatPhong
                var datPhong = new DatPhong
                {
                    MaKhachHang = request.MaKhachHang,
                    MaNhanVien = request.MaNhanVien,
                    NgayDat = request.NgayDat,
                    NgayNhan = request.NgayNhan,
                    NgayTra = request.NgayTra,
                    TrangThai = request.TrangThai ?? "Đã đặt"
                };

                var createdDatPhong = await _repository.AddAsync(datPhong);

                // Add ChiTietDatPhong
                foreach (var chiTiet in request.ChiTietDatPhongs)
                {
                    var chiTietDatPhong = new ChiTietDatPhong
                    {
                        MaDatPhong = createdDatPhong.MaDatPhong,
                        MaPhong = chiTiet.MaPhong,
                        DonGia = chiTiet.DonGia,
                        SoDem = chiTiet.SoDem
                    };
                    await _chiTietRepository.AddAsync(chiTietDatPhong);

                    // Update room status
                    var phong = await _context.Phongs.FindAsync(chiTiet.MaPhong);
                    if (phong != null)
                    {
                        phong.TinhTrang = "Đang ở";
                        _context.Phongs.Update(phong);
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
                        await _dichVuRepository.AddAsync(suDungDichVu);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = createdDatPhong.MaDatPhong }, createdDatPhong);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPut("{id}/checkin")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var datPhong = await _repository.GetByIdAsync(id);
            if (datPhong == null)
                return NotFound();

            datPhong.TrangThai = "Đã nhận";
            await _repository.UpdateAsync(datPhong);

            return Ok(datPhong);
        }

        [HttpPut("{id}/checkout")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var datPhong = await _repository.GetByIdAsync(id);
            if (datPhong == null)
                return NotFound();

            datPhong.TrangThai = "Đã trả";

            // Update room status back to available
            var chiTietPhongs = await _context.ChiTietDatPhongs
                .Where(ct => ct.MaDatPhong == id)
                .ToListAsync();

            foreach (var chiTiet in chiTietPhongs)
            {
                var phong = await _context.Phongs.FindAsync(chiTiet.MaPhong);
                if (phong != null)
                {
                    phong.TinhTrang = "Trống";
                    _context.Phongs.Update(phong);
                }
            }

            await _repository.UpdateAsync(datPhong);
            await _context.SaveChangesAsync();

            return Ok(datPhong);
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