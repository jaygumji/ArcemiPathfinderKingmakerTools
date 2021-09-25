namespace Arcemi.Pathfinder.Kingmaker
{
    public class PartsContainerModel : RefModel
    {
        public PartsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<PartModel> Items => A.List<PartModel>("m_Parts", a => new PartModel(a), createIfNull: true);
    }
}