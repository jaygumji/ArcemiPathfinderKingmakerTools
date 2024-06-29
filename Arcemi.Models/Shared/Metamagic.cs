using System;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class Metamagic
    {
        public static Metamagic Empower(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Empower, active, update);
        public static Metamagic Maximize(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Maximize, active, update);
        public static Metamagic Quicken(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Quicken, active, update);
        public static Metamagic Extend(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Extend, active, update);
        public static Metamagic Heighten(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Heighten, active, update);
        public static Metamagic Reach(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Reach, active, update);
        public static Metamagic CompletelyNormal(ISet<string> active, Action<Metamagic> update) => new Metamagic("Completely normal", MetamagicFlags.CompletelyNormal, active, update);
        public static Metamagic Persistent(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Persistent, active, update);
        public static Metamagic Selective(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Selective, active, update);
        public static Metamagic Bolstered(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Bolstered, active, update);
        public static Metamagic Intensified(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Intensified, active, update);
        public static Metamagic Piercing(ISet<string> active, Action<Metamagic> update) => new Metamagic(MetamagicFlags.Piercing, active, update);

        private readonly Action<Metamagic> _update;
        public string Name { get; }
        public string UID { get; }
        public MetamagicFlags Flag { get; }
        private bool _IsActive;
        public bool IsActive { get => _IsActive; set { _IsActive = value; _update(this); } }

        public Metamagic(MetamagicFlags flag, ISet<string> active, Action<Metamagic> update) : this(flag.ToString(), flag, active, update)
        {
        }

        public Metamagic(string name, MetamagicFlags flag, ISet<string> active, Action<Metamagic> update)
        {
            Name = name;
            UID = flag.ToString();
            Flag = flag;
            _update = update;
            _IsActive = active.Contains(UID);
        }

    }
}