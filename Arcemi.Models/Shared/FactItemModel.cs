using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Models
{
    public class FactItemModel : RefModel, ITypedModel
    {
        public FactItemModel(ModelDataAccessor accessor) : base(accessor) { }
        public virtual string DisplayName(IGameResourcesProvider res) => res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>("$type"); set => A.Value(value, "$type"); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
        public TimeSpan AttachTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public bool IsActive { get => A.Value<bool>(); set => A.Value(value); }
        public virtual FactContextModel Context { get => A.Object("m_Context", a => new FactContextModel(a)); set => A.Value(value.GetAccessor().UnderlyingObject.Root, "m_Context"); }
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
            if (string.Equals(type, QuestFactItemModel.TypeRef, System.StringComparison.Ordinal)
                || string.Equals(type, QuestFactItemModel.TypeRefCode, System.StringComparison.Ordinal)) {
                return new QuestFactItemModel(accessor);
            }
            if (string.Equals(type, BuffFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new BuffFactItemModel(accessor);
            }
            if (string.Equals(type, AbilityFactItemModel.TypeRef, System.StringComparison.Ordinal))
            {
                return new AbilityFactItemModel(accessor);
            }
            if (string.Equals(type, ActivatableAbilityFactItemModel.TypeRef, System.StringComparison.Ordinal))
            {
                return new ActivatableAbilityFactItemModel(accessor);
            }
            if (string.Equals(type, EtudeFactItemModel.TypeRef, System.StringComparison.Ordinal)) {
                return new EtudeFactItemModel(accessor);
            }
            if (string.Equals(type, W40KRTFeatFactItemModel.TypeRef, StringComparison.Ordinal)) {
                return new W40KRTFeatFactItemModel(accessor);
            }
            if (string.Equals(type, W40KRTAbilityFactItemModel.TypeRef, StringComparison.Ordinal)) {
                return new W40KRTAbilityFactItemModel(accessor);
            }
            if (string.Equals(type, W40KRTBuffFactItemModel.TypeRef, StringComparison.Ordinal)) {
                return new W40KRTBuffFactItemModel(accessor);
            }
            if (string.Equals(type, W40KRTSoulMarkFactItemModel.TypeRef, StringComparison.Ordinal)) {
                return new W40KRTSoulMarkFactItemModel(accessor);
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
            obj.Add(nameof(AttachTime), AttachTimes.GameStart.ToString());
            obj.Add(nameof(UniqueId), Guid.NewGuid().ToString());
            obj.Add(nameof(IsActive), true);
            obj.Add(nameof(Components), new JObject());
        }

        public void Import(FactItemModel obj)
        {
            Import(obj.Export());
        }
    }
}