import axios from 'axios';
import toast from 'react-hot-toast';

const axiosInstance = axios.create({
    baseURL: 'http://localhost:7120/api', // Kendi API portun
    headers: { 'Content-Type': 'application/json' }
});

axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        // FluentValidation hataları genellikle data.Errors içinde dizi olarak gelir
        if (error.response?.status === 400 && error.response.data?.Errors) {
            // Hataları sayfada setError ile yakalamak için fırlatıyoruz
            return Promise.reject(error.response.data.Errors);
        }
        
        const message = error.response?.data?.Message || "Bir hata oluştu.";
        toast.error(message);
        return Promise.reject(error);
    }
);

export default axiosInstance;