using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Services;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IThanhToanService _service;

        public ThanhToanController(IThanhToanService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var thanhToans = await _service.GetAllWithDatPhongAsync();
            return Ok(thanhToans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var thanhToan = await _service.GetByIdWithDatPhongAsync(id);

            if (thanhToan == null)
                return NotFound();
            return Ok(thanhToan);
        }

        [HttpGet("datphong/{maDatPhong}")]
        public async Task<IActionResult> GetByDatPhong(int maDatPhong)
        {
            var thanhToans = await _service.GetByDatPhongAsync(maDatPhong);
            return Ok(thanhToans);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThanhToan thanhToan)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.AddAsync(thanhToan);
            return CreatedAtAction(nameof(GetById), new { id = created.MaThanhToan }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ThanhToan thanhToan)
        {
            if (id != thanhToan.MaThanhToan)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(thanhToan);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _service.ExistsAsync(id);
            if (!exists)
                return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}