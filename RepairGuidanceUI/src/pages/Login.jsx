import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { Wrench, LogIn, Mail, Lock } from 'lucide-react';
import axiosInstance from '../api/axiosInstance';
import toast from 'react-hot-toast';

const Login = () => {
    const { register, handleSubmit, formState: { errors } } = useForm({
        mode: "onChange"
    });
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
            <div className="max-w-md w-full bg-slate-900 border border-slate-800 rounded-3xl shadow-2xl p-10">

                <div className="text-center mb-10">
                    <div className="inline-flex p-4 bg-orange-500/10 rounded-2xl mb-4 border border-orange-500/20">
                        <Wrench className="w-10 h-10 text-orange-500" />
                    </div>
                    <h2 className="text-4xl font-extrabold text-white tracking-tight">Tekrar Hoş Geldiniz</h2>
                    <p className="text-slate-400 mt-3 font-medium">Lütfen bilgilerinizi girerek devam edin.</p>
                </div>

                <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-8">

                    <div className="space-y-2">
                        <label className="text-sm font-semibold text-slate-300 ml-1">E-posta Adresi</label>
                        <div className="relative group">
                            <Mail className={`absolute left-3 top-3.5 w-5 h-5 transition-colors ${errors.Email ? 'text-red-500' : 'text-slate-500 group-focus-within:text-orange-500'}`} />
                            <input
                                {...register("Email", {
                                    required: "E-posta alanı boş bırakılamaz.",
                                    pattern: {
                                        value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                                        message: "Geçerli bir e-posta adresi giriniz."
                                    }
                                })}
                                className={`w-full bg-slate-950 border ${errors.Email ? 'border-red-500' : 'border-slate-800 focus:border-orange-500'} rounded-xl py-3.5 pl-12 pr-4 text-white outline-none transition-all shadow-inner`}
                                placeholder="ornek@mail.com"
                            />
                        </div>

                        {errors.Email && (
                            <p className="text-red-500 text-xs font-medium mt-1 ml-1 animate-pulse">
                                {errors.Email.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <label className="text-sm font-semibold text-slate-300 ml-1">Şifre</label>
                        <div className="relative group">
                            <Lock className={`absolute left-3 top-3.5 w-5 h-5 transition-colors ${errors.Password ? 'text-red-500' : 'text-slate-500 group-focus-within:text-orange-500'}`} />
                            <input
                                {...register("Password", { required: "Şifre alanı boş bırakılamaz." })}
                                type="password"
                                className={`w-full bg-slate-950 border ${errors.Password ? 'border-red-500' : 'border-slate-800 focus:border-orange-500'} rounded-xl py-3.5 pl-12 pr-4 text-white outline-none transition-all shadow-inner`}
                                placeholder="••••••••"
                            />
                        </div>
                        {errors.Password && (
                            <p className="text-red-500 text-xs font-medium mt-1 ml-1 animate-pulse">
                                {errors.Password.message}
                            </p>
                        )}
                    </div>

                    <button
                        type="submit"
                        className="w-full bg-gradient-to-r from-orange-600 to-orange-500 hover:from-orange-500 hover:to-orange-400 text-white font-bold py-4 rounded-2xl shadow-lg shadow-orange-500/20 transform active:scale-[0.98] transition-all flex items-center justify-center gap-3 text-lg"
                    >
                        <LogIn className="w-6 h-6" /> Giriş Yap
                    </button>
                </form>

                <div className="mt-10 pt-8 border-t border-slate-800 text-center">
                    <p className="text-slate-500 font-medium">
                        Hesabınız yok mu?{' '}
                        <Link to="/register" className="text-orange-500 hover:text-orange-400 transition-colors underline underline-offset-4">
                            Kaydolun
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default Login;