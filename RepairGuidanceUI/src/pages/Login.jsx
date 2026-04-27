import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { Wrench, LogIn, Mail, Lock } from 'lucide-react';
import axiosInstance from '../api/axiosInstance';
import toast from 'react-hot-toast';

const Login = () => {
    const { register, handleSubmit, formState: { errors } } = useForm();
    const navigate = useNavigate();

    const onSubmit = async (data) => {
        try {
            const response = await axiosInstance.post('/Auth/login', data);
            localStorage.setItem('token', response.data.Token);
            localStorage.setItem('user', JSON.stringify(response.data));

            toast.success(`Hoş geldiniz, ${response.data.FullName}!`);
            navigate('/dashboard');
        } catch (error) {

        }
    };

    return (
        <div className="min-h-screen bg-slate-950 flex items-center justify-center p-4">
            <div className="max-w-md w-full bg-slate-900 rounded-2xl shadow-2xl border border-slate-800 p-8">
                <div className="text-center mb-8">
                    <div className="inline-flex p-3 bg-orange-500/10 rounded-xl mb-4">
                        <Wrench className="w-8 h-8 text-orange-500" />
                    </div>
                    <h2 className="text-3xl font-bold text-white">Tekrar Hoş Geldiniz</h2>
                    <p className="text-slate-400 mt-2">Tamir asistanınıza giriş yapın</p>
                </div>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-slate-300 mb-2">E-posta</label>
                        <div className="relative">
                            <Mail className="absolute left-3 top-3 w-5 h-5 text-slate-500" />
                            <input
                                {...register("Email", { required: "E-posta gerekli" })}
                                type="email"
                                className="w-full bg-slate-800 border border-slate-700 rounded-lg py-2.5 pl-10 pr-4 text-white focus:ring-2 focus:ring-orange-500 outline-none transition-all"
                                placeholder="ornek@mail.com"
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-300 mb-2">Şifre</label>
                        <div className="relative">
                            <Lock className="absolute left-3 top-3 w-5 h-5 text-slate-500" />
                            <input
                                {...register("Password", { required: "Şifre gerekli" })}
                                type="password"
                                className="w-full bg-slate-800 border border-slate-700 rounded-lg py-2.5 pl-10 pr-4 text-white focus:ring-2 focus:ring-orange-500 outline-none transition-all"
                                placeholder="••••••••"
                            />
                        </div>
                    </div>

                    <button
                        type="submit"
                        className="w-full bg-orange-600 hover:bg-orange-700 text-white font-semibold py-3 rounded-lg flex items-center justify-center gap-2 transition-colors"
                    >
                        <LogIn className="w-5 h-5" /> Giriş Yap
                    </button>
                </form>

                <p className="text-center text-slate-400 mt-6 text-sm">
                    Hesabınız yok mu?{' '}
                    <Link to="/register" className="text-orange-500 hover:underline">Kaydolun</Link>
                </p>
            </div>
        </div>
    );
};

export default Login;