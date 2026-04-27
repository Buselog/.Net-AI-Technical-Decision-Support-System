import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { LogIn, Mail, Lock, Wrench, ChevronRight, Sparkles } from 'lucide-react';
import axiosInstance from '../api/axiosInstance';
import toast from 'react-hot-toast';
import repairVideo from '../assets/repair-video.mp4';

const Login = () => {
    // mode: "onChange" ile anlık validasyon aktif
    const { register, handleSubmit, formState: { errors } } = useForm({
        mode: "onChange"
    });
    const navigate = useNavigate();

    const onSubmit = async (data) => {
        try {
            const response = await axiosInstance.post('/Auth/login', data);
            localStorage.setItem('token', response.data.Token);
            localStorage.setItem('user', JSON.stringify(response.data));
            toast.success(`Hoş geldiniz!`);
            navigate('/dashboard');
        } catch (error) {
            // Backend'den gelen hata (400 BadRequest - FluentValidation) 
            // axiosInstance içindeki interceptor tarafından yakalanıp toast.error ile gösterilir.
        }
    };

    return (
        // overflow-hidden ve h-screen ile kaydırmayı tamamen yasakladık
        <div className="h-screen w-full bg-[#F1F5F9] flex items-center justify-center p-12 overflow-hidden font-sans">

            {/* Kartın yüksekliğini 580px'e sabitledik, taşmayı engelledik */}
            <div className="bg-white w-full max-w-5xl h-[540px] rounded-[40px] shadow-2xl flex overflow-hidden border border-slate-100">

                {/* SOL TARAF - Daha kompakt padding (p-10) */}
                <div className="w-full lg:w-[45%] p-10 flex flex-col justify-between">
                    <div className="flex items-center gap-2">
                        <div className="bg-orange-500 p-1.5 rounded-lg shadow-lg">
                            <Wrench className="text-white w-4 h-4" />
                        </div>
                        <span className="font-bold text-lg text-slate-800">RepairGuidance</span>
                    </div>

                    <div>
                        <h1 className="text-3xl font-black text-slate-900 mb-1">Giriş Yap</h1>
                        <p className="text-slate-500 mb-6 text-sm font-medium">Asistanına bağlan ve tamire başla.</p>

                        <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-4">
                            {/* E-POSTA */}
                            <div className="h-[85px]"> {/* Hata mesajı çıktığında tasarımı kaydırmasın diye sabit yer ayırdık */}
                                <label className="text-[11px] font-bold text-slate-400 ml-1 uppercase tracking-widest">E-posta</label>
                                <div className="relative mt-1">
                                    <Mail className={`absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 ${errors.Email ? 'text-red-500' : 'text-slate-400'}`} />
                                    <input
                                        {...register("Email", {
                                            required: "E-posta alanı boş bırakılamaz.", // Backend FluentValidation mesajınla aynı yaptık
                                            pattern: { value: /^\S+@\S+$/i, message: "Geçerli bir e-posta adresi giriniz." }
                                        })}
                                        className={`w-full bg-slate-50 border-2 ${errors.Email ? 'border-red-500' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-2xl py-3 pl-12 pr-4 text-slate-800 text-sm outline-none transition-all`}
                                        placeholder="mail@example.com"
                                    />
                                </div>
                                {errors.Email && <p className="text-red-500 text-[10px] font-bold mt-1 ml-2">{errors.Email.message}</p>}
                            </div>

                            {/* ŞİFRE */}
                            <div className="h-[85px]">
                                <label className="text-[11px] font-bold text-slate-400 ml-1 uppercase tracking-widest">Şifre</label>
                                <div className="relative mt-1">
                                    <Lock className={`absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 ${errors.Password ? 'text-red-500' : 'text-slate-400'}`} />
                                    <input
                                        {...register("Password", {
                                            required: "Şifre alanı boş bırakılamaz." // Backend FluentValidation mesajınla aynı
                                        })}
                                        type="password"
                                        className={`w-full bg-slate-50 border-2 ${errors.Password ? 'border-red-500' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-2xl py-3 pl-12 pr-4 text-slate-800 text-sm outline-none transition-all`}
                                        placeholder="••••••••"
                                    />
                                </div>
                                {errors.Password && <p className="text-red-500 text-[10px] font-bold mt-1 ml-2">{errors.Password.message}</p>}
                            </div>

                            <button type="submit" className="w-full bg-[#FFB800] hover:bg-[#F2AE00] text-slate-900 font-bold py-3.5 rounded-2xl shadow-xl transition-all flex items-center justify-center gap-2 mt-2">
                                Giriş Yap <ChevronRight className="w-4 h-4" />
                            </button>
                        </form>
                    </div>

                    <div className="text-center text-slate-400 text-xs font-medium">
                        Henüz hesabın yok mu? <Link to="/register" className="text-orange-600 font-extrabold hover:underline">Kaydol</Link>
                    </div>
                </div>

                {/* SAĞ TARAF - Daha sıkı yerleşim */}
                <div className="hidden lg:flex w-[55%] bg-slate-50 relative flex-col items-center justify-between p-10 overflow-hidden border-l border-slate-100">

                    <div className="flex items-center gap-2 bg-white px-4 py-2 rounded-full shadow-sm border border-slate-200">
                        <Sparkles className="text-orange-500 w-3.5 h-3.5" />
                        <span className="text-[10px] font-black text-slate-600 uppercase tracking-widest">Yapay Zeka Destekli</span>
                    </div>

                    <div className="relative w-full flex flex-col items-center">
                        <video
                            src={repairVideo}
                            autoPlay loop muted playsInline
                            className="w-[80%] scale-95 mix-blend-multiply pointer-events-none" // Scale'i 100'e çekerek taşmayı önledik
                        />
                    </div>

                    <div className="text-center space-y-1 mt-0 relative z-10">
                        <h2 className="text-2xl font-black text-slate-800 leading-tight">
                            Teknik Sorunları Birlikte Çözelim
                        </h2>
                        <p className="text-slate-400 text-[11px] max-w-[250px] mx-auto font-medium leading-relaxed">
                            En karmaşık cihazlar bile doğru rehberle kolayca tamir edilebilir.
                        </p>
                    </div>

                    {/* Köşe dekorasyonu */}
                    <div className="absolute top-0 right-0 w-24 h-24 bg-orange-100 rounded-bl-full opacity-40"></div>
                </div>

            </div>
        </div>
    );
};

export default Login;