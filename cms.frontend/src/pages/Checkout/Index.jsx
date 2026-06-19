/* 
 * CHECKOUTVIEW COMPONENT - SECURE TRANSACTION & DATABASE INTEGRATION
 * Sinh viên: Vũ Hoàng Chính
 * Môn học: Chuyên đề ASP.NET Core & ReactJS
 */

import { useState, useEffect } from 'react';
import { getCookie } from '../../utils/cookieHelper';
import orderService from '../../services/orderService';
import '../../assets/css/CheckoutView.css';

const CheckoutView = ({ cart, clearCart, navigate }) => {
  const [customer, setCustomer] = useState(null);

  // States cho Form
  const [fullName, setFullName] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [notes, setNotes] = useState('');

  // Trạng thái xử lý
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  // 1. Kiểm tra đăng nhập bảo mật & nạp dữ liệu từ Cookie
  useEffect(() => {
    const loggedCustomer = getCookie('customer');
    if (!loggedCustomer) {
      alert("Hệ thống bảo mật: Bạn phải đăng nhập để tiến hành đặt hàng!");
      navigate('login');
      return;
    }
    setCustomer(loggedCustomer);

    // Tự động điền (Pre-populate) thông tin khách hàng từ Cookie
    setFullName(loggedCustomer.fullName || '');
    setPhone(loggedCustomer.phone || '');
    setAddress(loggedCustomer.address || '');
  }, [navigate]);

  // 2. Xử lý lưu thông tin giao hàng tạm thời và chuyển đến trang Thanh toán
  const handleSubmit = (e) => {
    e.preventDefault();
    setErrorMessage('');

    if (cart.length === 0) {
      setErrorMessage("Giỏ hàng của bạn đang trống!");
      return;
    }

    if (!fullName.trim()) {
      setErrorMessage("Họ và tên nhận hàng không được để trống.");
      return;
    }

    if (!phone.trim()) {
      setErrorMessage("Số điện thoại liên hệ không được để trống.");
      return;
    }

    if (!/^(0[3|5|7|8|9])+([0-9]{8})$/.test(phone.trim())) {
      setErrorMessage("Số điện thoại Việt Nam không hợp lệ (phải bắt đầu bằng 03, 05, 07, 08, 09 và gồm 10 chữ số).");
      return;
    }

    if (!address.trim()) {
      setErrorMessage("Địa chỉ nhận hàng không được để trống.");
      return;
    }

    // Lưu thông tin giao hàng tạm thời vào sessionStorage để trang Thanh toán sử dụng khi bấm xác nhận
    sessionStorage.setItem('checkout_shipping_info', JSON.stringify({
      fullName: fullName.trim(),
      phone: phone.trim(),
      address: address.trim(),
      notes: notes.trim()
    }));

    navigate('payment');
  };

  if (!customer) return null;

  return (
    <div className="page-container page-transition">
      <h2 className="page-title">Thông tin <span>Thanh toán & Đơn hàng</span></h2>

      <div className="checkout-container" style={{
        display: 'grid',
        gridTemplateColumns: '1fr',
        gap: '2rem',
        marginTop: '2rem'
      }}>
        {/* Responsive Grid for Desktop */}
        <style>{`
          @media (min-width: 992px) {
            .checkout-container {
              grid-template-columns: 1.2fr 1fr !important;
            }
          }
        `}</style>

        {/* Left Column: Shipping Form */}
        <div className="form-card checkout-form-card" style={{ margin: 0, width: '100%' }}>
          {errorMessage && (
            <div className="checkout-error-alert">
              <i className="fa-solid fa-triangle-exclamation checkout-error-icon"></i> {errorMessage}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Họ và tên nhận hàng <span className="checkout-required-star">*</span></label>
              <input
                type="text"
                className="form-input"
                required
                placeholder="Nhập họ tên của bạn"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
              />
            </div>

            <div className="form-group">
              <label>Số điện thoại liên hệ <span className="checkout-required-star">*</span></label>
              <input
                type="tel"
                required
                className="form-input"
                placeholder="Nhập số điện thoại"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
              />
            </div>

            <div className="form-group">
              <label>Địa chỉ nhận hàng <span className="checkout-required-star">*</span></label>
              <input
                type="text"
                required
                className="form-input"
                placeholder="Số nhà, tên đường, phường/xã, quận/huyện, tỉnh/thành phố"
                value={address}
                onChange={(e) => setAddress(e.target.value)}
              />
            </div>

            <div className="form-group">
              <label>Ghi chú đơn hàng (Tùy chọn)</label>
              <textarea
                className="form-input"
                rows="3"
                placeholder="Ghi chú về thời gian giao hàng, lời nhắn..."
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
              ></textarea>
            </div>

            <div className="checkout-actions-row">
              <button type="button" className="btn btn-outline checkout-action-btn" onClick={() => navigate('cart')} disabled={loading}>
                Quay lại giỏ hàng
              </button>
              <button type="submit" className="btn btn-primary checkout-action-btn" disabled={loading}>
                {loading ? (
                  <><i className="fa-solid fa-spinner fa-spin checkout-spinner-icon"></i> Đang xử lý...</>
                ) : (
                  'Xác nhận đặt hàng'
                )}
              </button>
            </div>
          </form>
        </div>

        {/* Right Column: Order Summary */}
        <div className="form-card checkout-summary-card" style={{
          margin: 0,
          padding: '1.5rem',
          backgroundColor: 'var(--bg-card)',
          borderRadius: '12px',
          border: '1px solid var(--border-color)',
          display: 'flex',
          flexDirection: 'column',
          height: 'fit-content'
        }}>
          <h3 style={{
            marginBottom: '1.5rem',
            borderBottom: '1px solid var(--border-color)',
            paddingBottom: '0.8rem',
            fontFamily: 'var(--font-heading)',
            color: 'var(--text-main)',
            fontSize: '1.25rem',
            fontWeight: '600'
          }}>
            Tóm tắt đơn hàng ({cart.reduce((sum, item) => sum + (parseInt(item.qty) || 0), 0)} sản phẩm)
          </h3>

          <div className="checkout-items-list" style={{
            display: 'flex',
            flexDirection: 'column',
            gap: '1rem',
            marginBottom: '1.5rem',
            maxHeight: '350px',
            overflowY: 'auto',
            paddingRight: '5px'
          }}>
            {cart.map(item => (
              <div key={item.cartId} style={{
                display: 'flex',
                gap: '1rem',
                alignItems: 'center',
                justifyContent: 'space-between',
                borderBottom: '1px dashed var(--border-color)',
                paddingBottom: '0.8rem'
              }}>
                <div style={{ display: 'flex', gap: '0.8rem', alignItems: 'center' }}>
                  <img
                    src={item.image}
                    alt={item.name}
                    style={{
                      width: '60px',
                      height: '60px',
                      objectFit: 'cover',
                      borderRadius: '6px',
                      border: '1px solid var(--border-color)'
                    }}
                  />
                  <div>
                    <h4 style={{ margin: 0, fontSize: '0.95rem', fontWeight: '600', color: 'var(--text-main)' }}>{item.name}</h4>
                    <span style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>
                      Size: {item.size} &nbsp;&bull;&nbsp; Số lượng: {item.qty}
                    </span>
                  </div>
                </div>
                <div style={{ fontWeight: 'bold', color: 'var(--accent)', fontSize: '0.95rem' }}>
                  {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(item.price * (parseInt(item.qty) || 0))}
                </div>
              </div>
            ))}
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.8rem', borderTop: '1px solid var(--border-color)', paddingTop: '1rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem', color: 'var(--text-muted)' }}>
              <span>Tạm tính</span>
              <span style={{ color: 'var(--text-main)' }}>
                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(cart.reduce((sum, item) => sum + item.price * (parseInt(item.qty) || 0), 0))}
              </span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem', color: 'var(--text-muted)' }}>
              <span>Phí vận chuyển</span>
              <span style={{ color: '#22c55e', fontWeight: '500' }}>Miễn phí</span>
            </div>
            <div style={{
              display: 'flex',
              justifyContent: 'space-between',
              fontSize: '1.15rem',
              fontWeight: 'bold',
              borderTop: '1px solid var(--border-color)',
              paddingTop: '0.8rem',
              marginTop: '0.5rem',
              color: 'var(--text-main)'
            }}>
              <span>Tổng thanh toán</span>
              <span style={{ color: 'var(--accent)' }}>
                {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(cart.reduce((sum, item) => sum + item.price * (parseInt(item.qty) || 0), 0))}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CheckoutView;
