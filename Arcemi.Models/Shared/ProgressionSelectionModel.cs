namespace Arcemi.Models
{
    public class ProgressionSelectionModel : Model
    {
        public ProgressionSelectionModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Key { get => A.Value<string>(); set => A.Value(value); }
        public ProgressionSelectionValueModel Value => A.Object(factory: a => new ProgressionSelectionValueModel(a));
    }

    public class ProgressionSelectionValueModel : RefModel
    {
        public ProgressionSelectionValueModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public DictionaryOfValueListAccessor<string> ByLevel => A.DictionaryOfValueList<string>("m_SelectionsByLevel");
        public ProgressionSelectionSourceModel Source => A.Object(factory: a => new ProgressionSelectionSourceModel(a));
    }
}