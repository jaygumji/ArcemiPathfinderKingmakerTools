namespace Arcemi.Pathfinder.Kingmaker
{
    public class CharacterResourceContainerModel : RefModel
    {
        public CharacterResourceContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<CharacterResourceModel> PersistantResources => A.List(factory: a => new CharacterResourceModel(a));
    }
}