#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class CharacterModel : RefModel
    {
        public CharacterModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ProgressionModel Progression => A.Object(factory: a => new ProgressionModel(a));
        public UISettingsModel UISettings => A.Object(factory: a => new UISettingsModel(a));
        public AlignmentModel Alignment => A.Object(factory: a => new AlignmentModel(a));
        public StatsModel Stats => A.Object(factory: a => new StatsModel(a));
        public UnitModel Unit => A.Object(factory: a => new UnitModel(a));
        public InventoryModel Inventory => A.Object<InventoryModel>("m_Inventory");
        public BodyModel Body => A.Object(factory: a => new BodyModel(a));
        public CharacterResourceContainerModel Resources => A.Object(factory: a => new CharacterResourceContainerModel(a));
        public ListAccessor<KeyValuePairObjectModel<CharacterSpellbookModel>> Spellbooks => A.List("m_Spellbooks", a => new KeyValuePairObjectModel<CharacterSpellbookModel>(a, a2 => new CharacterSpellbookModel(a2)));

        public string Blueprint => A.Value<string>();
        public string CustomName { get => A.Value<string>(); set => A.Value(string.IsNullOrEmpty(value) ? null : value); }
        public string CustomAsks { get => A.Value<string>(); set => A.Value(string.IsNullOrEmpty(value) ? null : value); }

        private const string TricksterSpellbookBlueprint = "bbe483b903854104a11606412803f214";

        public bool HasTricksterSpellbook()
        {
            return Spellbooks.Any(s => string.Equals(s.Key, TricksterSpellbookBlueprint, System.StringComparison.Ordinal));
        }

        public void CreateTricksterSpellbook()
        {
            if (HasTricksterSpellbook()) return;

            var kv = Spellbooks.Add((refs, obj) => {
                obj.Add("Key", TricksterSpellbookBlueprint);
                var bookObj = refs.Create();
                obj.Add("Value", bookObj);
                bookObj.Add("m_KnownSpells", refs.NewArray(11, () => new JArray()));
                bookObj.Add("m_SpecialSpells", refs.NewArray(11, () => new JArray()));
                bookObj.Add("m_SpecialLists", new JArray());
                bookObj.Add("m_CustomSpells", refs.NewArray(11, () => new JArray()));
                bookObj.Add("m_MemorizedSpells", refs.NewArray(11, () => new JArray()));
                bookObj.Add("m_SpontaneousSlots", refs.NewArray(11, initialValue:0));
                bookObj.Add("OppositionSchools", new JArray());
                bookObj.Add("ExOppositionSchools", new JArray());
                bookObj.Add("BonusSpellSlots", refs.NewArray(11, initialValue: 0));
            });

            var book = kv.Value;
            book.BaseLevelInternal = Progression.CurrentLevel;
            book.MythicLevelInternal = 0;
            book.Type = "Normal";
            book.Blueprint = TricksterSpellbookBlueprint;
            book.OppositionDescriptors = "None";
        }
    }
}