using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class EnchantmentSpec
    {
        public EnchantmentSpec(string name, string blueprint, params string[] components)
        {
            Name = name;
            Blueprint = blueprint;
            Components = components;
        }

        public string Name { get; }
        public string Blueprint { get; }
        public IReadOnlyList<string> Components { get; }
    }
}