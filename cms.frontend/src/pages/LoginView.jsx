import { useState } from 'react';

const LoginView = ({ navigate }) => {
  const [isLogin, setIsLogin] = useState(true);
  
  return (
    <div className="page-container page-transition">
      <h2 className="page-title">{isLogin ? 'Đăng Nhập' : 'Đăng Ký'}</h2>
      <div className="form-card">
        <form onSubmit={(e) => { e.preventDefault(); navigate('user'); }}>
          {!isLogin && (
            <div className="form-group"><label>Họ và tên</label><input type="text" className="form-input" required /></div>
          )}
          <div className="form-group"><label>Email / Số điện thoại</label><input type="text" className="form-input" required /></div>
          <div className="form-group"><label>Mật khẩu</label><input type="password" className="form-input" required /></div>
          <button type="submit" className="btn btn-primary btn-block">{isLogin ? 'Đăng Nhập' : 'Đăng Ký'}</button>
          <div style={{textAlign: 'center', marginTop: '1.5rem', color: 'var(--text-muted)'}}>
            {isLogin ? 'Chưa có tài khoản? ' : 'Đã có tài khoản? '}
            <button type="button" style={{background: 'transparent', color: 'var(--accent)', fontWeight: 'bold'}} onClick={() => setIsLogin(!isLogin)}>
              {isLogin ? 'Đăng ký ngay' : 'Đăng nhập'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default LoginView;
