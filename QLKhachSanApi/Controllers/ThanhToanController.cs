using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKhachSanApi.DAL;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IRepository<ThanhToan> _repository;
        private readonly HotelDbContext _context;

        public ThanhToanController(IRepository<ThanhToan> repository, HotelDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var thanhToans = await _context.ThanhToans
                .Include(tt => tt.DatPhong)
                .ToListAsync();
            return Ok(thanhToans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var thanhToan = await _context.ThanhToans
                .Include(tt => tt.DatPhong)
                .FirstOrDefaultAsync(tt => tt.MaThanhToan == id);

            if (thanhToan == null)
                return NotFound();
            return Ok(thanhToan);
        }

        [HttpGet("datphong/{maDatPhong}")]
        public async Task<IActionResult> GetByDatPhong(int maDatPhong)
        {
            var thanhToans = await _context.ThanhToans
                .Include(tt => tt.DatPhong)
                .Where(tt => tt.MaDatPhong == maDatPhong)
                .ToListAsync();
            return Ok(thanhToans);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThanhToan thanhToan)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(thanhToan);
            return CreatedAtAction(nameof(GetById), new { id = created.MaThanhToan }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ThanhToan thanhToan)
        {
            if (id != thanhToan.MaThanhToan)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.UpdateAsync(thanhToan);
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