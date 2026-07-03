import React, { useState, useEffect } from 'react';
import postService from '../../services/postService';
import '../../assets/css/CategoryMenu.css';
import { IMAGE_BASE_URL } from '../../config';

const BASE_URL = IMAGE_BASE_URL;

const PostCategoryMenu = ({ activeCategoryId, onSelectCategory }) => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoading(true);
        const data = await postService.getBlogCategories();
        
        // Chuyển đổi ID của danh mục "Tất cả bài viết" thành 'all' để Frontend xử lý chuẩn xác
        let dynamicCategories = (data || []).map(c => 
          c.name === 'Tất cả bài viết' ? { ...c, id: 'all' } : c
        );
        
        const hasAll = dynamicCategories.some(c => c.id === 'all');
        if (!hasAll) {
          dynamicCategories = [{ id: 'all', name: 'Tất cả bài viết' }, ...dynamicCategories];
        }
        
        setCategories(dynamicCategories);
      } catch (err) {
        console.error("Lỗi khi tải danh mục bài viết:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchCategories();
  }, []);

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: '2rem 0', color: 'var(--text-muted)' }}>
        <i className="fa-solid fa-spinner fa-spin"></i> Đang tải danh mục bài viết...
      </div>
    );
  }

  return (
    <div className="home-category-menu">
      {categories.map(cat => {
        // Mặc định ảnh cho Tất cả bài viết nếu chưa có ảnh
        const imageSrc = cat.imageUrl
          ? (cat.imageUrl.startsWith('http') ? cat.imageUrl : `${BASE_URL}${cat.imageUrl}`)
          : (cat.id === 'all' 
             ? 'src/assets/images/hero_basketball_1778727871576.png' 
             : 'src/assets/images/hero_basketball_1778727871576.png');

        return (
          <button
            key={cat.id}
            className={`home-category-item ${activeCategoryId === cat.id ? 'active' : ''}`}
            onClick={() => onSelectCategory(cat.id)}
          >
            <div className="home-category-img-container">
              <img src={imageSrc} alt={cat.name} className="home-category-img" />
            </div>
            <span className="home-category-name">{cat.name}</span>
          </button>
        );
      })}
    </div>
  );
};

export default PostCategoryMenu;
