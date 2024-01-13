namespace Arcemi.Models
{
    public class CombatGroupPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Groups.PartCombatGroup, Code";

        public CombatGroupPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string ControlId { get => A.Value<string>("m_Id"); set => A.Value(value, "m_Id"); }
    }
}