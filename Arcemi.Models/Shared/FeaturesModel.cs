namespace Arcemi.Models
{
    public class FeaturesModel : RefModel
    {
        public FeaturesModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<FactItemModel> Facts => A.List("m_Facts", FactItemModel.Factory);

    }
}