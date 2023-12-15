namespace Arcemi.SaveGameEditor.Components
{
    public class TabIndexChangedArgs
    {
        public TabIndexChangedArgs(ITabControl tab, int newIndex, int oldIndex)
        {
            Tab = tab;
            NewIndex = newIndex;
            OldIndex = oldIndex;
        }

        public ITabControl Tab { get; }
        public int NewIndex { get; }
        public int OldIndex { get; }
    }
}
