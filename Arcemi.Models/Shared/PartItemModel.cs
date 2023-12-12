using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class CraftedPartItemModel : PartItemModel
    {
        public const string TypeRef = "Models.Craft.CraftedItemPart, Assembly-CSharp";
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
            if (type.Eq(CompanionPartItemModel.TypeRef)) {
                return new CompanionPartItemModel(accessor);
            }
            if (type.Eq(UnitWearinessPartItemModel.TypeRef)) {
                return new UnitWearinessPartItemModel(accessor);
            }
            if (type.Eq(UnitExtraSpellsPerDayPartItemModel.TypeRef)) {
                return new UnitExtraSpellsPerDayPartItemModel(accessor);
            }
            if (type.Eq(UnitKineticistPartItemModel.TypeRef)) {
                return new UnitKineticistPartItemModel(accessor);
            }
            if (type.Eq(UnitPetPartItemModel.TypeRef)) {
                return new UnitPetPartItemModel(accessor);
            }
            if (type.Eq(UnitDollDataPartItemModel.TypeRef)) {
                return new UnitDollDataPartItemModel(accessor);
            }
            if (type.Eq(CraftedPartItemModel.TypeRef)) {
                return new CraftedPartItemModel(accessor);
            }
            if (type.Eq(UnitDescriptionPartItemModel.TypeRef)) {
                return new UnitDescriptionPartItemModel(accessor);
            }
            if (type.Eq(UnitViewSettingsPartItemModel.TypeRef)) {
                return new UnitViewSettingsPartItemModel(accessor);
            }
            if (type.Eq(StatsContainerPartItemModel.TypeRef)) {
                return new StatsContainerPartItemModel(accessor);
            }
            if (type.Eq(UnitAlignmentPartItemModel.TypeRef)) {
                return new UnitAlignmentPartItemModel(accessor);
            }
            if (type.Eq(UnitProgressionPartItemModel.TypeRef)) {
                return new UnitProgressionPartItemModel(accessor);
            }
            if (type.Eq(UnitStatePartItemModel.TypeRef)) {
                return new UnitStatePartItemModel(accessor);
            }
            if (type.Eq(HealthPartItemModel.TypeRef)) {
                return new HealthPartItemModel(accessor);
            }
            if (type.Eq(UnitBodyPartItemModel.TypeRef)) {
                return new UnitBodyPartItemModel(accessor);
            }
            if (type.Eq(UnitAsksPartItemModel.TypeRef)) {
                return new UnitAsksPartItemModel(accessor);
            }
            if (type.Eq(UnitUISettingsPartItemModel.TypeRef)) {
                return new UnitUISettingsPartItemModel(accessor);
            }
            if (type.Eq(UnitCompanionPartItemModel.TypeRef)) {
                return new UnitCompanionPartItemModel(accessor);
            }
            return new PartItemModel(accessor);
        }
    }
}