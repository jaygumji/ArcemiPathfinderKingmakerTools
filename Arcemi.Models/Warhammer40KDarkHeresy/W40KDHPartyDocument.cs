using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHPartyDocument
    {
        internal static readonly SaveEntry<byte[]> PartyEntry = new("party.owl", SaveGameFile.ReadBytes);

        private readonly byte[] _original;
        private readonly IReadOnlyDictionary<W40KDHParsedUnitStats, IReadOnlyList<W40KDHAdvancementStat>> _advancements;

        public W40KDHPartyDocument(SaveGameFile archive)
        {
            _original = archive.Read(PartyEntry);
            Data = W40KDHOwlTraversal.ReadParty(_original);
            var writable = W40KDHOwlChecksums.CanUpdate(archive, PartyEntry.Name, _original);
            Inventory = new W40KDHInventoryModel("Party", Data.Inventory, GameDefinition.Warhammer40K_DarkHeresy.Resources, writable);
            var sharedOffsets = Data.Units.SelectMany(unit => unit.ProgressionSelections)
                .Where(fact => fact.ActiveFactRankOffset.HasValue)
                .GroupBy(fact => fact.ActiveFactRankOffset.Value)
                .Where(group => group.Count() != 1)
                .Select(group => group.Key).ToHashSet();
            var types = Data.Scene is not null ? Data.WritableFeatureTypes : null;
            _advancements = Data.Units.ToDictionary(unit => unit, unit => CreateStats(unit, writable, sharedOffsets, types));
        }

        public W40KDHPartyData Data { get; }
        public W40KDHInventoryModel Inventory { get; }
        public bool CanEdit => Inventory.CanEdit || AllAdvancements.Any(stat => !stat.IsReadOnly);

        private IEnumerable<W40KDHAdvancementStat> AllAdvancements => _advancements.Values.SelectMany(stats => stats);

        public IReadOnlyList<W40KDHAdvancementStat> GetAdvancements(W40KDHParsedUnitStats unit) => _advancements[unit];

        public void StageChanges(Dictionary<string, byte[]> changes)
        {
            if (!Inventory.HasChanges && !AllAdvancements.Any(stat => stat.HasChanged)) return;

            var bytes = (byte[])_original.Clone();
            Inventory.WriteTo(bytes);
            foreach (var stat in AllAdvancements.Where(stat => stat.HasWritableOffset)) {
                stat.WriteTo(bytes);
            }

            var pending = _advancements.Select(pair => (Unit: pair.Key, Stats: pair.Value.Where(stat => stat.NeedsInsertion).ToArray()))
                .Where(pair => pair.Stats.Length > 0).ToArray();
            if (pending.Length > 0) {
                var writer = new W40KDHOwlFeatureWriter(Data.WritableFeatureTypes, _original);
                var added = pending.Select(pair => (pair.Unit.FactList,
                    pair.Stats.Select(stat => stat.CreateFact(writer, pair.Unit.EntityId)).ToArray())).ToArray();
                bytes = W40KDHOwlFeatureWriter.Insert(bytes, Data.Scene, added);
                var result = W40KDHOwlTraversal.ReadParty(bytes);
                foreach (var (unit, stats) in pending) {
                    var reopened = result.Units.SingleOrDefault(candidate => candidate.EntityId == unit.EntityId);
                    if (reopened is null || stats.Any(stat => !reopened.ProgressionSelections.Any(stat.MatchesGrant))) {
                        throw new InvalidDataException("The new OWL advancement did not round-trip.");
                    }
                }
            }
            changes.Add(PartyEntry.Name, bytes);
        }

        public void AcceptChanges()
        {
            Inventory.AcceptChanges();
            foreach (var stat in AllAdvancements) stat.AcceptChanges();
        }

        private static IReadOnlyList<W40KDHAdvancementStat> CreateStats(
            W40KDHParsedUnitStats unit, bool writable, HashSet<int> sharedOffsets, IReadOnlyDictionary<string, OwlWritableType> types)
        {
            var selections = unit.ProgressionSelections.Where(selection => !selection.IsFeatureFact)
                .Select(selection => selection.FeatureBlueprint).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var candidates = unit.ProgressionSelections
                .Where(fact => fact.ActiveFactRankOffset.HasValue && W40KDHAdvancementCatalog.Find(fact.FeatureBlueprint) is not null)
                .GroupBy(fact => fact.FeatureBlueprint, StringComparer.OrdinalIgnoreCase);

            var stats = candidates.SelectMany(group => group.Select(fact => {
                var offset = fact.ActiveFactRankOffset.Value;
                var editable = writable && group.Count() == 1 && !sharedOffsets.Contains(offset) && !selections.Contains(group.Key);
                return new W40KDHAdvancementStat(group.Key, W40KDHAdvancementCatalog.Find(group.Key), fact.Rank, editable ? offset : null);
            })).ToList();

            var occupied = unit.ProgressionSelections.Select(fact => W40KDHAdvancementCatalog.Find(fact.FeatureBlueprint)?.Name)
                .ToHashSet(StringComparer.Ordinal);
            if (writable && types is not null && unit.FactList is not null && !string.IsNullOrEmpty(unit.EntityId)) {
                foreach (var (blueprint, definition) in W40KDHAdvancementCatalog.Grants) {
                    if (!occupied.Contains(definition.Name) && types.ContainsKey(definition.PointsPerRank == 5
                        ? W40KDHOwlFeatureWriter.AttributeType : W40KDHOwlFeatureWriter.SkillType)
                        && W40KDHAdvancementCatalog.AvailableInGame(blueprint, definition)) {
                        stats.Add(new W40KDHAdvancementStat(blueprint, definition, 0, null, canGrant: true));
                    }
                }
            }
            return stats.OrderBy(stat => stat.Name, StringComparer.Ordinal).ToArray();
        }
    }

    internal sealed class W40KDHAdvancementStat : IGameUnitStatsEntry
    {
        private readonly W40KDHAdvancementDefinition _definition;
        private readonly int? _offset;
        private readonly string _blueprint;
        private readonly string _newFactId;
        private readonly int _originalRank;
        private int _savedRank;
        private int _rank;

        public W40KDHAdvancementStat(string blueprint, W40KDHAdvancementDefinition definition, int originalRank, int? offset, bool canGrant = false)
        {
            _definition = definition;
            _blueprint = blueprint;
            if (canGrant) _newFactId = Guid.NewGuid().ToString("N");
            _originalRank = _savedRank = _rank = originalRank;
            _offset = offset;
        }

        public int Rank => _rank;
        public bool Owns(W40KDHParsedProgressionSelection fact) => _offset.HasValue && _offset == fact.ActiveFactRankOffset;

        public string Name => _definition.Name;
        public string Info => $"Rank {_rank}";
        public bool HasWritableOffset => _offset.HasValue && !IsReadOnly;
        public bool NeedsInsertion => _newFactId is not null && _rank > 0;
        public bool IsReadOnly => _newFactId is null &&
            (_offset is null || _definition.MaximumRank < 2 || _originalRank < 1 || _originalRank > _definition.MaximumRank);
        public int Step => _definition.PointsPerRank;
        public int? Minimum => _newFactId is null ? Step : 0;
        public int? Maximum => Step * _definition.MaximumRank;
        public bool HasChanged => _rank != _savedRank;

        public int Value
        {
            get => checked(_rank * Step);
            set {
                if (!TrySetValue(value)) {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Enter a multiple of {Step} from {Minimum} through {Maximum}.");
                }
            }
        }

        public bool TrySetValue(int value)
        {
            if (IsReadOnly || value < Minimum || value > Maximum || value % Step != 0) return false;
            _rank = value / Step;
            return true;
        }

        public byte[] CreateFact(W40KDHOwlFeatureWriter writer, string owner) =>
            writer.Create(owner, _newFactId, _blueprint, _definition.ComponentName, _rank, Step);

        public bool MatchesGrant(W40KDHParsedProgressionSelection fact) =>
            fact.IsFeatureFact && fact.ActiveFactRankOffset.HasValue && fact.Rank == _rank
            && string.Equals(fact.FeatureBlueprint, _blueprint, StringComparison.OrdinalIgnoreCase);

        public void WriteTo(byte[] party)
        {
            var field = party.AsSpan(_offset.Value, sizeof(int));
            if (BinaryPrimitives.ReadInt32LittleEndian(field) != _originalRank) {
                throw new InvalidDataException($"The serialized rank for {Name} differs from the value originally read.");
            }
            BinaryPrimitives.WriteInt32LittleEndian(field, _rank);
        }

        public void AcceptChanges() => _savedRank = _rank;
    }
}
