using Microsoft.ML;
using RepairGuidance.Application.Managers;
using RepairGuidance.Application.Models;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Exceptions;

namespace RepairGuidance.InnerInfrastructure.Managers.Prediction
{
    // 1. ML.NET'in ANLAYACAĞI VERİ SINIFLARI (DTO gibi düşün)
    // Bu sınıflar, veritabanındaki veriyi ML motoruna "tercüme" eder.


    public class PredictionManager : IPredictionManager
    {
        private readonly IRepairRequestRepository _repository;
        private readonly MLContext _mlContext;
        // Path.Combine: Farklı işletim sistemlerinde (Windows/Linux) klasör yollarının (\ veya /) 
        // hata vermeden birleştirilmesini sağlar.
        // Environment.CurrentDirectory: Uygulamanın o an çalıştığı (bin/Debug klasörü gibi) 
        // ana klasör yolunu verir.
        // Yani modelimiz projenin tam kalbine "repair_model.zip" adıyla kaydedilecek.
        //private string _modelPath = Path.Combine(AppContext.BaseDirectory, "Models", "repair_model.zip");
        private string _modelPath = Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName, "Models", "repair_model.zip");

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
                DeviceDifficulty = (float)r.DeviceDifficulty,
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

            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding("TargetEncoded", "TargetLevel")
           .Append(_mlContext.Transforms.Concatenate("Features", "TargetEncoded", "DeviceDifficulty", "ExperienceScore"))
             // FastTree zaten bool bir 'Label' sütunu bekliyor ve biz ona bunu sağladık.
             // .Append(_mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

             .Append(_mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            // Adım 4: Eğitim başlıyor (FİT):
            // Bu satırda model veriyi öğrenir ve örüntüleri (Pattern) çıkarır.
            // Fit: "Öğren" komutudur. Pipeline'daki tüm işlemler bu satırda gerçekleşir.
            var model = pipeline.Fit(dataView);

            var directory = Path.GetDirectoryName(_modelPath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            // Adım 5: Modeli dosyaya kaydet:
            // Modeli eğitip RAM'de bırakmıyoruz, bir .zip dosyası olarak saklıyoruz.
            // Save: Eğitilen zekayı dosyaya yazarız.
            // dataView.Schema: Modelin hangi sütun isimlerini (Label, DeviceName vb.) beklediğini kaydeder.
            // Bu şema olmadan modeli tekrar yüklediğimizde hangi sütunun ne olduğunu bilemez.
            _mlContext.Model.Save(model, dataView.Schema, _modelPath);

        }

        public ModelOutput Predict(int difficulty, string targetLevel, float experienceScore)
        {
            // Mühürlü modelden tahmin al
            if (!File.Exists(_modelPath)) throw new ModelNotTrainedException();

            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var schema);
            var engine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(loadedModel);

            // Uygulamadaki ExperienceScore'u User tablosundan alabilirsin ama mühürlü model 
            // dünkü 5000 veriye göre eğitildiği için burada bir baz değer (örn: 50) verebiliriz.
            return engine.Predict(new ModelInput
            {
                DeviceDifficulty = (float)difficulty,
                TargetLevel = targetLevel,
                ExperienceScore = experienceScore
            });
        }

    }

}

