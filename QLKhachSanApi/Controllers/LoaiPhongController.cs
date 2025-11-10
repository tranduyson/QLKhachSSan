using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiPhongController : ControllerBase
    {
        private readonly IRepository<LoaiPhong> _repository;

        public LoaiPhongController(IRepository<LoaiPhong> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loaiPhongs = await _repository.GetAllAsync();
            return Ok(loaiPhongs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loaiPhong = await _repository.GetByIdAsync(id);
            if (loaiPhong == null)
                return NotFound();
            return Ok(loaiPhong);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoaiPhong loaiPhong)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(loaiPhong);
            return CreatedAtAction(nameof(GetById), new { id = created.MaLoaiPhong }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LoaiPhong loaiPhong)
        {
            if (id != loaiPhong.MaLoaiPhong)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.UpdateAsync(loaiPhong);
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