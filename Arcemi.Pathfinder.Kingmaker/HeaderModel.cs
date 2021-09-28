namespace Arcemi.Pathfinder.Kingmaker
{
    public class HeaderModel : RefModel
    {
        public HeaderModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string GameId { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>(); set => A.Value(value); }
        public string PlayerCharacterName { get => A.Value<string>(); set => A.Value(value); }
        public string Name { get => A.Value<string>(); set => A.Value(value); }
    }
}
