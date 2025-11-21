using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DichVuController : ControllerBase
    {
        private readonly IRepository<DichVu> _repository;

        public DichVuController(IRepository<DichVu> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dichVus = await _repository.GetAllAsync();
            return Ok(dichVus);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dichVu = await _repository.GetByIdAsync(id);
            if (dichVu == null)
                return NotFound();
            return Ok(dichVu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DichVu dichVu)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(dichVu);
            return CreatedAtAction(nameof(GetById), new { id = created.MaDichVu }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DichVu dichVu)
        {
            // Ensure the ID from URL matches the object
            dichVu.MaDichVu = id;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.UpdateAsync(dichVu);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}