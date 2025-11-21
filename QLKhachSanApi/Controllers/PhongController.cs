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
        private readonly IPhongRepository _repository;

        public PhongController(IPhongRepository repo)
        {
            _repository = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var phongs = await _repository.GetAllAsync();
            return Ok(phongs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var phong = await _repository.GetByIdAsync(id);
            if (phong == null)
                return NotFound();
            return Ok(phong);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            var availableRooms = await _repository.GetAvailableRoomsAsync(checkIn, checkOut);
            return Ok(availableRooms);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Phong phong)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(phong);
            return CreatedAtAction(nameof(GetById), new { id = created.SoPhong }, created);
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