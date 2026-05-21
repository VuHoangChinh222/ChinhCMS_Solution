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
    // Controller CategoryController quản lý các hành động liên quan đến danh mục tin tức
    // , ví dụ: hiển thị danh sách các danh mục.
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        // "Tiêm" kết nối vào Controller
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm Index hiển thị danh sách toàn bộ danh mục tin tức hiện có
        public IActionResult Index()
        {
            // Lấy dữ liệu thực tế từ bảng Categories trong cơ sở dữ liệu
            var data = _context.Categories.ToList();
            
            // Truyền danh sách danh mục sang giao diện hiển thị
            return View(data);
        }

        // ==========================================
        // CHỨC NĂNG THÊM MỚI DANH MỤC (CREATE)
        // ==========================================

        // Hàm GET: Hiển thị giao diện biểu mẫu (Form) để người dùng nhập thông tin danh mục mới
        [HttpGet]
        public IActionResult Create()
        {
            // Trả về giao diện trống để người dùng điền thông tin
            return View();
        }

        // Hàm POST: Tiếp nhận thông tin danh mục mới được gửi lên từ trình duyệt và lưu vào cơ sở dữ liệu
        [HttpPost]
        public IActionResult Create(Category model)
        {
            // Loại bỏ kiểm tra ModelState.IsValid tự động để tránh lỗi xác thực thuộc tính liên kết (Posts) bị null trong .NET 8.
            // Thay vào đó, chúng ta chủ động kiểm tra xem người dùng có nhập Tên danh mục hay không.
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên danh mục bài viết.");
                return View(model);
            }

            // Bước 1: Thêm đối tượng mới vào bộ nhớ tạm của hệ thống
            _context.Categories.Add(model);

            // Bước 2: Lưu thay đổi thực sự xuống SQL Server
            _context.SaveChanges();

            // Sau khi lưu thành công, tự động chuyển hướng người dùng về trang danh sách danh mục
            return RedirectToAction("Index");
        }

        // ==========================================
        // CHỨC NĂNG CHỈNH SỬA DANH MỤC (EDIT)
        // ==========================================

        // Hàm GET: Tìm danh mục cần sửa dựa vào mã số (Id) và hiển thị thông tin hiện tại lên biểu mẫu
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Truy vấn lấy ra danh mục cần chỉnh sửa theo mã số
            var category = _context.Categories.Find(id);

            // Kiểm tra nếu không tồn tại danh mục có mã số tương ứng
            if (category == null)
            {
                // Trả về lỗi không tìm thấy trang
                return NotFound("Không tìm thấy danh mục bài viết này trong hệ thống.");
            }

            // Truyền thông tin danh mục tìm được sang giao diện chỉnh sửa
            return View(category);
        }

        // Hàm POST: Tiếp nhận thông tin danh mục đã được thay đổi từ người dùng và cập nhật vào cơ sở dữ liệu
        [HttpPost]
        public IActionResult Edit(Category model)
        {
            // Tương tự, chỉ kiểm tra trường Tên danh mục bắt buộc nhập để tránh lỗi thuộc tính liên kết Posts bị null
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên danh mục bài viết.");
                return View(model);
            }

            // Cập nhật thông tin danh mục vào bộ nhớ tạm của hệ thống
            _context.Categories.Update(model);

            // Lưu thay đổi thực sự xuống SQL Server
            _context.SaveChanges();

            // Quay lại trang danh sách danh mục để xem kết quả cập nhật
            return RedirectToAction("Index");
        }
        // ==========================================
        // CHỨC NĂNG XÓA DANH MỤC (DELETE)
        // ==========================================

        // Hàm GET: Tìm danh mục muốn xóa dựa theo mã số (Id), nạp kèm danh sách bài viết và hiển thị trang xác nhận xóa
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Tìm kiếm danh mục cần xóa theo mã số, đồng thời nạp kèm danh sách bài viết thuộc danh mục đó
            var category = _context.Categories
                                   .Include(c => c.Posts)
                                   .FirstOrDefault(c => c.Id == id);

            // Nếu không tìm thấy danh mục bài viết
            if (category == null)
            {
                // Báo lỗi không tìm thấy trang
                return NotFound("Không tìm thấy danh mục bài viết này trong hệ thống.");
            }

            // Truyền thông tin danh mục muốn xóa sang giao diện xác nhận xóa
            return View(category);
        }

        // Hàm POST: Xác nhận thực hiện việc xóa vĩnh viễn danh mục bài viết khỏi cơ sở dữ liệu
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Tìm kiếm đối tượng danh mục cần xóa trong cơ sở dữ liệu
            var category = _context.Categories.Find(id);

            // Nếu đối tượng tồn tại trong hệ thống
            if (category != null)
            {
                // Xóa đối tượng khỏi bộ nhớ tạm của hệ thống
                _context.Categories.Remove(category);

                // Ghi nhận thay đổi xóa thực sự xuống SQL Server
                _context.SaveChanges();
            }

            // Quay trở lại trang danh sách danh mục sau khi hoàn thành xóa
            return RedirectToAction("Index");
        }
    }
}
