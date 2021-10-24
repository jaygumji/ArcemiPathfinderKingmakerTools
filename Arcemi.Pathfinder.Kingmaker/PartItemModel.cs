using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class CraftedPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.Craft.CraftedItemPart, Assembly-CSharp";
        public CraftedPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int CasterLevel { get => A.Value<int>(); set => A.Value(value); }
        public int SpellLevel { get => A.Value<int>(); set => A.Value(value); }
        public int AbilityDC { get => A.Value<int>(); set => A.Value(value); }

        public static void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            obj.Add(nameof(CasterLevel), 1);
            obj.Add(nameof(SpellLevel), 1);
            obj.Add(nameof(AbilityDC), 1);
        }
    }
    public class PartItemModel : RefModel
    {
        public PartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public static PartItemModel Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();
            if (string.Equals(type, CompanionPartItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new CompanionPartItemModel(accessor);
            }
            if (string.Equals(type, UnitWearinessPartItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new UnitWearinessPartItemModel(accessor);
            }
            if (string.Equals(type, UnitExtraSpellsPerDayPartItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new UnitExtraSpellsPerDayPartItemModel(accessor);
            }
            if (string.Equals(type, UnitKineticistPartItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new UnitKineticistPartItemModel(accessor);
            }
            if (string.Equals(type, UnitPetPartItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new UnitPetPartItemModel(accessor);
            }
            if (string.Equals(type, UnitDollDataPartItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new UnitDollDataPartItemModel(accessor);
            }
            if (string.Equals(type, CraftedPartItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new CraftedPartItemModel(accessor);
            }
            return new PartItemModel(accessor);
        }
    }
}