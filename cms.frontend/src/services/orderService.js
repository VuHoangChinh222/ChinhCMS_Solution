import axiosClient from '../axiosClient';

const orderService = {
    // API lấy lịch sử đơn hàng theo ID khách hàng
    getOrdersByCustomerId: (customerId) => {
        const url = `/order/customer/${customerId}`;
        return axiosClient.get(url);
    }
};

export default orderService;