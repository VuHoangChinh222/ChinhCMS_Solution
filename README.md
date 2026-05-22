# HỆ THỐNG QUẢN LÝ NỘI DUNG VÀ BÁN HÀNG - CHINHCMS

Hệ thống quản lý nội dung (CMS) và bán hàng được xây dựng trên nền tảng **ASP.NET Core 8.0 MVC** và **Entity Framework Core**. Dự án được thiết kế theo cấu trúc 3 lớp, giúp dễ quản lý, nâng cấp và tích hợp trình soạn thảo văn bản **CKEditor 5** cho việc viết bài.

---

## THÔNG TIN SINH VIÊN

*   **Sinh viên thực hiện:** Vũ Hoàng Chính
*   **Mã số sinh viên:** 2122110380
*   **Lớp học:** CCQ2211J
*   **Môn học:** Chuyên đề ASP.NET
*   **Giáo viên hướng dẫn:** Nguyễn Cao Thái
*   **Phiên bản dự án:** 1.0

---

## CẤU TRÚC DỰ ÁN (SOLUTION)

Dự án `ChinhCMS_Solution` được chia làm 3 dự án nhỏ bên trong:

1.  **`CMS.Data` (Lớp dữ liệu)**:
    *   Chứa các bảng dữ liệu: `User` (Thành viên), `Category` (Danh mục bài viết), `Post` (Bài viết), `Customer` (Khách hàng), `Product` (Sản phẩm), `CategoryProduct` (Danh mục sản phẩm), `Order` (Đơn hàng), `OrderDetail` (Chi tiết đơn hàng).
    *   Sử dụng `ApplicationDbContext` để kết nối và cấu hình quan hệ giữa các bảng.
2.  **`CMS.Backend` (Trang quản trị)**:
    *   Xây dựng bằng ASP.NET Core 8.0 MVC để làm trang quản trị (Admin Panel) cho người điều hành.
    *   Gồm các bộ điều khiển (Controllers), giao diện (Views) và tài nguyên tĩnh (hình ảnh, css nằm trong wwwroot).
3.  **`cms.frontend` (Giao diện người dùng)**:
    *   Trang hiển thị tin tức cho người xem được viết bằng ReactJS, lấy dữ liệu thông qua API từ Backend.

---

## CÁC TÍNH NĂNG ĐÃ HOÀN THÀNH

### 1. Giao diện quản trị (Admin Layout)
*   **Thanh điều hướng bên cạnh (Sidebar):** Hiển thị danh sách các mục quản lý như Danh mục, Bài viết, Thành viên, Danh mục sản phẩm, Sản phẩm, Khách hàng và Đơn hàng.
*   **Tự động nhận diện trang:** Sidebar sẽ tự động tô đậm mục đang được chọn để người dùng dễ nhận biết.
*   **Hỗ trợ giao diện điện thoại (Responsive):** 
    *   Trên máy tính: Sidebar hiển thị cố định ở bên trái.
    *   Trên điện thoại: Sidebar tự động thu gọn lại, người dùng có thể nhấn vào nút Menu ở góc trên để mở danh sách chức năng dưới dạng trượt (Offcanvas).

### 2. Quản lý thành viên (User CRUD)
*   **Danh sách thành viên:** Hiển thị rõ danh sách các tài khoản trong hệ thống kèm nhãn phân quyền nổi bật (Quản trị viên / Biên tập viên).
*   **Thêm mới thành viên:** Form nhập đầy đủ thông tin với các ô nhập tên đăng nhập, họ tên, vai trò và ô ẩn mật khẩu.
*   **Chỉnh sửa thông tin linh hoạt:** 
    *   Không cho sửa tên đăng nhập để giữ an toàn hệ thống.
    *   Hỗ trợ ô nhập mật khẩu mới tùy chọn: Nếu muốn đổi mật khẩu thì nhập vào ô mật khẩu mới, nếu để trống thì hệ thống tự động giữ nguyên mật khẩu cũ trong database.
*   **Thông báo lỗi thân thiện:** Đã Việt hóa toàn bộ các thông báo lỗi xác thực của hệ thống (ví dụ: hiển thị "Vui lòng nhập mật khẩu" thay vì các câu thông báo mặc định bằng tiếng Anh).
*   **Xóa tài khoản:** Tích hợp hộp thoại hỏi ý kiến xác nhận trước khi xóa, tránh trường hợp người dùng ấn nhầm nút xóa.

### 3. Quản lý bài viết tin tức (Post CRUD)
*   **Giao diện nhập liệu tiện lợi:** Chia làm 2 phần gồm phần soạn thảo nội dung (bên trái) và phần thiết lập như danh mục, ảnh đại diện, ngày đăng (bên phải).
*   **Trình soạn thảo CKEditor 5:** Tích hợp trực tiếp giúp viết bài có thể định dạng chữ đậm, chữ nghiêng, căn lề, danh sách đầu dòng.
*   **Tải ảnh trực tiếp lên máy chủ:**
    *   Hỗ trợ tải file ảnh từ máy tính lên thư mục `wwwroot/uploads/` trên máy chủ.
    *   Tự động đổi tên file ảnh bằng chuỗi ngẫu nhiên `Guid` để tránh việc file mới tải lên đè lên file cũ trùng tên.
    *   Giữ lại ảnh cũ khi chỉnh sửa nếu người dùng không chọn ảnh mới.
*   **Tự động xóa file ảnh khi xóa bài viết:** Khi xóa bài viết khỏi cơ sở dữ liệu, file ảnh lưu trong thư mục `uploads` cũng tự động được xóa đi để tiết kiệm dung lượng ổ cứng.
*   **Lọc thẻ HTML khi xem tóm tắt:** Sử dụng Regex để loại bỏ các thẻ HTML khi hiển thị tóm tắt bài viết trên trang danh sách, giúp giao diện gọn gàng và không bị vỡ khung. Hiển thị đầy đủ định dạng HTML trong trang chi tiết bài viết.

### 4. Quản lý danh mục bài viết
*   **Xóa danh mục an toàn:** Khi xóa danh mục tin tức, hệ thống sẽ kiểm tra xem danh mục đó có chứa bài viết nào không. Nếu có bài viết, nút xóa sẽ bị khóa để tránh làm lỗi dữ liệu.

---

## HƯỚNG DẪN CÀI ĐẶT VÀ KHỞI CHẠY

### 1. Chuẩn bị:
*   Visual Studio 2022.
*   .NET 8.0 SDK.
*   SQL Server (khuyến nghị dùng LocalDB).

### 2. Cài đặt các thư viện (NuGet Packages):
Mở `Package Manager Console` trong Visual Studio và cài đặt các thư viện:
```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
```

### 3. Cấu hình kết nối cơ sở dữ liệu:
Mở file `CMS.Backend/appsettings.json` và điều chỉnh chuỗi kết nối đến SQL Server của bạn. Mặc định hệ thống sử dụng LocalDB:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ChinhCMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### 4. Khởi chạy:
1.  Mở file giải pháp `ChinhCMS_Solution.sln` bằng Visual Studio 2022.
2.  Nhấp chuột phải vào dự án **`CMS.Backend`** và chọn **Set as Startup Project**.
3.  Đảm bảo cơ sở dữ liệu `ChinhCMS_DB` đã được tạo và có sẵn dữ liệu.
4.  Nhấn nút **Play** (hoặc phím `F5`) trên Visual Studio để chạy chương trình.

---

## TIẾN ĐỘ THỰC HIỆN DỰ ÁN

| Buổi Học | Nội Dung Thực Hiện | Trạng Thái | Chi Tiết |
| :--- | :--- | :---: | :--- |
| **Buổi 1** | Khởi tạo cấu trúc dự án 3 lớp, thiết lập cơ sở dữ liệu `ChinhCMS_DB`. | **Đã hoàn thành** | Tạo các thực thể và cấu hình kết nối database. |
| **Buổi 2** | Quản lý đơn hàng và chi tiết đơn hàng trực quan. | **Đã hoàn thành** | Thiết kế bảng hiển thị danh sách hóa đơn theo trạng thái. |
| **Buổi 3** | Xây dựng chức năng CRUD Danh mục an toàn, lọc bài viết mới nhất lên Trang chủ. | **Đã hoàn thành** | Khóa xóa danh mục chứa bài viết, dùng LINQ lấy 3 bài viết mới nhất. |
| **Buổi 4** | Thiết kế giao diện quản trị Admin Panel, tích hợp tải ảnh và trình soạn thảo CKEditor 5. | **Đã hoàn thành** | Hoàn thiện các trang quản lý: Danh mục, Bài viết, Đơn hàng, Thành viên (User CRUD). |

---

*Dự án được thực hiện bởi sinh viên Vũ Hoàng Chính - CCQ2211J.*
