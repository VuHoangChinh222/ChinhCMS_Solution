/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Data;
using CMS.Data.Entities;
using CMS.Backend.Helpers;
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
        public IActionResult Create(Product model, IFormFile? uploadImage)
        {
            // 1. Loại bỏ các thuộc tính không cần xác thực trực tiếp khỏi ModelState.
            // Điều này rất quan trọng khi sử dụng Non-Nullable Reference Types trong .NET Core,
            // giúp tránh việc Form bị từ chối do các trường ảo (Navigation properties) hoặc các trường tùy chọn.
            ModelState.Remove("CategoryProduct");
            ModelState.Remove("Slug");
            ModelState.Remove("uploadImage");

            // 2. Xác định phương thức điền Slug: Tự gõ thủ công (Manual) hay Tự động sinh (Auto).
            bool isManuallyEntered = !string.IsNullOrEmpty(model.Slug);

            if (string.IsNullOrEmpty(model.Slug))
            {
                // Nếu người dùng không nhập, tự động tạo slug thô từ tên sản phẩm.
                model.Slug = SlugHelper.GenerateSlug(model.Name);
            }
            else
            {
                // Nếu tự nhập, chuyển đổi chuỗi thô của người dùng thành định dạng slug chuẩn SEO (không dấu, gạch ngang).
                model.Slug = SlugHelper.GenerateSlug(model.Slug);
            }

            // 3. Xử lý kiểm tra trùng lặp và xung đột Slug:
            if (isManuallyEntered)
            {
                // TRƯỜNG HỢP TỰ GÕ: Nếu slug tự nhập đã tồn tại trong DB, hiển thị cảnh báo lỗi (đúng nguyên tắc UX).
                if (_context.Products.Any(p => p.Slug == model.Slug))
                {
                    ModelState.AddModelError("Slug", "Đường dẫn thân thiện (Slug) này đã tồn tại trong hệ thống. Vui lòng chọn đường dẫn khác.");
                }
            }
            else
            {
                // TRƯỜNG HỢP TỰ ĐỘNG SINH: Tự động phát hiện trùng lặp và thêm số hậu tố để đảm bảo lưu thành công mà không gây lỗi (ví dụ: 'giay-nike-1', 'giay-nike-2').
                string baseSlug = model.Slug;
                int counter = 1;
                while (_context.Products.Any(p => p.Slug == model.Slug))
                {
                    model.Slug = $"{baseSlug}-{counter}";
                    counter++;
                }
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
        public IActionResult Edit(Product model, IFormFile? uploadImage)
        {
            // 1. Loại bỏ các thuộc tính không cần xác thực trực tiếp khỏi ModelState.
            // Loại bỏ 'uploadImage' để không bắt buộc tải lên hình ảnh mới khi cập nhật.
            ModelState.Remove("CategoryProduct");
            ModelState.Remove("Slug");
            ModelState.Remove("uploadImage");

            // 2. Xác định phương thức điền Slug: Tự gõ thủ công (Manual) hay Tự động sinh (Auto).
            bool isManuallyEntered = !string.IsNullOrEmpty(model.Slug);

            if (string.IsNullOrEmpty(model.Slug))
            {
                // Nếu để trống, tự sinh slug thô từ tên sản phẩm.
                model.Slug = SlugHelper.GenerateSlug(model.Name);
            }
            else
            {
                // Nếu tự gõ, chuẩn hóa về slug viết thường không dấu.
                model.Slug = SlugHelper.GenerateSlug(model.Slug);
            }

            // 3. Xử lý kiểm tra trùng lặp và xung đột Slug cho hành động chỉnh sửa (loại trừ chính sản phẩm hiện tại thông qua Id):
            if (isManuallyEntered)
            {
                // TRƯỜNG HỢP TỰ GÕ: Nếu trùng với bất kỳ sản phẩm nào khác trong DB, trả về thông báo lỗi.
                if (_context.Products.Any(p => p.Slug == model.Slug && p.Id != model.Id))
                {
                    ModelState.AddModelError("Slug", "Đường dẫn thân thiện (Slug) này đã tồn tại trong hệ thống. Vui lòng chọn đường dẫn khác.");
                }
            }
            else
            {
                // TRƯỜNG HỢP TỰ ĐỘNG SINH: Tự động thêm hậu tố số để tránh xung đột với các sản phẩm khác.
                string baseSlug = model.Slug;
                int counter = 1;
                while (_context.Products.Any(p => p.Slug == model.Slug && p.Id != model.Id))
                {
                    model.Slug = $"{baseSlug}-{counter}";
                    counter++;
                }
            }

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
        // 4. XÓA SẢN PHẨM (DELETE) - CÓ KIỂM TRA ĐƠN HÀNG LIÊN KẾT
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

            // Kiểm tra xem sản phẩm này có nằm trong đơn hàng nào không
            var associatedOrders = _context.OrderDetails
                                           .Include(od => od.Order)
                                               .ThenInclude(o => o.Customer)
                                           .Where(od => od.ProductId == id)
                                           .Select(od => od.Order)
                                           .Distinct()
                                           .ToList();

            // Truyền thông tin đơn hàng liên kết sang View để hiển thị cảnh báo
            ViewBag.AssociatedOrders = associatedOrders;
            ViewBag.HasAssociatedOrders = associatedOrders.Any();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // BẢO VỆ: Kiểm tra lại phía Server xem sản phẩm có đang tồn tại trong đơn hàng nào không
            // Ngăn chặn trường hợp gửi POST request trực tiếp vượt qua giao diện
            var hasOrders = _context.OrderDetails.Any(od => od.ProductId == id);
            if (hasOrders)
            {
                TempData["ErrorMessage"] = $"Không thể xóa sản phẩm '{product.Name}' vì đang có đơn hàng chứa sản phẩm này!";
                return RedirectToAction("Index");
            }

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
            return RedirectToAction("Index");
        }
    }
}
