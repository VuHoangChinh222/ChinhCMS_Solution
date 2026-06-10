/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CMS.Backend.Controllers
{
    // Controller CategoriesProductsController quản lý các hành động liên quan đến danh mục sản phẩm,
    // ví dụ: hiển thị danh sách, thêm, sửa, xóa các danh mục sản phẩm.
    [Authorize]
    public class CategoriesProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. TRANG DANH SÁCH DANH MỤC SẢN PHẨM (INDEX)
        // ==========================================
        public IActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            var query = _context.CategoriesProducts.Include(c => c.Products);
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;

            var data = query.OrderByDescending(c => c.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(data);
        }

        // ==========================================
        // 2. THÊM MỚI DANH MỤC SẢN PHẨM (CREATE)
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryProduct model, IFormFile? uploadImage)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên danh mục sản phẩm.");
                return View(model);
            }

            // Xử lý tải ảnh lên nếu có file được chọn
            if (uploadImage != null && uploadImage.Length > 0)
            {
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

                model.ImageUrl = "/uploads/" + fileName;
            }

            _context.CategoriesProducts.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ==========================================
        // 3. CHỈNH SỬA DANH MỤC SẢN PHẨM (EDIT)
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.CategoriesProducts.Find(id);
            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục sản phẩm này trong hệ thống.");
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(CategoryProduct model, IFormFile? uploadImage)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên danh mục sản phẩm.");
                return View(model);
            }

            // Xử lý tải ảnh lên nếu có file được chọn
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // Dọn dẹp tệp tin ảnh vật lý cũ
                var oldCategory = _context.CategoriesProducts.AsNoTracking().FirstOrDefault(c => c.Id == model.Id);
                if (oldCategory != null && !string.IsNullOrEmpty(oldCategory.ImageUrl) && oldCategory.ImageUrl.StartsWith("/uploads/"))
                {
                    string oldRelativePath = oldCategory.ImageUrl.TrimStart('/');
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldRelativePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi khi xóa tệp ảnh cũ: " + ex.Message);
                        }
                    }
                }

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

                model.ImageUrl = "/uploads/" + fileName;
            }

            _context.CategoriesProducts.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ==========================================
        // 4. XÓA DANH MỤC SẢN PHẨM (DELETE)
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Tìm danh mục kèm danh sách sản phẩm liên kết
            var category = _context.CategoriesProducts
                                   .Include(c => c.Products)
                                   .FirstOrDefault(c => c.Id == id);
            
            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục sản phẩm này trong hệ thống.");
            }

            // Nếu số lượng sản phẩm > 0, chặn không cho xóa
            if (category.Products != null && category.Products.Count > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục này vì đang có sản phẩm thuộc danh mục!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.CategoriesProducts
                                   .Include(c => c.Products)
                                   .FirstOrDefault(c => c.Id == id);

            if (category != null)
            {
                // Bảo vệ thêm ở phía server: nếu có sản phẩm, không thực hiện xóa
                if (category.Products != null && category.Products.Count > 0)
                {
                    TempData["ErrorMessage"] = "Không thể xóa danh mục vì đang chứa sản phẩm!";
                    return RedirectToAction("Index");
                }

                // Xóa ảnh vật lý nếu có
                if (!string.IsNullOrEmpty(category.ImageUrl) && category.ImageUrl.StartsWith("/uploads/"))
                {
                    string relativePath = category.ImageUrl.TrimStart('/');
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

                _context.CategoriesProducts.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
