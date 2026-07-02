import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import customerService from '../../services/customerService';
import { setCookie } from '../../utils/cookieHelper';
import '../../assets/css/UserInfoView.css';
import '../../assets/css/CompleteGoogleProfile.css';

const CompleteGoogleProfile = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const draftData = location.state?.draftData;

  useEffect(() => {
    // Nếu không có draftData (truy cập URL trực tiếp thay vì redirect từ login google), đẩy về login
    if (!draftData) {
      navigate('/login');
    }
  }, [draftData, navigate]);

  const [fullName, setFullName] = useState(draftData?.fullName || '');
  const [email] = useState(draftData?.email || '');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMsg('');

    if (!fullName.trim()) {
      setErrorMsg("Họ và tên không được để trống!");
      return;
    }

    if (!phone.trim()) {
      setErrorMsg("Vui lòng nhập số điện thoại để liên hệ giao hàng.");
      return;
    }

    if (!/^(0[3|5|7|8|9])+([0-9]{8})$/.test(phone.trim())) {
      setErrorMsg("Số điện thoại Việt Nam không đúng định dạng (VD: 0393807472)!");
      return;
    }

    if (!address.trim()) {
      setErrorMsg("Vui lòng nhập địa chỉ nhận hàng.");
      return;
    }

    if (!password || password.length < 6) {
      setErrorMsg("Vui lòng nhập mật khẩu (tối thiểu 6 ký tự) để đăng nhập hệ thống ở các lần sau.");
      return;
    }

    if (password !== confirmPassword) {
      setErrorMsg("Mật khẩu xác nhận không khớp!");
      return;
    }

    setLoading(true);
    try {
      const response = await customerService.registerGoogle({
        fullName: fullName.trim(),
        email: email,
        phone: phone.trim(),
        address: address.trim(),
        password: password
      });

      if (response && response.customer) {
        setCookie('customer', response.customer, 2);
        alert("Hoàn tất tạo tài khoản thành công!");
        navigate('/products');
        window.location.reload();
      }
    } catch (err) {
      console.error("Lỗi hoàn tất đăng ký:", err);
      setErrorMsg(err.response?.data?.message || "Đã xảy ra lỗi hệ thống, vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };

  if (!draftData) return null;

  return (
    <div className="page-container page-transition">
      <h2 className="page-title">Hoàn tất <span>Thông tin</span></h2>
      <p className="cgp-subtitle">
        Chào mừng bạn đến với ChinhCMS. Để hoàn tất việc tạo tài khoản qua Google, vui lòng bổ sung các thông tin còn thiếu.
      </p>

      <div className="user-info-card" style={{ maxWidth: '600px', margin: '0 auto' }}>
        <form onSubmit={handleSubmit}>
          {errorMsg && (
            <div className="cgp-error-message">
              <i className="fa-solid fa-circle-exclamation"></i> {errorMsg}
            </div>
          )}

          <div className="user-info-form-group">
            <label>Địa chỉ Email (Xác thực từ Google)</label>
            <input
              type="email"
              className="user-info-input"
              value={email}
              disabled
            />
          </div>

          <div className="user-info-form-group">
            <label>Họ và tên <span>*</span></label>
            <input
              type="text"
              className="user-info-input"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              placeholder="Nhập họ và tên đầy đủ"
              required
            />
          </div>

          <div className="user-info-form-group">
            <label>Số điện thoại liên hệ <span>*</span></label>
            <input
              type="tel"
              className="user-info-input"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              placeholder="Nhập số điện thoại (10 số)"
              required
            />
          </div>

          <div className="user-info-form-group">
            <label>Địa chỉ nhận hàng <span>*</span></label>
            <input
              type="text"
              className="user-info-input"
              value={address}
              onChange={(e) => setAddress(e.target.value)}
              placeholder="Nhập số nhà, tên đường, quận/huyện, tỉnh/thành phố"
              required
            />
          </div>

          <div className="user-info-form-group cgp-password-group">
            <label>Tạo mật khẩu <span>*</span> <small className="cgp-password-note">(Để có thể đăng nhập trực tiếp bằng email vào lần sau)</small></label>
            <div style={{ position: 'relative' }}>
              <input
                type={showPassword ? 'text' : 'password'}
                className="user-info-input cgp-password-input"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Tối thiểu 6 ký tự"
                required
              />
              <button
                type="button"
                className="cgp-password-toggle-btn"
                onClick={() => setShowPassword(!showPassword)}
              >
                <i className={`fa-solid ${showPassword ? 'fa-eye-slash' : 'fa-eye'}`}></i>
              </button>
            </div>
          </div>

          <div className="user-info-form-group">
            <label>Xác nhận mật khẩu <span>*</span></label>
            <div style={{ position: 'relative' }}>
              <input
                type={showConfirmPassword ? 'text' : 'password'}
                className="user-info-input cgp-password-input"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                placeholder="Nhập lại mật khẩu để xác nhận"
                required
              />
              <button
                type="button"
                className="cgp-password-toggle-btn"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
              >
                <i className={`fa-solid ${showConfirmPassword ? 'fa-eye-slash' : 'fa-eye'}`}></i>
              </button>
            </div>
          </div>

          <div className="user-info-btn-row" style={{ marginTop: '2rem' }}>
            <button
              type="submit"
              className="user-info-btn-save cgp-btn-submit"
              disabled={loading}
            >
              {loading ? (
                <>
                  <i className="fa-solid fa-spinner fa-spin"></i> Đang xử lý...
                </>
              ) : (
                <>
                  <i className="fa-solid fa-check"></i> Hoàn tất & Đăng nhập
                </>
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CompleteGoogleProfile;
