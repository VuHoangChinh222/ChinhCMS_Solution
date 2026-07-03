/* 
 * HOMEVIEW COMPONENT - AUTOMATED DATABASE & API INTEGRATION (HOMEPAGE VIEW)
 * Sinh viên: Vũ Hoàng Chính
 * Môn học: Chuyên đề ASP.NET Core & ReactJS
 */

import { useState, useEffect } from 'react';
import HeroBanner from '../../components/HeroBanner';
import productService from '../../services/productService';
import postService from '../../services/postService';
import FeaturedProducts from './FeaturedProducts';
import LatestBlogs from './LatestBlogs';
import CategoryMenu from './CategoryMenu';
import PostCategoryMenu from './PostCategoryMenu';

// Import các file CSS cần thiết
import '../../assets/css/HomeView.css';

const HomeView = ({ navigate, addToCart }) => {
  const [newestProducts, setNewestProducts] = useState([]);
  const [bestSellers, setBestSellers] = useState([]);
  const [latestPosts, setLatestPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activeCategoryId, setActiveCategoryId] = useState('all');
  const [activePostCategoryId, setActivePostCategoryId] = useState('all');

  // 1. Tải dữ liệu bài viết (chạy lại mỗi khi chọn danh mục bài viết khác)
  useEffect(() => {
    const loadPosts = async () => {
      try {
        let postRes;
        if (activePostCategoryId === 'all') {
          postRes = await postService.getLatestPosts(1, 5);
        } else {
          postRes = await postService.getPostsByCategory(activePostCategoryId, 1, 5);
        }

        const postsArray = postRes?.data || postRes?.Data || postRes;
        if (postsArray && Array.isArray(postsArray)) {
          setLatestPosts(postsArray.slice(0, 5));
        } else {
          setLatestPosts([]);
        }
      } catch (err) {
        console.error("Lỗi khi đồng bộ bài viết trang chủ:", err);
      }
    };

    loadPosts();
  }, [activePostCategoryId]);

  // 2. Tải sản phẩm (chạy lại mỗi khi chọn danh mục khác)
  useEffect(() => {
    const loadProducts = async () => {
      try {
        setLoading(true);

        // Tải top 5 sản phẩm mới nhất theo danh mục
        const newestRes = await productService.getNewestProducts(activeCategoryId);
        if (newestRes && Array.isArray(newestRes)) {
          setNewestProducts(newestRes.slice(0, 5));
        } else {
          setNewestProducts([]);
        }

        // Tải top 5 sản phẩm bán chạy nhất theo danh mục
        const sellerRes = await productService.getBestSellers(activeCategoryId);
        if (sellerRes && Array.isArray(sellerRes)) {
          setBestSellers(sellerRes.slice(0, 5));
        } else {
          setBestSellers([]);
        }

      } catch (err) {
        console.error("Lỗi khi đồng bộ sản phẩm trang chủ từ CSDL:", err);
      } finally {
        setLoading(false);
      }
    };

    loadProducts();
  }, [activeCategoryId]);

  return (
    <div className="page-transition">
      {/* SECTION 1: HERO BANNER */}
      <HeroBanner
        tag="Bộ sưu tập mới 2026"
        title={<>ELEVATE YOUR <span>GAME</span></>}
        desc="Trang bị những sản phẩm bóng rổ đỉnh cao nhất. Từ đôi giày hiệu năng cao đến trang phục chuyên nghiệp, Chinh Hoops đồng hành cùng bạn trên mọi mặt sân."
        image="src/assets/images/hero_basketball_1778727871576.png"
        buttonText="Mua Sắm Ngay"
        onButtonClick={() => navigate('products')}
      />

      {/* SECTION 2: DANH MỤC SẢN PHẨM */}
      <section style={{ padding: '0 4%' }}>
        <CategoryMenu
          activeCategoryId={activeCategoryId}
          onSelectCategory={(id) => setActiveCategoryId(id)}
        />
      </section>

      {/* SECTION 3: SẢN PHẨM NỔI BẬT & BÁN CHẠY */}
      <FeaturedProducts
        loading={loading}
        newestProducts={newestProducts}
        bestSellers={bestSellers}
        navigate={navigate}
        addToCart={addToCart}
      />

      {/* SECTION 4: DANH MỤC BÀI VIẾT */}
      <section style={{ padding: '2rem 4% 0' }}>
        <PostCategoryMenu
          activeCategoryId={activePostCategoryId}
          onSelectCategory={(id) => setActivePostCategoryId(id)}
        />
      </section>

      {/* SECTION 5: BẢNG TIN XU HƯỚNG THỜI TRANG (TOP 5 LATEST BLOGS) */}
      <LatestBlogs
        loading={loading}
        latestPosts={latestPosts}
        navigate={navigate}
      />
    </div>
  );
};

export default HomeView;
