using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class MetamagicCollection : IEnumerable<Metamagic>
    {
        public Metamagic Empower { get; }
        public Metamagic Maximize { get; }
        public Metamagic Quicken { get; }
        public Metamagic Extend { get; }
        public Metamagic Heighten { get; }
        public Metamagic Reach { get; }
        public Metamagic Persistent { get; }
        public Metamagic Selective { get; }
        public Metamagic Bolstered { get; }
        public Metamagic Intensified { get; }
        public Metamagic Piercing { get; }

        public MetamagicCollection(string value, Action<string> update)
        {
            var active = new HashSet<string>(value?.Split(',').Select(x => x.Trim()) ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            void Update(Metamagic v)
            {
                var newValue = string.Join(", ", this.Where(x => x.IsActive).Select(x => x.UID));
                update(newValue.OrIfEmpty("CompletelyNormal"));
            }

            Empower = Metamagic.Empower(active, Update);
            Maximize = Metamagic.Maximize(active, Update);
            Quicken = Metamagic.Quicken(active, Update);
            Extend = Metamagic.Extend(active, Update);
            Heighten = Metamagic.Heighten(active, Update);
            Reach = Metamagic.Reach(active, Update);
            Persistent = Metamagic.Persistent(active, Update);
            Selective = Metamagic.Selective(active, Update);
            Bolstered = Metamagic.Bolstered(active, Update);
            Intensified = Metamagic.Intensified(active, Update);
            Piercing = Metamagic.Piercing(active, Update);
        }

        public override string ToString()
        {
            var newValue = string.Join(", ", this.Where(x => x.IsActive).Select(x => x.UID));
            return newValue.OrIfEmpty("CompletelyNormal");
        }

        public IEnumerator<Metamagic> GetEnumerator()
        {
            yield return Empower;
            yield return Maximize;
            yield return Quicken;
            yield return Extend;
            yield return Heighten;
            yield return Reach;
            yield return Persistent;
            yield return Selective;
            yield return Bolstered;
            yield return Piercing;
            yield return Intensified;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    [Flags]
    public enum MetamagicFlags
    {
        Empower = 1,
        Maximize = 2,
        Quicken = 4,
        Extend = 8,
        Heighten = 0x10,
        Reach = 0x20,
        CompletelyNormal = 0x40,
        Persistent = 0x80,
        Selective = 0x100,
        Bolstered = 0x200,
        Piercing = 0x400,
        Intensified = 0x800
    }
}