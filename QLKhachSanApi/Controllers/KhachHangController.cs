using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IRepository<KhachHang> _repository;

        public KhachHangController(IRepository<KhachHang> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var khachHangs = await _repository.GetAllAsync();
            return Ok(khachHangs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var khachHang = await _repository.GetByIdAsync(id);
            if (khachHang == null)
                return NotFound();
            return Ok(khachHang);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhachHang khachHang)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(khachHang);
            return CreatedAtAction(nameof(GetById), new { id = created.MaKhachHang }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KhachHang khachHang)
        {
            if (id != khachHang.MaKhachHang)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.UpdateAsync(khachHang);
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