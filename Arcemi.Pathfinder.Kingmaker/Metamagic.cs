using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class Metamagic
    {
        public static Metamagic Empower(ISet<string> active, Action<Metamagic> update) => new Metamagic("Empower", 0x1, active, update);
        public static Metamagic Maximize(ISet<string> active, Action<Metamagic> update) => new Metamagic("Maximize", 0x2, active, update);
        public static Metamagic Quicken(ISet<string> active, Action<Metamagic> update) => new Metamagic("Quicken", 0x4, active, update);
        public static Metamagic Extend(ISet<string> active, Action<Metamagic> update) => new Metamagic("Extend", 0x8, active, update);
        public static Metamagic Heighten(ISet<string> active, Action<Metamagic> update) => new Metamagic("Heighten", 0x10, active, update);
        public static Metamagic Reach(ISet<string> active, Action<Metamagic> update) => new Metamagic("Reach", 0x20, active, update);
        public static Metamagic CompletelyNormal(ISet<string> active, Action<Metamagic> update) => new Metamagic("Completely normal", "CompletelyNormal", 0x40, active, update);
        public static Metamagic Persistent(ISet<string> active, Action<Metamagic> update) => new Metamagic("Persistent", 0x80, active, update);
        public static Metamagic Selective(ISet<string> active, Action<Metamagic> update) => new Metamagic("Selective", 0x100, active, update);
        public static Metamagic Bolstered(ISet<string> active, Action<Metamagic> update) => new Metamagic("Bolstered", 0x200, active, update);

        private readonly Action<Metamagic> _update;
        public string Name { get; }
        public string UID { get; }
        public int Flag { get; }
        private bool _IsActive;
        public bool IsActive { get => _IsActive; set { _IsActive = value; _update(this); } }

        public Metamagic(string name, int flag, ISet<string> active, Action<Metamagic> update) : this(name, name, flag, active, update)
        {
        }

        public Metamagic(string name, string uid, int flag, ISet<string> active, Action<Metamagic> update)
        {
            Name = name;
            UID = uid;
            Flag = flag;
            this._update = update;
            IsActive = active.Contains(uid);
        }

        public static IReadOnlyList<Metamagic> All(string value, Action<string> update)
        {
            var active = new HashSet<string>(value?.Split(',').Select(x => x.Trim()) ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            IReadOnlyList<Metamagic> all = default;

            void Update(Metamagic v)
            {
                if (all == null) return;
                update(string.Join(", ", all.Where(x => x.IsActive).Select(x => x.UID)));
            }

            all = new[] {
                Empower(active, Update),
                Maximize(active, Update),
                Quicken(active, Update),
                Extend(active, Update),
                Heighten(active, Update),
                Reach(active, Update),
                //CompletelyNormal(active, Update),
                Persistent(active, Update),
                Selective(active, Update),
                Bolstered(active, Update)
            };



            return all;
        }
    }
}