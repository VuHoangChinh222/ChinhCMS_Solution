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
        // Hàm Index trả về danh sách các bài viết mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
            // Lấy dữ liệu THẬT từ bảng Post trong SQL
            var data = _context.Posts.ToList();
            return View(data); // Truyền danh sách dữ liệu mẫu lên giao diện
        }

        // Hàm Details trả về chi tiết của một bài viết dựa trên id được truyền vào
        public IActionResult Details(int id)
        {
            var data = _context.Posts.ToList();
            // Lấy dữ liệu Post dưa theo ID 
            var detailPost = data.FirstOrDefault(p => p.Id == id);
            if (detailPost == null)
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy bài viết
            return View(detailPost); // Truyền dữ liệu mẫu lên giao diện chi tiết
        }
    }
}
