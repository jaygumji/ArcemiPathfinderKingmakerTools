using Arcemi.Models.Accessors;
using Newtonsoft.Json.Linq;
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
            CustomSpells = new KingmakerCustomSpellModelSlotCollection(@ref.Value.CustomSpells, this, unit);
            MemorizedSpells = new KingmakerMemorizedSpellModelSlotCollection(@ref.Value.MemorizedSpells, this, unit);
        }

        public KeyValuePairObjectModel<CharacterSpellbookModel> Ref { get; }
        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Value.Blueprint);
        public string Blueprint => Ref.Value.Blueprint;
        public string Type => null;
        public int Level { get => Ref.Value.GetAccessor().Value<int>("m_CasterLevelInternal"); set => Ref.Value.GetAccessor().Value(value, "m_CasterLevelInternal"); }
        public bool IsModifierSupported => false;
        public string ModifierName => "<N/A>";
        public int Modifier { get; set; }

        public IGameSpellSlotCollection<IGameSpellEntry> KnownSpells { get; }
        public IGameSpellSlotCollection<IGameSpellEntry> SpecialSpells { get; }
        public IGameSpellSlotCollection<IGameCustomSpellEntry> CustomSpells { get; }
        public IGameSpellSlotCollection<IGameMemorizedSpellEntry> MemorizedSpells { get; }
        public ListValueAccessor<string> SpecialLists => Ref.Value.SpecialLists;
        public ListValueAccessor<string> OppositionSchools => Ref.Value.OppositionSchools;
        public IEnumerable<SpellIndexAccessor> SpontaneousSlots => Ref.Value.SpontaneousSlots?.Count > 0 ? Ref.Value.SpontaneousSlotsAccessors : null;
        public IGameMagicInfusionSpellSlotCollection MagicInfusions => null;

        public void EnableCustomSpells()
        {
            Ref.Value.EnableCustomSpells();
        }
    }

    internal class KingmakerLearnedSpellModelSlotCollection : IGameSpellSlotCollection<IGameSpellEntry>
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
        public bool CanAddNew => Count > 0;
        public bool CanAddReference => false;
        public bool CanRemove => Count > 0;
        public ListD2Accessor<LearnedSpellModel> Ref { get; }

        public IGameSpellEntry AddNew(int index, string blueprint)
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
        public IGameSpellEntry AddReference(int index, IGameSpellEntry reference) => throw new NotImplementedException();

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

    internal class KingmakerMemorizedSpellModelSlotCollection : IGameSpellSlotCollection<IGameMemorizedSpellEntry>
    {
        private readonly List<KingmakerGameMemorizedSpellEntry>[] _accessors;
        private readonly KingmakerGameUnitSpellBookEntry _spellbook;
        private readonly UnitEntityModel _unit;

        public KingmakerMemorizedSpellModelSlotCollection(ListD2Accessor<MemorizedSpellModel> @ref, KingmakerGameUnitSpellBookEntry spellbook, UnitEntityModel unit)
        {
            Ref = @ref;
            _spellbook = spellbook;
            _unit = unit;
            _accessors = @ref?.Select(x => x.Select(m => new KingmakerGameMemorizedSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<KingmakerGameMemorizedSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameMemorizedSpellEntry> this[int index] => _accessors[index];
        public bool CanAddNew => false;
        public bool CanAddReference => Count > 0;
        public bool CanRemove => Count > 0;
        public ListD2Accessor<MemorizedSpellModel> Ref { get; }

        public IGameMemorizedSpellEntry AddNew(int index, string blueprint)
        {
            var memIndexes = Ref[index].OrderBy(x => x.Index).ToArray();
            int memIndex = memIndexes.Length;
            for (var i = 0; i < memIndexes.Length; i++) {
                if (memIndexes[i].Index != index) {
                    memIndex = i;
                    break;
                }
            }
            var spell = Ref.Add(index, (r, o) => {
                o.Add("SpellLevel", index);
                o.Add("Type", "Common");
                o.Add("Index", memIndex);

                var spellRef = r.Create();
                spellRef.Add("Blueprint", blueprint);
                spellRef.Add("Caster", r.CreateReference(o, _unit.Descriptor.Id));
                spellRef.Add("m_ConvertedFrom", null);
                spellRef.Add("DecorationColorNumber", -1);
                spellRef.Add("DecorationBorderNumber", -1);
                spellRef.Add("m_SpellbookBlueprint", _spellbook.Blueprint);
                spellRef.Add("IsSpellCopy", false);
                spellRef.Add("Fact", null);
                spellRef.Add("MetamagicData", null);
                o.Add("Spell", spellRef);

                o.Add("Available", true);

                var linkedSlots = new JArray();
                linkedSlots.Add(r.CreateReference(linkedSlots, o));
                o.Add("LinkedSlots", linkedSlots);
                o.Add("IsOpposition", false);
            });
            var entry = new KingmakerGameMemorizedSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }

        public IGameMemorizedSpellEntry AddReference(int index, IGameSpellEntry reference)
        {
            var memIndexes = Ref[index].OrderBy(x => x.Index).ToArray();
            int memIndex = memIndexes.Length;
            for (var i = 0; i < memIndexes.Length; i++) {
                if (memIndexes[i].Index != index) {
                    memIndex = i;
                    break;
                }
            }
            var spell = Ref.Add(index, (r, o) => {
                o.Add("SpellLevel", index);
                o.Add("Type", "Common");
                o.Add("Index", memIndex);

                if (reference is KingmakerGameMemorizedSpellEntry memEntry && memEntry.Ref.Spell is null) {
                    // No spell reference, spontaneous slots?
                    o.Add("Spell", null);
                    o.Add("Available", false);
                }
                else {
                    var spellRef = r.Create();
                    spellRef.Add("Blueprint", reference.Blueprint);
                    spellRef.Add("Caster", r.CreateReference(o, _unit.Descriptor.Id));
                    spellRef.Add("m_ConvertedFrom", null);
                    spellRef.Add("DecorationColorNumber", -1);
                    spellRef.Add("DecorationBorderNumber", -1);
                    spellRef.Add("m_SpellbookBlueprint", _spellbook.Blueprint);
                    spellRef.Add("IsSpellCopy", false);
                    spellRef.Add("Fact", null);
                    if (reference is KingmakerGameCustomSpellEntry custom) {
                        spellRef.Add("MetamagicData", new JObject {
                            {"MetamagicMask", custom.Ref.MetamagicData?.MetamagicMask},
                            {"SpellLevelCost", custom.Ref.MetamagicData?.SpellLevelCost ?? 0},
                            {"HeightenLevel", custom.Ref.MetamagicData?.HeightenLevel ?? 0}
                        });
                    }
                    else if (reference is KingmakerGameMemorizedSpellEntry memSpell) {
                        if (memSpell.Ref.Spell?.MetamagicData is null) {
                            spellRef.Add("MetamagicData", null);
                        }
                        else {
                            spellRef.Add("MetamagicData", new JObject {
                                {"MetamagicMask", memSpell.Ref.Spell.MetamagicData.MetamagicMask},
                                {"SpellLevelCost", memSpell.Ref.Spell.MetamagicData.SpellLevelCost},
                                {"HeightenLevel", memSpell.Ref.Spell.MetamagicData.HeightenLevel}
                            });
                        }
                    }
                    else if (reference is KingmakerGameSpellReferenceEntry refSpell) {
                        if (refSpell.Ref.MetamagicData is object) {
                            spellRef.Add("MetamagicData", new JObject {
                                {"MetamagicMask", refSpell.Ref.MetamagicData.MetamagicMask},
                                {"SpellLevelCost", refSpell.Ref.MetamagicData.SpellLevelCost},
                                {"HeightenLevel", refSpell.Ref.MetamagicData.HeightenLevel}
                            });
                        }
                        else {
                            spellRef.Add("MetamagicData", null);
                        }
                    }
                    else {
                        spellRef.Add("MetamagicData", null);
                    }
                    o.Add("Spell", spellRef);
                    o.Add("Available", true);
                    var linkedSlots = new JArray();
                    linkedSlots.Add(r.CreateReference(linkedSlots, o));
                    o.Add("LinkedSlots", linkedSlots);
                }

                o.Add("IsOpposition", false);
            });
            var entry = new KingmakerGameMemorizedSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }

        public bool Remove(IGameMemorizedSpellEntry item)
        {
            var entry = (KingmakerGameMemorizedSpellEntry)item;
            foreach (var index in _accessors) {
                if (index.Remove(entry)) break;
            }
            return Ref.Remove(entry.Ref);
        }
    }
    internal class KingmakerGameMemorizedSpellEntry : IGameMemorizedSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameMemorizedSpellEntry(MemorizedSpellModel @ref)
        {
            Ref = @ref;
            Reference = Ref.Spell is null ? null : new KingmakerGameSpellReferenceEntry(Ref.Spell);
        }
        public string Name
        {
            get {
                if (Ref.Spell is null) return "<No Spell Reference>";
                var name = Res.Blueprints.GetNameOrBlueprint(Ref.Spell.Blueprint);
                if (Ref.Spell.MetamagicData?.MetamagicMask.HasValue() ?? false) return string.Concat(name, " (", Ref.Spell.MetamagicData.MetamagicMask, ')');
                return name;
            }
        }
        public string Blueprint => Ref.Spell?.Blueprint;
        public bool IsAvailable { get => Ref.Available; set => Ref.Available = value; }
        public IGameSpellEntry Reference { get; }

        public MemorizedSpellModel Ref { get; }
    }

    internal class KingmakerGameSpellReferenceEntry : IGameCustomSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameSpellReferenceEntry(MemorizedSpellReferenceModel @ref)
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
        public MemorizedSpellReferenceModel Ref { get; }
    }

    internal class KingmakerCustomSpellModelSlotCollection : IGameSpellSlotCollection<IGameCustomSpellEntry>
    {
        private readonly List<KingmakerGameCustomSpellEntry>[] _accessors;
        private readonly KingmakerGameUnitSpellBookEntry _spellbook;
        private readonly UnitEntityModel _unit;

        public KingmakerCustomSpellModelSlotCollection(ListD2Accessor<CustomSpellModel> @ref, KingmakerGameUnitSpellBookEntry spellbook, UnitEntityModel unit)
        {
            Ref = @ref;
            _spellbook = spellbook;
            _unit = unit;
            _accessors = @ref?.Select(x => x.Select(m => new KingmakerGameCustomSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<KingmakerGameCustomSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameCustomSpellEntry> this[int index] => _accessors[index];
        public bool CanAddNew => false;
        public bool CanAddReference => Count > 0;
        public bool CanRemove => Count > 0;
        public ListD2Accessor<CustomSpellModel> Ref { get; }

        public IGameCustomSpellEntry AddNew(int index, string blueprint)
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
                o.Add("MetamagicData", new JObject {
                    {"MetamagicMask", ""},
                    {"SpellLevelCost", 0},
                    {"HeightenLevel", 0}
                });
            });
            var entry = new KingmakerGameCustomSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }
        public IGameCustomSpellEntry AddReference(int index, IGameSpellEntry reference) => AddNew(index, reference.Blueprint);

        public bool Remove(IGameCustomSpellEntry item)
        {
            var entry = (KingmakerGameCustomSpellEntry)item;
            foreach (var index in _accessors) {
                if (index.Remove(entry)) break;
            }
            return Ref.Remove(entry.Ref);
        }
    }
    internal class KingmakerGameCustomSpellEntry : IGameCustomSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameCustomSpellEntry(CustomSpellModel @ref)
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
        public MetamagicCollection Metamagic => Ref.MetamagicData.Metamagic;
        public CustomSpellModel Ref { get; }
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