namespace Arcemi.Pathfinder.Kingmaker
{
    public class UnitEntityModel : RefModel
    {
        public UnitEntityModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string UniqueId => A.Value<string>();

        public CharacterModel Descriptor => A.Object<CharacterModel>();
    }
}
