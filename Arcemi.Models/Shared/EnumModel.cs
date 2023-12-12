namespace Arcemi.Models
{
    public class EnumModel<TValue>
    {
        public EnumModel(string name, TValue value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public TValue Value { get; }
    }
}