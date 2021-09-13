namespace Arcemi.Pathfinder.Kingmaker
{
    [SaveFileType("Kingmaker.EntitySystem.Entities.UnitEntityData, Assembly-CSharp")]
    public class UnitEntityModel : RefModel
    {
        public UnitEntityModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string UniqueId => A.Value<string>();

        public CharacterModel Descriptor => A.Object<CharacterModel>();
    }
}
