namespace Arcemi.Models
{
    public class GameEnumValue
    {
        public GameEnumValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
