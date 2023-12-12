namespace Arcemi.SaveGameEditor.Components
{
    public class SearchSelectArgs<T>
    {
        public SearchSelectArgs(T item)
        {
            Item = item;
        }

        public T Item { get; }
    }
}
