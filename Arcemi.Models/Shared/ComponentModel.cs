namespace Arcemi.Models
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
            else if (string.Equals(type, AddFactsComponentModel.TypeRef, System.StringComparison.Ordinal))
            {
                return new AddFactsComponentModel(accessor);
            }
            return new ComponentModel(accessor);
        }
    }
}