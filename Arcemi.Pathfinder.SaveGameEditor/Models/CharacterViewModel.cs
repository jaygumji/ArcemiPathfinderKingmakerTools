using Arcemi.Pathfinder.Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class CharacterViewModel
    {
        private readonly ISaveDataProvider main;

        public CharacterViewModel(ISaveDataProvider main)
        {
            this.main = main;
        }

        public bool CanEdit => main.CanEdit;
        public IEnumerable<UnitEntityModel> Characters => main.Characters;

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

        public void DowngradeClass(UnitEntityModel unit, ClassModel cls)
        {
            if (cls.Level <= 1) return;

            var progression = unit.Descriptor.Progression;

            var level = progression.CurrentLevel;
            var clsLevel = cls.Level;
            var clsBlueprints = cls.Archetypes?.Any() ?? false
                ? new HashSet<string>(cls.Archetypes, StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal);
            clsBlueprints.Add(cls.CharacterClass);

            void RemoveProgression(ProgressionItemModel pitem, int index)
            {
                progression.Items.RemoveAt(index);
            }

            for (var i = progression.Items.Count - 1; i >= 0; i--) {
                var item = progression.Items[i];

                if (item.Value.Level != clsLevel && item.Value.Level != level) continue;

                if (!cls.IsMythic && !(item.Value.Archetypes?.Any() ?? false)) {
                    // If the item doesn't have any archetype coupled, then we base it on the total character level
                    if (item.Value.Level == level) {
                        RemoveProgression(item, i);
                    }
                    continue;
                }

                if (item.Value.Level == clsLevel && (item.Value.Archetypes?.Any(a => clsBlueprints.Contains(a)) ?? false)) {
                    RemoveProgression(item, i);
                }
            }

            var clsLevelStr = clsLevel.ToString();
            var levelStr = level.ToString();
            for (var i = progression.Selections.Count - 1; i >= 0; i--) {
                var selection = progression.Selections[i];

                if (selection.Value.ByLevel.ContainsKey(clsLevelStr) && clsBlueprints.Contains(selection.Value.Source.Blueprint)) {
                    selection.Value.ByLevel.Remove(clsLevelStr);
                    if (selection.Value.ByLevel.Count == 0) {
                        progression.Selections.RemoveAt(i);
                    }
                    continue;
                }

                if (!cls.IsMythic && selection.Value.ByLevel.ContainsKey(levelStr)) {
                    selection.Value.ByLevel.Remove(levelStr);
                    if (selection.Value.ByLevel.Count == 0) {
                        progression.Selections.RemoveAt(i);
                    }
                }
            }

            cls.Level = clsLevel - 1;
        }
    }
}
