import React from 'react';

const PostCard = ({ post, navigate }) => {
    // Hàm điều hướng bao quanh thẻ khi click vào vùng trống của Card
    const handleDetail = (e) => {
        if (e.target.tagName !== 'BUTTON' && e.target.tagName !== 'I') {
            // CHỈNH SỬA TẠI ĐÂY: Khớp chuẩn cấu trúc navigate của App.jsx
            navigate('postDetail', { id: post.id });
        }
    };

    return (
        <div className="product-card post-card-sync" onClick={handleDetail}>
            {/* Phần hình ảnh bài viết */}
            <div className="product-img">
                <img src={post.image} alt={post.title} />
                <div className="product-action">
                    {/* CHỈNH SỬA TẠI ĐÂY: Sửa nút bấm Xem chi tiết cho đồng bộ */}
                    <button onClick={(e) => { e.stopPropagation(); navigate('postDetail', { id: post.id }); }}>
                        <i className="fa-solid fa-eye"></i> Xem chi tiết
                    </button>
                </div>
            </div>

            {/* Phần thông tin chữ (giữ nguyên giao diện Dark Mode đồng bộ) */}
            <div className="product-info">
                <div className="product-category">{post.categoryName || 'Xu hướng'}</div>

                <h3 className="product-name" style={{
                    display: '-webkit-box',
                    WebkitLineClamp: 2,
                    WebkitBoxOrient: 'vertical',
                    overflow: 'hidden',
                    height: '44px',
                    lineHeight: '1.4',
                    textTransform: 'none',
                    letterSpacing: 'normal',
                    margin: '8px 0'
                }}>
                    {post.title}
                </h3>

                <div className="product-date" style={{ color: 'var(--text-muted)', fontSize: '0.85rem', marginTop: 'auto', display: 'flex', alignItems: 'center', gap: '4px' }}>
                    📅 {post.createdDate ? new Date(post.createdDate).toLocaleDateString('vi-VN') : '26/05/2026'}
                </div>
            </div>
        </div>
    );
};

export default PostCard;