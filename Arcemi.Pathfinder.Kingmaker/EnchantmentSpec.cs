using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class EnchantmentSpec
    {
        public EnchantmentSpec(string blueprint, params string[] components)
        {
            Blueprint = blueprint;
            Components = components;
        }

        public string Blueprint { get; }
        public IReadOnlyList<string> Components { get; }
    }
}