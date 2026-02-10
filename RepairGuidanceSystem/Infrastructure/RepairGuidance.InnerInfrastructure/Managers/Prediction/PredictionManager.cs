using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.ML;
using Microsoft.ML.Data;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.InnerInfrastructure.Managers.Prediction
{
    // 1. ML.NET'in ANLAYACAĞI VERİ SINIFLARI (DTO gibi düşün)
    // Bu sınıflar, veritabanındaki veriyi ML motoruna "tercüme" eder.

    public class ModelInput
    {
        [LoadColumn(0)]
        public string DeviceName { get; set; }

        [LoadColumn(1)]
        public float ExperienceScore { get; set; }

        [LoadColumn(2)]
        public string TargetLevel { get; set; }

        // Doğrudan bool yaparak karmaşayı bitiriyoruz.
        // ColumnName("Label") özniteliği, ML.NET'e bu sütunun tahmin hedefi olduğunu söyler.
        [ColumnName("Label")]
        public bool Status { get; set; }
    }

    public class ModelOutput
    {
        // Tahmin edilen sonuç ("Completed" veya "Failed")
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; } // Doğrudan başarı olasılığını verir. 
        public float Score { get; set; } // Olasılık değeri
    }


    public class PredictionManager : IPredictionManager
    {
        private readonly IRepairRequestRepository _repository;
        private readonly MLContext _mlContext;
        // Path.Combine: Farklı işletim sistemlerinde (Windows/Linux) klasör yollarının (\ veya /) 
        // hata vermeden birleştirilmesini sağlar.
        // Environment.CurrentDirectory: Uygulamanın o an çalıştığı (bin/Debug klasörü gibi) 
        // ana klasör yolunu verir.
        // Yani modelimiz projenin tam kalbine "repair_model.zip" adıyla kaydedilecek.
        private string _modelPath = Path.Combine(Environment.CurrentDirectory, "repair_model.zip");

        public PredictionManager(IRepairRequestRepository repository)
        {
            _repository = repository;
            _mlContext = new MLContext(seed: 0);
            // MLContext: ML.NET'in kalbidir. Tüm işlemler (eğitim, tahmin) buradan yönetilir.
            // MLContext: ML.NET'in "ana şalteridir". Eğitimden tahmine kadar her şey bu nesne üzerinden yapılır.
        }

        public async Task TrainModelAsync()
        {
            // Adım 1: Veritabanından veriyi çek (5000 adet RepairRequest verisi)
            var requests = await _repository.GetAllWithUsersAsync();

            // Veriyi ML.NET'in anlayacağı ModelInput formatına çevir:
            var trainingData = requests.Select(r => new ModelInput
            {
                DeviceName = r.DeviceName,
                ExperienceScore = (float)(r.AppUser?.ExperienceScore ?? 0),
                TargetLevel = r.TargetLevel,
                // Veritabanındaki "Completed" metnini C# seviyesinde True'ya çeviriyoruz.
                Status = r.Status == "Completed"
            }).ToList();


            // Adım 2: Veriyi ML.NET'e yükle:
            // 2. IDataView: ML.NET veriyi liste (List) olarak değil, IDataView (Veri Görünümü) olarak işler. 
            // Bu yapı, milyonlarca satırı belleği şişirmeden verimli okumayı sağlar.

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(trainingData);


            // Adım 3: Veri İşleme Hattı(Pipeline):
            // Makine öğrenimi metinleri anlamaz. "Laptop" kelimesini sayısal vektörlere çeviriyoruz.
            // Burası mutfaktır. Malzemeler (veriler) girer, işlenir.
            // Pipeline neden karmaşık? Çünkü bilgisayar "Laptop" kelimesini matematiksel olarak çarpamaz.
            // Önce algoritmayı Binary (İkili) sınıflandırma olarak kurgulayalım çünkü sonucumuz ya Başarılı ya Başarısız.
            // Önce algoritmayı Binary (İkili) sınıflandırma olarak kurgulayalım çünkü sonucumuz ya Başarılı ya Başarısız.
            // 1. Veriyi Boolean (True/False) formatına dönüştüren yeni Pipeline

            //   Artık pipeline içinde ConvertType veya MapValueToKey gibi riskli adımlara gerek kalmadı.
            //   Sadece metinleri sayısallaştırıp eğitime geçiyoruz:

            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("DeviceNameEncoded", "DeviceName")
            .Append(_mlContext.Transforms.Categorical.OneHotEncoding("TargetEncoded", "TargetLevel"))
            .Append(_mlContext.Transforms.Concatenate("Features", "DeviceNameEncoded", "TargetEncoded", "ExperienceScore"))
            // FastTree zaten bool bir 'Label' sütunu bekliyor ve biz ona bunu sağladık.
            .Append(_mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            // Adım 4: Eğitim başlıyor (FİT):
            // Bu satırda model veriyi öğrenir ve örüntüleri (Pattern) çıkarır.
            // Fit: "Öğren" komutudur. Pipeline'daki tüm işlemler bu satırda gerçekleşir.
            var model = pipeline.Fit(dataView);


            // Adım 5: Modeli dosyaya kaydet:
            // Modeli eğitip RAM'de bırakmıyoruz, bir .zip dosyası olarak saklıyoruz.
            // Save: Eğitilen zekayı dosyaya yazarız.
            // dataView.Schema: Modelin hangi sütun isimlerini (Label, DeviceName vb.) beklediğini kaydeder.
            // Bu şema olmadan modeli tekrar yüklediğimizde hangi sütunun ne olduğunu bilemez.
            _mlContext.Model.Save(model, dataView.Schema, _modelPath);

        }

        public async Task<float> GetSuccessProbabilityAsync(string deviceName, float experienceScore, string targetLevel)
        {
            // Eğer daha önce model eğitilmediyse önce eğit (Otomasyon sağlar)
            if (!File.Exists(_modelPath)) await TrainModelAsync();

            // Kaydedilen modeli yükle:
            // Load: Dondurulmuş modeli tekrar RAM'e yükler.
            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelSchema);

            //Tek bir Tahmin yapmak için PredictionEngine olustur:
            // PredictionEngine: "Tahmin Motoru". Tekil bir sorgu için modeli hazır hale getirir.
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(loadedModel);

            // Tahmin edilecek veriyi hazırla
            var input = new ModelInput
            {
                DeviceName = deviceName,
                ExperienceScore = experienceScore,
                TargetLevel = targetLevel
            };

            // Tahmini gerçekleştir:
            var result = predictionEngine.Predict(input);


            return result.Probability;
        }

    }

}

