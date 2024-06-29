using Arcemi.Models.Kingmaker;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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
        private const string MagicDeceiverSpellbookBlueprint = "587066af76a74f47a904bb017697ba08";
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;

        public WotrGameUnitSpellBookEntry(KeyValuePairObjectModel<CharacterSpellbookModel> @ref)
        {
            Ref = @ref;
            KnownSpells = new WotrLearnedSpellModelSlotCollection(@ref.Value.KnownSpells);
            SpecialSpells = new WotrLearnedSpellModelSlotCollection(@ref.Value.SpecialSpells);
            CustomSpells = new WotrCustomSpellModelSlotCollection(@ref.Value.CustomSpells);
            MemorizedSpells = new WotrMemorizedSpellModelSlotCollection(@ref.Value.MemorizedSpells);
            if (Ref.Value.Blueprint.IEq(MagicDeceiverSpellbookBlueprint)) {
                MagicInfusions = new WotrGameMagicInfusionSpellSlotCollection(@ref.Value.CustomSpells);
            }
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
        public IGameMagicInfusionSpellSlotCollection MagicInfusions { get; }

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
                o.Add("Index", memIndex);
                o.Add("Available", true);

                var spellRef = r.Create();
                spellRef.Add("Blueprint", blueprint);
                spellRef.Add("DecorationColorNumber", 0);
                spellRef.Add("DecorationBorderNumber", 0);
                spellRef.Add("MetamagicData", null);
                o.Add("Spell", spellRef);

                var linkedSlots = new JArray();
                linkedSlots.Add(r.CreateReference(linkedSlots, o));
                o.Add("LinkedSlots", linkedSlots);
            });
            var entry = new WotrGameMemorizedSpellEntry(spell);
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
                o.Add("Index", memIndex);

                if (reference is WotrGameMemorizedSpellEntry memEntry && memEntry.Ref.Spell is null) {
                    // No spell reference, spontaneous slots?
                    o.Add("Spell", null);
                    o.Add("Available", false);
                }
                else {
                    if (reference is WotrGameLearnedSpellEntry learnedSpell) {
                        o.Add("Spell", r.CreateReference(o, learnedSpell.Ref.Id));
                    }
                    else if (reference is WotrGameCustomSpellEntry custom) {
                        o.Add("Spell", r.CreateReference(o, custom.Ref.Id));
                    }
                    else if (reference is WotrGameMemorizedSpellEntry memSpell) {
                        o.Add("Spell", r.CreateReference(o, memSpell.Ref.Spell.Id));
                    }
                    else if (reference is WotrGameSpellReferenceEntry refSpell) {
                        o.Add("Spell", r.CreateReference(o, refSpell.Ref.Id));
                    }
                    o.Add("Available", true);
                    var linkedSlots = new JArray();
                    linkedSlots.Add(r.CreateReference(linkedSlots, o));
                    o.Add("LinkedSlots", linkedSlots);
                }
            });
            var entry = new WotrGameMemorizedSpellEntry(spell);
            _accessors[index].Add(entry);
            return entry;
        }

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
            Reference = Ref.Spell is null ? null : new WotrGameSpellReferenceEntry(Ref.Spell);
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

        public MemorizedSpellModel Ref { get; }

        public IGameSpellEntry Reference { get; }
    }

    internal class WotrGameSpellReferenceEntry : IGameCustomSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameSpellReferenceEntry(MemorizedSpellReferenceModel @ref)
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

    internal class WotrGameMagicInfusionSpellSlotCollection : IGameMagicInfusionSpellSlotCollection
    {
        internal static readonly ISet<string> MagicHackSlots = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "4fcc9f0db7f6495497e5163b71aa7bcc", // Slot 1
            "3115ca61fe804ec985f489b656a1646d", // Slot 2
            "0aa380aef40148c3b309d45cbc9649b0", // Slot 3
            "58682332276a43bbb02440cb714f4bf4", // Slot 4
            "aa8c1ef8bd1c4eeba4844f838f0a0375", // Slot 5
            "e6c6c67ec3e347f09506fa20ec06a58a", // Slot 6
            "ee7976f42ef4444293c45e0bfcf718fe", // Slot 7
            "0caa1806c4bb476a852e9d2c4b6c8d55", // Slot 8
            "0335ef23b97c4eb39e036c4262cde7d6", // Slot 9
            "5d8a76c7e3ba4446b0657fb6cf3f9416", // Slot 10
        };
        private const string BurningHandsBlueprint = "4783c3709a74a794dbe7c8e7e0b1b038";
        private const string SnowballBlueprint = "9f10909f0be1f5141bf1c102041f93d9";

        private readonly WotrGameMagicInfusionSpellEntry[] _accessors;

        public WotrGameMagicInfusionSpellSlotCollection(ListD2Accessor<CustomSpellModel> @ref)
        {
            Ref = @ref;
            _accessors = new WotrGameMagicInfusionSpellEntry[10];
            foreach (var entry in @ref?.SelectMany(x => x.Where(m => MagicHackSlots.Contains(m.Blueprint)).Select(m => new WotrGameMagicInfusionSpellEntry(m)))) {
                _accessors[entry.SlotId - 1] = entry;
            }
        }

        public bool CanAdd => _accessors.Any(x => x is null);
        public IReadOnlyList<IGameMagicInfusionSpellEntry> Slots => _accessors;

        public ListD2Accessor<CustomSpellModel> Ref { get; }

        private WotrGameMagicInfusionSpellEntry CreateEntry(int spellLevel, string blueprint)
        {
            var spell = Ref.Add(spellLevel, (r, o) => {
                o.Add("Blueprint", blueprint);
                o.Add("DecorationColorNumber", 0);
                o.Add("DecorationBorderNumber", 0);
                //o.Add("HasActionBarSlot", 0);
                //o.Add("TemporarilyDisabled", false);
                o.Add("MagicHackData", new JObject {
                        { "Spell1", BurningHandsBlueprint },
                        { "Spell2", SnowballBlueprint },
                        { "SpellLevel", spellLevel },
                        { "SpellSchool", "Evocation" },
                        { "SavingThrowType", "Reflex" },
                        { "SpellTargetType", "Cone" },
                        { "IsTouch", false },
                        { "DeliverBlueprint", BurningHandsBlueprint },
                        { "AdditionalAoeBlueprint", null },
                    });
            });
            var entry = new WotrGameMagicInfusionSpellEntry(spell);
            return entry;
        }

        public IGameMagicInfusionSpellEntry Add()
        {
            if (!CanAdd) throw new InvalidOperationException("Can't add more magic infusions");
            var slotIndex = 0;
            var blueprint = "";
            foreach (var slotBlueprint in MagicHackSlots) {
                blueprint = slotBlueprint;
                if (_accessors[slotIndex] is null) break;
                slotIndex++;
            }
            var entry = CreateEntry(1, blueprint);
            _accessors[slotIndex] = entry;
            return entry;
        }

        public void Remove(IGameMagicInfusionSpellEntry entry)
        {
            _accessors[entry.SlotId - 1] = null;
            Ref.Remove(((WotrGameMagicInfusionSpellEntry)entry).Ref);
        }

        public IEnumerator<IGameMagicInfusionSpellEntry> GetEnumerator()
        {
            for (var i = 0; i < _accessors.Length; i++) {
                var entry = _accessors[i];
                if (entry is null) continue;
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SetSpellLevel(IGameMagicInfusionSpellEntry spell, int level)
        {
            var entry = CreateEntry(level, ((WotrGameMagicInfusionSpellEntry)spell).Ref.Blueprint);
            entry.Spell1Blueprint = spell.Spell1Blueprint;
            entry.Spell2Blueprint = spell.Spell2Blueprint;
            entry.DeliverBlueprint = spell.DeliverBlueprint;
            entry.AdditionalAoeBlueprint = spell.AdditionalAoeBlueprint;
            entry.IsTouch = spell.IsTouch;
            entry.SavingThrowType = spell.SavingThrowType;
            entry.SpellSchool = spell.SpellSchool;
            entry.SpellTargetType = spell.SpellTargetType;
            Remove(spell);
            _accessors[spell.SlotId - 1] = entry;
        }
    }

    internal class WotrGameMagicInfusionSpellEntry : IGameMagicInfusionSpellEntry
    {
        private IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public CustomSpellModel Ref { get; }
        public ModelDataAccessor A { get; }

        public WotrGameMagicInfusionSpellEntry(CustomSpellModel @ref)
        {
            Ref = @ref;
            A = @ref.GetAccessor().Object<Model>("MagicHackData").GetAccessor();
            SlotId = 1;
            foreach (var blueprint in WotrGameMagicInfusionSpellSlotCollection.MagicHackSlots) {
                if (blueprint.IEq(@ref.Blueprint)) break;
                SlotId++;
            }
            SpellSchools = WotrSpellSchools.Get(SpellSchool);
            SavingThrowTypes = WotrSavingThrowTypes.Get(SavingThrowType);
            SpellTargetTypes = WotrSpellTargetTypes.Get(SpellTargetType);
        }
        public int SlotId { get; }

        public string Spell1Blueprint { get => A.Value<string>("Spell1"); set => A.Value(value, "Spell1"); }
        public string Spell1Name => Res.Blueprints.GetNameOrBlueprint(Spell1Blueprint);
        public string Spell2Blueprint { get => A.Value<string>("Spell2"); set => A.Value(value, "Spell2"); }
        public string Spell2Name => Res.Blueprints.GetNameOrBlueprint(Spell2Blueprint);
        public int SpellLevel { get => A.Value<int>(); set => A.Value(value); }

        public string SpellSchool { get => A.Value<string>(); set => A.Value(value); }
        public IEnumerable<DataOption> SpellSchools { get; }

        public string SavingThrowType { get => A.Value<string>(); set => A.Value(value); }
        public IEnumerable<DataOption> SavingThrowTypes { get; }

        public string SpellTargetType { get => A.Value<string>(); set => A.Value(value); }
        public IEnumerable<DataOption> SpellTargetTypes { get; }

        public bool IsTouch { get => A.Value<bool>(); set => A.Value(value); }
        public string DeliverBlueprint { get => A.Value<string>(); set => A.Value(value); }
        public string AdditionalAoeBlueprint { get => A.Value<string>(); set => A.Value(value); }
    }

    internal class WotrCustomSpellModelSlotCollection : IGameSpellSlotCollection<IGameCustomSpellEntry>
    {
        private readonly List<WotrGameCustomSpellEntry>[] _accessors;

        public WotrCustomSpellModelSlotCollection(ListD2Accessor<CustomSpellModel> @ref)
        {
            Ref = @ref;
            _accessors = @ref?.Select(x => x.Where(m => !WotrGameMagicInfusionSpellSlotCollection.MagicHackSlots.Contains(m.Blueprint)).Select(m => new WotrGameCustomSpellEntry(m)).ToList()).ToArray() ?? Array.Empty<List<WotrGameCustomSpellEntry>>();
        }
        public int Count => Ref?.Count ?? 0;
        public IReadOnlyList<IGameCustomSpellEntry> this[int index] => _accessors[index];
        public bool CanAddNew => Count > 0;
        public bool CanAddReference => true;
        public bool CanRemove => Count > 0;
        public ListD2Accessor<CustomSpellModel> Ref { get; }

        public IGameCustomSpellEntry AddNew(int index, string blueprint)
        {
            var spell = Ref.Add(index, (r, o) => {
                o.Add("Blueprint", blueprint);
                o.Add("DecorationColorNumber", 0);
                o.Add("DecorationBorderNumber", 0);
                //o.Add("HasActionBarSlot", 0);
                //o.Add("TemporarilyDisabled", false);
                o.Add("MetamagicData", new JObject {
                    {"MetamagicMask", ""},
                    {"SpellLevelCost", 0},
                    {"HeightenLevel", 0}
                });
            });
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