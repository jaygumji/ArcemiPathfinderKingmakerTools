namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactsContainerModel : RefModel
    {
        public FactsContainerModel(ModelDataAccessor accessor) : base(accessor) { }

        public ListAccessor<FactItemModel> Items => A.List("m_Facts", FactItemModel.Factory, createIfNull: true);
    }
}