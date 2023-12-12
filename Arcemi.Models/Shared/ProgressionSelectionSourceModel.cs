namespace Arcemi.Models
{
    public class ProgressionSelectionSourceModel : RefModel
    {
        public ProgressionSelectionSourceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint => A.Value<string>();
    }
}