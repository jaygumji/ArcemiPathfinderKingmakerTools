namespace Arcemi.Models
{
    public class UnitStatePartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.PartUnitState, Code";

        public UnitStatePartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Size { get => A.Value<string>("m_Size"); set => A.Value(value, "m_Size"); }
        //"SuppressedSpellSchools": [],
        public bool IsPanicked { get => A.Value<bool>(); set => A.Value(value); }
        public bool CanRemoveFromParty { get => A.Value<bool>(); set => A.Value(value); }

    }
}