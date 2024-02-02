using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitSpellCasterModel : IGameUnitSpellCasterModel
    {
        public WotrGameUnitSpellCasterModel(UnitEntityModel @ref)
        {
            Ref = @ref;
            BonusSpells = new WotrGameUnitSpellCasterBonusSpellModel(@ref);
            SpellBooks = new GameModelCollection<IGameUnitSpellBookEntry, KeyValuePairObjectModel<CharacterSpellbookModel>>(@ref.Descriptor.Spellbooks, m => new WotrGameUnitSpellBookEntry(m));
        }

        public UnitEntityModel Ref { get; }

        public IGameUnitSpellCasterBonusSpellModel BonusSpells { get; }
        public IGameModelCollection<IGameUnitSpellBookEntry> SpellBooks { get; }

        public bool IsSupported => true;
    }

    public class WotrGameUnitSpellBookEntry : IGameUnitSpellBookEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;

        public WotrGameUnitSpellBookEntry(KeyValuePairObjectModel<CharacterSpellbookModel> @ref)
        {
            Ref = @ref;
            KnownSpells = new WotrLearnedSpellModelSlotCollection(@ref.Value.KnownSpells);
            SpecialSpells = new WotrLearnedSpellModelSlotCollection(@ref.Value.SpecialSpells);
        }

        public KeyValuePairObjectModel<CharacterSpellbookModel> Ref { get; }
        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Value.Blueprint);
        public string Type => Ref.Value.Type;
        public int Level { get => Ref.Value.BaseLevelInternal; set => Ref.Value.BaseLevelInternal = value; }
        public bool IsModifierSupported => true;
        public string ModifierName => "Mythic";
        public int Modifier { get => Ref.Value.MythicLevelInternal; set => Ref.Value.MythicLevelInternal = value; }

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

    internal class WotrLearnedSpellModelSlotCollection : ISlotCollection<IGameSpellEntry>
    {
        private readonly List<WotrGameLearnedSpellEntry>[] _accessors;

        public WotrLearnedSpellModelSlotCollection(ListD2Accessor<LearnedSpellModel> @ref)
        {
            Ref = @ref;
            _accessors = @ref?.Select(x => x.Select(m => new WotrGameLearnedSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<WotrGameLearnedSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameSpellEntry> this[int index] => _accessors[index];
        public bool CanModify => Count > 0;
        public ListD2Accessor<LearnedSpellModel> Ref { get; }

        public IGameSpellEntry Add(int index, string blueprint)
        {
            var spell = Ref.Add(index);
            spell.Blueprint = blueprint;
            spell.CopiedFromScroll = false;
            spell.UniqueId = Guid.NewGuid().ToString();

            var entry = new WotrGameLearnedSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }

        public bool Remove(IGameSpellEntry item)
        {
            var entry = (WotrGameLearnedSpellEntry)item;
            foreach (var index in _accessors) {
                if (index.Remove(entry)) break;
            }
            return Ref.Remove(entry.Ref);
        }
    }
    internal class WotrGameLearnedSpellEntry : IGameSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameLearnedSpellEntry(LearnedSpellModel @ref)
        {
            Ref = @ref;
        }
        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Blueprint);
        public string Blueprint => Ref.Blueprint;

        public LearnedSpellModel Ref { get; }
    }

    internal class WotrGameUnitSpellCasterBonusSpellModel : IGameUnitSpellCasterBonusSpellModel
    {
        public WotrGameUnitSpellCasterBonusSpellModel(UnitEntityModel @ref)
        {
            Ref = @ref;
        }

        private UnitExtraSpellsPerDayPartItemModel UnitExtraSpellsPerDayPart => Ref.Parts.Items
            .OfType<UnitExtraSpellsPerDayPartItemModel>()
            .FirstOrDefault();

        public bool IsUnlocked => UnitExtraSpellsPerDayPart is object;
        public IEnumerable<SpellIndexAccessor> Accessors => UnitExtraSpellsPerDayPart.BonusSpellsAccessors;
        public bool IsSupported => true;
        public UnitEntityModel Ref { get; }

        public void Unlock()
        {
            UnitExtraSpellsPerDayPartItemModel.AddTo(Ref.Parts);
        }
    }
}