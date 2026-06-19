import { useState, useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import productService from '../services/productService';
import postService from '../services/postService';
import '../assets/css/headerCSS/Header.css';

const BASE_URL = "https://localhost:7291";

const Header = ({ currentView, cartCount }) => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [results, setResults] = useState({ products: [], posts: [] });
  const [isLoading, setIsLoading] = useState(false);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);

  const searchRef = useRef(null);

  // Debounced API call for autocomplete search
  useEffect(() => {
    if (!searchQuery.trim()) {
      setResults({ products: [], posts: [] });
      return;
    }

    const delayDebounceFn = setTimeout(() => {
      setIsLoading(true);
      Promise.all([
        productService.getAllProducts(1, 5, searchQuery),
        postService.getLatestPosts(1, 5, searchQuery)
      ])
        .then(([productsRes, postsRes]) => {
          setResults({
            products: productsRes?.data || productsRes?.Data || [],
            posts: postsRes?.data || postsRes?.Data || []
          });
          setIsLoading(false);
        })
        .catch(err => {
          console.error("Lỗi khi tìm kiếm:", err);
          setIsLoading(false);
        });
    }, 300);

    return () => clearTimeout(delayDebounceFn);
  }, [searchQuery]);

  // Click outside to close dropdown
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (searchRef.current && !searchRef.current.contains(event.target)) {
        setIsDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  const handleSearchChange = (e) => {
    setSearchQuery(e.target.value);
    setIsDropdownOpen(true);
  };

  const clearSearch = () => {
    setSearchQuery('');
    setResults({ products: [], posts: [] });
  };

  const handleResultClick = () => {
    setIsDropdownOpen(false);
    setSearchQuery('');
    setIsMobileMenuOpen(false);
  };

  const processImage = (imageUrl) => {
    if (!imageUrl) return '';
    return imageUrl.startsWith('http') ? imageUrl : `${BASE_URL}${imageUrl}`;
  };

  return (
    <header className="header">
      {/* Top Row: Logo, Search Bar, actions */}
      <div className="header-main-row">
        <Link to="/" className="logo" onClick={() => setIsMobileMenuOpen(false)} style={{ textDecoration: 'none', color: 'inherit' }}>
          <i className="fa-solid fa-basketball"></i> CHINH <span>HOOPS</span>
        </Link>

        {/* Centered Search Bar */}
        <div className="header-search-container" ref={searchRef}>
          <div className="header-search-wrapper">
            <input
              type="text"
              placeholder="Tìm kiếm sản phẩm, bài viết..."
              value={searchQuery}
              onChange={handleSearchChange}
              onFocus={() => setIsDropdownOpen(true)}
              className="header-search-input"
            />
            <i className="fa-solid fa-magnifying-glass search-icon"></i>
            {searchQuery && (
              <button className="search-clear-btn" onClick={clearSearch}>
                <i className="fa-solid fa-xmark"></i>
              </button>
            )}
          </div>

          {/* Autocomplete Dropdown */}
          {isDropdownOpen && (searchQuery.trim().length > 0 || isLoading) && (
            <div className="search-results-dropdown">
              {isLoading ? (
                <div className="search-dropdown-loading">
                  <i className="fa-solid fa-spinner fa-spin"></i> Đang tìm kiếm...
                </div>
              ) : (
                <div className="search-dropdown-content">
                  {/* Products Section */}
                  <div className="search-section">
                    <h4 className="search-section-title">
                      <i className="fa-solid fa-bag-shopping"></i> Sản phẩm ({results.products.length})
                    </h4>
                    {results.products.length > 0 ? (
                      <ul className="search-items-list">
                        {results.products.map(product => (
                          <li key={product.id} className="search-item">
                            <Link to={`/product/${product.slug || product.id}`} onClick={handleResultClick} className="search-item-link">
                              <img src={processImage(product.imageUrl)} alt={product.name} className="search-item-img" />
                              <div className="search-item-info">
                                <span className="search-item-name">{product.name}</span>
                                <span className="search-item-price">{product.price?.toLocaleString('vi-VN')} đ</span>
                              </div>
                            </Link>
                          </li>
                        ))}
                      </ul>
                    ) : (
                      <p className="no-results-text">Không tìm thấy sản phẩm nào</p>
                    )}
                  </div>

                  {/* Vertical Divider */}
                  <div className="search-dropdown-divider"></div>

                  {/* Posts Section */}
                  <div className="search-section">
                    <h4 className="search-section-title">
                      <i className="fa-regular fa-newspaper"></i> Bài viết ({results.posts.length})
                    </h4>
                    {results.posts.length > 0 ? (
                      <ul className="search-items-list">
                        {results.posts.map(post => (
                          <li key={post.id} className="search-item">
                            <Link to={`/blog/${post.slug || post.id}`} onClick={handleResultClick} className="search-item-link">
                              <img src={processImage(post.imageUrl)} alt={post.title} className="search-item-img" />
                              <div className="search-item-info">
                                <span className="search-item-name">{post.title}</span>
                                <span className="search-item-category">{post.categoryName}</span>
                              </div>
                            </Link>
                          </li>
                        ))}
                      </ul>
                    ) : (
                      <p className="no-results-text">Không tìm thấy bài viết nào</p>
                    )}
                  </div>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Actions (Account, Cart, Menu) */}
        <div className="header-actions">
          <Link className="action-btn" to="/user" title="Tài khoản" onClick={() => setIsMobileMenuOpen(false)}>
            <i className="fa-solid fa-user"></i>
          </Link>
          <Link className="action-btn" to="/cart" title="Giỏ hàng" onClick={() => setIsMobileMenuOpen(false)}>
            <i className="fa-solid fa-bag-shopping"></i>
            {cartCount > 0 && <span className="cart-count">{cartCount}</span>}
          </Link>
          <button className="mobile-menu-btn" onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}>
            <i className={`fa-solid ${isMobileMenuOpen ? 'fa-xmark' : 'fa-bars'}`}></i>
          </button>
        </div>
      </div>

      {/* Bottom Row: Centered Navigation Links */}
      <ul className={`nav-links ${isMobileMenuOpen ? 'mobile-open' : ''}`}>
        <li><Link className={currentView.name === 'home' ? 'active' : ''} to="/" onClick={() => setIsMobileMenuOpen(false)}>Trang chủ</Link></li>
        <li><Link className={currentView.name === 'products' ? 'active' : ''} to="/products" onClick={() => setIsMobileMenuOpen(false)}>Sản phẩm</Link></li>
        <li><Link className={currentView.name === 'blog' ? 'active' : ''} to="/blog" onClick={() => setIsMobileMenuOpen(false)}>Bài viết</Link></li>
        <li><Link className={currentView.name === 'about' ? 'active' : ''} to="/about" onClick={() => setIsMobileMenuOpen(false)}>Về chúng tôi</Link></li>
      </ul>
    </header>
  );
};

export default Header;
