using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Services;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IPhongService _service;

        public PhongController(IPhongService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var phongs = await _service.GetAllWithLoaiPhongAsync();
            return Ok(phongs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var phong = await _service.GetByIdWithLoaiPhongAsync(id);
            if (phong == null)
                return NotFound();
            return Ok(phong);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            var availableRooms = await _service.GetAvailableRoomsAsync(checkIn, checkOut);
            return Ok(availableRooms);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Phong phong)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.AddAsync(phong);
            return CreatedAtAction(nameof(GetById), new { id = created.MaPhong }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Phong phong)
        {
            if (id != phong.MaPhong)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(phong);
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