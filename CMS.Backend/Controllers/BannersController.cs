/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 11/06/2026
 * Version: 1.0
 */

using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace CMS.Backend.Controllers
{
    // Controller BannersController quản lý các hành động liên quan đến Banner quảng cáo
    // như danh sách, thêm, sửa, xóa banner.
    [Authorize(Roles = "Admin")]
    public class BannersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BannersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. TRANG DANH SÁCH BANNER (INDEX) - PHÂN TRANG (10 PHẦN TỬ / TRANG)
        // ==========================================
        public IActionResult Index(int page = 1)
        {
            // Bước 1: Ràng buộc trang hiện tại không được nhỏ hơn 1
            if (page < 1) page = 1;
            
            // Bước 2: Định nghĩa kích thước trang (yêu cầu hiển thị tối đa 10 phần tử trên một trang)
            int pageSize = 10;

            // Bước 3: Lấy đối tượng truy vấn bảng Banners trong Database
            var query = _context.Banners;
            
            // Bước 4: Đếm tổng số lượng bản ghi Banner có trong CSDL
            int totalItems = query.Count();
            
            // Bước 5: Tính toán tổng số trang dựa trên kích thước trang (Làm tròn lên bằng Math.Ceiling)
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            // Bước 6: Đảm bảo số trang yêu cầu không vượt quá tổng số trang hợp lệ
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Bước 7: Thực hiện phân trang và truy vấn dữ liệu thực tế từ Database
            var data = query.OrderByDescending(b => b.Id) // Sắp xếp theo ID giảm dần (mới nhất lên trên)
                            .Skip((page - 1) * pageSize)  // Bỏ qua các bản ghi của các trang trước
                            .Take(pageSize)               // Lấy số lượng bản ghi tương ứng kích thước trang
                            .ToList();                    // Chuyển đổi kết quả truy vấn thành danh sách List thực tế

            // Bước 8: Gửi các thông số phân trang sang giao diện hiển thị (Razor View) bằng ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            // Trả về giao diện cùng danh sách dữ liệu banner đã được phân trang
            return View(data);
        }

        // ==========================================
        // 2. THÊM MỚI BANNER (CREATE)
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Banner model, IFormFile? uploadImage)
        {
            // Kiểm tra tệp tin ảnh bắt buộc khi thêm mới banner
            if (uploadImage == null || uploadImage.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Vui lòng tải lên hình ảnh cho Banner.");
            }

            // Kiểm tra xem dữ liệu trong biểu mẫu (model) có hợp lệ theo các quy tắc Data Annotations không
            if (!ModelState.IsValid)
            {
                // Trả lại giao diện nhập liệu cùng với các thông báo lỗi tương ứng
                return View(model);
            }

            // Xác định thư mục lưu trữ ảnh tải lên trên máy chủ
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Tạo tên file duy nhất tránh trùng lặp tệp tin
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
            string filePath = Path.Combine(folder, fileName);

            // Ghi tệp tin vào đĩa cứng máy chủ
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                uploadImage.CopyTo(stream);
            }

            // Gán đường dẫn truy cập ảnh vào thực thể Banner để lưu xuống Database
            model.ImageUrl = "/uploads/" + fileName;

            _context.Banners.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ==========================================
        // 3. CHỈNH SỬA BANNER (EDIT)
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var banner = _context.Banners.Find(id);
            if (banner == null)
            {
                return NotFound("Không tìm thấy banner này trong hệ thống.");
            }
            return View(banner);
        }

        [HttpPost]
        public IActionResult Edit(Banner model, IFormFile? uploadImage)
        {
            // Kiểm tra tính hợp lệ của dữ liệu biểu mẫu
            if (!ModelState.IsValid)
            {
                // Trả về view kèm thông báo lỗi cụ thể
                return View(model);
            }

            // Xử lý tải ảnh lên nếu người dùng chọn file ảnh mới
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // Tìm kiếm thực thể cũ (không theo dõi trạng thái - AsNoTracking) để lấy thông tin ảnh cũ
                var oldBanner = _context.Banners.AsNoTracking().FirstOrDefault(b => b.Id == model.Id);
                if (oldBanner != null && !string.IsNullOrEmpty(oldBanner.ImageUrl) && oldBanner.ImageUrl.StartsWith("/uploads/"))
                {
                    // Chuyển đường dẫn tương đối thành đường dẫn vật lý trên server
                    string oldRelativePath = oldBanner.ImageUrl.TrimStart('/');
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldRelativePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            // Thực hiện xóa tệp ảnh cũ khỏi ổ cứng
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi khi xóa tệp ảnh cũ: " + ex.Message);
                        }
                    }
                }

                // Cấu hình lưu trữ ảnh mới
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                // Cập nhật đường dẫn ảnh mới vào mô hình dữ liệu
                model.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                // Nếu không tải ảnh mới lên, giữ nguyên đường dẫn ảnh cũ từ cơ sở dữ liệu
                var oldBanner = _context.Banners.AsNoTracking().FirstOrDefault(b => b.Id == model.Id);
                if (oldBanner != null)
                {
                    model.ImageUrl = oldBanner.ImageUrl;
                }
            }

            _context.Banners.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ==========================================
        // 4. XÓA BANNER (DELETE)
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var banner = _context.Banners.Find(id);
            if (banner == null)
            {
                return NotFound("Không tìm thấy banner này trong hệ thống.");
            }
            return View(banner);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var banner = _context.Banners.Find(id);
            if (banner != null)
            {
                // Xóa ảnh vật lý
                if (!string.IsNullOrEmpty(banner.ImageUrl) && banner.ImageUrl.StartsWith("/uploads/"))
                {
                    string relativePath = banner.ImageUrl.TrimStart('/');
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi khi xóa tệp ảnh cũ: " + ex.Message);
                        }
                    }
                }

                _context.Banners.Remove(banner);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Banners/ToggleStatus/id
        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var banner = _context.Banners.Find(id);
            if (banner == null)
            {
                return NotFound(new { message = "Không tìm thấy banner này." });
            }

            // Đổi trạng thái: 1 -> 0 hoặc 0 -> 1
            banner.Status = banner.Status == 1 ? 0 : 1;

            _context.Banners.Update(banner);
            _context.SaveChanges();

            return Json(new { success = true, newStatus = banner.Status });
        }
    }
}
