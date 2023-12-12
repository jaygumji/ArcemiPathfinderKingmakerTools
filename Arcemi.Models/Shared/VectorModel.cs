namespace Arcemi.Models
{
    public class VectorModel : Model
    {
        public VectorModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public double X { get => A.Value<double>("x"); set => A.Value(value, "x"); }
        public double Y { get => A.Value<double>("y"); set => A.Value(value, "y"); }
    }
}