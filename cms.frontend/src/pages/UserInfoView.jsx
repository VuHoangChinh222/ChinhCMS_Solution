const UserInfoView = ({ navigate }) => {
  return (
    <div className="page-container page-transition">
      <h2 className="page-title">Tài khoản <span>Của tôi</span></h2>
      <div className="user-profile">
        <div className="avatar-placeholder">V</div>
        <div>
          <h3 style={{fontSize: '1.5rem'}}>Vũ Hoàng Chính</h3>
          <p style={{color: 'var(--text-muted)'}}>vuhoangchinh@example.com | 0987654321</p>
          <p style={{color: 'var(--text-muted)'}}>Hạng thành viên: <strong style={{color: 'gold'}}>Vàng</strong></p>
        </div>
        <button className="btn btn-outline" style={{marginLeft: 'auto'}} onClick={() => navigate('login')}>Đăng xuất</button>
      </div>
      <div className="form-card" style={{maxWidth: '100%', margin: 0}}>
        <h3 style={{marginBottom: '1rem', fontFamily: 'var(--font-heading)'}}>Lịch sử đơn hàng</h3>
        <div className="empty-state" style={{padding: '2rem 0'}}>
          <i className="fa-solid fa-box-open" style={{fontSize: '2rem'}}></i>
          <p>Bạn chưa có đơn hàng nào.</p>
        </div>
      </div>
    </div>
  );
};

export default UserInfoView;
