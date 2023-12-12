namespace Arcemi.Models
{
    public class ComponentSpec
    {
        public ComponentSpec(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public virtual void AddTo(DictionaryAccessor<ComponentModel> components)
        {
            components.AddNull(Id);
        }
    }
    public class RuntimeComponentSpec : ComponentSpec
    {
        public RuntimeComponentSpec(string id, FactFeatureSpec addedFact) : base(id)
        {
            AddedFact = addedFact;
        }

        public FactFeatureSpec AddedFact { get; }

        public override void AddTo(DictionaryAccessor<ComponentModel> components)
        {
            var component = (RuntimeComponentModel)components.Add(Id, RuntimeComponentModel.Prepare);
            if (AddedFact != null) {
                AddedFact.AddTo(component.Data.NewAddedFact<FeatureFactItemModel>());
            }
        }
    }
}
