/*
 * Sinh viên : Vũ Hoàng Chính
 * Mã sinh viên: 2122110380
 * Lớp: CCQ2211J
 * Ngày tạo: 15/05/2026
 * Version: 1.0
 */

using CMS.Data;
using CMS.Data.Entities; // Kết nối tới lớp dữ liệu
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering; // Thư viện cần thiết để sử dụng danh sách chọn SelectList
using System.IO; // Thư viện để thao tác với file và đường dẫn thư mục
using Microsoft.AspNetCore.Authorization;

namespace CMS.Backend.Controllers
{
    // Controller PostController quản lý các hành động liên quan đến bài viết tin tức
    // , ví dụ: hiển thị danh sách các bài viết.
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Hàm hiển thị danh sách bài viết, hỗ trợ lọc theo mã danh mục bài viết (CategoryId)
        public IActionResult Index(int? id)
        {
            // Nếu không truyền mã danh mục (id là null), hệ thống sẽ lấy toàn bộ danh sách bài viết
            if (id == null)
            {
                // Truy vấn lấy toàn bộ bài viết từ cơ sở dữ liệu, sắp xếp theo ngày đăng mới nhất
                // và nạp kèm thông tin danh mục tương ứng để tránh lỗi hiển thị tên danh mục
                var allPosts = _context.Posts
                                      .Include(p => p.Category)
                                      .OrderByDescending(p => p.CreatedDate)
                                      .ToList();

                // Truyền danh sách tất cả bài viết sang giao diện hiển thị
                return View(allPosts);
            }

            // Nếu có truyền mã danh mục, tiến hành lọc các bài viết thuộc danh mục đó
            var filteredPosts = _context.Posts
                                        .Where(p => p.CategoryId == id)
                                        .Include(p => p.Category)
                                        .OrderByDescending(p => p.CreatedDate)
                                        .ToList();

            // Truyền danh sách bài viết đã được lọc sang giao diện hiển thị
            return View(filteredPosts);
        }

        // Hàm hiển thị chi tiết của một bài viết dựa vào mã bài viết được truyền từ URL
        public IActionResult Details(int? id)
        {
            // Kiểm tra xem mã bài viết truyền vào có hợp lệ hay không
            if (id == null)
            {
                // Trả về thông báo lỗi nếu không nhận được mã bài viết từ yêu cầu của trình duyệt
                return BadRequest("Vui lòng cung cấp mã bài viết.");
            }

            // Truy vấn lấy thông tin bài viết theo mã bài viết
            // Sử dụng lệnh nạp chồng để lấy kèm thông tin danh mục liên quan của bài viết đó
            var detailPost = _context.Posts
                                     .Include(p => p.Category)
                                     .FirstOrDefault(p => p.Id == id);

            // Kiểm tra xem có tìm thấy bài viết trong cơ sở dữ liệu hay không
            if (detailPost == null)
            {
                // Trả về lỗi không tìm thấy trang nếu bài viết không tồn tại trong hệ thống
                return NotFound("Không tìm thấy bài viết này trong hệ thống.");
            }

            // Truyền thông tin chi tiết bài viết sang giao diện chi tiết
            return View(detailPost);
        }

        // 1. Hàm hiển thị form tạo mới bài viết (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách danh mục đổ vào SelectList để làm menu chọn chuyên mục bài viết
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // 2. Hàm tiếp nhận dữ liệu và thực thi lưu trữ bài viết mới (POST)
        [HttpPost]
        public IActionResult Create(Post model, IFormFile uploadImage)
        {
            // Bỏ qua xác thực tự động đối với thuộc tính Category liên kết ảo của EF Core
            // để tránh lỗi không lưu được bài viết do quy tắc nghiêm ngặt Nullable của .NET 8
            ModelState.Remove("Category");

            // Xử lý tải ảnh vật lý lên máy chủ trước khi kiểm tra ModelState.IsValid
            // để nếu người dùng chọn tải ảnh lên, trường ImageUrl sẽ tự động có giá trị và không bị báo lỗi trống
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // 1. Định nghĩa thư mục lưu trữ vật lý: wwwroot/uploads bên trong dự án
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                
                // Tạo thư mục uploads nếu chưa tồn tại
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                // 2. Tạo tên file ngẫu nhiên duy nhất bằng GUID tránh ghi đè ảnh trùng tên
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                // 3. Sao chép và ghi file ảnh vào thư mục vật lý wwwroot/uploads
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                // 4. Gán đường dẫn tương đối để lưu vào cơ sở dữ liệu
                model.ImageUrl = "/uploads/" + fileName;
            }

            // Nếu ImageUrl đã có giá trị (do người dùng nhập link URL hoặc do vừa upload ảnh thành công)
            // thì loại bỏ lỗi kiểm tra trống tự động của ModelState đối với thuộc tính ImageUrl
            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                ModelState.Remove("ImageUrl");
            }

            // Kiểm tra thủ công: Bắt buộc phải có ít nhất 1 trong 2 nguồn ảnh (Nhập URL hoặc Upload file ảnh)
            if (string.IsNullOrEmpty(model.ImageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Vui lòng nhập đường dẫn URL ảnh hoặc chọn tải ảnh lên từ thiết bị.");
            }

            // Kiểm tra thủ công các trường bắt buộc nhập khác
            if (string.IsNullOrEmpty(model.Title))
            {
                ModelState.AddModelError("Title", "Vui lòng nhập tiêu đề bài viết.");
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError("Content", "Vui lòng nhập nội dung bài viết.");
            }

            if (ModelState.IsValid)
            {
                // Thêm bài viết mới vào Database
                _context.Posts.Add(model);
                _context.SaveChanges();

                // Quay về trang danh sách bài viết quản trị sau khi đăng bài thành công
                return RedirectToAction("Index");
            }

            // Nếu dữ liệu không hợp lệ, nạp lại danh sách danh mục và trả về giao diện biểu mẫu nhập liệu
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // 3. Hàm hiển thị biểu mẫu chỉnh sửa bài viết cũ (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Tìm bài viết theo Id trong cơ sở dữ liệu
            var post = _context.Posts.Find(id);
            if (post == null)
            {
                return NotFound("Không tìm thấy bài viết này trong hệ thống.");
            }

            // Nạp lại danh sách danh mục và giữ nguyên danh mục đang được lựa chọn của bài viết
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // 4. Hàm tiếp nhận dữ liệu đã chỉnh sửa và cập nhật thay đổi bài viết (POST)
        [HttpPost]
        public IActionResult Edit(Post model, IFormFile uploadImage)
        {
            // Bỏ qua xác thực tự động đối với thuộc tính Category liên kết ảo
            ModelState.Remove("Category");

            // Xử lý tải ảnh vật lý mới lên máy chủ trước khi kiểm tra ModelState.IsValid
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                model.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                // Nếu người dùng không upload ảnh mới, khôi phục lại ảnh cũ từ Database nhờ AsNoTracking
                var oldPost = _context.Posts.AsNoTracking().FirstOrDefault(p => p.Id == model.Id);
                if (oldPost != null && string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = oldPost.ImageUrl;
                }
            }

            // Nếu ImageUrl đã có giá trị (link URL hoặc đường dẫn ảnh cũ hoặc ảnh mới upload)
            // thì loại bỏ lỗi kiểm tra trống tự động của ModelState đối với thuộc tính ImageUrl
            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                ModelState.Remove("ImageUrl");
            }

            // Kiểm tra thủ công bắt buộc có ảnh
            if (string.IsNullOrEmpty(model.ImageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Vui lòng nhập đường dẫn URL ảnh hoặc chọn tải ảnh lên từ thiết bị.");
            }

            // Kiểm tra thủ công các trường bắt buộc
            if (string.IsNullOrEmpty(model.Title))
            {
                ModelState.AddModelError("Title", "Vui lòng nhập tiêu đề bài viết.");
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError("Content", "Vui lòng nhập nội dung bài viết.");
            }

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin bài viết vào Database
                _context.Posts.Update(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu dữ liệu bị lỗi, nạp lại danh sách danh mục và trả về giao diện biểu mẫu sửa
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // 5. Hàm thực thi tác vụ xóa bài viết ra khỏi cơ sở dữ liệu và ổ cứng
        public IActionResult Delete(int id)
        {
            // Tìm bài viết theo Id
            var post = _context.Posts.Find(id);
            if (post != null)
            {
                // Kiểm tra xem bài viết có chứa hình ảnh tải lên vật lý cục bộ hay không
                // Nếu đường dẫn ảnh bắt đầu bằng "/uploads/" thì đó là ảnh lưu trữ trên máy chủ của ta
                if (!string.IsNullOrEmpty(post.ImageUrl) && post.ImageUrl.StartsWith("/uploads/"))
                {
                    // Tính toán đường dẫn vật lý tuyệt đối của file ảnh trên máy chủ
                    // Loại bỏ ký tự gạch chéo đầu tiên '/' để ghép đường dẫn chuẩn xác
                    string relativePath = post.ImageUrl.TrimStart('/');
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                    // Kiểm tra xem tệp tin ảnh có thực sự tồn tại trên đĩa cứng hay không trước khi xóa
                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            // Tiến hành xóa vĩnh viễn tệp ảnh vật lý khỏi ổ cứng máy chủ
                            System.IO.File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            // Ghi nhận lỗi nếu có lỗi phân quyền hoặc tệp đang bị khóa, tránh làm treo ứng dụng
                            Console.WriteLine("Lỗi khi xóa tệp ảnh vật lý: " + ex.Message);
                        }
                    }
                }

                // Thực thi xóa bản ghi bài viết ra khỏi cơ sở dữ liệu SQL Server
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }

            // Quay về danh sách bài viết quản trị sau khi xóa thành công
            return RedirectToAction("Index");
        }
    }
}
