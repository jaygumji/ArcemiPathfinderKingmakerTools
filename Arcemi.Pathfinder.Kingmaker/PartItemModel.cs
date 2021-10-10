namespace Arcemi.Pathfinder.Kingmaker
{
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
            return new PartItemModel(accessor);
        }
    }
}