using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameManagementPlaceModelEntryWriter : GameModelCollectionWriter<IGameDataObject, W40KRTColonyModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public override bool IsRemoveEnabled => true;

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
            var def = ColonyLookup[args.Blueprint];
            args.Obj.Add("Area", def.AreaId);
            args.Obj.Add("Planet", def.PlanetId);

            var colonyResources = new JArray();
            foreach (var resource in def.Resources) {
                colonyResources.Add(new JObject {
                    {"Key", resource.Blueprint },
                    {"Value", resource.Count }
                });
            }
            var colonyEvents = new JArray();
            var colonyNotStartedEvents = new JArray();
            foreach (var evt in def.Events) {
                var evtObj = args.References.Create();
                evtObj.Add("m_ColonyEvent", new JObject { { "guid", evt.Blueprint } });
                evtObj.Add("Segments", evt.Segments);
                colonyEvents.Add(evtObj);
                colonyNotStartedEvents.Add(args.References.CreateReference(colonyNotStartedEvents, evtObj));
            }

            var colony = args.References.Create();
            colony.Add("Projects", new JArray());
            colony.Add("Planet", def.PlanetId);
            colony.Add("Blueprint", def.BlueprintId);
            colony.Add("Contentment", CreateValueSection());
            colony.Add("Efficiency", CreateValueSection());
            colony.Add("Security", CreateValueSection());
            colony.Add("AvailableProducedResources", colonyResources);
            colony.Add("InitialResources", colonyResources);
            colony.Add("UsedProducedResources", new JArray());
            colony.Add("ColonyTraits", new JArray());
            if (colonyEvents.Count > 0) {
                colony.Add("m_CurrentEvent", args.References.CreateReference(colonyEvents, (JObject)colonyEvents.First()));
            }
            colony.Add("AllEventsForColony", colonyEvents);
            colony.Add("m_NotStartedEvents", colonyNotStartedEvents);
            colony.Add("StartedEvents", new JArray());
            colony.Add("StartedChronicles", new JArray());
            colony.Add("Chronicles", new JArray());
            var lootToReceive = new JObject {
                { "m_Items", new JObject { { "m_Items", new JArray() } } },
                { "m_Cargo", new JArray() }
            };
            lootToReceive.Add("m_Colony", args.References.CreateReference(lootToReceive, colony));
            colony.Add("LootToReceive", lootToReceive);
            colony.Add("Consumables", new JArray());
            colony.Add("FinishedProjectsSinceLastVisit", new JArray());
            args.Obj.Add("Colony", colony);
        }
        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameDataObject> current)
        {
            var res = new List<IBlueprintMetadataEntry>();
            foreach (var colony in Colonies) {
                if (current.Any(c => ((W40KRTColonyModel)c.Ref).Blueprint.Eq(colony.BlueprintId))) continue;
                if (Res.Blueprints.TryGet(colony.BlueprintId, out var metadataEntry)) {
                    res.Add(metadataEntry);
                }
            }
            return res;
        }

        private static IReadOnlyList<ColonyDefinition> Colonies { get; } = new[] {
            new ColonyDefinition("883a5e2be9384e9f941ec3abf65f5d73", "d738b19f167343f6a975a8f3398571d3", "bbc87ed0a043441589c8db7f8657a8df", new [] {
                new ColonyDefinitionResource("7d5cb1d2264a40d8bff8350a8a09a464", 1)
            }, new [] {
                new ColonyDefinitionEvent("899737234f8a4895898ecd649cc197ed", segments: 1000),
                new ColonyDefinitionEvent("e7552c4110ea4cd0b1edaf7f78413a7b", segments: 1250),
                new ColonyDefinitionEvent("a32805098cc446c6964bbbefab263131", segments: 1500),
                new ColonyDefinitionEvent("097e2cd0274344819da1a46779560b36", segments: 2500),
                new ColonyDefinitionEvent("b89c52fc70cc4862aa96c8ac0c3e407d", segments: 2750),
                new ColonyDefinitionEvent("408f8a033ea54a0e9600d79f41631ee9", segments: 3000),
                new ColonyDefinitionEvent("71201d62ee314dc4a3a899ac1b4f2e22", segments: 3500),
                new ColonyDefinitionEvent("43ba5100cc014bf7ad26873adb58b1a1", segments: 5500),
                new ColonyDefinitionEvent("b42a92c6b12c4704b8e2ee425bdf5091", segments: 1750),
                new ColonyDefinitionEvent("c7f729920ae84735a050a3e0395fc9cd", segments: 2250),
            })
        };
        private static IReadOnlyDictionary<string, ColonyDefinition> ColonyLookup { get; } = Colonies.ToDictionary(c => c.BlueprintId, StringComparer.Ordinal);
        private class ColonyDefinition
        {
            public ColonyDefinition(string blueprintId, string areaId, string planetId, IReadOnlyList<ColonyDefinitionResource> resources, IReadOnlyList<ColonyDefinitionEvent> events)
            {
                BlueprintId = blueprintId;
                AreaId = areaId;
                PlanetId = planetId;
                Resources = resources;
                Events = events;
            }

            public string BlueprintId { get; }
            public string AreaId { get; }
            public string PlanetId { get; }
            public IReadOnlyList<ColonyDefinitionResource> Resources { get; }
            public IReadOnlyList<ColonyDefinitionEvent> Events { get; }
        }

        private class ColonyDefinitionResource
        {
            public ColonyDefinitionResource(string blueprint, int count)
            {
                Blueprint = blueprint;
                Count = count;
            }

            public string Blueprint { get; }
            public int Count { get; }
        }

        private class ColonyDefinitionEvent
        {
            public ColonyDefinitionEvent(string blueprint, int segments)
            {
                Blueprint = blueprint;
                Segments = segments;
            }

            public string Blueprint { get; }
            public int Segments { get; }
        }
    }
}