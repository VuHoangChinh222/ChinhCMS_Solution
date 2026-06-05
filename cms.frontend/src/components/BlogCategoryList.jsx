/* 
 * BLOGCATEGORYLIST COMPONENT - DYNAMIC DATABASE API CATEGORIES
 * Sinh viên: Vũ Hoàng Chính
 * Môn học: Chuyên đề ASP.NET Core & ReactJS
 */

import React, { useState, useEffect } from 'react';
import postService from '../services/postService';
import '../assets/css/BlogCategoryList.css';

const BlogCategoryList = ({ activeCategoryId, onSelectCategory }) => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);

  // 1. Tải danh sách chuyên mục bài viết từ Database API qua postService
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoading(true);
        const data = await postService.getBlogCategories();
        // Thêm lựa chọn mặc định "Tất cả chủ đề" vào đầu danh sách
        setCategories([{ id: 'all', name: 'Tất cả chủ đề' }, ...(data || [])]);
      } catch (err) {
        console.error("Lỗi khi tải chuyên mục bài viết:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchCategories();
  }, []);

  if (loading) {
    return (
      <div className="blog-category-loading">
        <i className="fa-solid fa-spinner fa-spin" style={{ marginRight: '6px' }}></i> Đang nạp chủ đề...
      </div>
    );
  }

  return (
    <div className="blog-category-card">
      <h5 className="blog-category-title">
        <i className="fa-solid fa-tags"></i> Chủ đề bài viết
      </h5>
      <div className="blog-category-list">
        {categories.map(cat => (
          <button
            key={cat.id}
            className={`blog-category-item ${activeCategoryId === cat.id ? 'active' : ''}`}
            onClick={() => onSelectCategory(cat.id)}
          >
            <span>
              <i className="fa-solid" style={{ marginRight: '6px', opacity: 0.6 }}></i>
              {cat.name}
            </span>
            <span className="blog-category-badge">Đọc</span>
          </button>
        ))}
      </div>
    </div>
  );
};

export default BlogCategoryList;
