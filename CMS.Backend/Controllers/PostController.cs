/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using Microsoft.AspNetCore.Mvc;
using CMS.Data.Entities; // Kết nối tới lớp dữ liệu

namespace CMS.Backend.Controllers
{
    // Controller PostController quản lý các hành động liên quan đến bài viết tin tức
    // , ví dụ: hiển thị danh sách các bài viết.
    public class PostController : Controller
    {
        // Tạo danh sách các dữ liệu mẫu toàn cục có thể dùng chung cho 2 hàm index và detail
        private List<Post> post = new List<Post> {
                new Post { Id = 1, Title = "Tin Giáo Dục Hôm Nay", Content = "Nội dung chi tiết về tin giáo dục hôm nay",
                    ImageUrl = "https://tse4.mm.bing.net/th/id/OIP._8oz8stVnLGxjKspfdnnGAHaEK?rs=1&pid=ImgDetMain&o=7&rm=3", CreatedDate = DateTime.Now.AddDays(-2) },
                new Post { Id = 2, Title = "Tin Thể Thao Hôm Nay", Content = "Nội dung chi tiết về tin thể thao hôm nay",
                    ImageUrl = "https://tse4.mm.bing.net/th/id/OIP._8oz8stVnLGxjKspfdnnGAHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedDate = DateTime.Now.AddDays(-2) },
                new Post { Id = 3, Title = "Tin Công Nghệ Hôm Nay", Content = "Nội dung chi tiết về tin công nghệ hôm nay",
                    ImageUrl = "https://tse4.mm.bing.net/th/id/OIP._8oz8stVnLGxjKspfdnnGAHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedDate = DateTime.Now.AddDays(-2)},
                new Post { Id = 4, Title = "AI hôm nay", Content = "AI đang tiến gần hơn tới thế giới vật lý",
                    ImageUrl = "https://tse4.mm.bing.net/th/id/OIP._8oz8stVnLGxjKspfdnnGAHaEK?rs=1&pid=ImgDetMain&o=7&rm=3",CreatedDate = DateTime.Now.AddDays(-2)}
            };
        // Hàm Index trả về danh sách các bài viết mẫu để hiển thị trên giao diện
        public IActionResult Index()
        {
            return View(post); // Truyền danh sách dữ liệu mẫu lên giao diện
        }

        // Hàm Details trả về chi tiết của một bài viết dựa trên id được truyền vào
        public IActionResult Details(int id)
        {
            // Lấy dữ liệu Post dưa theo ID 
            var detailPost = post.FirstOrDefault(p => p.Id == id);
            if (detailPost == null)
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy bài viết
            return View(detailPost); // Truyền dữ liệu mẫu lên giao diện chi tiết
        }
    }
}
