import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { LogIn, Mail, Lock, Wrench, ShieldCheck, ChevronRight } from 'lucide-react';
import axiosInstance from '../api/axiosInstance';
import toast from 'react-hot-toast';
import repairVideo from '../assets/repair-video.mp4';

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
        <div className="min-h-screen bg-[#F0F2F5] flex items-center justify-center p-6 font-sans">

            <div className="bg-white w-full max-w-5xl h-[700px] rounded-[40px] shadow-2xl flex overflow-hidden border border-white">


                <div className="w-full lg:w-1/2 p-12 flex flex-col justify-between">
                    <div className="flex items-center gap-2 mb-8">
                        <div className="bg-orange-500 p-2 rounded-lg">
                            <Wrench className="text-white w-5 h-5" />
                        </div>
                        <span className="font-bold text-xl text-slate-800 tracking-tight">RepairGuidance</span>
                    </div>

                    <div className="flex-1 flex flex-col justify-center">
                        <h1 className="text-4xl font-extrabold text-slate-900 mb-2">Giriş Yap</h1>
                        <p className="text-slate-500 mb-10 font-medium">Tamir asistanına bağlan ve rehberlerini yönet.</p>

                        <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-6">
                            <div className="space-y-2">
                                <label className="text-sm font-bold text-slate-700 ml-1">E-posta</label>
                                <div className="relative">
                                    <Mail className={`absolute left-4 top-4 w-5 h-5 ${errors.Email ? 'text-red-500' : 'text-slate-400'}`} />
                                    <input
                                        {...register("Email", {
                                            required: "E-posta alanı boş bırakılamaz.",
                                            pattern: {
                                                value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                                                message: "Geçerli bir e-posta adresi giriniz."
                                            }
                                        })}
                                        className={`w-full bg-slate-50 border-2 ${errors.Email ? 'border-red-500 focus:ring-red-100' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-2xl py-4 pl-12 pr-4 text-slate-800 outline-none transition-all font-medium`}
                                        placeholder="mail@example.com"
                                    />
                                </div>
                                {errors.Email && <p className="text-red-500 text-xs font-bold mt-1 ml-2">{errors.Email.message}</p>}
                            </div>

                            <div className="space-y-2">
                                <label className="text-sm font-bold text-slate-700 ml-1">Şifre</label>
                                <div className="relative">
                                    <Lock className={`absolute left-4 top-4 w-5 h-5 ${errors.Password ? 'text-red-500' : 'text-slate-400'}`} />
                                    <input
                                        {...register("Password", { required: "Şifre alanı boş bırakılamaz." })}
                                        type="password"
                                        className={`w-full bg-slate-50 border-2 ${errors.Password ? 'border-red-500 focus:ring-red-100' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-2xl py-4 pl-12 pr-4 text-slate-800 outline-none transition-all font-medium`}
                                        placeholder="••••••••"
                                    />
                                </div>
                                {errors.Password && <p className="text-red-500 text-xs font-bold mt-1 ml-2">{errors.Password.message}</p>}
                            </div>

                            <button
                                type="submit"
                                className="w-full bg-[#FFB800] hover:bg-[#E6A600] text-slate-900 font-bold py-4 rounded-2xl shadow-lg shadow-yellow-500/20 transition-all flex items-center justify-center gap-2 text-lg mt-4"
                            >
                                Giriş Yap <ChevronRight className="w-5 h-5" />
                            </button>
                        </form>
                    </div>

                    <div className="mt-8 text-center text-slate-500 font-medium">
                        Henüz hesabın yok mu?{' '}
                        <Link to="/register" className="text-orange-600 font-bold hover:underline underline-offset-4">Kaydol</Link>
                    </div>
                </div>

                <div className="hidden lg:flex w-1/2 bg-gradient-to-br from-orange-400 to-orange-600 relative p-12 flex-col justify-between">
                    <div className="absolute top-[-50px] right-[-50px] w-64 h-64 bg-white/10 rounded-full blur-3xl"></div>
                    <div className="absolute bottom-[-50px] left-[-50px] w-64 h-64 bg-orange-900/10 rounded-full blur-3xl"></div>

                    <div className="relative z-10">
                        <div className="bg-white/20 backdrop-blur-md p-6 rounded-[30px] border border-white/30 inline-block">
                            <ShieldCheck className="text-white w-8 h-8" />
                            <p className="text-white font-bold mt-2">Yapay Zeka Destekli</p>
                            <p className="text-white/80 text-sm">Hassas ve güvenilir tamir rehberleri.</p>
                        </div>
                    </div>


                    <div className="relative z-10 flex justify-center items-center">
                        <video
                            src={repairVideo}
                            autoPlay
                            loop
                            muted
                            playsInline
                            className="w-full max-w-[450px] h-auto rounded-3xl drop-shadow-2xl"
                        />
                    </div>

                    <div className="relative z-10 text-white">
                        <h2 className="text-3xl font-bold leading-tight">Teknik Sorunları Birlikte Çözelim.</h2>
                        <p className="text-white/80 mt-4 font-medium italic">
                            "En karmaşık cihazlar bile doğru rehberle kolayca tamir edilebilir."
                        </p>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default Login;