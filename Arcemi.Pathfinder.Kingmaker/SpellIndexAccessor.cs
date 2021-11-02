namespace Arcemi.Pathfinder.Kingmaker
{
    public class SpellIndexAccessor
    {
        private readonly ListValueAccessor<int> _list;

        public int Index { get; }
        public int Level { get; }
        public int Value { get => _list[Index]; set => _list[Index] = value; }
        public SpellIndexAccessor(int index, ListValueAccessor<int> list)
        {
            _list = list;
            Index = index;
            Level = index;
        }
    }
}