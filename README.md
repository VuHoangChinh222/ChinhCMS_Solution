# TỔNG QUAN DỰ ÁN CHINHCMS

Dự án này là hệ thống quản trị nội dung (CMS) và quản lý đơn hàng được xây dựng trên nền tảng công nghệ ASP.NET Core MVC kết hợp Entity Framework Core. Hệ thống được thiết kế theo mô hình phân lớp rõ ràng, áp dụng các chuẩn bảo mật và tối ưu hóa truy vấn dữ liệu nhằm phục vụ cho mục đích quản lý tin tức, bài viết, khách hàng và đơn hàng.

---

## 1. THÔNG TIN TÁC GIẢ

- **Sinh viên thực hiện**: Vũ Hoàng Chính
- **Mã số sinh viên**: 2122110380
- **Lớp**: CCQ2211J
- **Phiên bản dự án**: 1.0

---

## 2. CẤU TRÚC GIẢI PHÁP (SOLUTION ARCHITECTURE)

Giải pháp `ChinhCMS_Solution` được phân chia thành ba dự án thành phần với các vai trò chuyên biệt:

1. **`CMS.Data` (Lớp truy cập dữ liệu - Class Library)**:
   - Quản lý toàn bộ cấu trúc cơ sở dữ liệu và các thực thể (Entities) trong hệ thống.
   - Khai báo lớp `ApplicationDbContext` kế thừa từ `DbContext` để thiết lập ánh xạ cơ sở dữ liệu.
   - Các bảng dữ liệu chính bao gồm:
     - `User`: Nhân viên quản trị hệ thống.
     - `Category`: Danh mục bài viết tin tức.
     - `Post`: Bài viết tin tức (liên kết nhiều-một với danh mục Category).
     - `Customer`: Khách hàng mua sắm.
     - `Order`: Đơn hàng tổng quát.
     - `OrderDetail`: Chi tiết từng dòng sản phẩm trong đơn hàng (liên kết với sản phẩm Product và đơn hàng Order).
     - `Product`: Sản phẩm trong kho hàng.

2. **`CMS.Backend` (Trang quản trị hệ thống - ASP.NET Core MVC)**:
   - Cung cấp giao diện Web dành cho nhà quản lý sử dụng thư viện Bootstrap để thiết kế giao diện đồng bộ, hiện đại.
   - Chứa các Controller xử lý luồng công việc logic và điều hướng dữ liệu.
   - Chứa các View kết xuất mã HTML hiển thị dữ liệu thực tế từ SQL Server.

3. **`cms.frontend` (Giao diện người dùng - ReactJS)**:
   - Ứng dụng độc lập phục vụ hiển thị tin tức và tương tác cho độc giả bên ngoài, kết nối và lấy nguyên liệu thô (JSON) từ các cổng API của Backend.

---

## 3. CÁC TÍNH NĂNG CHÍNH ĐÃ TRIỂN KHAI

### Chức năng Quản lý Đơn hàng và Chi tiết Đơn hàng
- **Đơn hàng (`Orders`)**: Hiển thị bảng tổng hợp toàn bộ danh sách đơn hàng bao gồm mã đơn, ngày đặt, ghi chú, trạng thái đơn hàng (sử dụng các nhãn trạng thái trực quan của Bootstrap như Chờ duyệt, Đang giao, Đã xong).
- **Chi tiết đơn hàng (`OrderDetails`)**: Tích hợp luồng điều hướng mượt mà. Khi người dùng bấm nút **"Chi tiết"** ở một đơn hàng cụ thể, hệ thống sẽ gọi phương thức `Details` lọc các dòng dữ liệu trong bảng `OrderDetails` theo đúng mã đơn hàng (`OrderId`), tính toán thành tiền của từng dòng sản phẩm (Số lượng x Đơn giá) và hiển thị lên giao diện chi tiết kèm theo nút quay lại danh sách nhanh chóng.

### Chức năng Quản lý Danh mục (CRUD) nâng cấp an toàn
- **Thêm mới và Chỉnh sửa**: Xây dựng các biểu mẫu nhập liệu trực quan. Khắc phục lỗi cơ chế kiểm tra dữ liệu nghiêm ngặt trong .NET 8 (khi kích hoạt thuộc tính kiểm tra Nullable) bằng cách chủ động kiểm tra logic bỏ trống của các trường bắt buộc, tránh việc các trường liên kết ảo chặn đứng quá trình lưu dữ liệu.
- **Xóa Danh mục nâng cấp**: Khi truy cập trang xác nhận xóa danh mục, hệ thống sử dụng truy vấn nạp chồng `.Include(c => c.Posts)` để lấy toàn bộ danh sách bài viết thuộc danh mục đó:
  - Nếu danh mục đang chứa bài viết: Hệ thống sẽ hiển thị bảng kê chi tiết toàn bộ bài viết liên quan, đưa ra cảnh báo không được phép xóa để bảo vệ toàn vẹn dữ liệu trong cơ sở dữ liệu và tự động khóa (vô hiệu hóa) nút bấm xóa vĩnh viễn.
  - Nếu danh mục trống: Hệ thống hiển thị thông báo an toàn và cho phép bấm nút xác nhận xóa bình thường.

### Chức năng Truy vấn Bài viết trên Trang chủ
- Tiêm kết nối dữ liệu vào `HomeController`.
- Sử dụng các câu lệnh truy vấn LINQ kết hợp tối ưu: `.Include(p => p.Category)` nạp kèm tên danh mục, `.OrderByDescending(p => p.CreatedDate)` sắp xếp bài viết mới nhất lên trước và `.Take(3)` để hiển thị đúng 3 bài viết tiêu điểm trên giao diện trang chủ dưới dạng lưới thẻ Bootstrap cân đối.

---

## 4. HƯỚNG DẪN CẤU HÌNH VÀ CHẠY DỰ ÁN

### Yêu cầu về môi trường phát triển
- Microsoft Visual Studio 2022.
- .NET 8.0 SDK.
- SQL Server (hoặc SQL Server Express / LocalDB).

### Các gói thư viện cần cài đặt (NuGet Packages)
Để hệ thống biên dịch không bị lỗi, đảm bảo các gói sau đã được cài đặt đầy đủ:
```shell
Install-Package BCrypt.Net-Next
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
```

### Cấu hình kết nối Cơ sở dữ liệu
Chuỗi kết nối cơ sở dữ liệu nằm trong file `appsettings.json` của thư mục dự án `CMS.Backend`. Theo mặc định, dự án đang kết nối tới SQL Server LocalDB:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ChinhCMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```
*Lưu ý: Nếu bạn sử dụng SQL Server phiên bản khác, hãy chỉnh sửa chuỗi kết nối này cho phù hợp với máy của bạn trước khi chạy.*

### Các bước khởi động dự án
1. Mở tệp tin giải pháp `ChinhCMS_Solution.sln` bằng Visual Studio 2022.
2. Nhấp chuột phải vào dự án `CMS.Backend` và chọn **Set as Startup Project** (Đặt làm dự án khởi động).
3. Đảm bảo cơ sở dữ liệu `ChinhCMS_DB` đã được tạo và chứa đầy đủ dữ liệu thử nghiệm trong SQL Server.
4. Nhấn phím `F5` hoặc bấm nút **Play** trên thanh công cụ Visual Studio để khởi chạy dự án trên trình duyệt. Giao diện trang chủ quản trị sẽ xuất hiện tại địa chỉ cổng Localhost do hệ thống tự cấp phát.
