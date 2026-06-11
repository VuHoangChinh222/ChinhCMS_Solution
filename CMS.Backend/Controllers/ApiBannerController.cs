/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 11/06/2026
 * Version: 1.0
 */

using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using System.Linq;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn gọi API cho Banner: https://localhost:7291/api/banners
    [Route("api/banners")]
    [ApiController]
    public class ApiBannerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiBannerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách banner đang hoạt động (Status = 1)
        // GET: api/banners
        [HttpGet]
        public IActionResult GetActiveBanners()
        {
            var banners = _context.Banners
                .Where(b => b.Status == 1)
                .OrderByDescending(b => b.Id)
                .ToList();

            return Ok(banners);
        }
    }
}
