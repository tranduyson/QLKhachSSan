using Microsoft.AspNetCore.Mvc;
using QLKhachSanApi.Models;
using QLKhachSanApi.Services;

namespace QLKhachSanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatPhongController : ControllerBase
    {
        private readonly IDatPhongService _service;

        public DatPhongController(IDatPhongService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var datPhongs = await _service.GetAllExpandedAsync();
            return Ok(datPhongs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var datPhong = await _service.GetByIdExpandedAsync(id);

            if (datPhong == null)
                return NotFound();
            return Ok(datPhong);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DatPhongRequest request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.MaDatPhong }, created);
        }

        [HttpPut("{id}/checkin")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var exists = await _service.ExistsAsync(id);
            if (!exists)
                return NotFound();
            await _service.CheckInAsync(id);
            var updated = await _service.GetByIdExpandedAsync(id);
            return Ok(updated);
        }

        [HttpPut("{id}/checkout")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var exists = await _service.ExistsAsync(id);
            if (!exists)
                return NotFound();
            await _service.CheckOutAsync(id);
            var updated = await _service.GetByIdExpandedAsync(id);
            return Ok(updated);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DatPhong datPhong)
        {
            if (id != datPhong.MaDatPhong)
                return BadRequest();

            await _service.UpdateAsync(datPhong);
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

    public class DatPhongRequest
    {
        public int MaKhachHang { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public string? TrangThai { get; set; }
        public List<ChiTietDatPhongRequest> ChiTietDatPhongs { get; set; } = new();
        public List<SuDungDichVuRequest>? SuDungDichVus { get; set; }
    }

    public class ChiTietDatPhongRequest
    {
        public int MaPhong { get; set; }
        public decimal DonGia { get; set; }
        public int SoDem { get; set; }
    }

    public class SuDungDichVuRequest
    {
        public int MaDichVu { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}