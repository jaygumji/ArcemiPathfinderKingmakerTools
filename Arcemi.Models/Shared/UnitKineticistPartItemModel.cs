namespace Arcemi.Models
{
    public class UnitKineticistPartItemModel : PartItemModel
    {
        public const string TypeRef = "Models.UnitLogic.Class.Kineticist.UnitPartKineticist, Assembly-CSharp";
        public UnitKineticistPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int AcceptedBurn { get => A.Value<int>(); set => A.Value(value); }
    }
}