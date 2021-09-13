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
    public class ProgressionModel : RefModel
    {
        public ProgressionModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<ClassModel> Classes => A.List(factory: a => new ClassModel(a));
        public FeaturesModel Features => A.Object(factory: a => new FeaturesModel(a));
        public ListAccessor<ProgressionItemModel> Items => A.List("m_Progressions", a => new ProgressionItemModel(a));
        public ListAccessor<ProgressionSelectionModel> Selections => A.List("m_Selections", a => new ProgressionSelectionModel(a));
        public int Experience { get => A.Value<int>(); set => A.Value(value); }
        public string Race { get => A.Value<string>("m_Race"); set => A.Value(value, "m_Race"); }
        public string RaceName => Mappings.GetRaceName(Race);
        public int CurrentLevel => Classes?.Sum(c => c.Level) ?? 0;

        public void DowngradeClass(ClassModel cls)
        {
            var level = CurrentLevel;
            var clsLevel = cls.Level;
            var clsBlueprints = cls.Archetypes?.Any() ?? false
                ? new HashSet<string>(cls.Archetypes, StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal);
            clsBlueprints.Add(cls.CharacterClass);

            for (var i = Items.Count - 1; i >= 0; i--) {
                var item = Items[i];

                if (item.Value.Level != clsLevel && item.Value.Level != level) continue;

                if (!(item.Value.Archetypes?.Any() ?? false)) {
                    // If the item doesn't have any archetype coupled, then we base it on the total character level
                    if (cls.Level == level) {
                        Items.RemoveAt(i);
                    }
                    continue;
                }

                if (item.Value.Level == clsLevel && item.Value.Archetypes.Any(a => clsBlueprints.Contains(a))) {
                    Items.RemoveAt(i);
                }
            }

            var clsLevelStr = clsLevel.ToString();
            var levelStr = level.ToString();
            for (var i = Selections.Count - 1; i >= 0; i--) {
                var selection = Selections[i];

                if (selection.Value.ByLevel.ContainsKey(clsLevelStr) && clsBlueprints.Contains(selection.Value.Source.Blueprint)) {
                    selection.Value.ByLevel.Remove(clsLevelStr);
                    if (selection.Value.ByLevel.Count == 0) {
                        Selections.RemoveAt(i);
                    }
                    continue;
                }

                if (selection.Value.ByLevel.ContainsKey(levelStr)) {
                    selection.Value.ByLevel.Remove(levelStr);
                    if (selection.Value.ByLevel.Count == 0) {
                        Selections.RemoveAt(i);
                    }
                }
            }

            cls.Level = clsLevel - 1;
        }
    }
}