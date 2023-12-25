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
                var blueprint = Res.Blueprints.Get(args.Blueprint);
                var itemType = W40KRTItemType.Get(blueprint.Type);
                args.Obj.Add("$type", itemType.TypeRef);
                args.Obj.Add("Blueprint", args.Blueprint);
                args.Obj.Add("OriginalBlueprint", args.Blueprint);
                args.Obj.Add("m_IsInGame", true);
                args.Obj.Add("m_Initiative", new JObject());
                args.Obj.Add("Abilities", new JArray());
                args.Obj.Add("IsIdentified", true);
                args.Obj.Add("UniqueId", Guid.NewGuid().ToString("N"));
                args.Obj.Add("Facts", new JObject { { "m_Facts", new JArray { new JObject {
                    { "$type", "Kingmaker.Items.ItemFact, Code" },
                    { "m_Components", new JArray() },
                    { "Blueprint", args.Blueprint },
                    { "UniqueId", Guid.NewGuid().ToString("N") },
                    { "IsActive", true }
                } } } });
                args.Obj.Add("Parts", new JObject { { "Container", new JArray { new JObject {
                    { "$type", "Kingmaker.UnitLogic.PartMechanicFeatures, Code" }
                } } } });
                args.Obj.Add("m_InventorySlotIndex", Ref.GetAccessor().List<RefModel>("m_Items").Max(i => i.GetAccessor().Value<int>("m_InventorySlotIndex")) + 1);
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