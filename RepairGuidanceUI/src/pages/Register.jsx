import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { UserPlus, Mail, Lock, Wrench, ChevronRight, Sparkles, User } from 'lucide-react';
import axiosInstance from '../api/axiosInstance';
import toast from 'react-hot-toast';
import repairVideo from '../assets/repair-video.mp4';

const Register = () => {
    const { register, handleSubmit, formState: { errors } } = useForm({
        mode: "onChange"
    });
    const navigate = useNavigate();

    const onSubmit = async (data) => {
        try {
            await axiosInstance.post('/Auth/register', data);
            toast.success("Kayıt başarılı! Giriş yapabilirsiniz.");
            navigate('/login');
        } catch (backendErrors) {
            // BACKEND'DEN GELEN DİNAMİK MESAJLARI BURADA YAKALIYORUZ
            if (Array.isArray(backendErrors)) {
                backendErrors.forEach((err) => {
                    // err.Property (örn: Email) ve err.Message (örn: E-posta adresi gereklidir)
                    setError(err.Property, { type: "manual", message: err.Message });
                });
            }
        }
    };

    return (
        <div className="h-screen w-full bg-[#F1F5F9] flex items-center justify-center p-12 overflow-hidden font-sans">
            <div className="bg-white w-full max-w-5xl h-[540px] rounded-[40px] shadow-2xl flex overflow-hidden border border-slate-100">

                {/* SOL TARAF - Form Alanı */}
                <div className="w-full lg:w-[45%] p-10 flex flex-col justify-between">
                    <div className="flex items-center gap-2">
                        <div className="bg-orange-500 p-1.5 rounded-lg shadow-lg"><Wrench className="text-white w-4 h-4" /></div>
                        <span className="font-bold text-lg text-slate-800">RepairGuidance</span>
                    </div>

                    <div>
                        <h1 className="text-3xl font-black text-slate-900 mb-1">Kaydol</h1>
                        <p className="text-slate-500 mb-6 text-sm font-medium">Ustalara özel asistanına hemen kaydol.</p>

                        <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-3">
                            <div className="grid grid-cols-2 gap-3 h-[75px]">
                                <div className="flex flex-col">
                                    <label className="text-[10px] font-bold text-slate-400 ml-1 uppercase tracking-widest">Ad</label>
                                    <input {...register("FirstName")} className={`w-full bg-slate-50 border-2 ${errors.FirstName ? 'border-red-500' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-xl py-2.5 px-4 text-slate-800 text-sm outline-none transition-all`} placeholder="Adınız" />
                                    {errors.FirstName && <p className="text-red-500 text-[9px] font-bold mt-0.5 ml-1">{errors.FirstName.message}</p>}
                                </div>
                                <div className="flex flex-col">
                                    <label className="text-[10px] font-bold text-slate-400 ml-1 uppercase tracking-widest">Soyad</label>
                                    <input {...register("LastName")} className={`w-full bg-slate-50 border-2 ${errors.LastName ? 'border-red-500' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-xl py-2.5 px-4 text-slate-800 text-sm outline-none transition-all`} placeholder="Soyadınız" />
                                    {errors.LastName && <p className="text-red-500 text-[9px] font-bold mt-0.5 ml-1">{errors.LastName.message}</p>}
                                </div>
                            </div>

                            <div className="h-[75px]">
                                <label className="text-[10px] font-bold text-slate-400 ml-1 uppercase tracking-widest">E-posta</label>
                                <div className="relative mt-1">
                                    <Mail className={`absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 ${errors.Email ? 'text-red-500' : 'text-slate-400'}`} />
                                    <input {...register("Email")} className={`w-full bg-slate-50 border-2 ${errors.Email ? 'border-red-500' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-xl py-2.5 pl-9 pr-4 text-slate-800 text-sm outline-none transition-all`} placeholder="mail@example.com" />
                                </div>
                                {errors.Email && <p className="text-red-500 text-[9px] font-bold mt-0.5 ml-1">{errors.Email.message}</p>}
                            </div>

                            <div className="h-[75px]">
                                <label className="text-[10px] font-bold text-slate-400 ml-1 uppercase tracking-widest">Şifre</label>
                                <div className="relative mt-1">
                                    <Lock className={`absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 ${errors.Password ? 'text-red-500' : 'text-slate-400'}`} />
                                    <input {...register("Password")} type="password" className={`w-full bg-slate-50 border-2 ${errors.Password ? 'border-red-500' : 'border-transparent focus:border-orange-500 focus:bg-white'} rounded-xl py-2.5 pl-9 pr-4 text-slate-800 text-sm outline-none transition-all`} placeholder="••••••••" />
                                </div>
                                {errors.Password && <p className="text-red-500 text-[9px] font-bold mt-0.5 ml-1">{errors.Password.message}</p>}
                            </div>

                            <button type="submit" className="w-full bg-[#FFB800] hover:bg-[#F2AE00] text-slate-900 font-bold py-3 rounded-2xl shadow-xl transition-all flex items-center justify-center gap-2 mt-2">Kaydı Tamamla <ChevronRight className="w-4 h-4" /></button>
                        </form>
                    </div>

                    <div className="text-center text-slate-400 text-xs font-medium">Zaten hesabın var mı? <Link to="/login" className="text-orange-600 font-extrabold hover:underline">Giriş Yap</Link></div>
                </div>

                {/* SAĞ TARAF - Aynı video yapısı */}
                <div className="hidden lg:flex w-[55%] bg-slate-50 relative flex-col items-center justify-between p-10 overflow-hidden border-l border-slate-100">
                    <div className="flex items-center gap-2 bg-white px-4 py-2 rounded-full shadow-sm border border-slate-200">
                        <Sparkles className="text-orange-500 w-3.5 h-3.5" />
                        <span className="text-[10px] font-black text-slate-600 uppercase tracking-widest">Yapay Zeka Destekli</span>
                    </div>

                    <div className="relative w-full flex flex-col items-center">
                        <video
                            src={repairVideo}
                            autoPlay loop muted playsInline
                            className="w-[80%] scale-95 mix-blend-multiply pointer-events-none"
                        />
                    </div>

                    <div className="text-center space-y-1 relative z-10">
                        <h2 className="text-2xl font-black text-slate-800 leading-tight">Ustalara Katıl</h2>
                        <p className="text-slate-400 text-[11px] max-w-[250px] mx-auto font-medium leading-relaxed">
                            Kendi tamir rehberlerini oluştur ve asistanın gücünü keşfet.
                        </p>
                    </div>

                    <div className="absolute top-0 right-0 w-24 h-24 bg-orange-100 rounded-bl-full opacity-40"></div>
                </div>

            </div>
        </div>
    );
};

export default Register;