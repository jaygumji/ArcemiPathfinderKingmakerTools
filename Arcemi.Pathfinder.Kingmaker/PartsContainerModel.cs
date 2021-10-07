namespace Arcemi.Pathfinder.Kingmaker
{
    public class PartsContainerModel : RefModel
    {
        public PartsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<PartItemModel> Items => A.List("m_Parts", PartItemModel.Factory, createIfNull: true);
    }
}