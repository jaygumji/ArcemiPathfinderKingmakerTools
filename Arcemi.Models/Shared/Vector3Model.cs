namespace Arcemi.Models.Shared
{
    public class Vector3Model : Model
    {
        public Vector3Model(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public double X { get => A.Value<double>("x"); set => A.Value(value, "x"); }
        public double Y { get => A.Value<double>("y"); set => A.Value(value, "y"); }
        public double Z { get => A.Value<double>("z"); set => A.Value(value, "z"); }
    }
}
