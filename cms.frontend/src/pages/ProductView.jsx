// export default HomeView;
import { useState, useEffect } from 'react';
import ProductCard from '../components/ProductCard';

// IMPORT các file gọi API chuyên biệt của bạn (Hãy điều chỉnh lại đường dẫn ../ cho đúng thư mục dự án)
import productService from '../services/productService';
import categoryProductService from '../services/categoryProductService';

// Import css
import '../assets/css/ProductView.css';

const BASE_URL = "https://localhost:7291"; // Cấu hình lấy ảnh từ wwwroot/uploads của Backend

export const formatPrice = (price) =>
    new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);

const ProductView = ({ navigate }) => {
    // --- Khai báo các State quản lý dữ liệu ---
    const [products, setProducts] = useState([]);          // Mảng chứa danh sách sản phẩm hiển thị
    const [categories, setCategories] = useState([]);      // Mảng chứa danh mục [{id: 'all', name: 'Tất cả'}, {id: 1, name: 'Giày'}, ...]
    const [activeCategoryId, setActiveCategoryId] = useState('all'); // Lưu ID danh mục đang chọn ('all' hoặc số nguyên ID)

    // State quản lý phân trang
    const [pageNumber, setPageNumber] = useState(1);       // Trang hiện tại
    const [totalPages, setTotalPages] = useState(1);       // Tổng số trang do API tính toán trả về
    const [loading, setLoading] = useState(false);         // Trạng thái chờ tải dữ liệu

    const pageSize = 20; // Yêu cầu: Hiển thị tối đa 20 sản phẩm trên 1 trang

    // ==========================================
    // 1. GỌI API LẤY DANH MỤC SẢN PHẨM (Chạy 1 lần duy nhất khi load trang)
    // ==========================================
    useEffect(() => {
        categoryProductService.getAllCategoryProducts()
            .then(data => {
                // Tạo phần tử "Tất cả" cứng ở đầu mảng, sau đó rải (spread) dữ liệu danh mục thực tế từ API vào sau
                const dynamicCategories = [{ id: 'all', name: 'Tất cả' }, ...data];
                setCategories(dynamicCategories);
            })
            .catch(err => console.error("Lỗi khi tải danh mục từ API:", err));
    }, []);

    // ==========================================
    // 2. GỌI API LẤY SẢN PHẨM (Chạy lại khi ĐỔI TRANG hoặc ĐỔI DANH MỤC)
    // ==========================================
    useEffect(() => {
        setLoading(true);

        // Khai báo biến hứng dữ liệu phản hồi chung
        let apiCall;

        if (activeCategoryId === 'all') {
            // Nếu đang chọn danh mục "Tất cả" -> Gọi API lấy toàn bộ sản phẩm (có phân trang)
            apiCall = productService.getAllProducts(pageNumber, pageSize);
        } else {
            // Nếu chọn danh mục cụ thể (Giày, Áo, Quần...) -> Gọi API lọc theo Category ID (có phân trang)
            apiCall = productService.getProductsByCategory(activeCategoryId, pageNumber, pageSize);
        }

        // Tiến hành xử lý dữ liệu nhận về sau khi bóc tách qua AxiosClient
        apiCall
            .then(result => {
                // Do backend trả về cấu trúc phân trang: { totalItems, totalPages, pageNumber, pageSize, data: [...] }
                setProducts(result.data || []);
                setTotalPages(result.totalPages || 1);
                setLoading(false);
            })
            .catch(err => {
                console.error("Lỗi khi tải danh sách sản phẩm:", err);
                setProducts([]);
                setLoading(false);
            });
    }, [pageNumber, activeCategoryId]); // Lắng nghe sự thay đổi của cả số trang lẫn bộ lọc danh mục

    // ==========================================
    // 3. CÁC HÀM XỬ LÝ SỰ KIỆN (EVENT HANDLERS)
    // ==========================================

    // Hàm xử lý khi người dùng bấm chọn Danh mục
    const handleCategoryClick = (categoryId) => {
        setActiveCategoryId(categoryId);
        setPageNumber(1); // QUAN TRỌNG: Phải reset số trang về 1 khi đổi danh mục để tránh lỗi tràn trang
    };

    // Hàm xử lý chuyển trang điều hướng
    const handlePageChange = (newPage) => {
        if (newPage >= 1 && newPage <= totalPages) {
            setPageNumber(newPage);
            // Cuộn trang mượt mà lên vị trí lưới sản phẩm để khách hàng tiện theo dõi
            document.getElementById('products-sec').scrollIntoView({ behavior: 'smooth' });
        }
    };

    return (
        <div className="page-transition">
            {/* SECTION HERO */}
            <section className="hero">
                <div className="hero-bg">
                    <img src="src/assets/images/hero_basketball_1778727871576.png" alt="Hero" />
                </div>
                <div className="hero-content">
                    <span className="hero-tag">Bộ sưu tập mới 2026</span>
                    <h1 className="hero-title">ELEVATE YOUR <span>GAME</span></h1>
                    <p className="hero-desc">Trang bị những sản phẩm bóng rổ đỉnh cao nhất. Từ đôi giày hiệu năng cao đến trang phục chuyên nghiệp, Astra Hoops đồng hành cùng bạn trên mọi mặt sân.</p>
                    <button className="btn btn-primary" onClick={() => document.getElementById('products-sec').scrollIntoView({ behavior: 'smooth' })}>Mua Sắm Ngay</button>
                </div>
            </section>

            {/* SECTION DANH SÁCH SẢN PHẨM */}
            <section id="products-sec" className="products-section">
                <div className="section-header">
                    <div>
                        <h2 className="section-title">Sản phẩm</h2>
                    </div>

                    {/* Menu danh mục nút lọc */}
                    <div className="categories">
                        {categories.map(cat => (
                            <button
                                key={cat.id}
                                className={`category-btn ${activeCategoryId === cat.id ? 'active' : ''}`}
                                onClick={() => handleCategoryClick(cat.id)}
                            >
                                {cat.name}
                            </button>
                        ))}
                    </div>
                </div>

                {/* Khối hiển thị dữ liệu hoặc thông báo Loading */}
                {loading ? (
                    <div className="loading-text">Đang tải sản phẩm từ hệ thống...</div>
                ) : products.length === 0 ? (
                    <div className="loading-text">Danh mục này hiện tại chưa có sản phẩm nào.</div>
                ) : (
                    <>
                        {/* LƯỚI HIỂN THỊ CHUẨN 5 SẢN PHẨM TRÊN 1 HÀNG */}
                        <div className="products-grid-5-columns">
                            {products.map(product => {
                                // Xử lý gắn link domain Backend (https://localhost:7291) vào đường dẫn ảnh cục bộ (/uploads/xxx.png)
                                const processedProduct = {
                                    ...product,
                                    image: product.imageUrl.startsWith('http') ? product.imageUrl : `${BASE_URL}${product.imageUrl}`
                                };

                                return (
                                    <ProductCard
                                        key={product.id}
                                        product={processedProduct}
                                        navigate={navigate}
                                    />
                                );
                            })}
                        </div>

                        {/* THANH ĐIỀU HƯỚNG PHÂN TRANG (Chỉ hiển thị khi tổng số trang lớn hơn 1) */}
                        {totalPages > 1 && (
                            <div className="pagination-container">
                                <button
                                    className="page-btn"
                                    disabled={pageNumber === 1}
                                    onClick={() => handlePageChange(pageNumber - 1)}
                                >
                                    ❮ Trước
                                </button>

                                {/* Vòng lặp tự động render các số trang dựa trên totalPages */}
                                {Array.from({ length: totalPages }, (_, index) => (
                                    <button
                                        key={index + 1}
                                        className={`page-btn ${pageNumber === index + 1 ? 'active' : ''}`}
                                        onClick={() => handlePageChange(index + 1)}
                                    >
                                        {index + 1}
                                    </button>
                                ))}

                                <button
                                    className="page-btn"
                                    disabled={pageNumber === totalPages}
                                    onClick={() => handlePageChange(pageNumber + 1)}
                                >
                                    Sau ❯
                                </button>
                            </div>
                        )}
                    </>
                )}
            </section>
        </div>
    );
};

export default ProductView;