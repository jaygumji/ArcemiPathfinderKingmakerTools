namespace Arcemi.Models
{
    public class BuffUniquePartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.UnitPartUniqueBuffs, Assembly-CSharp";
        public BuffUniquePartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<BuffFactItemModel> Buffs => A.List(factory: a => new BuffFactItemModel(a));
    }
}