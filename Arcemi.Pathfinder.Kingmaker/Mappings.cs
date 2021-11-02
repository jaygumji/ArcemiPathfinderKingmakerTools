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

        public static readonly Dictionary<string, ClassDataMapping> Classes;
        public static readonly Dictionary<string, RaceDataMapping> Races;
        public static readonly Dictionary<string, CharacterDataMapping> Characters;
        public static readonly Dictionary<string, LeaderDataMapping> Leaders;

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

            RawItems = RawItems.LoadFromDefault();
            DescriptiveItems = DescriptiveItems.LoadFromDefault();
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
    }
}