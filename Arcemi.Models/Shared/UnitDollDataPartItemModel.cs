namespace Arcemi.Models
{
    public class UnitDollDataPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.UnitPartDollData, Assembly-CSharp";
        public UnitDollDataPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public SpecialDollDataModel Special => A.Object("m_Special", a => new SpecialDollDataModel(a));
        public DollDataModel Default => A.Object(factory: a => new DollDataModel(a));
    }
}