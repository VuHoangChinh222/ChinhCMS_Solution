const CheckoutView = ({ navigate }) => {
  const handleSubmit = (e) => {
    e.preventDefault();
    navigate('payment');
  };
  
  return (
    <div className="page-container page-transition">
      <h2 className="page-title">Thông tin <span>Giao hàng</span></h2>
      <div className="form-card">
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Họ và tên</label>
            <input type="text" className="form-input" required placeholder="Nhập họ tên của bạn" />
          </div>
          <div className="form-group">
            <label>Số điện thoại</label>
            <input type="tel" className="form-input" required placeholder="Nhập số điện thoại" />
          </div>
          <div className="form-group">
            <label>Địa chỉ giao hàng</label>
            <input type="text" className="form-input" required placeholder="Số nhà, tên đường, phường/xã, quận/huyện, tỉnh/thành phố" />
          </div>
          <div className="form-group">
            <label>Ghi chú (Tùy chọn)</label>
            <textarea className="form-input" rows="3" placeholder="Ghi chú thêm về đơn hàng..."></textarea>
          </div>
          <div style={{display: 'flex', gap: '1rem', marginTop: '2rem'}}>
            <button type="button" className="btn btn-outline" style={{flex: 1}} onClick={() => navigate('cart')}>Quay lại</button>
            <button type="submit" className="btn btn-primary" style={{flex: 1}}>Chọn phương thức thanh toán</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CheckoutView;
