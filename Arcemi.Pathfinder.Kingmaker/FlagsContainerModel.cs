namespace Arcemi.Pathfinder.Kingmaker
{
    public class FlagsContainerModel : RefModel
    {
        public FlagsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<FlagModel> Items => A.List("m_UnlockedFlags", a => new FlagModel(a));
    }
}