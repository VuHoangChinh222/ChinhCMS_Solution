import { useState } from 'react';
// import { productsData, formatPrice } from '../../data/products';

export const productsData = [
  { id: 1, name: 'Ignite Red X', category: 'Giày bóng rổ', price: 3500000, image: 'src/assets/images/shoe_product_1_1778727884422.png', badge: 'Mới', desc: 'Đôi giày bứt phá mọi giới hạn tốc độ. Thiết kế ôm sát cổ chân, đế đệm bật nảy cực cao, giúp bạn thực hiện những pha lên rổ hoàn hảo.' },
  { id: 2, name: 'Velocity HX-1 Neo', category: 'Giày bóng rổ', price: 4200000, image: 'src/assets/images/shoe_product_2_1778727899404.png', badge: 'Bán chạy', desc: 'Trang bị công nghệ viền đèn Neon ẩn, Velocity HX-1 mang đến vẻ ngoài đến từ tương lai cùng hiệu năng đỉnh cao. Chất liệu siêu nhẹ hỗ trợ bứt tốc.' },
  { id: 3, name: 'Nights Owl Jersey', category: 'Áo', price: 1200000, image: 'src/assets/images/shirt_product_1778727913549.png', badge: 'Limited', desc: 'Áo đấu phiên bản giới hạn "Nights Owl" với chất liệu siêu thoáng khí, công nghệ dệt 3D giúp thấm hút mồ hôi cực tốt trong các trận đấu căng thẳng.' },
  { id: 4, name: 'Elite Performance Shorts', category: 'Quần', price: 850000, image: 'src/assets/images/pants_product_1778727928285.png', badge: 'Hot', desc: 'Quần short siêu nhẹ, viền sọc cam đặc trưng. Form chuẩn dành cho những pha di chuyển mượt mà trên sân.' },
  { id: 5, name: 'Nike Classic Elite Socks', category: 'Vớ', price: 350000, image: 'src/assets/images/socks_product_1778727946646.png', desc: 'Vớ bóng rổ dày dặn, đệm lót ở gót và mũi chân hỗ trợ giảm chấn thương vùng mắt cá và bàn chân tối đa.' }
];

export const categories = ['Tất cả', 'Giày bóng rổ', 'Áo', 'Quần', 'Vớ'];

export const formatPrice = (price) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);


const ProductDetailView = ({ params, addToCart, navigate }) => {
  const product = productsData.find(p => p.id === params.id);
  const [size, setSize] = useState('M');
  const [qty, setQty] = useState(1);

  if (!product) return <div className="page-container page-transition">Không tìm thấy sản phẩm</div>;

  const sizes = product.category === 'Giày bóng rổ' ? ['40', '41', '42', '43', '44'] : 
                product.category === 'Vớ' ? ['Free'] : ['S', 'M', 'L', 'XL'];

  const handleAdd = () => {
    addToCart(product, size, qty);
    navigate('cart');
  };

  return (
    <div className="page-container page-transition">
      <div className="detail-grid">
        <div className="detail-img">
          <img src={product.image} alt={product.name} />
        </div>
        <div className="detail-info">
          <div className="product-category">{product.category}</div>
          <h1>{product.name}</h1>
          <div className="detail-price">{formatPrice(product.price)}</div>
          <p className="detail-desc">{product.desc}</p>
          
          <span className="detail-section-title">Kích cỡ / Size:</span>
          <div className="size-selector">
            {sizes.map(s => (
              <button key={s} className={`size-btn ${size === s ? 'active' : ''}`} onClick={() => setSize(s)}>{s}</button>
            ))}
          </div>
          
          <span className="detail-section-title">Số lượng:</span>
          <div className="add-cart-wrap">
            <input type="number" className="form-input qty-input" value={qty} min="1" onChange={(e)=>setQty(Math.max(1, parseInt(e.target.value)||1))} />
            <button className="btn btn-primary" style={{flex: 1}} onClick={handleAdd}>
              <i className="fa-solid fa-cart-plus" style={{marginRight: '10px'}}></i> Thêm vào giỏ
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetailView;
