using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class EnchantmentSpec
    {
        public EnchantmentSpec(string name, string blueprint)
            : this(name, blueprint, Array.Empty<ComponentSpec>())
        {
        }

        public EnchantmentSpec(string name, string blueprint, params string[] components)
            : this(name, blueprint, components.Select(c => new ComponentSpec(c)).ToArray())
        {
        }

        public EnchantmentSpec(string name, string blueprint, params ComponentSpec[] components)
        {
            Name = name;
            Blueprint = blueprint;
            Components = components;
        }

        public string Name { get; }
        public string Blueprint { get; }
        public IReadOnlyList<ComponentSpec> Components { get; }
    }
}