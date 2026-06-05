/* 
 * LOGINVIEW COMPONENT - REGISTER & LOGIN API & COOKIE INTEGRATION
 * Sinh viên: Vũ Hoàng Chính
 * Môn học: Chuyên đề ASP.NET Core & ReactJS
 */

import { useState } from 'react';
import customerService from '../services/customerService';
import { setCookie } from '../utils/cookieHelper';

const LoginView = ({ navigate }) => {
  const [isLogin, setIsLogin] = useState(true);
  
  // States cho Form
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');
  const [password, setPassword] = useState('');
  
  // Trạng thái xử lý
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage('');
    setLoading(true);

    try {
      if (isLogin) {
        // LUỒNG ĐĂNG NHẬP
        const response = await customerService.login(email, password);
        if (response && response.customer) {
          // Bảo mật phiên làm việc bằng Cookie lưu trữ 2 ngày (48 tiếng)
          setCookie('customer', response.customer, 2);
          alert("Đăng nhập tài khoản thành công!");
          navigate('user');
          window.location.reload(); // Reload để đồng bộ lại trạng thái header
        } else {
          setErrorMessage("Đăng nhập không thành công, vui lòng kiểm tra lại.");
        }
      } else {
        // LUỒNG ĐĂNG KÝ
        const registerData = {
          fullName,
          email,
          phone: phone || null,
          address: address || null,
          password
        };
        const response = await customerService.register(registerData);
        alert(response.message || "Đăng ký tài khoản thành công! Hãy tiến hành đăng nhập.");
        setIsLogin(true); // Chuyển sang form đăng nhập
        setPassword('');
      }
    } catch (err) {
      console.error("Lỗi xác thực:", err);
      const msg = err.response?.data?.message || "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau.";
      setErrorMessage(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-container page-transition">
      <h2 className="page-title">{isLogin ? 'Đăng Nhập' : 'Đăng Ký Khách Hàng'}</h2>
      <div className="form-card" style={{ maxWidth: '480px', margin: '0 auto' }}>
        
        {errorMessage && (
          <div className="error-alert" style={{ background: '#fef2f2', borderLeft: '4px solid #ef4444', color: '#b91c1c', padding: '10px 15px', borderRadius: '4px', marginBottom: '1.5rem', fontSize: '0.9rem' }}>
            <i className="fa-solid fa-triangle-exclamation" style={{ marginRight: '8px' }}></i> {errorMessage}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          {!isLogin && (
            <div className="form-group">
              <label>Họ và tên <span style={{ color: 'red' }}>*</span></label>
              <input 
                type="text" 
                className="form-input" 
                placeholder="Nhập họ và tên" 
                required 
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
              />
            </div>
          )}

          <div className="form-group">
            <label>Địa chỉ Email <span style={{ color: 'red' }}>*</span></label>
            <input 
              type="email" 
              className="form-input" 
              placeholder="example@gmail.com" 
              required 
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          {!isLogin && (
            <>
              <div className="form-group">
                <label>Số điện thoại (SĐT VN)</label>
                <input 
                  type="tel" 
                  className="form-input" 
                  placeholder="09xx xxx xxx" 
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                />
              </div>
              <div className="form-group">
                <label>Địa chỉ nhận hàng</label>
                <input 
                  type="text" 
                  className="form-input" 
                  placeholder="Số nhà, tên đường, quận/huyện, tỉnh/thành" 
                  value={address}
                  onChange={(e) => setAddress(e.target.value)}
                />
              </div>
            </>
          )}

          <div className="form-group">
            <label>Mật khẩu <span style={{ color: 'red' }}>*</span></label>
            <input 
              type="password" 
              className="form-input" 
              placeholder="Nhập mật khẩu (tối thiểu 6 ký tự)" 
              required 
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading ? (
              <><i className="fa-solid fa-spinner fa-spin" style={{ marginRight: '8px' }}></i> Đang xử lý...</>
            ) : (
              isLogin ? 'Đăng Nhập' : 'Đăng Ký Ngay'
            )}
          </button>

          <div style={{ textAlign: 'center', marginTop: '1.5rem', color: 'var(--text-muted)' }}>
            {isLogin ? 'Chưa có tài khoản khách hàng? ' : 'Đã đăng ký tài khoản? '}
            <button 
              type="button" 
              style={{ background: 'transparent', color: 'var(--accent)', fontWeight: 'bold', border: 'none', cursor: 'pointer' }} 
              onClick={() => {
                setIsLogin(!isLogin);
                setErrorMessage('');
              }}
            >
              {isLogin ? 'Đăng ký ngay' : 'Quay lại đăng nhập'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default LoginView;
