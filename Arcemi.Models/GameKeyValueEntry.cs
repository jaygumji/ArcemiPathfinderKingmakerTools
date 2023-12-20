namespace Arcemi.Models
{
    public class GameKeyValueEntry<T>
    {
        public GameKeyValueEntry(string key, int value)
        {
            Key = key;
            Value = value;
        }

        public string DisplayName(IGameResourcesProvider res) => res.Blueprints.GetNameOrBlueprint(Key);
        public string Key { get; }
        public int Value { get; }
    }
}