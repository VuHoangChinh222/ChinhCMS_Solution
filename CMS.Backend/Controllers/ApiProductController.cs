/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 26/05/2026
 * Version: 1.1 (Cập nhật phân trang)
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System;
using System.Linq;

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
        // 2. API LẤY TOÀN BỘ DANH SÁCH SẢN PHẨM (CÓ PHÂN TRANG)
        // ==========================================
        // GET: api/product?pageNumber=1&pageSize=10
        [HttpGet]
        public IActionResult GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Ràng buộc dữ liệu đầu vào tối thiểu
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                // 1. Tính tổng số sản phẩm hiện có
                var totalItems = _context.Products.Count();

                // 2. Lấy dữ liệu đã phân trang
                var products = _context.Products
                    .OrderByDescending(p => p.Id) // Sản phẩm mới nhất lên đầu
                    .Skip((pageNumber - 1) * pageSize) // Bỏ qua các bản ghi của các trang trước
                    .Take(pageSize) // Lấy số lượng bản ghi tương ứng kích thước trang
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

                // 3. Tính toán số trang
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                // 4. Trả về cấu trúc phân trang chuẩn
                return Ok(new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải danh sách sản phẩm", error = ex.Message });
            }
        }

        // ==========================================
        // 3. API LỌC SẢN PHẨM THEO DANH MỤC (CÓ PHÂN TRANG)
        // ==========================================
        // GET: api/product/category/{categoryId}?pageNumber=1&pageSize=10
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                // 1. Đếm tổng số sản phẩm thuộc danh mục này
                var totalItems = _context.Products.Count(p => p.CategoryProductId == categoryId);

                // 2. Lấy dữ liệu phân trang theo danh mục
                var products = _context.Products
                    .Where(p => p.CategoryProductId == categoryId)
                    .OrderByDescending(p => p.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
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

                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                return Ok(new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });
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