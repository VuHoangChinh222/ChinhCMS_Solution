import axiosClient from '../axiosClient';

const customerService = {
    // API Đăng nhập khách hàng (Tạm thời nhận Email và Password)
    login: (email, password) => {
        const url = '/customer/login';
        return axiosClient.post(url, { email, password });
    },

    // API Đăng ký tài khoản khách hàng mới
    register: (customerData) => {
        const url = '/customer/register';
        return axiosClient.post(url, customerData);
    },

    // API Cập nhật thông tin tài khoản khách hàng
    updateCustomer: (id, customerData) => {
        const url = `/customer/update/${id}`;
        return axiosClient.put(url, customerData);
    },

    // API Quên mật khẩu khách hàng
    forgotPassword: (email) => {
        const url = '/customer/forgot-password';
        return axiosClient.post(url, { email });
    },

    // API Đăng nhập bằng Google
    googleLogin: (credential) => {
        const url = '/customer/google-login';
        return axiosClient.post(url, { credential });
    },

    // API Hoàn tất đăng ký Google
    registerGoogle: (customerData) => {
        const url = '/customer/register-google';
        return axiosClient.post(url, customerData);
    }
};

export default customerService;