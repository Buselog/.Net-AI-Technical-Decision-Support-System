namespace RepairGuidance.Domain.Exceptions
{
    public class ModelNotTrainedException : BaseBusinessException
    {
        public ModelNotTrainedException() : base("ML modeli henüz eğitilmemiş.")
        {

        }
    }
}
