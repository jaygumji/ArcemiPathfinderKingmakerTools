using Arcemi.Pathfinder.Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class CharacterViewModel
    {
        private readonly MainViewModel main;

        public CharacterViewModel(MainViewModel main)
        {
            this.main = main;
        }

        public IEnumerable<UnitEntityModel> Characters => main.Characters;

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

            void RemoveProgression(ProgressionItemModel pitem, int index, bool verifyFacts)
            {
                progression.Items.RemoveAt(index);
                if (!verifyFacts) return;
                var facts = unit.Facts.Items
                    .OfType<FeatureFactItemModel>()
                    .Where(f => string.Equals(f.Source, pitem.Key, StringComparison.Ordinal));

                foreach (var fact in facts) {
                    for (var i = fact.RankToSource.Count - 1; i >= 0; i--) {
                        var rank = fact.RankToSource[i];
                        if (rank.Level == clsLevel) {
                            fact.RankToSource.RemoveAt(i);
                            fact.Rank--;
                            fact.SourceLevel = fact.RankToSource.Count > 0 ? fact.RankToSource.Max(r => r.Level) : 1;
                        }
                    }
                }
            }

            for (var i = progression.Items.Count - 1; i >= 0; i--) {
                var item = progression.Items[i];

                if (item.Value.Level != clsLevel && item.Value.Level != level) continue;

                if (!(item.Value.Archetypes?.Any() ?? false)) {
                    // If the item doesn't have any archetype coupled, then we base it on the total character level
                    if (item.Value.Level == level) {
                        RemoveProgression(item, i, verifyFacts: false);
                    }
                    continue;
                }

                if (item.Value.Level == clsLevel && item.Value.Archetypes.Any(a => clsBlueprints.Contains(a))) {
                    RemoveProgression(item, i, verifyFacts: true);
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

                if (selection.Value.ByLevel.ContainsKey(levelStr)) {
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
