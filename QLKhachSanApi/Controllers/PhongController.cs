using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IRepository<Phong> _repository;
        private readonly HotelDbContext _context;

        public PhongController(IRepository<Phong> repository, HotelDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var phongs = await _context.Phongs.Include(p => p.LoaiPhong).ToListAsync();
            return Ok(phongs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var phong = await _context.Phongs.Include(p => p.LoaiPhong).FirstOrDefaultAsync(p => p.MaPhong == id);
            if (phong == null)
                return NotFound();
            return Ok(phong);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            var bookedRooms = await _context.ChiTietDatPhongs
                .Include(ct => ct.DatPhong)
                .Where(ct => ct.DatPhong.TrangThai != "Hủy" &&
                           ((ct.DatPhong.NgayNhan <= checkOut && ct.DatPhong.NgayTra >= checkIn)))
                .Select(ct => ct.MaPhong)
                .Distinct()
                .ToListAsync();

            var availableRooms = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => !bookedRooms.Contains(p.MaPhong) && p.TinhTrang == "Trống")
                .ToListAsync();

            return Ok(availableRooms);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Phong phong)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(phong);
            return CreatedAtAction(nameof(GetById), new { id = created.MaPhong }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Phong phong)
        {
            if (id != phong.MaPhong)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.UpdateAsync(phong);
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
}