using Arcemi.Pathfinder.Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class CharacterViewModel
    {
        private readonly ISaveDataProvider main;

        public CharacterViewModel(ISaveDataProvider main, IGameResourcesProvider resources)
        {
            this.main = main;
            Resources = resources;
        }

        public bool CanEdit => main.CanEdit;
        public IEnumerable<UnitEntityModel> Characters => main.Characters;

        public IGameResourcesProvider Resources { get; }

        public bool IsPlayerButNotMainCharacter(UnitEntityModel unit)
        {
            if (!unit.IsPlayer) return false;
            if (string.IsNullOrEmpty(unit.UniqueId)) return false;
            return !string.Equals(main.Player.MainCharacterId, unit.UniqueId, StringComparison.Ordinal);
        }

        public UnitEntityModel GetOwnerOf(UnitEntityModel unit)
        {
            var petPart = unit.Parts.Items.OfType<UnitPetPartItemModel>().FirstOrDefault();
            if (petPart == null || string.IsNullOrEmpty(petPart.MasterRef?.Ref)) return null;
            var owner = Characters.FirstOrDefault(c => string.Equals(c.UniqueId, petPart.MasterRef.Ref));
            return owner;
        }

        public void SetAsHero(UnitEntityModel unit)
        {
            main.Player.MainCharacterId = unit.UniqueId;
            main.PlayerEntity = unit;
        }

        public void DeleteCharacter(UnitEntityModel unit)
        {
            main.Party.UnitEntities.Remove(unit);
        }
    }
}
