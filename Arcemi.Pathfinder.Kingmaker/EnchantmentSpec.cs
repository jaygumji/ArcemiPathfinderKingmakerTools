namespace Arcemi.Pathfinder.Kingmaker
{
    public class EnchantmentSpec
    {
        public EnchantmentSpec(string blueprint, string component)
        {
            Blueprint = blueprint;
            Component = component;
        }

        public string Blueprint { get; }
        public string Component { get; }
    }
}