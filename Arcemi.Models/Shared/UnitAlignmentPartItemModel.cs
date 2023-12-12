namespace Arcemi.Models
{
    public class UnitAlignmentPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Alignments.PartUnitAlignment, Code";

        public UnitAlignmentPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public VectorModel Vector => A.Object<VectorModel>("m_Vector");
        public string LockedAlignmentMask { get => A.Value<string>("m_LockedAlignmentMask"); set => A.Value(value, "m_LockedAlignmentMask"); }
        public string TargetAlignment { get => A.Value<string>("m_TargetAlignment"); set => A.Value(value, "m_TargetAlignment"); }
        public ListAccessor<AlignmentHistoryOnPartItemModel> History => A.List<AlignmentHistoryOnPartItemModel>("m_History");
    }
}