namespace Arcemi.Models
{
    public class SpecialDollDataModel : Model
    {
        public SpecialDollDataModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public DollDataModel KitsunePolymorph => A.Object(factory: a => new DollDataModel(a));
    }
}