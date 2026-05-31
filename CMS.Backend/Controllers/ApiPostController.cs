/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 26/05/2026
 * Version: 1.1 (Cập nhật phân trang cho Bài viết)
 */

using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using System;
using System.Linq;

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

        // ===================================================
        // PHẦN 2: LẤY DANH SÁCH BÀI VIẾT CÓ PHÂN TRANG (GET)
        // ===================================================

        // 1. API lấy toàn bộ bài viết mới nhất (Có phân trang)
        // GET: api/post?pageNumber=1&pageSize=10
        [HttpGet]
        public IActionResult GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Ràng buộc dữ liệu đầu vào tối thiểu để tránh lỗi hệ thống
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                // Bước 1: Đếm tổng số bài viết hiện có trong Database
                var totalItems = _context.Posts.Count();

                // Bước 2: Truy cập và phân trang dữ liệu
                var posts = _context.Posts
                    .OrderByDescending(p => p.Id) // Sắp xếp bài mới nhất lên đầu (từ ID lớn đến bé)
                    .Skip((pageNumber - 1) * pageSize) // Bỏ qua các bài viết của các trang trước đó
                    .Take(pageSize) // Lấy số lượng bài viết đúng bằng kích thước trang hiện tại
                    .Select(p => new {
                        p.Id,
                        p.Title,
                        p.ImageUrl,
                        CreatedDate = p.CreatedDate,
                        CategoryName = p.Category != null ? p.Category.Name : "Không xác định"
                    })
                    .ToList();

                // Bước 3: Tính toán tổng số trang dựa trên tổng số bài viết
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Bước 4: Trả về cấu trúc JSON phân trang chuẩn gọn cho Frontend dễ xử lý
                return Ok(new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = posts
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh sách bài viết", error = ex.Message });
            }
        }

        //2. API lấy tất cả danh sách danh mục bài viết 
        // GET: api/post/categories
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _context.Categories
                    .OrderByDescending(c => c.Id)
                    .Select(c => new {
                        c.Id,
                        c.Name
                    })
                    .ToList();
                return Ok(categories);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh sách chuyên mục", error = ex.Message });
            }
        }
        // 3. API lấy danh sách bài viết theo chuyên mục có phân trang (Category)
        // GET: api/post/category/{categoryId}?pageNumber=1&pageSize=10
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Ràng buộc dữ liệu đầu vào tối thiểu
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                // Bước 1: Đếm tổng số bài viết thuộc riêng chuyên mục này
                var totalItems = _context.Posts.Count(p => p.CategoryId == categoryId);

                // Bước 2: Lấy dữ liệu bài viết đã lọc theo chuyên mục và phân trang
                var posts = _context.Posts
                    .Where(p => p.CategoryId == categoryId)
                    .OrderByDescending(p => p.Id) // Sắp xếp bài mới nhất lên đầu
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new {
                        p.Id,
                        p.Title,
                        p.ImageUrl,
                        CreatedDate = p.CreatedDate
                    })
                    .ToList();

                // Bước 3: Tính toán tổng số trang của chuyên mục này
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // Bước 4: Trả về kết quả kèm Metadata phân trang
                return Ok(new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = posts
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh sách bài viết theo chuyên mục", error = ex.Message });
            }
        }

        // ==========================================
        // PHẦN 4: CHI TIẾT BÀI VIẾT (GET BY ID)
        // ==========================================

        // 4. API lấy chi tiết một bài viết cụ thể dựa trên ID (Giữ nguyên không đổi)
        // GET: api/post/{id}
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