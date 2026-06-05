import { useState } from 'react';

const Header = ({ currentView, navigate, cartCount }) => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const handleNav = (view) => {
    navigate(view);
    setIsMobileMenuOpen(false);
  };

  return (
    <header className="header">
      <div className="logo" onClick={() => handleNav('home')}>
        <i className="fa-solid fa-basketball"></i> ASTRA <span>HOOPS</span>
      </div>
      <ul className={`nav-links ${isMobileMenuOpen ? 'mobile-open' : ''}`}>
        <li><button className={currentView.name === 'home' ? 'active' : ''} onClick={() => handleNav('home')}>Trang chủ</button></li>
        <li><button className={currentView.name === 'products' ? 'active' : ''} onClick={() => handleNav('products')}>Sản phẩm</button></li>
        <li><button className={currentView.name === 'blog' ? 'active' : ''} onClick={() => handleNav('blog')}>Bài viết</button></li>
        <li><button className={currentView.name === 'about' ? 'active' : ''} onClick={() => handleNav('about')}>Về chúng tôi</button></li>
      </ul>
      <div className="header-actions">
        <button className="action-btn" onClick={() => handleNav('search')} title="Tìm kiếm">
          <i className="fa-solid fa-magnifying-glass"></i>
        </button>
        <button className="action-btn" onClick={() => handleNav('user')} title="Tài khoản">
          <i className="fa-solid fa-user"></i>
        </button>
        <button className="action-btn" onClick={() => handleNav('cart')} title="Giỏ hàng">
          <i className="fa-solid fa-bag-shopping"></i>
          {cartCount > 0 && <span className="cart-count">{cartCount}</span>}
        </button>
        <button className="mobile-menu-btn" onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}>
          <i className={`fa-solid ${isMobileMenuOpen ? 'fa-xmark' : 'fa-bars'}`}></i>
        </button>
      </div>
    </header>
  );
};

export default Header;
