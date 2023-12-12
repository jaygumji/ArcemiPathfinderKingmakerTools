namespace Arcemi.Models
{
    public class ProgressionItemModel : Model
    {
        public ProgressionItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public ProgressionItemValueModel Value => A.Object(factory: a => new ProgressionItemValueModel(a));
    }
}