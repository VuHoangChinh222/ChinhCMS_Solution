/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 26/05/2026
 * Version: 1.0
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn gọi API cho Sản phẩm. Đã cấu hình cứng thành "api/product"
    // Địa chỉ gọi API thực tế: https://localhost:7291/api/product
    [Route("api/product")]
    [ApiController]
    public class ApiProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo (Constructor): Tiêm ApplicationDbContext vào
        public ApiProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. API LẤY TOÀN BỘ DANH MỤC SẢN PHẨM
        // ==========================================
        // GET: api/product/categories
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _context.CategoriesProducts
                    .OrderByDescending(c => c.Id)
                    .Select(c => new {
                        c.Id,
                        c.Name,
                        c.Description
                    })
                    .ToList();

                return Ok(categories);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh mục sản phẩm", error = ex.Message });
            }
        }

        // ==========================================
        // 2. API LẤY TOÀN BỘ DANH SÁCH SẢN PHẨM
        // ==========================================
        // GET: api/product
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var products = _context.Products
                    .OrderByDescending(p => p.Id) // Sắp xếp sản phẩm mới nhất lên hàng đầu
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.ImageUrl,
                        p.CategoryProductId,
                        CategoryName = p.CategoryProduct != null ? p.CategoryProduct.Name : "Chưa phân loại"
                    })
                    .ToList();

                return Ok(products);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh sách sản phẩm", error = ex.Message });
            }
        }

        // ==========================================
        // 3. API LỌC SẢN PHẨM THEO DANH MỤC
        // ==========================================
        // GET: api/product/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.CategoryProductId == categoryId)
                    .OrderByDescending(p => p.Id)
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.ImageUrl,
                        p.CategoryProductId,
                        CategoryName = p.CategoryProduct != null ? p.CategoryProduct.Name : "Chưa phân loại"
                    })
                    .ToList();

                return Ok(products);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi lọc sản phẩm theo danh mục", error = ex.Message });
            }
        }

        // ==========================================
        // 4. API LẤY CHI TIẾT SẢN PHẨM
        // ==========================================
        // GET: api/product/{id}
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                var product = _context.Products
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.ImageUrl,
                        p.CategoryProductId,
                        CategoryName = p.CategoryProduct != null ? p.CategoryProduct.Name : "Chưa phân loại"
                    })
                    .FirstOrDefault(p => p.Id == id);

                // Xử lý kịch bản không tồn tại sản phẩm
                if (product == null)
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm này trong kho hàng" });
                }

                return Ok(product);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải chi tiết sản phẩm", error = ex.Message });
            }
        }
    }
}
