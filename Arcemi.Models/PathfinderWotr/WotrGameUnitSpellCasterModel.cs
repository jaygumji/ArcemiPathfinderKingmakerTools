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
            CustomSpells = new WotrCustomSpellModelSlotCollection(@ref.Value.CustomSpells);
            MemorizedSpells = new WotrMemorizedSpellModelSlotCollection(@ref.Value.MemorizedSpells);
        }

        public KeyValuePairObjectModel<CharacterSpellbookModel> Ref { get; }
        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Value.Blueprint);
        public string Type => Ref.Value.Type;
        public int Level { get => Ref.Value.BaseLevelInternal; set => Ref.Value.BaseLevelInternal = value; }
        public bool IsModifierSupported => true;
        public string ModifierName => "Mythic";
        public int Modifier { get => Ref.Value.MythicLevelInternal; set => Ref.Value.MythicLevelInternal = value; }

        public IGameSpellSlotCollection<IGameSpellEntry> KnownSpells { get; }
        public IGameSpellSlotCollection<IGameSpellEntry> SpecialSpells { get; }
        public IGameSpellSlotCollection<IGameCustomSpellEntry> CustomSpells { get; }
        public IGameSpellSlotCollection<IGameMemorizedSpellEntry> MemorizedSpells { get; }
        public ListValueAccessor<string> SpecialLists => Ref.Value.SpecialLists;
        public ListValueAccessor<string> OppositionSchools => Ref.Value.OppositionSchools;
        public IEnumerable<SpellIndexAccessor> SpontaneousSlots => Ref.Value.SpontaneousSlots?.Count > 0 ? Ref.Value.SpontaneousSlotsAccessors : null;

        public void EnableCustomSpells()
        {
            Ref.Value.EnableCustomSpells();
        }
    }

    internal class WotrLearnedSpellModelSlotCollection : IGameSpellSlotCollection<IGameSpellEntry>
    {
        private readonly List<WotrGameLearnedSpellEntry>[] _accessors;

        public WotrLearnedSpellModelSlotCollection(ListD2Accessor<LearnedSpellModel> @ref)
        {
            Ref = @ref;
            _accessors = @ref?.Select(x => x.Select(m => new WotrGameLearnedSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<WotrGameLearnedSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameSpellEntry> this[int index] => _accessors[index];
        public bool CanAddNew => Count > 0;
        public bool CanAddReference => false;
        public bool CanRemove => Count > 0;
        public ListD2Accessor<LearnedSpellModel> Ref { get; }

        public IGameSpellEntry AddNew(int index, string blueprint)
        {
            var spell = Ref.Add(index);
            spell.Blueprint = blueprint;
            spell.CopiedFromScroll = false;
            spell.UniqueId = Guid.NewGuid().ToString();

            var entry = new WotrGameLearnedSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }

        public IGameSpellEntry AddReference(int index, IGameSpellEntry entry) => throw new NotImplementedException();

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

    internal class WotrMemorizedSpellModelSlotCollection : IGameSpellSlotCollection<IGameMemorizedSpellEntry>
    {
        private readonly List<WotrGameMemorizedSpellEntry>[] _accessors;

        public WotrMemorizedSpellModelSlotCollection(ListD2Accessor<MemorizedSpellModel> @ref)
        {
            Ref = @ref;
            _accessors = @ref?.Select(x => x.Select(m => new WotrGameMemorizedSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<WotrGameMemorizedSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameMemorizedSpellEntry> this[int index] => _accessors[index];
        public bool CanAddNew => false;
        public bool CanAddReference => false;
        public bool CanRemove => false;
        public ListD2Accessor<MemorizedSpellModel> Ref { get; }

        public IGameMemorizedSpellEntry AddNew(int index, string blueprint)
        {
            throw new NotImplementedException();
            //var spell = Ref.Add(index);

            //var entry = new WotrGameMemorizedSpellEntry(spell);
            //_accessors[index].Add(entry);
            //return entry;
        }

        public IGameMemorizedSpellEntry AddReference(int index, IGameSpellEntry entry) => throw new NotImplementedException();

        public bool Remove(IGameMemorizedSpellEntry item)
        {
            var entry = (WotrGameMemorizedSpellEntry)item;
            foreach (var index in _accessors) {
                if (index.Remove(entry)) break;
            }
            return Ref.Remove(entry.Ref);
        }
    }
    internal class WotrGameMemorizedSpellEntry : IGameMemorizedSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameMemorizedSpellEntry(MemorizedSpellModel @ref)
        {
            Ref = @ref;
        }
        public string Name
        {
            get {
                if (Ref.Spell is null) return "<Unknown>";
                var name = Res.Blueprints.GetNameOrBlueprint(Ref.Spell.Blueprint);
                if (Ref.Spell.MetamagicData?.MetamagicMask.HasValue() ?? false) return string.Concat(name, " (", Ref.Spell.MetamagicData.MetamagicMask, ')');
                return name;
            }
        }

        public string Blueprint => Ref.Spell?.Blueprint;
        public bool IsAvailable { get => Ref.Available; set => Ref.Available = value; }

        public MemorizedSpellModel Ref { get; }
    }

    internal class WotrCustomSpellModelSlotCollection : IGameSpellSlotCollection<IGameCustomSpellEntry>
    {
        private readonly List<WotrGameCustomSpellEntry>[] _accessors;

        public WotrCustomSpellModelSlotCollection(ListD2Accessor<CustomSpellModel> @ref)
        {
            Ref = @ref;
            _accessors = @ref?.Select(x => x.Select(m => new WotrGameCustomSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<WotrGameCustomSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameCustomSpellEntry> this[int index] => _accessors[index];
        public bool CanAddNew => Count > 0;
        public bool CanAddReference => false;
        public bool CanRemove => Count > 0;
        public ListD2Accessor<CustomSpellModel> Ref { get; }

        public IGameCustomSpellEntry AddNew(int index, string blueprint)
        {
            var spell = Ref.Add(index);
            spell.Blueprint = blueprint;
            spell.UniqueId = Guid.NewGuid().ToString();
            spell.DecorationBorderNumber = 0;
            spell.DecorationColorNumber = 0;
            spell.MetamagicData.SpellLevelCost = 0;
            spell.MetamagicData.MetamagicMask = "";

            var entry = new WotrGameCustomSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }
        public IGameCustomSpellEntry AddReference(int index, IGameSpellEntry reference) => AddNew(index, reference.Blueprint);

        public bool Remove(IGameCustomSpellEntry item)
        {
            var entry = (WotrGameCustomSpellEntry)item;
            foreach (var index in _accessors) {
                if (index.Remove(entry)) break;
            }
            return Ref.Remove(entry.Ref);
        }
    }
    internal class WotrGameCustomSpellEntry : IGameCustomSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameCustomSpellEntry(CustomSpellModel @ref)
        {
            Ref = @ref;
        }
        public string Name
        {
            get {
                var name = Res.Blueprints.GetNameOrBlueprint(Ref.Blueprint);
                if (Ref.MetamagicData?.MetamagicMask.HasValue() ?? false) return string.Concat(name, " (", Ref.MetamagicData.MetamagicMask, ')');
                return name;
            }
        }
        public string Blueprint => Ref.Blueprint;
        public int DecorationColor { get => Ref.DecorationColorNumber; set => Ref.DecorationColorNumber = value; }
        public int DecorationBorder { get => Ref.DecorationBorderNumber; set => Ref.DecorationBorderNumber = value; }
        public int SpellLevelCost { get => Ref.MetamagicData.SpellLevelCost; set => Ref.MetamagicData.SpellLevelCost = value; }
        public int HeightenLevel { get => Ref.MetamagicData.HeightenLevel; set => Ref.MetamagicData.HeightenLevel = value; }
        public MetamagicCollection Metamagic => Ref.MetamagicData?.Metamagic;

        public CustomSpellModel Ref { get; }
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