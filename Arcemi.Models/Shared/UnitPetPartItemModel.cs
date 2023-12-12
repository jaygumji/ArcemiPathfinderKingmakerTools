namespace Arcemi.Models
{
    public class UnitPetPartItemModel : PartItemModel
    {
        public const string TypeRef = "Models.UnitLogic.Parts.UnitPartPet, Assembly-CSharp";
        public UnitPetPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public SingleReferenceModel MasterRef => A.Object("m_MasterRef", a => new SingleReferenceModel(a));
    }
}