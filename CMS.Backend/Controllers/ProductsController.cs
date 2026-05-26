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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace CMS.Backend.Controllers
{
    // Controller ProductsController quản lý các hành động liên quan đến sản phẩm: hiển thị, thêm mới, chỉnh sửa, xóa sản phẩm.
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. TRANG DANH SÁCH SẢN PHẨM (INDEX)
        // ==========================================
        public IActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 10;

            var query = _context.Products.Include(p => p.CategoryProduct);
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;

            var data = query.OrderByDescending(p => p.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(data);
        }

        // ==========================================
        // 2. THÊM MỚI SẢN PHẨM (CREATE)
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            // Nạp danh sách danh mục sản phẩm vào ViewBag để làm menu thả xuống
            ViewBag.CategoryProductList = new SelectList(_context.CategoriesProducts, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product model, IFormFile uploadImage)
        {
            // Loại bỏ kiểm tra ModelState đối với thuộc tính liên kết ảo của EF Core
            ModelState.Remove("CategoryProduct");

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

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                ModelState.Remove("ImageUrl");
            }

            // Kiểm tra thủ công các trường bắt buộc
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên sản phẩm.");
            }
            if (model.Price < 0)
            {
                ModelState.AddModelError("Price", "Giá sản phẩm không được nhỏ hơn 0.");
            }
            if (model.StockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "Số lượng tồn kho không được nhỏ hơn 0.");
            }
            if (model.CategoryProductId <= 0)
            {
                ModelState.AddModelError("CategoryProductId", "Vui lòng chọn danh mục sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryProductList = new SelectList(_context.CategoriesProducts, "Id", "Name", model.CategoryProductId);
            return View(model);
        }

        // ==========================================
        // 3. CHỈNH SỬA SẢN PHẨM (EDIT)
        // ==========================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm này trong hệ thống.");
            }

            ViewBag.CategoryProductList = new SelectList(_context.CategoriesProducts, "Id", "Name", product.CategoryProductId);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product model, IFormFile uploadImage)
        {
            ModelState.Remove("CategoryProduct");

            if (uploadImage != null && uploadImage.Length > 0)
            {
                // Dọn dẹp tệp tin ảnh vật lý cũ
                var oldProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == model.Id);
                if (oldProduct != null && !string.IsNullOrEmpty(oldProduct.ImageUrl) && oldProduct.ImageUrl.StartsWith("/uploads/"))
                {
                    string oldRelativePath = oldProduct.ImageUrl.TrimStart('/');
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
            else
            {
                // Khôi phục lại ảnh cũ nếu người dùng không cập nhật
                var oldProduct = _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == model.Id);
                if (oldProduct != null && string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = oldProduct.ImageUrl;
                }
            }

            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                ModelState.Remove("ImageUrl");
            }

            // Kiểm tra lỗi thủ công
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên sản phẩm.");
            }
            if (model.Price < 0)
            {
                ModelState.AddModelError("Price", "Giá sản phẩm không được nhỏ hơn 0.");
            }
            if (model.StockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "Số lượng tồn kho không được nhỏ hơn 0.");
            }
            if (model.CategoryProductId <= 0)
            {
                ModelState.AddModelError("CategoryProductId", "Vui lòng chọn danh mục sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                _context.Products.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryProductList = new SelectList(_context.CategoriesProducts, "Id", "Name", model.CategoryProductId);
            return View(model);
        }

        // ==========================================
        // 4. XÓA SẢN PHẨM (DELETE)
        // ==========================================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                                  .Include(p => p.CategoryProduct)
                                  .FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm này trong hệ thống.");
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                // Xóa file ảnh vật lý nếu có
                if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl.StartsWith("/uploads/"))
                {
                    string relativePath = product.ImageUrl.TrimStart('/');
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi khi xóa tệp ảnh sản phẩm: " + ex.Message);
                        }
                    }
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
