namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerMainCharacterModel : Model
    {
        public PlayerMainCharacterModel(ModelDataAccessor accessor) : base(accessor) {
        }

        public string UniqueId => A.Value<string>("m_UniqueId");
    }
}