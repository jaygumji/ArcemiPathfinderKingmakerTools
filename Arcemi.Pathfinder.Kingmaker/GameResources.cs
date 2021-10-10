using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class GameResources : IGameResourcesProvider
    {
        public PathfinderAppData AppData { get; set; }
        public BlueprintData Blueprints { get; set; }

        public IReadOnlyList<Portrait> GetAvailableFor(string characterBlueprint)
        {
            if (AppData.Portraits.TryGetPortrait(characterBlueprint, out var portrait)) {
                return new[] { portrait }.Concat(AppData.Portraits.Available).ToArray();
            }
            else {
                return AppData.Portraits.Available;
            }
        }

        public string GetLeaderPortraitUri(string blueprint)
        {
            if (TryGetLeader(blueprint, out var leader)) {
                return GetPortraitsUri(leader.Portrait);
            }
            return GetPortraitsUri(blueprint);
        }

        public string GetPortraitsUri(string id)
        {
            return AppData.Portraits.GetPortraitsUri(id);
        }

        public bool TryGetPortraitsUri(string key, out string uri)
        {
            return AppData.Portraits.TryGetPortraitsUri(key, out uri);
        }

        public string GetPortraitId(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) {
                return null;
            }

            if (Mappings.Leaders.TryGetValue(blueprint, out var leader)) {
                return leader.Portrait.OrIfEmpty("_c_" + leader.Name);
            }
            if (Mappings.Characters.TryGetValue(blueprint, out var character)) {
                return "_c_" + character.Name;
            }
            return null;
        }

        public string GetCharacterPotraitIdentifier(string blueprint)
        {
            if (Mappings.Characters.TryGetValue(blueprint, out var character)) {
                if (!string.IsNullOrEmpty(character.Portrait)) {
                    return character.Portrait;
                }
            }
            return blueprint;
        }

        public string GetCharacterName(string blueprint)
        {
            return Mappings.Characters.TryGetValue(blueprint, out var character)
                ? character.Name
                : Blueprints.TryGetName(blueprint, out var name) ? BlueprintDisplayName.Transform(name, suffix: "Companion") : "";
        }

        public string GetArmyUnitName(string blueprint)
        {
            return Mappings.Resources.TryGetValue(blueprint, out var armyUnit)
                ? armyUnit.Name
                : Blueprints.TryGetName(blueprint, out var name) ? name : blueprint;
        }

        public IEnumerable<IBlueprint> GetAvailableArmyUnits()
        {
            var blueprints = Blueprints.GetEntries(BlueprintTypes.Unit);
            if (blueprints.Count > 0) {
                return blueprints.Where(b => b.Name.StartsWith("Army", StringComparison.Ordinal));
            }
            return GetResources(ResourceMappingType.ArmyUnit);
        }

        public IEnumerable<IBlueprint> GetBlueprints(string type)
        {
            if (string.IsNullOrEmpty(type)) return Array.Empty<IBlueprint>();
            return Blueprints.GetEntries(type);
        }

        public IEnumerable<ResourceMapping> GetResources(ResourceMappingType type)
        {
            return Mappings.Resources.Values.Where(x => x.Type == type).ToArray();
        }

        public string GetFactName(string blueprint)
        {
            return Mappings.Resources.TryGetValue(blueprint, out var res)
                ? res.Name
                : Blueprints.TryGetName(blueprint, out var name) ? name : blueprint;
        }

        public bool TryGetLeader(string blueprint, out LeaderDataMapping leader)
        {
            return Mappings.Leaders.TryGetValue(blueprint, out leader);
        }

        public string GetLeaderName(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) {
                return "";
            }

            if (Mappings.Leaders.TryGetValue(blueprint, out var leader)) {
                return leader.Name;
            }
            if (Mappings.Characters.TryGetValue(blueprint, out var character)) {
                return character.Name;
            }
            if (Blueprints.TryGetName(blueprint, out var name)) {
                return name;
            }
            return "";
        }

        public string GetRaceName(string id)
        {
            if (string.IsNullOrEmpty(id)) return "N/A";
            return Mappings.Races.TryGetValue(id, out var m) ? m.Name : Blueprints.TryGetName(id, out var name) ? name : id;
        }

        public string GetClassTypeName(string id)
        {
            return Mappings.Classes.TryGetValue(id, out var mapping)
                ? mapping.Name
                : Blueprints.TryGetName(id, out var name) ? name : id;
        }

        public string GetClassArchetypeName(IReadOnlyList<string> archetypes)
        {
            if (archetypes == null || archetypes.Count == 0) return null;

            var name = archetypes
                .Select(a => Mappings.Classes.TryGetValue(a, out var cls) ? cls.Name : Blueprints.TryGetName(a, out var n) ? n : null)
                .Where(a => a != null)
                .FirstOrDefault();

            if (name != null) return name;

            return archetypes.First();
        }

        public bool IsMythicClass(string blueprint)
        {
            return Mappings.Classes.TryGetValue(blueprint, out var cls) && (cls.Flags?.Contains("M") ?? false);
        }

        public string GetItemName(string blueprint)
        {
            return Blueprints.TryGetName(blueprint, out var name) ? name : null;
        }
    }
}
