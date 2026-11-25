using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameInventoryItemWriter : GameModelCollectionWriter<IGameItemEntry, RefModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameInventoryItemWriter(RefModel @ref)
        {
            Ref = @ref;
        }

        public RefModel Ref { get; }

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            if (args.Blueprint.HasValue()) {
                var itemType = args.Data as W40KRTItemType ?? W40KRTItemType.Get(Res.Blueprints.Get(args.Blueprint).Type);
                args.Obj.Add("$type", itemType.TypeRef);
                args.Obj.Add("Blueprint", args.Blueprint);
                args.Obj.Add("OriginalBlueprint", args.Blueprint);
                args.Obj.Add("m_IsInGame", true);
                args.Obj.Add("m_Initiative", new JObject());
                args.Obj.Add("Abilities", new JArray());
                args.Obj.Add("IsIdentified", true);
                args.Obj.Add("UniqueId", Guid.NewGuid().ToString("N"));
                var mainFact = args.References.Create();
                mainFact.Add("$type", "Kingmaker.Items.ItemFact, Code");
                mainFact.Add("m_Components", new JArray());
                mainFact.Add("Blueprint", args.Blueprint);
                mainFact.Add("UniqueId", Guid.NewGuid().ToString("N"));
                mainFact.Add("IsActive", true);
                args.Obj.Add("Facts", new JObject { { "m_Facts", new JArray(mainFact) } });
                args.Obj.Add("MainFact", args.References.CreateReference(args.Obj, mainFact));
                args.Obj.Add("Parts", new JObject { { "Container", new JArray { new JObject {
                    { "$type", "Kingmaker.UnitLogic.PartMechanicFeatures, Code" }
                } } } });
                var lastSlot = Ref.GetAccessor().List<RefModel>("m_Items")
                    .Select(item => item.GetAccessor().Value<int>("m_InventorySlotIndex"))
                    .DefaultIfEmpty(-1).Max();
                args.Obj.Add("m_InventorySlotIndex", lastSlot + 1);
                args.Obj.Add("m_Count", 1);
                if (itemType == W40KRTItemType.Usable) {
                    args.Obj.Add("Charges", 1);
                }
            }
            else {
                throw new NotSupportedException();
            }
        }
    }
}