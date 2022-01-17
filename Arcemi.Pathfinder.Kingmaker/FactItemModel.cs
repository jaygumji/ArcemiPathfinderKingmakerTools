using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class FactItemModel : RefModel, ITypedModel
    {
        public FactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public string DisplayName => A.Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public FactContextModel Context { get => A.Object("m_Context", a => new FactContextModel(a)); }
        public DictionaryAccessor<ComponentModel> Components => A.Dictionary(factory: ComponentModel.Factory, createIfNull: true);
        public ParentContextModel ParentContext => A.Object(factory: a => new ParentContextModel(a), createIfNull: true);

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

        public static Action<IReferences, JObject> GetPreparation<T>()
        {
            var type = typeof(T);
            if (type == typeof(FeatureFactItemModel)) {
                return FeatureFactItemModel.Prepare;
            }
            if (type == typeof(EnchantmentFactItemModel)) {
                return EnchantmentFactItemModel.Prepare;
            }
            if (type == typeof(QuestFactItemModel)) {
                return QuestFactItemModel.Prepare;
            }
            if (type == typeof(BuffFactItemModel)) {
                return BuffFactItemModel.Prepare;
            }
            if (type == typeof(AbilityFactItemModel)) {
                return AbilityFactItemModel.Prepare;
            }
            if (type == typeof(EtudeFactItemModel)) {
                return EtudeFactItemModel.Prepare;
            }
            return Prepare;
        }

        protected static void Prepare(IReferences refs, JObject obj)
        {
            obj.Add(nameof(Components), new JObject());
        }

        public string Export()
        {
            return A.ExportCode();
        }
    }
}