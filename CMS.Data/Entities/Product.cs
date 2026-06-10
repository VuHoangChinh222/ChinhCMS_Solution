/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Data.Entities
{
    // Lớp Product đại diện cho một sản phẩm, chứa thông tin chi tiết về sản phẩm như tên,
    // mô tả, giá, số lượng tồn kho, hình ảnh và liên kết tới danh mục sản phẩm.
    public class Product
    {
        [Key]
        public int Id { get; set; } // Khóa chính và tự động tăng

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string Name { get; set; } // Tên sản phẩm

        // Slug: Đường dẫn thân thiện phục vụ cho SEO URL.
        // Kiểu dữ liệu string? (nullable) được sử dụng để tránh lỗi tự động kiểm tra bắt buộc (Required) 
        // của ASP.NET Core Model Validation phía Client/Server khi người dùng để trống để hệ thống tự sinh.
        // Tuy nhiên dưới Database, cột này vẫn được cấu hình NOT NULL và có chỉ mục Unique Index.
        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự")]
        public string? Slug { get; set; } = string.Empty;

        public string? Description { get; set; } // Mô tả chi tiết về sản phẩm

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Giá sản phẩm

        public int StockQuantity { get; set; } // Số lượng tồn kho của sản phẩm

        public string? ImageUrl { get; set; } // URL hình ảnh đại diện cho sản phẩm

        // Khóa ngoại nối tới CategoryProduct
        public int CategoryProductId { get; set; } // Khóa ngoại liên kết tới CategoryProduct

        [ForeignKey("CategoryProductId")]
        public virtual CategoryProduct? CategoryProduct { get; set; } 
    }
}
