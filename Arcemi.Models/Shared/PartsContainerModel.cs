namespace Arcemi.Models
{
    public class PartsContainerModel : RefModel
    {
        public PartsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<PartItemModel> Items => A.List("m_Parts", PartItemModel.Factory, createIfNull: true);
        public ListAccessor<PartItemModel> Container => A.List(factory: PartItemModel.Factory, createIfNull: true);
    }
}