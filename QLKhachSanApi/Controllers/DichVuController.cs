using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Services;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DichVuController : ControllerBase
    {
        private readonly IDichVuService _service;

        public DichVuController(IDichVuService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dichVus = await _service.GetAllAsync();
            return Ok(dichVus);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dichVu = await _service.GetByIdAsync(id);
            if (dichVu == null)
                return NotFound();
            return Ok(dichVu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DichVu dichVu)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.AddAsync(dichVu);
            return CreatedAtAction(nameof(GetById), new { id = created.MaDichVu }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DichVu dichVu)
        {
            if (id != dichVu.MaDichVu)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(dichVu);
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