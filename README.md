# HỆ THỐNG QUẢN LÝ NỘI DUNG VÀ BÁN HÀNG - CHINHCMS

Hệ thống quản lý nội dung (CMS) và bán hàng được xây dựng trên nền tảng **ASP.NET Core 8.0 MVC** và **Entity Framework Core**. Dự án được thiết kế theo cấu trúc 3 lớp, giúp dễ quản lý, nâng cấp và tích hợp trình soạn thảo văn bản **CKEditor 5** cùng với các tiêu chuẩn thiết kế hiện đại, responsive.

---

## THÔNG TIN SINH VIÊN

- **Sinh viên thực hiện:** Vũ Hoàng Chính
- **Mã số sinh viên:** 2122110380
- **Lớp học:** CCQ2211J
- **Môn học:** Chuyên đề ASP.NET
- **Giáo viên hướng dẫn:** Nguyễn Cao Thái
- **Phiên bản dự án:** 1.0

---

## CẤU TRÚC DỰ ÁN (SOLUTION)

Dự án `ChinhCMS_Solution` được chia làm 3 dự án nhỏ bên trong:

1.  **`CMS.Data` (Lớp dữ liệu)**:
    - Chứa các bảng dữ liệu: `User` (Thành viên), `Category` (Danh mục bài viết), `Post` (Bài viết), `Customer` (Khách hàng), `Product` (Sản phẩm), `CategoryProduct` (Danh mục sản phẩm), `Order` (Đơn hàng), `OrderDetail` (Chi tiết đơn hàng).
    - Sử dụng `ApplicationDbContext` để kết nối và cấu hình quan hệ giữa các bảng.
2.  **`CMS.Backend` (Trang quản trị)**:
    - Xây dựng bằng ASP.NET Core 8.0 MVC để làm trang quản trị (Admin Panel) cho người điều hành.
    - Gồm các bộ điều khiển (Controllers), giao diện (Views) và tài nguyên tĩnh (hình ảnh, css nằm trong wwwroot).
3.  **`cms.frontend` (Giao diện người dùng)**:
    - Trang hiển thị tin tức cho người xem được viết bằng ReactJS, lấy dữ liệu thông qua API từ Backend.

---

## CÁC TÍNH NĂNG ĐÃ HOÀN THÀNH

### 1. Giao diện quản trị (Admin Layout) & Nhúng Thông tin tài khoản

- **Thanh điều hướng bên cạnh (Sidebar):** Hiển thị danh sách các mục quản lý như Danh mục bài viết, Bài viết, Thành viên, Danh mục sản phẩm, Sản phẩm, Khách hàng và Đơn hàng.
- **Hỗ trợ hiển thị trên điện thoại (Responsive):**
  - Trên máy tính: Sidebar cố định ở bên trái.
  - Trên điện thoại: Sidebar tự động thu gọn dạng trượt (Offcanvas).
- **Thông tin tài khoản tích hợp Sidebar:** Tên người dùng, vai trò (Admin / Editor) cùng nút "Đăng xuất" được nhúng trực tiếp lên đầu Sidebar (ở cả phiên bản máy tính và điện thoại), tối ưu hóa trải nghiệm tương tự trên thiết bị di động.

### 2. Xác thực & Phân quyền (Cookie Authentication & Authorization) - [MỚI]

- **Dịch vụ xác thực Cookie**: Cấu hình trong `Program.cs` sử dụng `CookieAuthentication` chuẩn, tự động điều hướng về `/Account/Login` khi chưa đăng nhập và `/Account/AccessDenied` khi truy cập sai quyền.
- **Mã hóa mật khẩu an toàn**: Kiểm tra mật khẩu mã hóa hash nâng cao bằng công nghệ BCrypt (`PasswordHelper.VerifyPassword`), chống rò rỉ thông tin tuyệt đối.
- **Duy trì phiên đăng nhập bền vững (Data Protection)**:
  - Cấu hình lưu trữ bộ khóa mã hóa (Data Protection Keys) cố định vào thư mục dự án `App_Data/Keys`.
  - _Hiệu quả_: Cookie không bị mất hoặc bắt đăng nhập lại mỗi khi bạn biên dịch (rebuild) lại dự án hoặc khởi động lại server.
  - Thiết lập thuộc tính Cookie bền vững `IsPersistent = true` lưu trữ trên ổ đĩa trình duyệt trong vòng 7 ngày.
- **Bảo vệ Controller nghiêm ngặt**: Khóa toàn bộ các controller quản trị bằng thuộc tính `[Authorize]`. Cấu hình quyền hạn phân cấp vai trò:
  - `Admin`: Toàn quyền thao tác trên toàn bộ hệ thống (quản lý Thành viên, Danh mục sản phẩm, Khách hàng,...).
  - `Editor`: Bị hạn chế truy cập vào các chức năng nhạy cảm của Admin, tự động chuyển hướng sang trang báo lỗi 403 cao cấp khi cố ý truy cập.
- **Trang Access Denied 403 cực đẹp**: Thiết kế giao diện báo lỗi từ chối truy cập bằng hiệu ứng chuyển động chiếc khiên đỏ nhấp nháy (`animate-pulse`), nội dung lịch sự và đầy đủ nút quay lại an toàn.

### 3. Quản lý danh mục sản phẩm (CategoryProduct CRUD) - [MỚI]

- **Bảng danh sách chuẩn UI/UX**: Hiển thị số lượng sản phẩm liên kết thực tế của từng danh mục một cách chính xác nhất.
- **Khóa nút Xóa thông minh**:
  - Nếu số lượng sản phẩm thuộc danh mục đang lớn hơn 0 (`Products.Count > 0`), hệ thống sẽ **ẩn hoàn toàn nút Xóa** ở trang danh sách để ngăn chặn hành động sơ suất của người dùng.
  - Tích hợp bộ bảo vệ 2 lớp ở server-side trong `DeleteConfirmed` để trả về thông báo lỗi dạng Toast/Alert và ngăn chặn hành vi cố tình gửi yêu cầu xóa danh mục không rỗng.

### 4. Quản lý Sản phẩm (Product CRUD) - [MỚI]

- **Thông số kho chi tiết**: Hiển thị ảnh đại diện sản phẩm nhỏ gọn, tên sản phẩm, danh mục cha, giá bán định dạng tiền tệ VNĐ và số lượng tồn kho.
- **Nhãn trạng thái tồn kho thông minh**:
  - Số lượng `> 10`: Huy hiệu xanh lá (Đủ hàng).
  - Số lượng từ `1` đến `10`: Huy hiệu màu cam (Cảnh báo ít hàng).
  - Số lượng `= 0`: Huy hiệu màu đỏ (Hết hàng).
- **Dọn rác hình ảnh cũ khi cập nhật/xóa**:
  - Khi **Sửa sản phẩm** và tải lên một ảnh mới thay thế, hệ thống tự động tìm và xóa vĩnh viễn tệp ảnh cũ khỏi thư mục vật lý `wwwroot/uploads` trên máy chủ.
  - Khi **Xóa sản phẩm**, tệp tin ảnh đại diện của sản phẩm đó cũng được dọn sạch khỏi ổ đĩa để đảm bảo dung lượng lưu trữ của server luôn tối ưu nhất.

### 5. Quản lý bài viết tin tức (Post CRUD)

- **Trình soạn thảo CKEditor 5:** Tích hợp trực tiếp giúp viết bài có thể định dạng chữ, chèn bảng dễ dàng.
- **Tải ảnh trực tiếp lên máy chủ:** Hỗ trợ tải file ảnh lên thư mục `wwwroot/uploads/` bằng tên ngẫu nhiên `Guid`.
- **Dọn dẹp ảnh khi sửa/xóa**: Tự động dọn sạch file ảnh vật lý trên ổ cứng khi sửa đổi ảnh mới hoặc xóa hẳn bài viết.

### 6. Quản lý thành viên (User CRUD)

- **Đổi mật khẩu tùy chọn**: Cho phép bỏ trống trường mật khẩu mới khi sửa tài khoản để hệ thống tự động giữ nguyên mật khẩu cũ trong database, loại bỏ hoàn toàn các thông báo lỗi xác thực khó chịu.

### 7. Web API & RESTful Service (Buổi 6) - [MỚI]

- **Kiến trúc API & CORS**: Cấu hình CORS với chính sách `"AllowAll"` tại `Program.cs` hỗ trợ gọi API liên nguồn từ ứng dụng ReactJS. Tích hợp bộ sinh tài liệu tự động **Swagger UI** truy cập tại `/swagger/index.html`.
- **Mã nguồn 4 API Controller Restful độc lập & Chi tiết các Endpoint**:

#### 📰 A. API Bài viết Tin tức (`ApiPostController.cs` - Route: `/api/post`)

- **`GET /api/post`**: Lấy toàn bộ danh sách bài viết mới nhất (ID giảm dần). Gọt dữ liệu (Select) chỉ truyền `Id`, `Title`, `ImageUrl`, `CreatedDate`, `CategoryName`.
- **`GET /api/post/category/{categoryId}`**: Lọc danh sách bài viết thuộc chuyên mục tin tức tương ứng.
- **`GET /api/post/{id}`**: Lấy chi tiết bài viết (gồm trường `Content` chứa mã HTML). Trả về lỗi `404 Not Found` nếu ID không tồn tại.

#### 📦 B. API Danh mục & Sản phẩm (`ApiProductController.cs` - Route: `/api/product`)

- **`GET /api/product/categories`**: Lấy danh sách toàn bộ danh mục sản phẩm (`CategoryProducts` gồm `Id`, `Name`, `Description`).
- **`GET /api/product`**: Lấy toàn bộ danh sách sản phẩm mới nhất (gồm `Id`, `Name`, `Description`, `Price`, `StockQuantity`, `ImageUrl`, `CategoryName`).
- **`GET /api/product/category/{categoryId}`**: Lọc sản phẩm thuộc về một danh mục sản phẩm cụ thể.
- **`GET /api/product/{id}`**: Lấy thông tin chi tiết của một sản phẩm cụ thể. Trả về `404 Not Found` nếu không tìm thấy.

#### 👥 C. API Đăng ký & Đăng nhập Khách hàng (`ApiCustomerController.cs` - Route: `/api/customer`)

- **`POST /api/customer/register`**: Đăng ký khách hàng mới.
  - _Tham số truyền vào_: Đối tượng JSON gồm `FullName`, `Email`, `Phone`, `Address`, `Password`.
  - _Nghiệp vụ_: Kiểm tra định dạng Regex Email và SĐT Việt Nam, chặn trùng email. Tự động **mã hóa băm mật khẩu bằng BCrypt** trước khi ghi xuống CSDL.
- **`POST /api/customer/login`**: Đăng nhập hệ thống.
  - _Tham số truyền vào_: Đối tượng JSON gồm `Email`, `Password`.
  - _Nghiệp vụ_: Tìm email và dùng `VerifyPassword` đối soát mật khẩu băm. Trả về mã trạng thái và thông tin khách hàng nếu thành công.

#### 🛒 D. API Giỏ hàng & Đơn hàng (`ApiOrderController.cs` - Route: `/api/order`)

- **`POST /api/order/checkout`**: Đặt hàng (thực hiện thanh toán giỏ hàng).
  - _Tham số truyền vào_: JSON gồm `CustomerId`, `Notes` và danh sách sản phẩm `Items` (mỗi phần tử chứa `ProductId` và `Quantity`).
  - _Quy trình xử lý_:
    1. Sử dụng **Database Transaction** bảo đảm tính toàn vẹn (tự động Rollback hoàn toàn nếu phát sinh lỗi).
    2. Kiểm tra tồn tại khách hàng và sản phẩm.
    3. Kiểm tra số lượng hàng trong kho. Nếu thiếu hàng, trả về lỗi `400 Bad Request` chỉ rõ tên sản phẩm và số lượng tồn còn lại.
    4. Tự động **trừ trực tiếp số lượng tồn kho** (`StockQuantity`) của sản phẩm.
    5. Tự động áp giá bán hiện thời của sản phẩm vào `UnitPrice` trong chi tiết đơn hàng (`OrderDetail`).
- **`GET /api/order/customer/{customerId}`**: Lịch sử đơn hàng của một khách hàng (sắp xếp đơn hàng mới nhất lên đầu, trả về tổng tiền đơn hàng `TotalAmount` và tổng số sản phẩm `TotalItems`).
- **`GET /api/order/{id}`**: Lấy chi tiết đơn hàng (bao gồm thông tin khách hàng và danh sách chi tiết các sản phẩm đã mua gồm ảnh, tên, giá, số lượng và thành tiền).

### 8. Kết nối Frontend ReactJS & Web API (Buổi 7) - [ĐÃ HOÀN THÀNH]

- **Cấu hình CORS ở Backend**: Thiết lập chính sách `AllowReactApp` trong `Program.cs` cho phép cổng giao diện ReactJS (`http://localhost:3000`) thực hiện các truy vấn API.
- **Trục gọi API tập trung (Axios Client)**: Khởi tạo và cấu hình `axiosClient.js` trong ReactJS hỗ trợ tự động bóc tách dữ liệu JSON và quản lý lỗi tập trung.
- **Hiển thị danh mục sản phẩm thời trang (CategoryProductList)**: Thiết kế component gọi API `/api/categoriesproducts` real-time từ SQL Server để hiển thị bộ lọc danh mục.
- **Hiển thị danh sách sản phẩm thời trang (ProductList) - [Mở rộng]**: Tự xây dựng component kết nối endpoint `/api/Products`, trình bày dạng lưới Grid Card Bootstrap kèm định dạng tiền tệ VND (`Intl.NumberFormat`).
- **Hiển thị danh mục & bài viết tin tức (PostList) - [Mở rộng]**: Thiết lập kết nối endpoint `/api/Posts`, hiển thị các bài viết chia sẻ xu hướng phối đồ công sở, dạ hội kèm định dạng ngày đăng vi-VN (`toLocaleDateString`).

### 9. Hoàn thiện Trang cá nhân, Hạng VIP động, Luồng Đặt hàng & Tách CSS (Buổi 8) - [ĐÃ HOÀN THÀNH]

- **Trang Danh mục sản phẩm độc lập (`ProductView.jsx`)**: Tách biệt hoàn toàn phần lưới sản phẩm chính, bộ lọc danh mục và phân trang từ trang chủ sang một trang riêng, trỏ menu "Sản phẩm" ở Header đến đúng định tuyến mới.
- **Trang chi tiết sản phẩm (`ProductDetailView.jsx`)**: Tải chi tiết sản phẩm thực tế từ API, tự động hiển thị size dựa trên danh mục, giới hạn số lượng mua theo tồn kho thực tế, kiểm tra trạng thái đăng nhập trước khi thêm sản phẩm vào giỏ hàng.
- **Trang Cá Nhân & Hạng VIP Động (`UserInfoView.jsx`)**: Tải thông tin tài khoản khách hàng từ Cookie `customer` (hạn 2 ngày). Gọi API tải lịch sử mua hàng, hiển thị nhãn trạng thái hóa đơn có màu sắc trực quan. Tự động cộng dồn tổng tiền các hóa đơn để phân cấp VIP động.
- **Luồng Thanh toán & Ràng buộc kho (`CheckoutView.jsx` & `PaymentView.jsx`)**: Ràng buộc bảo mật yêu cầu đăng nhập trước khi thanh toán. Gom toàn bộ giỏ hàng và thông tin khách hàng gửi lên Backend API. Nếu thành công, chuyển hướng đến trang chọn phương thức thanh toán và xóa sạch giỏ hàng.
- **Trang Bài viết chuyên biệt (`BlogView.jsx`)**: Tách biệt hoàn toàn bộ lọc chuyên mục bài viết khỏi trang chủ. Khi bấm vào mục "Bài viết" ở Header, người dùng sẽ được chuyển tới giao diện tin tức độc lập có 2 cột (Trái: Danh sách chuyên mục `BlogCategoryList.jsx` tải real-time từ API `/post/categories`; Phải: Lưới các bài viết tương ứng có tích hợp bộ phân trang động `currentPage`/`totalPages`).
- **Thống nhất mã nguồn Services**: Hợp nhất các cuộc gọi API bài viết và chuyên mục bài viết vào tệp `postService.js` thay vì tạo file dịch vụ trùng lặp, giúp tối ưu hóa cấu trúc dự án ReactJS.
- **Tách mã CSS sạch**: Tách toàn bộ CSS nhúng (inline styles) trong các file JSX sang các file `.css` chuyên biệt như `AboutView.css`, `SearchView.css`, `ProductDetailView.css`, `CheckoutView.css`, `BlogView.css`, `BlogCategoryList.css` và `UserInfoView.css` trong thư mục `src/assets/css`.

### 10. Tích hợp Quản lý Banner, Slider bài viết mượt mà, Footer động & Khóa danh mục hệ thống (Buổi 10) - [MỚI]

- **Thêm trường hình ảnh cho danh mục**:
  - Bổ sung trường `ImageUrl` cho cả hai thực thể `Category` và `CategoryProduct`.
  - Cập nhật chức năng Upload ảnh trực tiếp từ thiết bị của quản trị viên (loại bỏ nhập URL tĩnh) và hiển thị trên giao diện thẻ danh mục sản phẩm đẹp mắt.
- **Nâng cấp trang chủ & Slider tin tức**:
  - Trích xuất phần Hero Banner lặp lại thành component dùng chung `HeroBanner.jsx`.
  - Thiết kế Slider trình chiếu 5 bài viết mới nhất với hiệu ứng chuyển trang mượt mà trên trang tin tức `Blog/Index.jsx`.
- **Hệ thống Quản lý Banner động**:
  - Tạo thực thể CSDL `Banner` (chứa các trường ID, Tên, Mô tả, Đường dẫn ảnh, Trạng thái ẩn/hiện, Ngày tạo).
  - Viết bộ API `ApiBannerController.cs` và dịch vụ `bannerService.js` tải các banner kích hoạt lên giao diện.
  - Thiết kế trang CRUD quản lý Banner trong Admin Dashboard kèm chức năng upload ảnh vào thư mục `wwwroot/uploads` và tự động xóa tệp tin ảnh vật lý cũ trên máy chủ khi cập nhật/xóa.
  - Tích hợp hiệu ứng Banner Slider/Carousel cao cấp tự động chuyển slide mỗi 6 giây có nút bấm trái/phải và chấm tròn chỉ mục tại component `HeroBanner.jsx`. Khi cơ sở dữ liệu trống, component tự động chuyển về chế độ hiển thị tĩnh (fallback) an toàn.
- **Bảo mật chuyên mục hệ thống & Footer động**:
  - Khóa cứng hai danh mục hệ thống mặc định là "Tất cả bài viết" (ID: 13) và "Tất cả sản phẩm" (ID: 7) trên Server Controller. Nếu cố tình gửi yêu cầu xóa, hệ thống sẽ chặn lại, lưu thông báo tiếng Việt trực quan vào `TempData["ErrorMessage"]` và chuyển hướng an toàn kèm Alert thông báo.
  - Cập nhật menu liên kết "Danh mục" ở chân trang (Footer) tự động truy vấn dữ liệu thực tế từ cơ sở dữ liệu và lọc bỏ mục "Tất cả sản phẩm".
- **Việt hóa thông báo lỗi & Sửa lỗi Sidebar cuộn**:
  - Cập nhật thuộc tính xác thực Data Annotations tiếng Việt thân thiện tại lớp thực thể Banner.cs.
  - Cấu hình lại chiều cao `.admin-sidebar { height: 100vh; }` thay vì `min-height` để thanh menu Sidebar cố định bên trái của Admin Panel có thể cuộn dọc mượt mà khi màn hình có độ phân giải thấp, giúp quản trị viên click được mục "Quản lý thành viên".

### 11. Tái cấu trúc Định tuyến React Router DOM & Cân bằng Header UI (Buổi 11) - [MỚI]

- **React Router DOM SPA Integration**:
  - Thay thế hệ thống định tuyến tự chế bằng thư viện định tuyến chuẩn `react-router-dom` (`BrowserRouter`, `Routes`, `Route`).
  - Chuyển đổi toàn bộ các liên kết tĩnh và động (ở Header, Footer, thẻ sản phẩm `ProductCard`, thẻ bài viết `PostCard`, Slider Blog) sang thẻ `<Link>` chuẩn của React Router. Nhờ đó, ứng dụng hoạt động mượt mà dạng Single Page Application (SPA), hỗ trợ lịch sử duyệt web (Back/Forward) và mở liên kết trong tab mới.
- **Căn giữa thanh Menu đầu trang**:
  - Thiết lập CSS Flexbox thông minh (`flex: 1` cho Logo và Actions) trên Header giúp phần menu chính `.nav-links` tự động căn giữa cân đối chính xác 100% trên giao diện Desktop.
- **Sửa lỗi lưu tiêu đề bài viết (Post Controller)**:
  - Đổi kiểu dữ liệu tham số tải ảnh từ `IFormFile uploadImage` bắt buộc sang nullable `IFormFile? uploadImage`, giải quyết triệt để lỗi không lưu được tiêu đề khi quản trị viên cập nhật bài viết mà không tải lên ảnh mới.

### 12. Tích hợp giỏ hàng nâng cao, ô nhập số lượng bàn phím, trì hoãn luồng đặt hàng & đổi nhanh trạng thái Banner (Buổi 12) - [MỚI]

- **Nâng cấp thẻ ProductCard tiện lợi**:
  - Tách biệt hành vi click thẻ: Click vào vùng trống của thẻ sản phẩm sẽ xem chi tiết; tích hợp 2 nút hành động trực tiếp "Giỏ hàng" và "Mua ngay" mà không gây xung đột định tuyến nhờ `e.stopPropagation()`.
  - Bộ đệm đăng nhập tự động: Nếu khách hàng chưa đăng nhập, thao tác thêm giỏ/mua nhanh được lưu tạm trong `localStorage`. Sau khi đăng nhập thành công, hệ thống tự động hoàn tất tác vụ (thêm vào giỏ hoặc chuyển thẳng đến trang đặt hàng `/checkout`).
- **Cập nhật thông tin khách hàng từ xa**:
  - Bổ sung Endpoint PUT `api/customer/update/{id}` cho phép chỉnh sửa thông tin tài khoản (Họ tên, Điện thoại, Địa chỉ, Mật khẩu mới) từ giao diện frontend.
- **Giới hạn kho và Nhập số lượng trực tiếp**:
  - Ngăn chặn tuyệt đối việc đặt mua vượt quá hàng tồn kho trong chi tiết sản phẩm và giỏ hàng.
  - Thay thế số lượng giỏ hàng tĩnh bằng ô nhập số (`input type="number"`), hỗ trợ gõ trực tiếp từ bàn phím kết hợp giữ nguyên hai nút tăng giảm `+`/`-`.
- **Trang Đặt hàng 2 cột & Trì hoãn ghi CSDL**:
  - Giao diện Checkout mới dạng 2 cột: Cột trái nhập thông tin giao nhận hàng; Cột phải tóm tắt chi tiết các sản phẩm kèm hình ảnh, số lượng và tổng thanh toán.
  - Trì hoãn tạo đơn hàng ở database: Nhấp xác nhận giao hàng sẽ không ghi vào CSDL ngay, thông tin được lưu tạm trong `sessionStorage`. Đơn hàng chỉ thực sự được tạo và trừ kho khi khách hàng xác nhận & thanh toán thành công tại trang `/payment`.
- **Thay đổi nhanh trạng thái hiển thị Banner**:
  - Tích hợp thêm Action POST `ToggleStatus` trong `BannersController.cs` và Ajax Fetch trong `Banners/Index.cshtml`.
  - Người điều hành chỉ cần click trực tiếp vào nhãn trạng thái "Hiển thị" hoặc "Ẩn" để bật tắt hiển thị slide mà không phải vào trang sửa.
- **Tái cấu trúc Sidebar Danh mục sản phẩm & CSS productCSS**:
  - Phân tách và rút gọn tệp `Product/Index.jsx` bằng cách trích xuất thanh danh mục dọc thành component `ProductCategoryList.jsx` nằm ngay trong thư mục `src/pages/Product/`.
  - Sửa đổi giao diện từ các thẻ tròn cuộn ngang cũ thành dạng danh mục sidebar dọc tương đồng với Blog, mang lại trải nghiệm nhất quán.
  - Chuyển toàn bộ CSS liên quan vào thư mục `src/assets/css/productCSS/` (bao gồm `Product.css` và `ProductCategoryList.css`), loại bỏ hoàn toàn mã CSS inline hoặc CSS lộn xộn.
- **Thanh tìm kiếm Autocomplete & SEO Slug cho Bài viết (Post)**:
  - Loại bỏ hoàn toàn trang Tìm kiếm cũ (`Search/Index.jsx`). Thay vào đó là thanh Live Search dạng autocomplete tức thì tích hợp ngay chính giữa của `Header.jsx`. Khi gõ từ khóa, hệ thống hiển thị bảng kết quả phân loại song song: Sản phẩm và Bài viết một cách trực quan.
  - Cập nhật các API lấy danh sách bài viết (`ApiPostController`) và sản phẩm (`ApiProductController`) để hỗ trợ lọc theo tham số `keyword`.
  - Cập nhật thực thể bài viết `Post.cs` và CSDL bổ sung cột `Slug` (SEO URL) với ràng buộc Unique Index trong `ApplicationDbContext.cs`.
  - Tích hợp tự động sinh Slug tiếng Việt không dấu từ tiêu đề khi tạo/sửa bài viết (`PostController.cs`) và nâng cấp trang chi tiết tin tức `Blog/Detail.jsx` nhận dạng tải theo cả ID hoặc SEO Slug.
  - Tái thiết kế bố cục Header thành 2 hàng: Hàng trên chứa Logo, Thanh tìm kiếm, và Tiện ích cá nhân/Giỏ hàng; Hàng dưới chứa Menu điều hướng căn giữa. Hỗ trợ tự động co giãn thông minh, chuyển thanh tìm kiếm xuống hàng riêng biệt trên điện thoại để tối ưu trải nghiệm.
- **Tối ưu hóa Cấu trúc CSS (CSS Modularization)**:
  - Phân tách tệp `main.css` cồng kềnh thành các file CSS Module riêng biệt cho từng thành phần giao diện chính để tối ưu hóa hiệu năng tải trang và khả năng bảo trì:
    - `Header.css` cho Header và Live Search Autocomplete.
    - `Footer.css` cho Footer chân trang.
    - `Cart.css` cho trang Giỏ hàng.
    - `ProductCard.css` cho thẻ Card sản phẩm.
    - `ProductDetail.css` cho trang chi tiết sản phẩm.
  - Tệp `main.css` giờ đây chỉ chứa các biến toàn cục (colors, fonts), reset CSS và các class biểu mẫu/nút bấm dùng chung.
- **Cấu hình Phân trang 8 thành phần & Đồng nhất giao diện**:
  - Thiết lập giá trị `pageSize = 8` cho cả trang Danh sách sản phẩm (`Product/Index.jsx`) và trang Tin tức (`Blog/Index.jsx`) để chỉ hiển thị tối đa 8 đối tượng trên một trang.
  - Đồng nhất bộ phân trang toàn cục: Chuyển các lớp CSS phân trang `.pagination-container` và `.page-btn` thành các lớp dùng chung trong tệp `main.css`, mang lại giao diện và hiệu ứng hover/active/disabled đồng điệu 100% trên toàn website.
- **Ẩn danh mục hệ thống mặc định trong Admin**:
  - Loại bỏ các danh mục hệ thống "Tất cả sản phẩm" (ID: 7) và "Tất cả bài viết" (ID: 13) khỏi danh sách chọn danh mục (dropdown list) khi quản trị viên thực hiện Thêm mới hoặc Chỉnh sửa sản phẩm và bài viết ở giao diện quản trị Admin.
- **Nâng cấp Header Đăng nhập & Tối ưu hóa UI/UX trên Di động (Mobile)**:
  - Header hiển thị lời chào `"Chào, {Họ tên}"` kèm avatar hình tròn chứa chữ cái đầu tiên của khách hàng khi đã đăng nhập (lấy dữ liệu tự động từ cookie).
  - Khi xem trên điện thoại, thanh Header thu gọn tinh giản tối đa (chỉ hiện Logo và nút Menu Hamburger). Các tiện ích như thông tin khách hàng, thanh tìm kiếm Autocomplete trực quan và liên kết văn bản `"Giỏ hàng ({Số lượng})"` đều được gom gọn gàng bên trong menu trượt xuống, hỗ trợ thanh cuộn độc lập (`overflow-y: auto`) tránh tràn màn hình.
- **Tích hợp Chú thích tiếng Việt chi tiết (Code Annotations)**:
  - Bổ sung hệ thống chú thích và giải thích chi tiết bằng tiếng Việt trong các file mã nguồn cốt lõi (`Header.jsx`, `Header.css`, `main.css`, `Blog/Index.jsx`) phục vụ tốt nhất cho việc học tập, báo cáo và tự giải thích mã nguồn.
- **Tạo các trang Hỗ trợ mới (Bảo mật & Hướng dẫn size)**:
  - Trang **Chính sách bảo mật** (`pages/PrivacyPolicy/Index.jsx`): Cam kết bảo mật, mã hóa dữ liệu BCrypt và cách thu thập/sử dụng thông tin khách hàng.
  - Trang **Hướng dẫn chọn size** (`pages/SizeGuide/Index.jsx`): Tích hợp bảng so sánh size giày (US, UK, EU, CM) và quần áo (S, M, L, XL, XXL) với các tab chuyển đổi mượt mà cùng hướng dẫn tự đo kích cỡ chi tiết.
  - Cập nhật định tuyến chuẩn React Router và liên kết đầy đủ ở chân trang `Footer.jsx`.
- **Trang Cập nhật thông tin Khách hàng (UserInfo)**:
  - Tạo trang chỉnh sửa thông tin cá nhân (`pages/User/UserInfo.jsx`): cho phép khách hàng tự cập nhật Họ tên, Địa chỉ Email (với kiểm tra định dạng regex), Số điện thoại, Địa chỉ nhận hàng và thay đổi Mật khẩu mới (tối thiểu 6 ký tự).
  - Cập nhật DTO `UpdateRequest` và hành vi logic của PUT API `api/customer/update/{id}` trên C# Backend (`ApiCustomerController.cs`): Thêm trường Email, kiểm tra trùng lặp email với các tài khoản khác trên Database và định dạng hợp lệ trước khi lưu.
  - Tự động lưu trữ thông tin mới vào Cookie `'customer'` để đồng bộ hóa lời chào/avatar ngay trên Header mà không cần đăng nhập lại.
  - Thiết kế nút bấm "Sửa thông tin" tinh tế bên cạnh nút "Đăng xuất" trên trang tổng quan tài khoản (`User/Index.jsx`).
- **Tối ưu hóa Quy trình trừ Kho hàng theo Trạng thái Đơn hàng (Inventory Stock Control)**:
  - Loại bỏ việc trừ hàng tồn kho tự động ngay khi vừa đặt hàng (khi đơn hàng đang ở trạng thái `Status = 0` (Chờ duyệt)).
  - Trong `OrdersController.cs` phía admin, bổ sung cơ chế kiểm soát tồn kho chặt chẽ:
    - Trừ kho sản phẩm **chỉ khi** đơn hàng chuyển từ trạng thái `0` (Chờ duyệt) hoặc `3` (Đã hủy) sang trạng thái `1` (Đang giao) hoặc `2` (Đã hoàn thành). Kiểm tra và từ chối duyệt (bằng ModelState error) nếu có bất kỳ mặt hàng nào không đủ tồn kho.
    - Tự động hoàn lại (cộng thêm) số lượng vào kho nếu đơn hàng bị chuyển ngược về trạng thái `0` (Chờ duyệt) hoặc `3` (Đã hủy).
    - Hoàn trả lại số lượng tồn kho của các sản phẩm tương ứng nếu đơn hàng đang giao hoặc đã hoàn thành bị xóa trực tiếp khỏi hệ thống.
- **Chuẩn hóa thống kê Sản phẩm Bán Chạy Nhất (Best Sellers)**:
  - Cập nhật API `GetBestSellers` tại `ApiProductController.cs` để chỉ tính toán doanh số `TotalSold` từ các đơn hàng có trạng thái **Đã hoàn thành** (`Status == 2`), đồng bộ chính xác với logic của trang dashboard Admin (`HomeController.cs`).

---

## HƯỚNG DẪN CÀI ĐẶT VÀ KHỞI CHẠY

### 1. Chuẩn bị:

- Visual Studio 2022 hoặc VS Code.
- .NET 8.0 SDK.
- SQL Server (khuyến nghị dùng LocalDB).

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

| Buổi Học   | Nội Dung Thực Hiện                                                                       |    Trạng Thái     | Chi Tiết                                                                                                                                                    |
| :--------- | :--------------------------------------------------------------------------------------- | :---------------: | :---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Buổi 1** | Khởi tạo cấu trúc dự án 3 lớp, thiết lập cơ sở dữ liệu `ChinhCMS_DB`.                    | **Đã hoàn thành** | Tạo các thực thể và cấu hình kết nối database.                                                                                                              |
| **Buổi 2** | Quản lý đơn hàng và chi tiết đơn hàng trực quan.                                         | **Đã hoàn thành** | Thiết kế bảng hiển thị danh sách hóa đơn theo trạng thái.                                                                                                  |
| **Buổi 3** | Xây dựng chức năng CRUD Danh mục an toàn, lọc bài viết mới nhất lên Trang chủ.           | **Đã hoàn thành** | Khóa xóa danh mục chứa bài viết, dùng LINQ lấy 3 bài viết mới nhất.                                                                                         |
| **Buổi 4** | Thiết kế giao diện quản trị Admin Panel, tích hợp tải ảnh và trình soạn thảo CKEditor 5. | **Đã hoàn thành** | Hoàn thiện các trang quản lý: Danh mục, Bài viết, Đơn hàng, Thành viên (User CRUD).                                                                         |
| **Buổi 5** | Bảo mật Cookie nâng cao, Phân quyền chi tiết, Quản lý sản phẩm & Danh mục sản phẩm.      | **Đã hoàn thành** | **Xác thực Cookie, mã hóa BCrypt, dọn rác ảnh cũ, cố định ổ khóa Data Protection, phân trang, ẩn nút Xóa nếu chứa sản phẩm.**                               |
| **Buổi 6** | Phát triển Web API RESTful & cấu hình CORS, tích hợp bộ tạo tài liệu tự động Swagger UI. | **Đã hoàn thành** | **Xây dựng hệ thống 4 API Controllers (Bài viết, Sản phẩm, Khách hàng, Đơn hàng), băm mật khẩu BCrypt, trừ kho, Transaction checkout, CORS & Swashbuckle.** |
| **Buổi 7** | Kết nối Frontend ReactJS với Backend ASP.NET Core Web API. | **Đã hoàn thành** | **Cấu hình CORS trên Backend, thiết lập Axios Client tập trung (`axiosClient.js`), xây dựng component hiển thị danh mục sản phẩm (`CategoryProductList.jsx`). Tự thực hiện bài tập mở rộng kết nối API danh sách sản phẩm (`ProductList.jsx` hiển thị Grid Card, định dạng VND) và tin tức (`PostList.jsx` hiển thị bài viết, định dạng ngày vi-VN).** |
| **Buổi 8** | Hoàn thiện trang cá nhân, xếp hạng VIP động (chỉ mới làm ở frontend), luồng đặt hàng thật, tách CSS và tối ưu hóa UI/UX. | **Đã hoàn thành** | **Tách biệt trang danh sách sản phẩm độc lập (ProductView.jsx) và trang Bài viết chuyên biệt (BlogView.jsx) kèm bộ lọc chuyên mục bài viết (BlogCategoryList.jsx) có phân trang. Tải thông tin tài khoản và tính hạng VIP động ở Frontend. Ràng buộc bảo mật đăng nhập giỏ hàng/thanh toán. Gửi hóa đơn lên Backend thực hiện Database Transaction trừ tồn kho. Tách toàn bộ CSS nhúng sang thư mục `src/assets/css`.** |
| **Buổi 9** | Nâng cấp hệ thống SEO Slug và cấu trúc dữ liệu cho thực thể sản phẩm (Product). | **Đã hoàn thành** | **Tích hợp SlugHelper tự sinh URL thân thiện tiếng Việt không dấu, ràng buộc Unique Index trên database SQL Server. Xây dựng API và client service tải sản phẩm theo Slug, nâng cấp ProductCard và ProductDetailView sang định tuyến SEO.** |
| **Buổi 10** | Tích hợp Banner Carousel động, khóa danh mục hệ thống & Sửa lỗi cuộn Sidebar. | **Đã hoàn thành** | **Tạo bảng Banner, ApiBannerController, CRUD Banner Admin Dashboard upload ảnh và xóa tệp vật lý cũ, slider động HeroBanner. Khóa cứng danh mục 7 & 13. Sửa lỗi Sidebar cuộn.** |
| **Buổi 11** | Tái cấu trúc SPA với React Router DOM, sửa lỗi cập nhật bài viết & Căn giữa Header. | **Đã hoàn thành** | **Tích hợp BrowserRouter/Link thay thế custom navigate, sửa tham số IFormFile? cho PostController, cân bằng flex Header căn giữa menu.** |
| **Buổi 12** | Tích hợp giỏ hàng nâng cao, ô nhập số lượng bàn phím, trì hoãn luồng đặt hàng, đổi nhanh trạng thái Banner, Live Search Autocomplete, Tách nhỏ CSS, Phân trang 8 & Ẩn danh mục hệ thống. | **Đã hoàn thành** | **Thiết kế lại ProductCard; đệm đăng nhập tự động; ô nhập số lượng bàn phím giỏ hàng; giao diện Checkout 2 cột; AJAX đổi nhanh trạng thái Banner; phân tách Component Sidebar; tích hợp live-search Autocomplete trung tâm Header và SEO Slug cho bài viết (Post); phân rã main.css cồng kềnh thành các file CSS module riêng biệt (Header.css, Footer.css, Cart.css, ProductCard.css, ProductDetail.css); cấu hình phân trang hiển thị tối đa 8 thành phần mỗi trang cho cả sản phẩm và bài viết; loại bỏ danh mục mặc định "Tất cả" (ID: 7 và 13) khỏi dropdown list trong màn hình Thêm mới/Chỉnh sửa ở Admin; đồng nhất CSS phân trang toàn cục; thiết kế lại Header thông minh trên Mobile (tích hợp profile, search, và text giỏ hàng vào menu trượt); viết hệ thống chú thích tiếng Việt cho toàn bộ mã nguồn.** |
---


_Dự án được thực hiện bởi sinh viên Vũ Hoàng Chính - CCQ2211J._
