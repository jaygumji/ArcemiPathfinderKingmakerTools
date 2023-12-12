namespace Arcemi.Models
{
    public class AddFactsComponentDataModel : RefModel
    {
        public AddFactsComponentDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<FactItemModel> AppliedFacts => A.List(factory: FactItemModel.Factory);

        public T NewAddedFact<T>()
            where T : FactItemModel
        {
            return (T)A.NewObject(nameof(AppliedFacts), FactItemModel.GetPreparation<T>(), FactItemModel.Factory);
        }
    }
}
