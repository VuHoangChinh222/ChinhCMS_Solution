import React from 'react';
import '../assets/css/CheckoutLoadingOverlay.css';

const CheckoutLoadingOverlay = ({ isVisible }) => {
  if (!isVisible) return null;

  return (
    <div className="checkout-loading-overlay">
      <div className="checkout-loading-spinner"></div>
      <h3 className="checkout-loading-title">ĐANG XỬ LÝ ĐƠN HÀNG</h3>
      <p className="checkout-loading-desc">
        Hệ thống đang mã hóa và xử lý yêu cầu của bạn.<br />
        Vui lòng không tải lại hoặc đóng trang web trong lúc này.
      </p>
    </div>
  );
};

export default CheckoutLoadingOverlay;
