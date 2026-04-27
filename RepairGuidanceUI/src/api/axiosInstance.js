import axios from 'axios';
import toast from 'react-hot-toast';

const axiosInstance = axios.create({
    baseURL: 'http://localhost:7120/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

axiosInstance.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        const message = error.response?.data?.Message || "Bir hata oluştu.";

        if (error.response?.status === 401) {
            toast.error("Oturum süreniz doldu, lütfen tekrar giriş yapın.");
        } else {

            toast.error(message);
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;