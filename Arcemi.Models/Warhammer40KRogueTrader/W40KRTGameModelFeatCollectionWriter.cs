using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameModelFeatCollectionWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        private readonly W40KRTGameUnitModel _owner;

        public W40KRTGameModelFeatCollectionWriter(W40KRTGameUnitModel owner)
        {
            _owner = owner;
        }
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            PrepareFeature(args.Obj, args.Blueprint, _owner.UniqueId);
        }

        internal static void PrepareFeature(JObject obj, string blueprint, string ownerUniqueId)
        {
            obj.Add("$type", "Kingmaker.UnitLogic.Feature, Code");
            var factUniqueId = Guid.NewGuid().ToString("N");
            obj.Add("m_Context", new JObject {
                {"m_OwnerRef",  ownerUniqueId},
                {"m_CasterRef", ownerUniqueId},
                {"m_Ranks", new JArray {0, 0, 0, 0, 0, 0, 0} },
                {"m_SharedValues", new JArray {0, 0, 0, 0, 0, 0, 0} },
                {"AssociatedBlueprint", blueprint},
            });
            obj.Add("m_Components", new JArray());
            obj.Add("m_Sources", new JArray());
            obj.Add("Rank", 1);
            obj.Add("Param", new JObject());
            obj.Add("IgnorePrerequisites", true);
            obj.Add("DisabledBecauseOfPrerequisites", false);
            obj.Add("Blueprint", blueprint);
            obj.Add("UniqueId", factUniqueId);
            obj.Add("IsActive", true);
        }

        public override void AfterRemove(AfterRemoveCollectionItemArgs<IGameUnitFeatEntry, FactItemModel> args)
        {
            W40KRTProgressionFacts.RemoveGrants(_owner, args.Model);
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitFeatEntry> current)
        {
            var currentIds = new HashSet<string>(current.Select(x => x.Blueprint), StringComparer.Ordinal);
            return Res.Blueprints.GetEntries(BlueprintTypeId.Feature)
                .Concat(Res.Blueprints.GetEntries(W40KRTBlueprintProvider.CareerPath))
                .Concat(Res.Blueprints.GetEntries(W40KRTBlueprintProvider.AttributeAdvancement))
                .Concat(Res.Blueprints.GetEntries(W40KRTBlueprintProvider.ShipPostExpertise))
                .Where(e => !currentIds.Contains(e.Id))
                .ToArray();
        }
    }
}