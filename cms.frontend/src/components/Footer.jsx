/* 
 * FOOTER COMPONENT - HIGH-END DYNAMIC FOOTER
 * Sinh viên: Vũ Hoàng Chính
 * Môn học: Chuyên đề ASP.NET Core & ReactJS
 */

import React, { useState, useEffect } from 'react';
import categoryProductService from '../services/categoryProductService';

const Footer = ({ navigate }) => {
  const [categories, setCategories] = useState([]);

  useEffect(() => {
    categoryProductService.getAllCategoryProducts()
      .then(data => {
        if (data && Array.isArray(data)) {
          // Lọc bỏ chuyên mục mặc định "Tất cả sản phẩm" (ID: 7 hoặc Tên: "Tất cả sản phẩm")
          const filtered = data.filter(c => c.id !== 7 && c.name !== 'Tất cả sản phẩm');
          setCategories(filtered);
        }
      })
      .catch(err => console.error("Lỗi tải danh mục sản phẩm chân trang:", err));
  }, []);

  return (
    <footer>
      <div className="footer-content">
        <div className="footer-col">
          <div className="logo" style={{ marginBottom: '1rem', fontSize: '1.5rem' }} onClick={() => navigate('home')}>
            <i className="fa-solid fa-basketball"></i> ASTRA <span>HOOPS</span>
          </div>
          <p>Nâng tầm đam mê bóng rổ của bạn với những trang bị chất lượng hàng đầu. Chúng tôi cung cấp những sản phẩm chính hãng tốt nhất.</p>
          <div className="social-links">
            <a href="#"><i className="fa-brands fa-facebook-f"></i></a>
            <a href="#"><i className="fa-brands fa-instagram"></i></a>
            <a href="#"><i className="fa-brands fa-tiktok"></i></a>
          </div>
        </div>
        <div className="footer-col">
          <h3>Danh mục</h3>
          <div className="footer-links">
            {categories.length > 0 ? (
              categories.map(cat => (
                <button
                  key={cat.id}
                  onClick={() => navigate('products', { categoryId: cat.id })}
                >
                  {cat.name}
                </button>
              ))
            ) : (
              <>
                <button onClick={() => navigate('products')}>Sản phẩm</button>
              </>
            )}
          </div>
        </div>
        <div className="footer-col">
          <h3>Hỗ trợ</h3>
          <div className="footer-links">
            <button onClick={() => navigate('about')}>Về chúng tôi</button>
            <a href="#">Chính sách đổi trả</a>
            <a href="#">Hướng dẫn chọn size</a>
          </div>
        </div>
        <div className="footer-col">
          <h3>Liên hệ</h3>
          <div className="footer-links">
            <p><i className="fa-solid fa-location-dot" style={{ color: 'var(--accent)', marginRight: '10px' }}></i> 123 Đường Cầu Giấy, Hà Nội</p>
            <p><i className="fa-solid fa-phone" style={{ color: 'var(--accent)', marginRight: '10px' }}></i> 0123.456.789</p>
            <p><i className="fa-solid fa-envelope" style={{ color: 'var(--accent)', marginRight: '10px' }}></i> support@astrahoops.vn</p>
          </div>
        </div>
      </div>
      <div className="footer-bottom">
        <p>&copy; 2026 Astra Hoops. Tất cả quyền được bảo lưu.</p>
      </div>
    </footer>
  );
};

export default Footer;
