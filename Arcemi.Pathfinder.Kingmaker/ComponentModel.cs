namespace Arcemi.Pathfinder.Kingmaker
{
    public class ComponentModel : RefModel
    {
        public ComponentModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public static ComponentModel Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();
            if (string.Equals(type, RuntimeComponentModel.TypeRef, System.StringComparison.Ordinal))
            {
                return new RuntimeComponentModel(accessor);
            }
            return new ComponentModel(accessor);
        }
    }
}