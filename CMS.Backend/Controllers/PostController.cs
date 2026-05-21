/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Data;
using CMS.Data.Entities; // Kết nối tới lớp dữ liệu
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    // Controller PostController quản lý các hành động liên quan đến bài viết tin tức
    // , ví dụ: hiển thị danh sách các bài viết.
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Hàm hiển thị danh sách bài viết, hỗ trợ lọc theo mã danh mục bài viết (CategoryId)
        public IActionResult Index(int? id)
        {
            // Nếu không truyền mã danh mục (id là null), hệ thống sẽ lấy toàn bộ danh sách bài viết
            if (id == null)
            {
                // Truy vấn lấy toàn bộ bài viết từ cơ sở dữ liệu, sắp xếp theo ngày đăng mới nhất
                // và nạp kèm thông tin danh mục tương ứng để tránh lỗi hiển thị tên danh mục
                var allPosts = _context.Posts
                                      .Include(p => p.Category)
                                      .OrderByDescending(p => p.CreatedDate)
                                      .ToList();

                // Truyền danh sách tất cả bài viết sang giao diện hiển thị
                return View(allPosts);
            }

            // Nếu có truyền mã danh mục, tiến hành lọc các bài viết thuộc danh mục đó
            var filteredPosts = _context.Posts
                                        .Where(p => p.CategoryId == id)
                                        .Include(p => p.Category)
                                        .OrderByDescending(p => p.CreatedDate)
                                        .ToList();

            // Truyền danh sách bài viết đã được lọc sang giao diện hiển thị
            return View(filteredPosts);
        }

        // Hàm hiển thị chi tiết của một bài viết dựa vào mã bài viết được truyền từ URL
        public IActionResult Details(int? id)
        {
            // Kiểm tra xem mã bài viết truyền vào có hợp lệ hay không
            if (id == null)
            {
                // Trả về thông báo lỗi nếu không nhận được mã bài viết từ yêu cầu của trình duyệt
                return BadRequest("Vui lòng cung cấp mã bài viết.");
            }

            // Truy vấn lấy thông tin bài viết theo mã bài viết
            // Sử dụng lệnh nạp chồng để lấy kèm thông tin danh mục liên quan của bài viết đó
            var detailPost = _context.Posts
                                     .Include(p => p.Category)
                                     .FirstOrDefault(p => p.Id == id);

            // Kiểm tra xem có tìm thấy bài viết trong cơ sở dữ liệu hay không
            if (detailPost == null)
            {
                // Trả về lỗi không tìm thấy trang nếu bài viết không tồn tại trong hệ thống
                return NotFound("Không tìm thấy bài viết này trong hệ thống.");
            }

            // Truyền thông tin chi tiết bài viết sang giao diện chi tiết
            return View(detailPost);
        }
    }
}
