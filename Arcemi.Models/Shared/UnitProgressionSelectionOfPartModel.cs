namespace Arcemi.Models
{
    public class UnitProgressionSelectionOfPartModel : Model
    {
        public UnitProgressionSelectionOfPartModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Path { get => A.Value<string>(); set => A.Value(value); }
        public int Level { get => A.Value<int>(); set => A.Value(value); }
        public string Selection { get => A.Value<string>(); set => A.Value(value); }
        public string Feature { get => A.Value<string>(); set => A.Value(value); }
        public int Rank { get => A.Value<int>(); set => A.Value(value); }
    }
}