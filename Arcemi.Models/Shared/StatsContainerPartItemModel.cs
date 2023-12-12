namespace Arcemi.Models
{
    public class StatsContainerPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.PartStatsContainer, Code";

        public StatsContainerPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public StatsContainerModel Container => A.Object<StatsContainerModel>();
    }
}