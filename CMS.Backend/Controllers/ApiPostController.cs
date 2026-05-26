/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 26/05/2026
 * Version: 1.0
 */

using Microsoft.AspNetCore.Mvc;
using CMS.Data;

namespace CMS.Backend.Controllers
{
    // 1. Định nghĩa đường dẫn gọi API. Đã cấu hình cứng thành "api/post" theo yêu cầu của bạn
    // Địa chỉ gọi API thực tế: https://localhost:7291/api/post
    [Route("api/post")]
    
    // 2. Đánh dấu đây là một API Controller để hệ thống hỗ trợ các tính năng RESTful và tự động kiểm tra dữ liệu
    [ApiController]
    
    // 3. API Controller kế thừa từ ControllerBase để giảm tải các tính năng MVC HTML không cần thiết
    public class ApiPostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // 4. Hàm khởi tạo (Constructor): Tiêm ApplicationDbContext kết nối Database vào để sử dụng
        public ApiPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // PHẦN 2: LẤY DANH SÁCH BÀI VIẾT (GET METHOD)
        // ==========================================

        // 1. API lấy toàn bộ bài viết mới nhất
        // GET: api/ApiPost
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var posts = _context.Posts
                    .OrderByDescending(p => p.Id) // Sắp xếp bài mới nhất lên đầu (từ ID lớn đến bé)
                    .Select(p => new {
                        p.Id,
                        p.Title,
                        p.ImageUrl,
                        CreatedDate = p.CreatedDate,
                        CategoryName = p.Category != null ? p.Category.Name : "Không xác định"
                    })
                    .ToList();

                return Ok(posts); // Trả về mã trạng thái 200 OK kèm dữ liệu JSON
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh sách bài viết", error = ex.Message });
            }
        }

        // 2. API lấy danh sách bài viết theo chuyên mục (Category)
        // GET: api/ApiPost/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            try
            {
                var posts = _context.Posts
                    .Where(p => p.CategoryId == categoryId)
                    .OrderByDescending(p => p.Id) // Sắp xếp bài mới nhất lên đầu
                    .Select(p => new {
                        p.Id,
                        p.Title,
                        p.ImageUrl,
                        CreatedDate = p.CreatedDate
                    })
                    .ToList();

                return Ok(posts); // Trả về danh sách bài viết thuộc chuyên mục
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh sách bài viết theo chuyên mục", error = ex.Message });
            }
        }

        // ==========================================
        // PHẦN 3: CHI TIẾT BÀI VIẾT (GET BY ID)
        // ==========================================

        // 3. API lấy chi tiết một bài viết cụ thể dựa trên ID
        // GET: api/ApiPost/{id}
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                // Tìm bài viết theo ID và nạp thông tin danh mục liên kết
                var post = _context.Posts
                    .Select(p => new {
                        p.Id,
                        p.Title,
                        p.Content,
                        p.ImageUrl,
                        CreatedDate = p.CreatedDate,
                        p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : "Không xác định"
                    })
                    .FirstOrDefault(p => p.Id == id);

                // Nếu không tìm thấy bài viết trong hệ thống
                if (post == null)
                {
                    // Trả về mã lỗi 404 Not Found kèm thông báo lỗi dạng JSON
                    return NotFound(new { message = "Không tìm thấy bài viết này trong hệ thống" });
                }

                return Ok(post); // Trả về thông tin đầy đủ của bài viết
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải chi tiết bài viết", error = ex.Message });
            }
        }
    }
}
