import axiosClient from '../axiosClient';

const postService = {
    // API lấy danh sách bài viết mới nhất (Cấu hình mặc định trang 1, lấy 5 bài)
    getLatestPosts: (pageNumber = 1, pageSize = 5) => {
        const url = `/post?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        return axiosClient.get(url);
    },

    // API lấy chi tiết bài viết theo ID (Nếu cần thiết)
    getPostById: (id) => {
        const url = `/post/${id}`;
        return axiosClient.get(url);
    }
};

export default postService;