namespace Arcemi.Pathfinder.Kingmaker
{
    public class MemorizedSpellModel : RefModel
    {
        public MemorizedSpellModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int SpellLevel { get => A.Value<int>(); set => A.Value(value); }
        public string Type { get => A.Value<string>(); set => A.Value(value); }
        public int Index { get => A.Value<int>(); set => A.Value(value); }
        public bool Available { get => A.Value<bool>(); set => A.Value(value); }
        public SpellModel Spell => A.Object(factory: a => new SpellModel(a));
        public ListAccessor<MemorizedSpellModel> LinkedSlots => A.List(factory: a => new MemorizedSpellModel(a));
    }
}