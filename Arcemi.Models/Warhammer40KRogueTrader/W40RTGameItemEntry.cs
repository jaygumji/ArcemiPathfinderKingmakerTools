namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40RTGameItemEntry : IGameItemEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40RTGameItemEntry(RefModel @ref)
        {
            Ref = @ref;
            A = Ref.GetAccessor();
        }

        public RefModel Ref { get; }
        private ModelDataAccessor A { get; }
        public IBlueprintMetadataEntry BlueprintMetadata { get; }

        public string Name => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => A.Value<string>();
        public string Type => Res.Blueprints.TryGet(Blueprint, out var entry) ? entry.Type?.DisplayName : "";
        public string Description => "";
        public int Index => A.Value<int>("m_InventorySlotIndex");
        public bool IsChargable => W40KRTItemType.Usable.TypeRef.Eq(A.TypeValue());
        public int Charges { get => A.Value<int>(); set => A.Value(value); }
        public bool IsStackable => true;
        public int Count { get => A.Value<int>("m_Count"); set => A.Value(value, "m_Count"); }
        public bool CanEdit => false;
        public string IconUrl => "/images/ItemTypes/Other.png";
    }
}