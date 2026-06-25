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

using Microsoft.EntityFrameworkCore;
using CMS.Data.Entities;

namespace CMS.Data
{
    /// <summary>
    /// Lớp ngữ cảnh cơ sở dữ liệu (DbContext) cốt lõi của toàn bộ giải pháp ChinhCMS.
    /// Kế thừa từ class DbContext của Entity Framework Core để quản lý các phiên kết nối, 
    /// truy vấn dữ liệu từ CSDL SQL Server và đồng bộ các thay đổi xuống CSDL.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor của DbContext. Nhận các cấu hình DbContextOptions (bao gồm chuỗi kết nối Database, 
        /// nhà cung cấp dịch vụ SQL Server) được cấu hình và tiêm vào qua cơ chế Dependency Injection tại Program.cs.
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // =========================================================================
        // KHAI BÁO CÁC BẢNG DỮ LIỆU (DbSet) ÁNH XẠ SANG CÁC BẢNG TRONG CƠ SỞ DỮ LIỆU
        // =========================================================================

        /// <summary>
        /// Bảng Categories: Lưu trữ các danh mục bài viết tin tức (Ví dụ: Tin thể thao, Xu hướng giày...).
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Bảng Posts: Lưu trữ nội dung chi tiết bài viết (Hỗ trợ định dạng HTML được CKEditor 5 sinh ra).
        /// </summary>
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// Bảng Users: Quản lý danh sách nhân viên quản trị hệ thống (Phân vai trò: Admin, Editor).
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Bảng CategoriesProducts: Quản lý danh mục sản phẩm (Ví dụ: Giày bóng rổ, Trang phục bóng rổ...).
        /// </summary>
        public DbSet<CategoryProduct> CategoriesProducts { get; set; }

        /// <summary>
        /// Bảng Products: Quản lý danh sách các sản phẩm quần áo, giày dép, phụ kiện bóng rổ trong shop.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Bảng Customers: Lưu thông tin tài khoản của khách hàng mua hàng (được dùng để đăng nhập phía ReactJS).
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// Bảng Orders: Lưu thông tin đơn đặt hàng chung (Họ tên, Ngày đặt, Trạng thái chờ duyệt/đang giao/đã xong).
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Bảng OrderDetails: Lưu chi tiết các sản phẩm được mua trong từng đơn hàng (Giá mua thực tế, số lượng).
        /// </summary>
        public DbSet<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// Bảng Banners: Lưu danh sách ảnh bìa quảng cáo trượt (Hero Slideshow) hiển thị trên trang chủ FrontEnd.
        /// </summary>
        public DbSet<Banner> Banners { get; set; }

        /// <summary>
        /// Phương thức cấu hình nâng cao các quy tắc ràng buộc (Fluent API) của Cơ sở dữ liệu.
        /// Được EF Core gọi tự động khi khởi tạo cấu trúc bảng (Migration).
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Gọi phương thức OnModelCreating gốc của DbContext lớp cha để đảm bảo cấu hình mặc định được thiết lập
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình chỉ mục duy nhất (Unique Index) cho cột Slug trong bảng Products.
            // Đảm bảo không tồn tại hai sản phẩm trùng nhau về đường dẫn thân thiện (Slug) giúp tối ưu SEO tốt hơn.
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            // 2. Cấu hình chỉ mục duy nhất (Unique Index) cho cột Slug trong bảng Posts.
            // Ngăn chặn trùng lặp đường dẫn thân thiện cho bài viết tin tức, tránh lỗi xung đột URL khi tải tin chi tiết.
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Slug)
                .IsUnique();
        }
    }
}

