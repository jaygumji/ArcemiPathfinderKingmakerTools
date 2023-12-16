using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameModelFeatCollectionWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        private readonly string ownerUniqueId;

        public W40KRTGameModelFeatCollectionWriter(string ownerUniqueId)
        {
            this.ownerUniqueId = ownerUniqueId;
        }
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            args.Obj.Add("$type", "Kingmaker.UnitLogic.Feature, Code");
            var factUniqueId = Guid.NewGuid().ToString("N");
            args.Obj.Add("m_Context", new JObject {
                {"m_OwnerRef",  ownerUniqueId},
                {"m_CasterRef", ownerUniqueId},
                {"m_Ranks", new JArray {0, 0, 0, 0, 0, 0, 0} },
                {"m_SharedValues", new JArray {0, 0, 0, 0, 0, 0, 0} },
                {"AssociatedBlueprint", args.Blueprint},
            });
            args.Obj.Add("m_Components", new JArray());
            args.Obj.Add("m_Sources", new JArray());
            //args.Obj.Add("m_Sources", new JArray {
            //    new JObject {
            //        {"m_FactRef", new JObject {
            //            {"EntityId", ownerUniqueId},
            //            {"FactId", factUniqueId}
            //        } }
            //    }
            //});
            args.Obj.Add("Rank", 1);
            args.Obj.Add("Param", new JObject());
            args.Obj.Add("IgnorePrerequisites", true);
            args.Obj.Add("DisabledBecauseOfPrerequisites", false);
            args.Obj.Add("Blueprint", args.Blueprint);
            args.Obj.Add("UniqueId", ownerUniqueId);
            args.Obj.Add("IsActive", true);
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitFeatEntry> current)
        {
            var currentIds = new HashSet<string>(current.Select(x => x.Blueprint), StringComparer.Ordinal);
            return Res.Blueprints.GetEntries(BlueprintTypeId.Feature).Where(e => !currentIds.Contains(e.Id)).ToArray();
        }
    }
}