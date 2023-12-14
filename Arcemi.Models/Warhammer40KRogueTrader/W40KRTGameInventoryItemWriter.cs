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
                args.Obj.Add("$type", blueprint.Type.FullName);
                args.Obj.Add("Facts", new JObject { "m_Facts", new JArray() });
                args.Obj.Add("Parts", new JObject { "Container", new JArray() });
                args.Obj.Add("m_InventorySlotIndex", Ref.GetAccessor().List<RefModel>().Max(i => i.GetAccessor().Value<int>("m_InventorySlotIndex")) + 1);
            }
            else {
                throw new NotSupportedException();
            }
        }
    }
}