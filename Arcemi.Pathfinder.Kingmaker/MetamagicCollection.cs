using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
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

        public MetamagicCollection(string value, Action<string> update)
        {
            var active = new HashSet<string>(value?.Split(';').Select(x => x.Trim()) ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            void Update(Metamagic v)
            {
                var newValue = string.Join("; ", this.Where(x => x.IsActive).Select(x => x.UID));
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
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}