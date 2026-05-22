# HỆ THỐNG QUẢN LÝ NỘI DUNG VÀ BÁN HÀNG - CHINHCMS

Hệ thống quản lý nội dung (CMS) và bán hàng được xây dựng trên nền tảng **ASP.NET Core 8.0 MVC** và **Entity Framework Core**. Dự án được thiết kế theo cấu trúc 3 lớp, giúp dễ quản lý, nâng cấp và tích hợp trình soạn thảo văn bản **CKEditor 5** cùng với các tiêu chuẩn thiết kế hiện đại, responsive.

---

## THÔNG TIN SINH VIÊN

*   **Sinh viên thực hiện:** Vũ Hoàng Chính
*   **Mã số sinh viên:** 2122110380
*   **Lớp học:** CCQ2211J
*   **Môn học:** Chuyên đề ASP.NET
*   **Giáo viên hướng dẫn:** Nguyễn Cao Thái
*   **Phiên bản dự án:** 1.2 (Cập nhật Buổi 5)

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

### 1. Giao diện quản trị (Admin Layout) & Nhúng Thông tin tài khoản
*   **Thanh điều hướng bên cạnh (Sidebar):** Hiển thị danh sách các mục quản lý như Danh mục bài viết, Bài viết, Thành viên, Danh mục sản phẩm, Sản phẩm, Khách hàng và Đơn hàng.
*   **Hỗ trợ hiển thị trên điện thoại (Responsive):** 
    *   Trên máy tính: Sidebar cố định ở bên trái.
    *   Trên điện thoại: Sidebar tự động thu gọn dạng trượt (Offcanvas).
*   **Thông tin tài khoản tích hợp Sidebar:** Tên người dùng, vai trò (Admin / Editor) cùng nút "Đăng xuất" được nhúng trực tiếp lên đầu Sidebar (ở cả phiên bản máy tính và điện thoại), tối ưu hóa trải nghiệm tương tự trên thiết bị di động.

### 2. Xác thực & Phân quyền (Cookie Authentication & Authorization) - [MỚI]
*   **Dịch vụ xác thực Cookie**: Cấu hình trong `Program.cs` sử dụng `CookieAuthentication` chuẩn, tự động điều hướng về `/Account/Login` khi chưa đăng nhập và `/Account/AccessDenied` khi truy cập sai quyền.
*   **Mã hóa mật khẩu an toàn**: Kiểm tra mật khẩu mã hóa hash nâng cao bằng công nghệ BCrypt (`PasswordHelper.VerifyPassword`), chống rò rỉ thông tin tuyệt đối.
*   **Duy trì phiên đăng nhập bền vững (Data Protection)**:
    *   Cấu hình lưu trữ bộ khóa mã hóa (Data Protection Keys) cố định vào thư mục dự án `App_Data/Keys`.
    *   *Hiệu quả*: Cookie không bị mất hoặc bắt đăng nhập lại mỗi khi bạn biên dịch (rebuild) lại dự án hoặc khởi động lại server.
    *   Thiết lập thuộc tính Cookie bền vững `IsPersistent = true` lưu trữ trên ổ đĩa trình duyệt trong vòng 7 ngày.
*   **Bảo vệ Controller nghiêm ngặt**: Khóa toàn bộ các controller quản trị bằng thuộc tính `[Authorize]`. Cấu hình quyền hạn phân cấp vai trò:
    *   `Admin`: Toàn quyền thao tác trên toàn bộ hệ thống (quản lý Thành viên, Danh mục sản phẩm, Khách hàng,...).
    *   `Editor`: Bị hạn chế truy cập vào các chức năng nhạy cảm của Admin, tự động chuyển hướng sang trang báo lỗi 403 cao cấp khi cố ý truy cập.
*   **Trang Access Denied 403 cực đẹp**: Thiết kế giao diện báo lỗi từ chối truy cập bằng hiệu ứng chuyển động chiếc khiên đỏ nhấp nháy (`animate-pulse`), nội dung lịch sự và đầy đủ nút quay lại an toàn.

### 3. Quản lý danh mục sản phẩm (CategoryProduct CRUD) - [MỚI]
*   **Bảng danh sách chuẩn UI/UX**: Hiển thị số lượng sản phẩm liên kết thực tế của từng danh mục một cách chính xác nhất.
*   **Khóa nút Xóa thông minh**:
    *   Nếu số lượng sản phẩm thuộc danh mục đang lớn hơn 0 (`Products.Count > 0`), hệ thống sẽ **ẩn hoàn toàn nút Xóa** ở trang danh sách để ngăn chặn hành động sơ suất của người dùng.
    *   Tích hợp bộ bảo vệ 2 lớp ở server-side trong `DeleteConfirmed` để trả về thông báo lỗi dạng Toast/Alert và ngăn chặn hành vi cố tình gửi yêu cầu xóa danh mục không rỗng.

### 4. Quản lý sản phẩm kho hàng (Product CRUD) - [MỚI]
*   **Thông số kho chi tiết**: Hiển thị ảnh đại diện sản phẩm nhỏ gọn, tên sản phẩm, danh mục cha, giá bán định dạng tiền tệ VNĐ và số lượng tồn kho.
*   **Nhãn trạng thái tồn kho thông minh**:
    *   Số lượng `> 10`: Huy hiệu xanh lá (Đủ hàng).
    *   Số lượng từ `1` đến `10`: Huy hiệu màu cam (Cảnh báo ít hàng).
    *   Số lượng `= 0`: Huy hiệu màu đỏ (Hết hàng).
*   **Dọn rác hình ảnh cũ khi cập nhật/xóa**:
    *   Khi **Sửa sản phẩm** và tải lên một ảnh mới thay thế, hệ thống tự động tìm và xóa vĩnh viễn tệp ảnh cũ khỏi thư mục vật lý `wwwroot/uploads` trên máy chủ.
    *   Khi **Xóa sản phẩm**, tệp tin ảnh đại diện của sản phẩm đó cũng được dọn sạch khỏi ổ đĩa để đảm bảo dung lượng lưu trữ của server luôn tối ưu nhất.

### 5. Quản lý bài viết tin tức (Post CRUD)
*   **Trình soạn thảo CKEditor 5:** Tích hợp trực tiếp giúp viết bài có thể định dạng chữ, chèn bảng dễ dàng.
*   **Tải ảnh trực tiếp lên máy chủ:** Hỗ trợ tải file ảnh lên thư mục `wwwroot/uploads/` bằng tên ngẫu nhiên `Guid`.
*   **Dọn dẹp ảnh khi sửa/xóa**: Tự động dọn sạch file ảnh vật lý trên ổ cứng khi sửa đổi ảnh mới hoặc xóa hẳn bài viết.

### 6. Quản lý thành viên (User CRUD)
*   **Đổi mật khẩu tùy chọn**: Cho phép bỏ trống trường mật khẩu mới khi sửa tài khoản để hệ thống tự động giữ nguyên mật khẩu cũ trong database, loại bỏ hoàn toàn các thông báo lỗi xác thực khó chịu.

---

## HƯỚNG DẪN CÀI ĐẶT VÀ KHỞI CHẠY

### 1. Chuẩn bị:
*   Visual Studio 2022 hoặc VS Code.
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

---

## TIẾN ĐỘ THỰC HIỆN DỰ ÁN

| Buổi Học | Nội Dung Thực Hiện | Trạng Thái | Chi Tiết |
| :--- | :--- | :---: | :--- |
| **Buổi 1** | Khởi tạo cấu trúc dự án 3 lớp, thiết lập cơ sở dữ liệu `ChinhCMS_DB`. | **Đã hoàn thành** | Tạo các thực thể và cấu hình kết nối database. |
| **Buổi 2** | Quản lý đơn hàng và chi tiết đơn hàng trực quan. | **Đã hoàn thành** | Thiết kế bảng hiển thị danh sách hóa đơn theo trạng thái. |
| **Buổi 3** | Xây dựng chức năng CRUD Danh mục an toàn, lọc bài viết mới nhất lên Trang chủ. | **Đã hoàn thành** | Khóa xóa danh mục chứa bài viết, dùng LINQ lấy 3 bài viết mới nhất. |
| **Buổi 4** | Thiết kế giao diện quản trị Admin Panel, tích hợp tải ảnh và trình soạn thảo CKEditor 5. | **Đã hoàn thành** | Hoàn thiện các trang quản lý: Danh mục, Bài viết, Đơn hàng, Thành viên (User CRUD). |
| **Buổi 5** | Bảo mật Cookie nâng cao, Phân quyền chi tiết, Quản lý sản phẩm & Danh mục sản phẩm. | **Đã hoàn thành** | **Xác thực Cookie, mã hóa BCrypt, dọn rác ảnh cũ, cố định ổ khóa Data Protection, phân trang, ẩn nút Xóa nếu chứa sản phẩm.** |

---

*Dự án được thực hiện bởi sinh viên Vũ Hoàng Chính - CCQ2211J.*
