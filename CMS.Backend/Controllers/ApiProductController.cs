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
using CMS.Data.Entities;
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
                        c.Description,
                        c.ImageUrl
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
        public IActionResult GetAll(
            [FromQuery] string? keyword = null, 
            [FromQuery] decimal? minPrice = null, 
            [FromQuery] decimal? maxPrice = null, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            // Ràng buộc dữ liệu đầu vào tối thiểu
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            // Ràng buộc đơn giá tối thiểu không cao hơn đơn giá tối đa
            if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
            {
                return BadRequest(new { message = "Đơn giá tối thiểu (minPrice) không được cao hơn đơn giá tối đa (maxPrice)." });
            }

            try
            {
                IQueryable<Product> query = _context.Products;

                // Lọc theo khoảng giá minPrice nếu có
                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice.Value);
                }

                // Lọc theo khoảng giá maxPrice nếu có
                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }

                // Thực hiện lọc theo từ khóa tìm kiếm nếu có
                if (!string.IsNullOrEmpty(keyword))
                {
                    string kw = keyword.Trim().ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(kw) 
                                          || (p.Description != null && p.Description.ToLower().Contains(kw)) 
                                          || (p.CategoryProduct != null && p.CategoryProduct.Name.ToLower().Contains(kw)));
                }

                // 1. Tính tổng số sản phẩm sau khi lọc
                var totalItems = query.Count();

                // 2. Lấy dữ liệu đã phân trang
                var products = query
                    .OrderByDescending(p => p.Id) // Sản phẩm mới nhất lên đầu
                    .Skip((pageNumber - 1) * pageSize) // Bỏ qua các bản ghi của các trang trước
                    .Take(pageSize) // Lấy số lượng bản ghi tương ứng kích thước trang
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Slug,
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
                        p.Slug,
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
        // 4. API LẤY 5 SẢN PHẨM MỚI NHẤT (NEWEST)
        // ==========================================
        // GET: api/product/newest
        [HttpGet("newest")]
        public IActionResult GetNewest()
        {
            try
            {
                var products = _context.Products
                    .OrderByDescending(p => p.Id) // Sắp xếp ID lớn nhất (mới nhất) lên đầu
                    .Take(5) // Chỉ lấy đúng 5 sản phẩm
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Slug,
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
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải top sản phẩm mới nhất", error = ex.Message });
            }
        }

        // ==========================================
        // 5. API LẤY 5 SẢN PHẨM BÁN CHẠY NHẤT (BEST SELLERS)
        // ==========================================
        // GET: api/product/best-sellers
        [HttpGet("best-sellers")]
        public IActionResult GetBestSellers()
        {
            try
            {
                // Truy vấn tính tổng số lượng bán dựa trên bảng OrderDetails
                var products = _context.Products
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Slug,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.ImageUrl,
                        p.CategoryProductId,
                        CategoryName = p.CategoryProduct != null ? p.CategoryProduct.Name : "Chưa phân loại",
                        // Tính tổng số lượng Quantity trong bảng OrderDetails tương ứng với ProductId này (chỉ những đơn hàng đã hoàn thành - Status == 2)
                        TotalSold = _context.OrderDetails.Where(od => od.ProductId == p.Id && od.Order.Status == 2).Sum(od => (int?)od.Quantity) ?? 0
                    })
                    .OrderByDescending(p => p.TotalSold) // Ưu tiên số lượng bán nhiều nhất xếp lên đầu
                    .ThenByDescending(p => p.Id) // Nếu lượt bán bằng nhau (ví dụ đều bằng 0), ưu tiên sản phẩm mới hơn
                    .Take(5) // Lấy ra top 5 sản phẩm
                    .ToList();

                return Ok(products);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải top sản phẩm bán chạy", error = ex.Message });
            }
        }

        // ==========================================
        // 6. API LẤY CHI TIẾT SẢN PHẨM THEO ID
        // ==========================================
        // GET: api/product/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetDetail(int id)
        {
            try
            {
                var product = _context.Products
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Slug,
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

        // ==========================================
        // 7. API LẤY CHI TIẾT SẢN PHẨM THEO SLUG (SEO FRIENDLY)
        // ==========================================
        // GET: api/product/slug/{slug}
        [HttpGet("slug/{slug}")]
        public IActionResult GetDetailBySlug(string slug)
        {
            try
            {
                var product = _context.Products
                    .Select(p => new {
                        p.Id,
                        p.Name,
                        p.Slug,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.ImageUrl,
                        p.CategoryProductId,
                        CategoryName = p.CategoryProduct != null ? p.CategoryProduct.Name : "Chưa phân loại"
                    })
                    .FirstOrDefault(p => p.Slug == slug);

                // Xử lý kịch bản không tồn tại sản phẩm
                if (product == null)
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm này trong kho hàng dựa trên slug" });
                }

                return Ok(product);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống khi tải chi tiết sản phẩm theo slug", error = ex.Message });
            }
        }
    }
}