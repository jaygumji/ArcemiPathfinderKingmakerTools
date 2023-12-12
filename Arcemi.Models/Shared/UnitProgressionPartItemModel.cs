namespace Arcemi.Models
{
    public class UnitProgressionPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.PartUnitProgression, Code";

        public UnitProgressionPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Race { get => A.Value<string>("m_Race"); set => A.Value(value, "m_Race"); }
        public ListAccessor<UnitProgressionSelectionOfPartModel> Selections => A.List<UnitProgressionSelectionOfPartModel>("m_Selections");
        public int CharacterLevel { get => A.Value<int>(); set => A.Value(value); }
        public int Experience { get => A.Value<int>(); set => A.Value(value); }
    }
}