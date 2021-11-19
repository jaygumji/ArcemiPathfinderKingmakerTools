using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactItemModel : RefModel, ITypedModel
    {
        public FactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public FactContextModel Context { get => A.Object("m_Context", a => new FactContextModel(a)); }
        public DictionaryAccessor<ComponentModel> Components => A.Dictionary(factory: a => new ComponentModel(a), createIfNull: true);

        public static FactItemModel Factory(ModelDataAccessor accessor)
        {
            var type = accessor.TypeValue();
            if (string.Equals(type, FeatureFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new FeatureFactItemModel(accessor);
            }
            if (string.Equals(type, EnchantmentFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new EnchantmentFactItemModel(accessor);
            }
            if (string.Equals(type, QuestFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new QuestFactItemModel(accessor);
            }
            if (string.Equals(type, BuffFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new BuffFactItemModel(accessor);
            }
            if (string.Equals(type, AbilityFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new AbilityFactItemModel(accessor);
            }
            if (string.Equals(type, EtudeFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new EtudeFactItemModel(accessor);
            }
            return new FactItemModel(accessor);
        }

        protected static void Prepare(IReferences refs, JObject obj)
        {
            obj.Add(nameof(Components), new JObject());
        }
    }
}