using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Services;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiPhongController : ControllerBase
    {
        private readonly ILoaiPhongService _service;

        public LoaiPhongController(ILoaiPhongService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loaiPhongs = await _service.GetAllAsync();
            return Ok(loaiPhongs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loaiPhong = await _service.GetByIdAsync(id);
            if (loaiPhong == null)
                return NotFound();
            return Ok(loaiPhong);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoaiPhong loaiPhong)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.AddAsync(loaiPhong);
            return CreatedAtAction(nameof(GetById), new { id = created.MaLoaiPhong }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LoaiPhong loaiPhong)
        {
            if (id != loaiPhong.MaLoaiPhong)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(loaiPhong);
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