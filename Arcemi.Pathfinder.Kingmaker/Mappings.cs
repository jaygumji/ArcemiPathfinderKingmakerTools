#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{

    public static class Mappings
    {
        private static readonly Dictionary<Type, Func<ModelDataAccessor, object>> UntypedFactories;
        private static readonly Dictionary<Type, object> Factories;

        private static readonly Dictionary<BlueprintIdentifier, Type> BlueprintTypes;
        private static readonly Dictionary<Type, List<BlueprintIdentifier>> TypeToBlueprint;
        private static readonly Dictionary<string, ClassDataMapping> Classes;
        private static readonly Dictionary<string, RaceDataMapping> Races;
        private static readonly Dictionary<string, CharacterDataMapping> Characters;
        private static readonly Dictionary<string, LeaderDataMapping> Leaders;
        private static readonly Dictionary<string, ResourceMapping> Resources;

        public static RawItems RawItems { get; }
        public static DescriptiveItems DescriptiveItems { get; }

        static Mappings()
        {
            UntypedFactories = new Dictionary<Type, Func<ModelDataAccessor, object>>();
            Factories = new Dictionary<Type, object>();

            RegisterFactory(m => new HeaderModel(m));
            RegisterFactory(m => new PartyModel(m));
            RegisterFactory(m => new UnitEntityModel(m));
            RegisterFactory(m => new CharacterModel(m));
            RegisterFactory(m => new CharacterAttributeModel(m));
            RegisterFactory(m => new ClassSkillModel(m));
            RegisterFactory(m => new PlayerModel(m));
            RegisterFactory(m => new PersistentModifierModel(m));
            RegisterFactory(m => new InventoryModel(m));
            RegisterFactory(ItemModel.Create);
            RegisterFactory(m => new HoldingSlotModel(m));
            RegisterFactory(m => new HandsEquipmentSetModel(m));
            RegisterFactory(m => new PlayerKingdomLeaderModel(m));
            RegisterFactory(m => new PlayerKingdomTaskModel(m));
            RegisterFactory(m => new PlayerKingdomChangeModel(m));
            RegisterFactory(m => new PlayerKingdomEventHistoryModel(m));
            RegisterFactory(m => new PlayerKingdomEventModel(m));
            RegisterFactory(m => new PlayerKingdomRegionModel(m));
            RegisterFactory(m => new PlayerKingdomLeaderSpecificBonusModel(m));

            var dataMappings = DataMappings.LoadFromDefault();
            Classes = dataMappings.Classes
                .Concat(dataMappings.Classes.Where(c => c.Archetypes != null).SelectMany(c => c.Archetypes))
                .Where(a => !string.IsNullOrEmpty(a.Id))
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            Races = dataMappings.Races
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            Characters = dataMappings.Characters
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            Leaders = dataMappings.Leaders
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            Resources = dataMappings.Resources
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            BlueprintTypes = dataMappings.Characters.ToDictionary(x => new BlueprintIdentifier(x.Id), x => typeof(CharacterModel));
            TypeToBlueprint = BlueprintTypes
                .GroupBy(kv => kv.Value)
                .ToDictionary(g => g.Key, g => g.Select(kv => kv.Key).ToList());

            RawItems = RawItems.LoadFromDefault();
            DescriptiveItems = DescriptiveItems.LoadFromDefault();
        }

        public static string GetCharacterPotraitIdentifier(string blueprint)
        {
            var characterName = GetCharacterName(blueprint);
            var characterKey = "_c_" + characterName;
            return characterKey;
        }

        public static string GetCharacterName(string blueprint)
        {
            return Characters.TryGetValue(blueprint, out var character) ? character.Name : "";
        }

        public static string GetArmyUnitName(string blueprint)
        {
            return Resources.TryGetValue(blueprint, out var armyUnit) ? armyUnit.Name : blueprint;
        }

        public static IEnumerable<ResourceMapping> GetResources(ResourceMappingType type)
        {
            return Resources.Values.Where(x => x.Type == type).ToArray();
        }

        public static string GetFactName(string blueprint)
        {
            return Resources.TryGetValue(blueprint, out var res) ? res.Name : blueprint;
        }

        public static bool TryGetLeader(string blueprint, out LeaderDataMapping leader)
        {
            return Leaders.TryGetValue(blueprint, out leader);
        }

        public static string GetLeaderName(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) {
                return "";
            }

            if (Leaders.TryGetValue(blueprint, out var leader)) {
                return leader.Name;
            }
            if (Characters.TryGetValue(blueprint, out var character)) {
                return character.Name;
            }
            return "";
        }

        public static string GetPortraitId(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) {
                return null;
            }

            if (Leaders.TryGetValue(blueprint, out var leader)) {
                return leader.Portrait.OrIfEmpty("_c_" + leader.Name);
            }
            if (Characters.TryGetValue(blueprint, out var character)) {
                return "_c_" + character.Name;
            }
            return null;
        }

        public static bool IsPlayerCharacter(string blueprint)
        {
            var cn = GetCharacterName(blueprint);
            return string.Equals(cn, "Player", StringComparison.Ordinal);
        }

        public static bool IsCustomCharacter(string blueprint)
        {
            var cn = GetCharacterName(blueprint);
            return string.Equals(cn, "Custom", StringComparison.Ordinal);
        }

        public static bool IsCompanionCharacter(string blueprint)
        {
            return !IsPlayerCharacter(blueprint) && !IsCustomCharacter(blueprint);
        }

        public static IReadOnlyCollection<BlueprintIdentifier> GetBlueprintId<T>()
        {
            return TypeToBlueprint.TryGetValue(typeof(T), out var identifier)
                ? identifier
                : throw new ArgumentException("No blueprint registration exists for type " + typeof(T).FullName);
        }

        public static void RegisterFactory<T>(Func<ModelDataAccessor, T> factory)
        {
            UntypedFactories.Add(typeof(T), m => factory(m));
            Factories.Add(typeof(T), factory);
        }

        public static Func<ModelDataAccessor, T> GetFactory<T>()
        {
            return Factories.TryGetValue(typeof(T), out var factory)
                ? (Func<ModelDataAccessor, T>)factory
                : throw new ArgumentException("No factory registered for type " + typeof(T).FullName);
        }

        public static bool TryGetFactory(BlueprintIdentifier blueprintId, out Func<ModelDataAccessor, object> factory)
        {
            if (BlueprintTypes.TryGetValue(blueprintId, out var type)) {
                return UntypedFactories.TryGetValue(type, out factory);
            }
            factory = null;
            return false;
        }

        public static string GetRaceName(string id)
        {
            if (string.IsNullOrEmpty(id)) return "N/A";
            return Races.TryGetValue(id, out var m) ? m.Name : id;
        }

        public static string GetClassTypeName(string id)
        {
            return Classes.TryGetValue(id, out var mapping)
                ? mapping.Name
                : id;
        }

        public static string GetClassArchetypeName(IReadOnlyList<string> archetypes)
        {
            if (archetypes == null || archetypes.Count == 0) return null;

            var name = archetypes
                .Select(a => Classes.TryGetValue(a, out var cls) ? cls.Name : null)
                .Where(a => a != null)
                .FirstOrDefault();

            if (name != null) return name;

            return archetypes.First();
        }

        public static string GetClassName(string id, IReadOnlyList<string> archetypes)
        {
            var name = archetypes?
                .Select(a => Classes.TryGetValue(a, out var cls) ? cls.Name : null)
                .Where(a => a != null)
                .FirstOrDefault();

            if (name != null) return name;

            return Classes.TryGetValue(id, out var mapping)
                ? mapping.Name
                : id;
        }

    }
}