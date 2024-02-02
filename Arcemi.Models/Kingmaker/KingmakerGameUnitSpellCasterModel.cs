using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitSpellCasterModel : IGameUnitSpellCasterModel
    {
        public KingmakerGameUnitSpellCasterModel(UnitEntityModel @ref)
        {
            Ref = @ref;
            BonusSpells = new KingmakerGameUnitSpellCasterBonusSpellModel(@ref);
            SpellBooks = new GameModelCollection<IGameUnitSpellBookEntry, KeyValuePairObjectModel<CharacterSpellbookModel>>(@ref.Descriptor.Spellbooks, m => new KingmakerGameUnitSpellBookEntry(m, @ref));
        }

        public UnitEntityModel Ref { get; }

        public IGameUnitSpellCasterBonusSpellModel BonusSpells { get; }
        public IGameModelCollection<IGameUnitSpellBookEntry> SpellBooks { get; }

        public bool IsSupported => true;
    }

    public class KingmakerGameUnitSpellBookEntry : IGameUnitSpellBookEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;

        public KingmakerGameUnitSpellBookEntry(KeyValuePairObjectModel<CharacterSpellbookModel> @ref, UnitEntityModel unit)
        {
            Ref = @ref;
            KnownSpells = new KingmakerLearnedSpellModelSlotCollection(@ref.Value.KnownSpells, this, unit);
            SpecialSpells = new KingmakerLearnedSpellModelSlotCollection(@ref.Value.SpecialSpells, this, unit);
        }

        public KeyValuePairObjectModel<CharacterSpellbookModel> Ref { get; }
        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Value.Blueprint);
        public string Blueprint => Ref.Value.Blueprint;
        public string Type => null;
        public int Level { get => Ref.Value.GetAccessor().Value<int>("m_CasterLevelInternal"); set => Ref.Value.GetAccessor().Value(value, "m_CasterLevelInternal"); }
        public bool IsModifierSupported => false;
        public string ModifierName => "<N/A>";
        public int Modifier { get; set; }

        public ISlotCollection<IGameSpellEntry> KnownSpells { get; }
        public ISlotCollection<IGameSpellEntry> SpecialSpells { get; }
        public ListD2Accessor<CustomSpellModel> CustomSpells => Ref.Value.CustomSpells;
        public ListD2Accessor<MemorizedSpellModel> MemorizedSpells => Ref.Value.MemorizedSpells;
        public ListValueAccessor<string> SpecialLists => Ref.Value.SpecialLists;
        public ListValueAccessor<string> OppositionSchools => Ref.Value.OppositionSchools;
        public IEnumerable<SpellIndexAccessor> SpontaneousSlots => Ref.Value.SpontaneousSlots?.Count > 0 ? Ref.Value.SpontaneousSlotsAccessors : null;

        public void EnableCustomSpells()
        {
            Ref.Value.EnableCustomSpells();
        }
    }

    internal class KingmakerLearnedSpellModelSlotCollection : ISlotCollection<IGameSpellEntry>
    {
        private readonly List<KingmakerGameLearnedSpellEntry>[] _accessors;
        private readonly KingmakerGameUnitSpellBookEntry _spellbook;
        private readonly UnitEntityModel _unit;

        public KingmakerLearnedSpellModelSlotCollection(ListD2Accessor<LearnedSpellModel> @ref, KingmakerGameUnitSpellBookEntry spellbook, UnitEntityModel unit)
        {
            Ref = @ref;
            _spellbook = spellbook;
            _unit = unit;
            _accessors = @ref?.Select(x => x.Select(m => new KingmakerGameLearnedSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<KingmakerGameLearnedSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameSpellEntry> this[int index] => _accessors[index];
        public bool CanModify => Count > 0;
        public ListD2Accessor<LearnedSpellModel> Ref { get; }

        public IGameSpellEntry Add(int index, string blueprint)
        {
            var spell = Ref.Add(index, (r, o) => {
                o.Add("Blueprint", blueprint);
                o.Add("Caster", r.CreateReference(o, _unit.Descriptor.Id));
                o.Add("m_ConvertedFrom", null);
                o.Add("DecorationColorNumber", -1);
                o.Add("DecorationBorderNumber", -1);
                o.Add("m_SpellbookBlueprint", _spellbook.Blueprint);
                o.Add("IsSpellCopy", false);
                o.Add("Fact", null);
                o.Add("MetamagicData", null);
            });
            var entry = new KingmakerGameLearnedSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }

        public bool Remove(IGameSpellEntry item)
        {
            var entry = (KingmakerGameLearnedSpellEntry)item;
            foreach (var index in _accessors) {
                if (index.Remove(entry)) break;
            }
            return Ref.Remove(entry.Ref);
        }
    }
    internal class KingmakerGameLearnedSpellEntry : IGameSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameLearnedSpellEntry(LearnedSpellModel @ref)
        {
            Ref = @ref;
        }
        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Blueprint);
        public string Blueprint => Ref.Blueprint;

        public LearnedSpellModel Ref { get; }
    }

    internal class KingmakerGameUnitSpellCasterBonusSpellModel : IGameUnitSpellCasterBonusSpellModel
    {
        public KingmakerGameUnitSpellCasterBonusSpellModel(UnitEntityModel @ref)
        {
            Ref = @ref;
        }

        public bool IsUnlocked => false;
        public IEnumerable<SpellIndexAccessor> Accessors => Array.Empty<SpellIndexAccessor>();
        public bool IsSupported => false;
        public UnitEntityModel Ref { get; }

        public void Unlock()
        {
        }
    }
}