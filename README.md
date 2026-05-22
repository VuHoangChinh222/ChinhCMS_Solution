# 🚀 HỆ THỐNG QUẢN TRỊ NỘI DUNG VÀ BÁN HÀNG - CHINHCMS

Hệ thống quản trị nội dung (CMS) và quản lý kinh doanh chuyên nghiệp, được xây dựng trên nền tảng **ASP.NET Core 8.0 MVC** kết hợp **Entity Framework Core**. Giải pháp được thiết kế theo kiến trúc phân lớp chuẩn mực, áp dụng các cơ chế bảo mật tối tân (mã hóa băm Bcrypt), tối ưu hóa truy vấn dữ liệu (LINQ nâng cao), và sở hữu giao diện quản trị Admin Panel tương thích toàn diện (Responsive) tích hợp trình biên tập văn bản giàu tính năng **CKEditor 5**.

---

## 👨‍💻 THÔNG TIN TÁC GIẢ

*   **Sinh viên thực hiện:** Vũ Hoàng Chính
*   **Mã số sinh viên:** 2122110380
*   **Lớp học:** CCQ2211J
*   **Môn học:** Chuyên đề ASP.NET
*   **Giáo viên hướng dẫn:** Nguyễn Cao Thái
*   **Phiên bản dự án:** 1.0

---

## 🏗️ KIẾN TRÚC DỰ ÁN (SOLUTION ARCHITECTURE)

Giải pháp `ChinhCMS_Solution` được tổ chức thành 3 phân lớp chuyên biệt nhằm đảm bảo tính tái sử dụng, dễ bảo trì và nâng cấp:

### 📂 Cấu trúc chi tiết các lớp:
1.  **`CMS.Data` (Lớp Dữ liệu - Class Library)**:
    *   Quản lý các thực thể ánh xạ xuống Database: `User`, `Category`, `Post`, `Customer`, `Product`, `ProductCategory`, `Order`, `OrderDetail`.
    *   Lớp ngữ cảnh dữ liệu `ApplicationDbContext` thiết lập quan hệ ràng buộc và liên kết khóa ngoại chặt chẽ.
2.  **`CMS.Backend` (Lớp Quản trị - ASP.NET Core 8.0 MVC)**:
    *   Trái tim vận hành của hệ thống quản trị (Admin Panel).
    *   Quản lý luồng xử lý (Controllers), định dạng hiển thị (Views) và lưu trữ tĩnh (wwwroot).
3.  **`cms.frontend` (Lớp Giao diện Người dùng - ReactJS)**:
    *   Ứng dụng độc lập phục vụ hiển thị tin tức và tương tác cho độc giả bên ngoài, kết nối và lấy nguyên liệu thô (JSON) từ các cổng API của Backend.

---

## ✨ CÁC TÍNH NĂNG NỔI BẬT ĐÃ HOÀN THÀNH

### 📺 1. Giao Diện Quản Trị Toàn Diện & Tương Thích (`_LayoutAdmin.cshtml`)
*   **Sidebar Cố định Thông minh:** Tự động so khớp tuyến đường (Route) để tô màu nổi bật (`active`) liên kết đang duyệt, giúp định hướng thao tác trực quan.
*   **Liên kết Controller thực tế:** Tích hợp đầy đủ các danh mục quản trị thực tế bao gồm Quản lý danh mục, Bài viết, Thành viên, Danh mục sản phẩm, Sản phẩm, Khách hàng và Đơn hàng.
*   **Đáp ứng (Responsive) 100%:** Thiết kế tương thích hoàn hảo trên mọi kích thước màn hình từ Desktop, Tablet đến Smartphone:
    *   *Trên màn hình Desktop:* Sidebar hiển thị cố định 240px chuyên nghiệp ở bên trái.
    *   *Trên màn hình di động:* Sidebar tự động thu gọn thành nút Menu 3 dấu gạch ngang (Hamburger Menu) ở thanh tiêu đề phía trên. Khi nhấn nút, Sidebar sẽ trượt ra mượt mà dưới dạng ngăn kéo nền tối (**Offcanvas Menu**) sang trọng, giải quyết triệt để lỗi mất thanh bar hoặc nền trắng/màn hình xám trên di động.

### 📝 2. Quản Lý Bài Viết Tin Tức Nâng Cao (Post CRUD)
*   **Cấu trúc 2 cột chuẩn khoa học:** Giao diện Thêm mới (`Create.cshtml`) và Chỉnh sửa (`Edit.cshtml`) được thiết kế đồng bộ theo tỷ lệ vàng `col-md-8` (Tiêu đề, Ô soạn thảo nội dung bên trái) và `col-md-4` (Chuyên mục, Ảnh đại diện, Ngày đăng, Nút hành động bên phải) chuẩn xác theo tài liệu học tập của thầy Nguyễn Cao Thái.
*   **Tích hợp Trình soạn thảo CKEditor 5:** Biến ô nhập văn bản thô truyền thống thành trình soạn thảo văn bản phong phú (Rich Text Editor) chuyên nghiệp qua mạng CDN, hỗ trợ định dạng in đậm, in nghiêng, căn lề, danh sách tự động...
*   **Cơ chế tải và quản lý ảnh vật lý thông minh:**
    *   **Tải ảnh đĩa cứng:** Cho phép người dùng vừa dán link URL ảnh, vừa chọn tải ảnh trực tiếp từ máy tính lên. Ảnh tải lên được lưu vật lý vào thư mục cục bộ `CMS.Backend/wwwroot/uploads/`.
    *   **Sinh tên file duy nhất:** Tự động tạo tên file ảnh bằng chuỗi ngẫu nhiên `Guid.NewGuid()` kết hợp đuôi mở rộng gốc để chống ghi đè ảnh trùng tên khi upload cùng lúc.
    *   **Chống lỗi bắt buộc nhập URL:** Xử lý và lưu ảnh vật lý lên đĩa cứng trước khi kiểm tra trạng thái ModelState, giải quyết triệt để lỗi không lưu được dữ liệu khi người dùng chọn tải ảnh từ máy tính thay vì nhập link URL.
    *   **Giữ ảnh cũ khi sửa:** Trong trang Chỉnh sửa, hệ thống có khung xem trước hình ảnh cũ. Nếu người dùng không tải ảnh mới lên, hệ thống tự động sử dụng `.AsNoTracking()` truy vấn để giữ nguyên hình ảnh cũ mà không ghi đè giá trị rỗng.
*   **Tự động dọn dẹp ảnh mồ côi khi xóa bài viết:**
    *   Khi bạn thực thi xóa một bài viết ra khỏi cơ sở dữ liệu, bộ điều khiển sẽ tự động kiểm tra xem ảnh của bài viết đó có phải là ảnh cục bộ nằm trong thư mục `/uploads/` hay không. 
    *   Nếu đúng, hệ thống sẽ sử dụng lệnh `System.IO.File.Delete` xóa sạch tệp tin ảnh đó khỏi ổ cứng máy chủ trước khi xóa bản ghi khỏi SQL Server, tránh lãng phí dung lượng bộ nhớ máy chủ.
*   **Trình diễn hiển thị bài viết hoàn mỹ:**
    *   *Trang danh sách (Post/Index) và Trang chủ (Home/Index):* Sử dụng biểu thức chính quy (Regex) `@System.Text.RegularExpressions.Regex.Replace(..., "<.*?>", string.Empty)` để lọc sạch hoàn toàn các thẻ HTML được tạo bởi CKEditor, giúp phần văn bản tóm tắt bài viết trên các card Bootstrap luôn sạch sẽ, thẳng hàng và không làm vỡ bố cục.
    *   *Trang chi tiết (Post/Details):* Áp dụng cú pháp `@Html.Raw(Model.Content)` giúp trình duyệt biên dịch và hiển thị bài viết tin tức đầy đủ định dạng bắt mắt nhất.


### 📦 3. Quản Lý Danh Mục Tin Tức Khóa An Toàn
*   Áp dụng liên kết bảng sâu `.Include(c => c.Posts)`. Nếu danh mục tin tức đang chứa bài viết liên quan, hệ thống sẽ hiển thị bảng thống kê các bài viết và vô hiệu hóa nút xóa danh mục để bảo vệ toàn vẹn dữ liệu hệ thống.

---

## 🛠️ HƯỚNG DẪN CẤU HÌNH VÀ VẬN HÀNH DỰ ÁN

### 1. Yêu cầu hệ thống:
*   Visual Studio 2022 (phiên bản 17.8 trở lên).
*   .NET 8.0 SDK.
*   SQL Server (phiên bản 2016 trở lên, khuyến nghị dùng LocalDB).

### 2. Cài đặt các thư viện lõi (NuGet Packages):
Mở cửa sổ `Package Manager Console` trong Visual Studio và chạy các lệnh sau:
```powershell
Install-Package BCrypt.Net-Next
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
```

### 3. Cấu hình Chuỗi kết nối Cơ sở dữ liệu:
Mở tệp tin `CMS.Backend/appsettings.json` và cấu hình kết nối SQL Server của bạn. Theo mặc định hệ thống đang kết nối qua LocalDB cực kỳ tiện lợi:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ChinhCMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### 4. Các bước khởi chạy dự án:
1.  Khởi chạy Visual Studio 2022 và mở tệp giải pháp `ChinhCMS_Solution.sln`.
2.  Nhấp chuột phải vào dự án **`CMS.Backend`** ở cây thư mục bên phải và chọn **Set as Startup Project**.
3.  Đảm bảo cơ sở dữ liệu `ChinhCMS_DB` của bạn đã được thiết lập đúng cấu trúc bảng.
4.  Nhấn nút **Play (IIS Express / CMS.Backend)** hoặc nhấn phím `F5` để biên dịch dự án. Giao diện trang quản trị sẽ tự động bật lên trên trình duyệt web của bạn.

---

## 📈 TIẾN TRÌNH THỰC HIỆN DỰ ÁN (MILESTONES)

| Buổi Học | Nội Dung Công Việc | Trạng Thái | Chi Tiết |
| :--- | :--- | :---: | :--- |
| **Buổi 1** | Khởi tạo cấu trúc giải pháp 3 phân lớp, thiết lập cơ sở dữ liệu `ChinhCMS_DB`. | **Đã hoàn thành** | Tạo các thực thể và cấu hình liên kết DbContext. |
| **Buổi 2** | Triển khai mã hóa mật khẩu băm Bcrypt, quản lý đơn hàng & chi tiết đơn hàng trực quan. | **Đã hoàn thành** | Xây dựng class PasswordHelper, hiển thị danh sách hóa đơn theo trạng thái. |
| **Buổi 3** | Xây dựng tính năng CRUD Danh mục an toàn, lọc bài viết tiêu điểm trên Trang chủ. | **Đã hoàn thành** | Khóa xóa danh mục chứa bài viết, truy vấn LINQ lấy 3 bài viết mới nhất. |
| **Buổi 4** | Thiết kế Layout quản trị Admin Panel tương thích, tích hợp upload ảnh và CKEditor 5. | **Đang triển khai** | Hoàn thành Layout Admin, Menu Offcanvas di động, CKEditor 5, Tự động dọn dẹp ảnh mồ côi. |

---

*Hệ thống được thiết kế và phát triển bởi sinh viên Vũ Hoàng Chính - CCQ2211J.*
