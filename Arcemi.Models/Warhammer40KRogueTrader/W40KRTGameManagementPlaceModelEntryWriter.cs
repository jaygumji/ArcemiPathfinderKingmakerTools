using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameManagementPlaceModelEntryWriter : GameModelCollectionWriter<IGameManagementPlaceModelEntry, RefModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            JObject CreateValueSection()
            {
                return new JObject {
                    {"InitialValue", 0},
                    {"MinValue", 0 },
                    {"MaxValue", 10 }
                };
            }
            var colony = ColonyLookup[args.Blueprint];
            args.Obj.Add("Area", colony.AreaId);
            args.Obj.Add("Planet", colony.PlanetId);
            args.Obj.Add("Colony", new JObject {
                {"Projects", new JArray()},
                {"Planet", colony.PlanetId },
                {"Blueprint", colony.BlueprintId },
                {"Contentment", CreateValueSection()},
                {"Efficiency", CreateValueSection() },
                {"Security", CreateValueSection() },
                //{"AvailableProducedResources", new JArray() },
                //{"InitialResources", new JArray() },
                //{"UsedProducedResources", new JArray() },
                //{"ColonyTraits", new JArray() },
                {"m_CurrentEvent", new JObject { { "m_ColonyEvent", new JObject() } } },
                //{"AllEventsForColony", new JArray() },
                //{"m_NotStartedEvents", new JArray() },
                //{"StartedEvents", new JArray() },
                //{"StartedChronicles", new JArray() },
                //{"Chronicles", new JArray() },
                {"LootToReceive", new JObject { { "m_Items", new JObject { { "m_Items", new JArray()} } }, { "m_Cargo", new JArray() } } },
                //{"Consumables", new JArray() },
                //{"FinishedProjectsSinceLastVisit", new JArray() }
            });
        }
        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameManagementPlaceModelEntry> current)
        {
            var res = new List<IBlueprintMetadataEntry>();
            foreach (var colony in Colonies) {
                if (current.Any(c => c.Blueprint.Eq(colony.BlueprintId))) continue;
                if (Res.Blueprints.TryGet(colony.BlueprintId, out var metadataEntry)) {
                    res.Add(metadataEntry);
                }
            }
            return res;
        }

        private static IReadOnlyList<ColonyDefinition> Colonies { get; } = new[] {
            new ColonyDefinition("883a5e2be9384e9f941ec3abf65f5d73", "d738b19f167343f6a975a8f3398571d3", "bbc87ed0a043441589c8db7f8657a8df")
        };
        private static IReadOnlyDictionary<string, ColonyDefinition> ColonyLookup { get; } = Colonies.ToDictionary(c => c.BlueprintId, StringComparer.Ordinal);
        private class ColonyDefinition
        {
            public ColonyDefinition(string blueprintId, string areaId, string planetId)
            {
                BlueprintId = blueprintId;
                AreaId = areaId;
                PlanetId = planetId;
            }

            public string BlueprintId { get; }
            public string AreaId { get; }
            public string PlanetId { get; }
        }
    }
}