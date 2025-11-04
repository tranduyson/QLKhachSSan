using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Services;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangService _service;

        public KhachHangController(IKhachHangService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var khachHangs = await _service.GetAllAsync();
            return Ok(khachHangs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var khachHang = await _service.GetByIdAsync(id);
            if (khachHang == null)
                return NotFound();
            return Ok(khachHang);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhachHang khachHang)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.AddAsync(khachHang);
            return CreatedAtAction(nameof(GetById), new { id = created.MaKhachHang }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KhachHang khachHang)
        {
            if (id != khachHang.MaKhachHang)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(khachHang);
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