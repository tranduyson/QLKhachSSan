using BLL;
using HotelManagement.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
namespace HotelManagement.API.Customer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhachController : ControllerBase
    {
        private readonly KhachBLL _bll;

        public KhachController()
        {
            _bll = new KhachBLL();
        }

        // GET: api/khach
        [HttpGet]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult GetAll()
        {
            try
            {
                DataTable dt = _bll.GetAll();
                var list = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        MaKhach = row["MaKhach"],
                        HoTen = row["HoTen"],
                        Email = row["Email"],
                        DienThoai = row["DienThoai"],
                        DiaChi = row["DiaChi"]
                    });
                }
                return Ok(new { success = true, message = "Lấy danh sách khách thành công", data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // GET: api/khach/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult GetById(int id)
        {
            try
            {
                DataTable dt = _bll.GetById(id);
                if (dt.Rows.Count == 0)
                    return NotFound(new { success = false, message = "Không tìm thấy khách" });

                DataRow r = dt.Rows[0];
                var obj = new
                {
                    MaKhach = r["MaKhach"],
                    HoTen = r["HoTen"],
                    Email = r["Email"],
                    DienThoai = r["DienThoai"],
                    DiaChi = r["DiaChi"]
                };
                return Ok(new { success = true, message = "OK", data = obj });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: api/khach
        [HttpPost]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult Create([FromBody] Khach khach)
        {
            try
            {
                if (khach == null)
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

                string result = _bll.Them(khach);
                if (result.Contains("Lỗi"))
                    return BadRequest(new { success = false, message = result });

                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // PUT: api/khach/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrLeTanOrKeToan")]
        public IActionResult Update(int id, [FromBody] Khach khach)
        {
            try
            {
                if (khach == null)
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });

                khach.MaKhach = id;
                string result = _bll.Sua(khach);
                if (result.Contains("Lỗi"))
                    return BadRequest(new { success = false, message = result });

                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // DELETE: api/khach/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            try
            {
                string result = _bll.Xoa(id);
                if (result.Contains("Lỗi"))
                    return BadRequest(new { success = false, message = result });

                return Ok(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
