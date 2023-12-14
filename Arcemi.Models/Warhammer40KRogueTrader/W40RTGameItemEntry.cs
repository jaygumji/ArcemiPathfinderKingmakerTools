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

        public string Name => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => A.Value<string>();
        public string Type => Res.Blueprints.Get(Blueprint)?.Type?.DisplayName;
        public string Description => "";
        public int Index => A.Value<int>("m_InventorySlotIndex");
        public bool IsChargable => A.HasProperty(nameof(Charges));
        public int Charges { get => A.Value<int>(); set => A.Value(value); }
        public bool IsStackable => A.HasProperty(nameof(Count));
        public int Count { get => A.Value<int>(); set => A.Value(value); }
        public bool CanEdit => false;
        public string IconUrl => "/images/ItemTypes/Other.png";
    }
}