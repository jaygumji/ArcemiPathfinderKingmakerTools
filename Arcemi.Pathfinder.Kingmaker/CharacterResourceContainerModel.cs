namespace Arcemi.Pathfinder.Kingmaker
{
    public class CharacterResourceContainerModel : RefModel
    {
        public CharacterResourceContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<AbilityResourceModel> PersistantResources => A.List(factory: a => new AbilityResourceModel(a));
    }
}