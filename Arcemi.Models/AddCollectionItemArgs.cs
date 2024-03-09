namespace Arcemi.Models
{
    public class AddCollectionItemArgs
    {
        public AddCollectionItemArgs(string blueprint, object data)
        {
            Blueprint = blueprint;
            Data = data;
        }

        public string Blueprint { get; }
        public object Data { get; }
    }
}