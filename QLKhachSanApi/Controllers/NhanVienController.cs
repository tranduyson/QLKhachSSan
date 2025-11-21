using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Repositories;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private readonly INhanVienRepository _repository;

        public NhanVienController(INhanVienRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var nhanViens = await _repository.GetAllAsync();
            return Ok(nhanViens);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var nhanVien = await _repository.GetByIdAsync(id);
            if (nhanVien == null)
                return NotFound();
            return Ok(nhanVien);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var nhanVien = await _repository.FindByCredentialsAsync(request.email, request.password);
            if (nhanVien == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { nhanVien.MaNhanVien, nhanVien.HoTen, nhanVien.ChucVu });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NhanVien nhanVien)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.AddAsync(nhanVien);
            return CreatedAtAction(nameof(GetById), new { id = created.MaNhanVien }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NhanVien nhanVien)
        {
            if (id != nhanVien.MaNhanVien)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.UpdateAsync(nhanVien);
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

    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}