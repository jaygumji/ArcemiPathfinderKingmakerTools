namespace Arcemi.Models
{
    public class FlagsContainerModel : RefModel
    {
        public FlagsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<KeyValuePairModel<int>> Items => A.List("m_UnlockedFlags", a => new KeyValuePairModel<int>(a));
    }
}