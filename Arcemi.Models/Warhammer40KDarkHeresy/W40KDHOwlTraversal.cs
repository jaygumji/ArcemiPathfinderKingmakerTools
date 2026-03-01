using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHParsedUnitStats
    {
        public W40KDHParsedUnitStats()
        {
            Characteristics = new Dictionary<string, W40KDHParsedCharacteristic>(StringComparer.Ordinal);
            ProgressionSelections = new List<W40KDHParsedProgressionSelection>();
            DebugFactTypes = new Dictionary<string, int>(StringComparer.Ordinal);
        }

        public int EntityIndex { get; set; }
        public bool IsMainCharacter { get; set; }
        public bool IsCompanion { get; set; }
        public bool? InParty { get; set; }
        public bool? ExCompanion { get; set; }
        public string PortraitBlueprint { get; set; }
        public string UnitBlueprint { get; set; }
        public string CustomName { get; set; }
        public int? CharacterLevel { get; set; }
        public int? CharacterLevelOffset { get; set; }
        public int? Experience { get; set; }
        public int? ExperienceOffset { get; set; }
        public float? MissingHpFraction { get; set; }
        public Dictionary<string, W40KDHParsedCharacteristic> Characteristics { get; }
        public List<W40KDHParsedProgressionSelection> ProgressionSelections { get; }
        public int DebugFactCount { get; set; }
        public int DebugFeatureFactCount { get; set; }
        public Dictionary<string, int> DebugFactTypes { get; }
    }

    internal sealed class W40KDHParsedCharacteristic
    {
        public W40KDHParsedCharacteristic(string name, int value, int? offset)
        {
            Name = name;
            Value = value;
            Offset = offset;
        }

        public string Name { get; }
        public int Value { get; }
        public int? Offset { get; }
    }

    internal sealed class W40KDHParsedProgressionSelection
    {
        public string FeatureBlueprint { get; set; }
        public int? FeatureBlueprintOffset { get; set; }
        public int? FeatureBlueprintLength { get; set; }
        public int Rank { get; set; }
        public int? RankOffset { get; set; }
    }

    internal static class W40KDHOwlTraversal
    {
        private const byte ObjectKindNull = 0;
        private const byte ObjectKindRef = 1;
        private const byte ObjectKindObject = 2;
        private const ushort NullableBit = 0x8000;

        private const string SceneEntitiesStateType = "Kingmaker.EntitySystem.SceneEntitiesState";
        private const string UnitEntityType = "Kingmaker.EntitySystem.Entities.UnitEntity";
        private const string EntityPartsManagerType = "Kingmaker.EntitySystem.Entities.Base.EntityPartsManager";
        private const string EntityFactsManagerType = "Kingmaker.EntitySystem.EntityFactsManager";
        private const string FeatureFactType = "Kingmaker.UnitLogic.Feature";
        private const string PartUnitMainCharacterType = "Kingmaker.UnitLogic.Parts.UnitPartMainCharacter";
        private const string PartUnitCompanionType = "Kingmaker.UnitLogic.Parts.UnitPartCompanion";
        private const string PartUnitProgressionType = "Kingmaker.UnitLogic.PartUnitProgression";
        private const string FeatureSelectionDataType = "Kingmaker.UnitLogic.FeatureSelectionData";
        private const string PartHealthType = "Kingmaker.UnitLogic.Parts.PartHealth";
        private const string PartStatsContainerType = "Kingmaker.UnitLogic.Parts.PartStatsContainer";
        private const string PartStatsAttributesType = "Kingmaker.UnitLogic.Parts.PartStatsAttributes";
        private const string PartStatsSkillsType = "Kingmaker.UnitLogic.Parts.PartStatsSkills";
        private const string StatsContainerType = "StatsContainer";
        private const string ModifiableValueType = "ModifiableValue";
        private const string ModifiableValueAttributeStatType = "ModifiableValueAttributeStat";
        private static readonly Dictionary<string, HashSet<string>> RelevantFieldsByType = new(StringComparer.Ordinal) {
            { SceneEntitiesStateType, new HashSet<string>(StringComparer.Ordinal) { "m_EntityData" } },
            { UnitEntityType, new HashSet<string>(StringComparer.Ordinal) { "Facts", "Parts", "m_Blueprint", "CustomName" } },
            { EntityPartsManagerType, new HashSet<string>(StringComparer.Ordinal) { "Container" } },
            { EntityFactsManagerType, new HashSet<string>(StringComparer.Ordinal) { "m_Facts" } },
            { PartUnitMainCharacterType, new HashSet<string>(StringComparer.Ordinal) { "m_Portrait" } },
            { PartUnitCompanionType, new HashSet<string>(StringComparer.Ordinal) { "InParty", "ExCompanion", "m_Portrait" } },
            { PartUnitProgressionType, new HashSet<string>(StringComparer.Ordinal) { "CharacterLevel", "Experience", "m_Selections" } },
            { PartHealthType, new HashSet<string>(StringComparer.Ordinal) { "m_MissingHpFraction" } },
            { PartStatsContainerType, new HashSet<string>(StringComparer.Ordinal) { "Container", "m_Container", "m_List" } },
            { PartStatsAttributesType, new HashSet<string>(StringComparer.Ordinal) { "Container", "m_Container", "m_List" } },
            { PartStatsSkillsType, new HashSet<string>(StringComparer.Ordinal) { "Container", "m_Container", "m_List" } },
            { StatsContainerType, new HashSet<string>(StringComparer.Ordinal) { "m_Container", "Container", "AllStats" } },
            { ModifiableValueType, new HashSet<string>(StringComparer.Ordinal) { "m_Type", "Type", "m_BaseValue", "BaseValue", "m_ActualBaseValue", "ActualBaseValue", "m_ModifiedValue", "ModifiedValue", "m_PermanentValue", "PermanentValue", "m_Value", "Value" } },
            { ModifiableValueAttributeStatType, new HashSet<string>(StringComparer.Ordinal) { "m_Type", "Type", "m_BaseValue", "BaseValue", "m_ActualBaseValue", "ActualBaseValue", "m_ModifiedValue", "ModifiedValue", "m_PermanentValue", "PermanentValue", "m_Value", "Value" } },
            { FeatureSelectionDataType, new HashSet<string>(StringComparer.Ordinal) { "Feature", "Rank" } },
            { FeatureFactType, new HashSet<string>(StringComparer.Ordinal) { "m_Blueprint", "Rank" } },
        };
        private static readonly (string Token, string Name)[] CharacteristicNameTokens = new[] {
            ("WeaponSkill", "Weapon Skill"),
            ("WarhammerWeaponSkill", "Weapon Skill"),
            ("BallisticSkill", "Ballistic Skill"),
            ("WarhammerBallisticSkill", "Ballistic Skill"),
            ("Strength", "Strength"),
            ("WarhammerStrength", "Strength"),
            ("Toughness", "Toughness"),
            ("WarhammerToughness", "Toughness"),
            ("Agility", "Agility"),
            ("WarhammerAgility", "Agility"),
            ("Intelligence", "Intelligence"),
            ("WarhammerIntelligence", "Intelligence"),
            ("Perception", "Perception"),
            ("WarhammerPerception", "Perception"),
            ("Willpower", "Willpower"),
            ("WarhammerWillpower", "Willpower"),
            ("Fellowship", "Fellowship"),
            ("WarhammerFellowship", "Fellowship"),
        };
        private static readonly Dictionary<int, string> CharacteristicNameByStatType = new() {
            [1] = "Ballistic Skill",
            [2] = "Weapon Skill",
            [3] = "Strength",
            [4] = "Toughness",
            [5] = "Agility",
            [6] = "Intelligence",
            [7] = "Willpower",
            [8] = "Perception",
            [9] = "Fellowship",
        };

        private sealed class TypeDef
        {
            public TypeDef(ushort id, string name, byte flags, IReadOnlyList<FieldDef> fields)
            {
                Id = id;
                Name = name;
                Flags = flags;
                Fields = fields;
            }

            public ushort Id { get; }
            public string Name { get; }
            public byte Flags { get; }
            public IReadOnlyList<FieldDef> Fields { get; }
            public bool IsExternal => (Flags & 1) != 0;
        }

        private sealed class FieldDef
        {
            public FieldDef(string name, string typeName)
            {
                Name = name ?? string.Empty;
                TypeName = typeName ?? string.Empty;
            }

            public string Name { get; }
            public string TypeName { get; }
        }

        private sealed class ObjRef
        {
            public ObjRef(uint id) => Id = id;
            public uint Id { get; }
        }

        private sealed class PrimitiveValue
        {
            public PrimitiveValue(object value, int valueOffset, int length)
            {
                Value = value;
                ValueOffset = valueOffset;
                Length = length;
            }

            public object Value { get; }
            public int ValueOffset { get; }
            public int Length { get; }
        }

        private sealed class StringValue
        {
            public StringValue(string value, int valueOffset, int length)
            {
                Value = value;
                ValueOffset = valueOffset;
                Length = length;
            }

            public string Value { get; }
            public int ValueOffset { get; }
            public int Length { get; }

            public override string ToString() => Value ?? string.Empty;
        }

        private ref struct SpanReader
        {
            private ReadOnlySpan<byte> _span;
            private int _position;
            private readonly int _baseOffset;

            public SpanReader(ReadOnlySpan<byte> span, int baseOffset = 0)
            {
                _span = span;
                _position = 0;
                _baseOffset = baseOffset;
            }

            public int Position
            {
                readonly get => _position;
                set
                {
                    if (value < 0 || value > _span.Length) throw new EndOfStreamException();
                    _position = value;
                }
            }

            public readonly int Remaining => _span.Length - _position;
            public readonly int AbsolutePosition => _baseOffset + _position;

            private readonly void Ensure(int count)
            {
                if (count < 0 || count > Remaining) throw new EndOfStreamException();
            }

            public byte ReadByte()
            {
                Ensure(1);
                return _span[_position++];
            }

            public sbyte ReadSByte()
            {
                return unchecked((sbyte)ReadByte());
            }

            public short ReadInt16()
            {
                Ensure(2);
                var value = BinaryPrimitives.ReadInt16LittleEndian(_span.Slice(_position, 2));
                _position += 2;
                return value;
            }

            public ushort ReadUInt16()
            {
                Ensure(2);
                var value = BinaryPrimitives.ReadUInt16LittleEndian(_span.Slice(_position, 2));
                _position += 2;
                return value;
            }

            public int ReadInt32()
            {
                Ensure(4);
                var value = BinaryPrimitives.ReadInt32LittleEndian(_span.Slice(_position, 4));
                _position += 4;
                return value;
            }

            public uint ReadUInt32()
            {
                Ensure(4);
                var value = BinaryPrimitives.ReadUInt32LittleEndian(_span.Slice(_position, 4));
                _position += 4;
                return value;
            }

            public long ReadInt64()
            {
                Ensure(8);
                var value = BinaryPrimitives.ReadInt64LittleEndian(_span.Slice(_position, 8));
                _position += 8;
                return value;
            }

            public ulong ReadUInt64()
            {
                Ensure(8);
                var value = BinaryPrimitives.ReadUInt64LittleEndian(_span.Slice(_position, 8));
                _position += 8;
                return value;
            }

            public float ReadSingle()
            {
                return BitConverter.Int32BitsToSingle(ReadInt32());
            }

            public double ReadDouble()
            {
                return BitConverter.Int64BitsToDouble(ReadInt64());
            }

            public ReadOnlySpan<byte> ReadSpan(int length, out int absoluteOffset)
            {
                Ensure(length);
                absoluteOffset = AbsolutePosition;
                var result = _span.Slice(_position, length);
                _position += length;
                return result;
            }

            public void SkipRemaining()
            {
                _position = _span.Length;
            }
        }

        public static IReadOnlyList<W40KDHParsedUnitStats> ReadUnitStats(byte[] bytes)
        {
            if (bytes is null || bytes.Length < 4) return Array.Empty<W40KDHParsedUnitStats>();

            try {
                var reader = new SpanReader(bytes);
                if (!HasOwlHeader(ref reader)) return Array.Empty<W40KDHParsedUnitStats>();

                var references = new Dictionary<uint, object>();
                var types = ReadTypeLibrary(ref reader, references);
                if (types.Count == 0) return Array.Empty<W40KDHParsedUnitStats>();

                var root = ReadObject(ref reader, references, types, expectedTypeHint: null);
                if (ResolveRef(root, references) is not Dictionary<string, object> rootObject) return Array.Empty<W40KDHParsedUnitStats>();
                if (!TypeEq(rootObject, SceneEntitiesStateType)) return Array.Empty<W40KDHParsedUnitStats>();

                if (ResolveRef(GetValue(rootObject, "m_EntityData"), references) is not List<object> entities) {
                    return Array.Empty<W40KDHParsedUnitStats>();
                }

                var result = new List<W40KDHParsedUnitStats>();
                for (var entityIndex = 0; entityIndex < entities.Count; entityIndex++) {
                    try {
                        var entityValue = entities[entityIndex];
                        var resolved = ResolveRef(entityValue, references);
                        if (resolved is not Dictionary<string, object> entity) continue;
                        if (!TypeEq(entity, UnitEntityType)) continue;

                        var stats = new W40KDHParsedUnitStats {
                            EntityIndex = entityIndex,
                            UnitBlueprint = ExtractBlueprintGuid(GetValue(entity, "m_Blueprint"), references),
                            CustomName = AsString(GetValue(entity, "CustomName"), references),
                        };

                        ExtractFeatureFactsAsSelections(stats, entity, references);

                        var partsValue = ResolveRef(GetValue(entity, "Parts"), references);
                        if (partsValue is Dictionary<string, object> parts && TypeEq(parts, EntityPartsManagerType)) {
                            var containerValue = ResolveRef(GetValue(parts, "Container"), references);
                            if (containerValue is List<object> partList) {
                                foreach (var partRef in partList) {
                                    var partValue = ResolveRef(partRef, references);
                                    if (partValue is not Dictionary<string, object> part) continue;
                                    if (!TryGetType(part, out var partType)) continue;

                                    if (partType == PartUnitMainCharacterType) {
                                        stats.IsMainCharacter = true;
                                        stats.PortraitBlueprint = stats.PortraitBlueprint.OrIfEmpty(ExtractBlueprintGuid(GetValue(part, "m_Portrait"), references));
                                    }
                                    else if (partType == PartUnitCompanionType) {
                                        stats.IsCompanion = true;
                                        stats.InParty = AsBool(GetValue(part, "InParty"), references);
                                        stats.ExCompanion = AsBool(GetValue(part, "ExCompanion"), references);
                                        stats.PortraitBlueprint = stats.PortraitBlueprint.OrIfEmpty(ExtractBlueprintGuid(GetValue(part, "m_Portrait"), references));
                                    }
                                    else if (partType == PartUnitProgressionType) {
                                        stats.CharacterLevel = AsInt(GetValue(part, "CharacterLevel"), references);
                                        stats.CharacterLevelOffset = AsIntOffset(GetValue(part, "CharacterLevel"), references);
                                        stats.Experience = AsInt(GetValue(part, "Experience"), references);
                                        stats.ExperienceOffset = AsIntOffset(GetValue(part, "Experience"), references);
                                        ExtractProgressionSelections(stats, part, references);
                                    }
                                    else if (partType == PartHealthType) {
                                        stats.MissingHpFraction = AsFloat(GetValue(part, "m_MissingHpFraction"), references);
                                    }
                                    else if (partType == PartStatsContainerType
                                             || partType == PartStatsAttributesType
                                             || partType == PartStatsSkillsType) {
                                        ExtractCharacteristics(stats, part, references);
                                    }
                                }
                            }
                        }

                        if (stats.IsMainCharacter
                            || stats.IsCompanion
                            || stats.CharacterLevel.HasValue
                            || stats.Experience.HasValue
                            || stats.MissingHpFraction.HasValue) {
                            result.Add(stats);
                        }
                    }
                    catch {
                        continue;
                    }
                }

                return result;
            }
            catch {
                return Array.Empty<W40KDHParsedUnitStats>();
            }
        }

        private static void ExtractProgressionSelections(
            W40KDHParsedUnitStats unitStats,
            Dictionary<string, object> progressionPart,
            IReadOnlyDictionary<uint, object> references)
        {
            var selectionsValue = ResolveRef(GetValue(progressionPart, "m_Selections"), references);
            if (selectionsValue is not List<object> selections) return;

            foreach (var selectionRef in selections) {
                var selectionValue = ResolveRef(selectionRef, references);
                if (selectionValue is not Dictionary<string, object> selectionObj) continue;
                if (!TypeEq(selectionObj, FeatureSelectionDataType)) continue;

                var featureInfo = ExtractBlueprintInfo(GetValue(selectionObj, "Feature"), references);
                var rank = AsInt(GetValue(selectionObj, "Rank"), references);
                if (!rank.HasValue) continue;

                unitStats.ProgressionSelections.Add(new W40KDHParsedProgressionSelection {
                    FeatureBlueprint = featureInfo.Guid ?? string.Empty,
                    FeatureBlueprintOffset = featureInfo.Offset,
                    FeatureBlueprintLength = featureInfo.Length,
                    Rank = rank.Value,
                    RankOffset = AsIntOffset(GetValue(selectionObj, "Rank"), references),
                });
            }
        }

        private static void ExtractFeatureFactsAsSelections(
            W40KDHParsedUnitStats unitStats,
            Dictionary<string, object> unitEntity,
            IReadOnlyDictionary<uint, object> references)
        {
            var factsValue = ResolveRef(GetValue(unitEntity, "Facts"), references);
            if (factsValue is not Dictionary<string, object> factsManager) return;
            if (!TypeEq(factsManager, EntityFactsManagerType)) return;

            var factsListValue = ResolveRef(GetValue(factsManager, "m_Facts"), references);
            if (factsListValue is not List<object> factsList) return;
            unitStats.DebugFactCount += factsList.Count;

            foreach (var factRef in factsList) {
                var factValue = ResolveRef(factRef, references);
                if (factValue is not Dictionary<string, object> factObj) continue;
                if (TryGetType(factObj, out var factType)) {
                    if (!unitStats.DebugFactTypes.TryGetValue(factType, out var count)) count = 0;
                    unitStats.DebugFactTypes[factType] = count + 1;
                }
                if (!TypeEq(factObj, FeatureFactType)) continue;
                unitStats.DebugFeatureFactCount++;

                var featureInfo = ExtractBlueprintInfo(GetValue(factObj, "m_Blueprint"), references);
                var rank = AsInt(GetValue(factObj, "Rank"), references);
                if (!featureInfo.Guid.HasValue() || !rank.HasValue || rank.Value < 0) continue;

                unitStats.ProgressionSelections.Add(new W40KDHParsedProgressionSelection {
                    FeatureBlueprint = featureInfo.Guid,
                    FeatureBlueprintOffset = featureInfo.Offset,
                    FeatureBlueprintLength = featureInfo.Length,
                    Rank = rank.Value,
                    RankOffset = AsIntOffset(GetValue(factObj, "Rank"), references),
                });
            }
        }

        private static void ExtractCharacteristics(
            W40KDHParsedUnitStats unitStats,
            Dictionary<string, object> root,
            IReadOnlyDictionary<uint, object> references)
        {
            var visited = new HashSet<int>();
            Visit(root);

            void Visit(object node)
            {
                node = ResolveRef(node, references);
                if (node is Dictionary<string, object> obj) {
                    var id = obj.TryGetValue("$id", out var idRaw) ? idRaw?.GetHashCode() ?? RuntimeHelpers.GetHashCode(obj) : RuntimeHelpers.GetHashCode(obj);
                    if (!visited.Add(id)) return;

                    if (TryReadCharacteristic(obj, references, out var characteristic)) {
                        unitStats.Characteristics[characteristic.Name] = characteristic;
                    }

                    foreach (var kv in obj) {
                        if (kv.Key.StartsWith("$", StringComparison.Ordinal)) continue;
                        Visit(kv.Value);
                    }
                    return;
                }

                if (node is List<object> list) {
                    foreach (var item in list) Visit(item);
                    return;
                }

                if (node is KeyValuePair<object, object> kvp) {
                    Visit(kvp.Key);
                    Visit(kvp.Value);
                }
            }
        }

        private static bool TryReadCharacteristic(
            Dictionary<string, object> obj,
            IReadOnlyDictionary<uint, object> references,
            out W40KDHParsedCharacteristic characteristic)
        {
            characteristic = null;
            if (!TryGetType(obj, out var typeName)) return false;
            var isModifiableValue = TypeNameEq(typeName, ModifiableValueAttributeStatType)
                || TypeNameEq(typeName, ModifiableValueType)
                || typeName.Contains("ModifiableValue", StringComparison.Ordinal);
            if (!isModifiableValue) return false;

            var statTypeRaw = GetValue(obj, "m_Type")
                ?? GetValue(obj, "Type")
                ?? GetValue(obj, "m_StatType")
                ?? GetValue(obj, "StatType");
            var characteristicName = MapCharacteristicName(statTypeRaw, references);
            if (!characteristicName.HasValue()) return false;

            // Dark Heresy frequently keeps 0 in one field while the meaningful value is in another.
            // Prefer the explicit base value, then pick the first non-zero fallback, then any fallback.
            var baseValue = ReadIntField(obj, "m_BaseValue", references);
            var actualBaseValue = ReadIntField(obj, "m_ActualBaseValue", references);
            var modifiedValue = ReadIntField(obj, "m_ModifiedValue", references);
            var permanentValue = ReadIntField(obj, "m_PermanentValue", references);
            var rawValue = ReadIntField(obj, "m_Value", references);
            var rawValue2 = ReadIntField(obj, "Value", references);
            var baseValue2 = ReadIntField(obj, "BaseValue", references);
            var actualBaseValue2 = ReadIntField(obj, "ActualBaseValue", references);
            var modifiedValue2 = ReadIntField(obj, "ModifiedValue", references);
            var permanentValue2 = ReadIntField(obj, "PermanentValue", references);

            var selected = baseValue
                ?? baseValue2
                ?? FirstNonZero(actualBaseValue, actualBaseValue2, modifiedValue, modifiedValue2, permanentValue, permanentValue2, rawValue, rawValue2)
                ?? actualBaseValue
                ?? actualBaseValue2
                ?? modifiedValue
                ?? modifiedValue2
                ?? permanentValue
                ?? permanentValue2
                ?? rawValue
                ?? rawValue2;
            if (!selected.HasValue) return false;

            characteristic = new W40KDHParsedCharacteristic(characteristicName, selected.Value.Value, selected.Value.Offset);
            return true;
        }

        private static (int Value, int? Offset)? ReadIntField(
            Dictionary<string, object> obj,
            string fieldName,
            IReadOnlyDictionary<uint, object> references)
        {
            var raw = GetValue(obj, fieldName);
            var value = AsInt(raw, references);
            if (!value.HasValue) return null;
            return (value.Value, AsIntOffset(raw, references));
        }

        private static (int Value, int? Offset)? FirstNonZero(params (int Value, int? Offset)?[] candidates)
        {
            foreach (var candidate in candidates) {
                if (candidate.HasValue && candidate.Value.Value != 0) return candidate;
            }
            return null;
        }

        private static string MapCharacteristicName(object statTypeRaw, IReadOnlyDictionary<uint, object> references)
        {
            var statType = AsString(statTypeRaw, references);
            if (statType.HasValue()) {
                foreach (var (token, name) in CharacteristicNameTokens) {
                    if (statType.Contains(token, StringComparison.OrdinalIgnoreCase)) {
                        return name;
                    }
                }
            }

            var statTypeInt = AsInt(statTypeRaw, references);
            return statTypeInt.HasValue && CharacteristicNameByStatType.TryGetValue(statTypeInt.Value, out var mapped)
                ? mapped
                : null;
        }

        private static bool HasOwlHeader(ref SpanReader reader)
        {
            if (reader.Remaining < 4) return false;
            var h0 = reader.ReadByte();
            var h1 = reader.ReadByte();
            var h2 = reader.ReadByte();
            _ = reader.ReadByte(); // version
            return h0 == (byte)'O' && h1 == (byte)'W' && h2 == (byte)'L';
        }

        private static IReadOnlyDictionary<ushort, TypeDef> ReadTypeLibrary(ref SpanReader reader, IDictionary<uint, object> references)
        {
            var first = ReadObjectHeader(ref reader);
            if (first.Kind != ObjectKindObject) return new Dictionary<ushort, TypeDef>();

            _ = reader.ReadByte();
            var size = Read7BitEncodedInt(ref reader);
            var payload = reader.ReadSpan(size, out var payloadOffset);
            var payloadReader = new SpanReader(payload, payloadOffset);

            var listHeader = ReadObjectHeader(ref payloadReader);
            if (listHeader.Kind != ObjectKindObject) return new Dictionary<ushort, TypeDef>();
            var count = payloadReader.ReadInt32();
            if (count < 0 || count > 50000) return new Dictionary<ushort, TypeDef>();

            var types = new Dictionary<ushort, TypeDef>();
            for (ushort typeId = 0; typeId < count; typeId++) {
                var serializedType = ReadSerializedTypeInfo(ref payloadReader);
                if (serializedType is null) continue;
                types[typeId] = serializedType;
            }

            references[first.ObjectId] = new object();
            return types;
        }

        private static TypeDef ReadSerializedTypeInfo(ref SpanReader reader)
        {
            var header = ReadObjectHeader(ref reader);
            if (header.Kind != ObjectKindObject) return null;

            string typeName = null;
            string runtimeTypeName = null;
            byte flags = 0;
            var fields = new List<FieldDef>();

            for (var i = 0; i < 2; i++) {
                _ = reader.ReadByte();
                var fieldSize = Read7BitEncodedInt(ref reader);
                var fieldPayload = reader.ReadSpan(fieldSize, out var fieldPayloadOffset);
                var fpReader = new SpanReader(fieldPayload, fieldPayloadOffset);

                if (i == 0) {
                    typeName = ReadStringPayload(ref fpReader);
                }
                else {
                    var typeInfoHeader = ReadObjectHeader(ref fpReader);
                    if (typeInfoHeader.Kind != ObjectKindObject) continue;

                    for (var j = 0; j < 3; j++) {
                        _ = fpReader.ReadByte();
                        var size = Read7BitEncodedInt(ref fpReader);
                        var payload = fpReader.ReadSpan(size, out var payloadOffset);
                        var pReader = new SpanReader(payload, payloadOffset);

                        if (j == 0) {
                            runtimeTypeName = ReadStringPayload(ref pReader);
                        }
                        else if (j == 1) {
                            var fieldsHeader = ReadObjectHeader(ref pReader);
                            if (fieldsHeader.Kind != ObjectKindObject) continue;
                            var fieldCount = pReader.ReadInt32();
                            for (var k = 0; k < fieldCount; k++) {
                                var field = ReadFieldInfo(ref pReader);
                                if (field is object) fields.Add(field);
                            }
                        }
                        else if (j == 2) {
                            var rawFlags = ReadTypedPrimitivePayload(ref pReader);
                            if (rawFlags is PrimitiveValue primitiveFlags) rawFlags = primitiveFlags.Value;
                            if (rawFlags is byte b) flags = b;
                            else if (rawFlags is int intFlags && intFlags >= byte.MinValue && intFlags <= byte.MaxValue) flags = (byte)intFlags;
                        }
                    }
                }
            }

            var name = ShortTypeName(typeName).OrIfEmpty(ShortTypeName(runtimeTypeName));
            return new TypeDef(header.TypeId, name, flags, fields);
        }

        private static FieldDef ReadFieldInfo(ref SpanReader reader)
        {
            var header = ReadObjectHeader(ref reader);
            if (header.Kind != ObjectKindObject) return null;

            string fieldName = null;
            string fieldType = null;
            for (var i = 0; i < 2; i++) {
                _ = reader.ReadByte();
                var size = Read7BitEncodedInt(ref reader);
                var payload = reader.ReadSpan(size, out var payloadOffset);
                var pReader = new SpanReader(payload, payloadOffset);
                if (i == 0) fieldName = ReadStringPayload(ref pReader);
                if (i == 1) fieldType = ReadStringPayload(ref pReader);
            }

            return new FieldDef(fieldName, ShortTypeName(fieldType));
        }

        private static object ReadObject(
            ref SpanReader reader,
            IDictionary<uint, object> references,
            IReadOnlyDictionary<ushort, TypeDef> types,
            string expectedTypeHint)
        {
            var header = ReadObjectHeader(ref reader);
            if (header.Kind == ObjectKindNull) return null;
            if (header.Kind == ObjectKindRef) return new ObjRef(header.ObjectId);

            if (!types.TryGetValue(header.TypeId, out var typeDef)) {
                return new Dictionary<string, object> {
                    { "$id", header.ObjectId },
                    { "$typeId", header.TypeId },
                    { "$type", "unknown" },
                };
            }

            if (typeDef.IsExternal) {
                var external = ReadExternalObject(ref reader, references, types, typeDef.Name, expectedTypeHint);
                references[header.ObjectId] = external;
                return external;
            }

            var obj = new Dictionary<string, object> {
                { "$id", header.ObjectId },
                { "$typeId", header.TypeId },
                { "$type", typeDef.Name },
            };
            references[header.ObjectId] = obj;

            foreach (var _ in typeDef.Fields) {
                if (reader.Remaining <= 0) break;
                var fieldId = reader.ReadByte();
                var size = Read7BitEncodedInt(ref reader);
                var payload = reader.ReadSpan(size, out var payloadOffset);
                if (fieldId >= typeDef.Fields.Count) continue;

                var fieldDef = typeDef.Fields[fieldId];
                if (!ShouldReadField(typeDef.Name, fieldDef.Name)) {
                    continue;
                }

                var payloadReader = new SpanReader(payload, payloadOffset);
                try {
                    obj[fieldDef.Name] = ReadFieldPayload(ref payloadReader, references, types, fieldDef.TypeName);
                }
                catch {
                }
            }

            return obj;
        }

        private static bool ShouldReadField(string typeName, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(fieldName)) return false;
            if (IsBlueprintGuidField(typeName, fieldName)) return true;
            if (!TryGetRelevantFields(typeName, out var relevantFields)) {
                return IsStatsContainerRelatedType(typeName);
            }
            return relevantFields.Contains(fieldName);
        }

        private static bool IsStatsContainerRelatedType(string typeName)
        {
            if (!typeName.HasValue()) return false;
            return typeName.Contains("Kingmaker.EntitySystem.Stats.", StringComparison.Ordinal)
                || typeName.Contains("Kingmaker.UnitLogic.Parts.PartStats", StringComparison.Ordinal)
                || typeName.Contains("StatsContainer", StringComparison.Ordinal)
                || typeName.Contains("ModifiableValue", StringComparison.Ordinal);
        }

        private static bool TryGetRelevantFields(string typeName, out HashSet<string> relevantFields)
        {
            if (RelevantFieldsByType.TryGetValue(typeName, out relevantFields)) return true;

            var shortName = ShortTypeName(typeName);
            if (RelevantFieldsByType.TryGetValue(shortName, out relevantFields)) return true;

            var simpleName = GetSimpleTypeName(shortName);
            if (RelevantFieldsByType.TryGetValue(simpleName, out relevantFields)) return true;

            relevantFields = null;
            return false;
        }

        private static bool IsBlueprintGuidField(string typeName, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(fieldName)) return false;
            var isBlueprintType = typeName.StartsWith("Kingmaker.Blueprints.", StringComparison.Ordinal)
                || typeName.StartsWith("Kingmaker.UnitLogic.Mechanics.Blueprints.", StringComparison.Ordinal)
                || typeName.StartsWith("Blueprint", StringComparison.Ordinal)
                || typeName.EndsWith("Reference", StringComparison.Ordinal)
                || typeName.EndsWith("+Reference", StringComparison.Ordinal);
            if (!isBlueprintType) {
                return false;
            }

            return fieldName == "guid"
                || fieldName == "m_Guid"
                || fieldName == "m_AssetGuid"
                || fieldName == "Guid"
                || fieldName == "AssetGuid"
                || fieldName == "AssetId"
                || fieldName == "deserializedGuid";
        }

        private static object ReadExternalObject(
            ref SpanReader reader,
            IDictionary<uint, object> references,
            IReadOnlyDictionary<ushort, TypeDef> types,
            string typeName,
            string expectedTypeHint)
        {
            var shortTypeName = ShortTypeName(typeName);

            if (IsDictionaryType(shortTypeName)) {
                var count = reader.ReadInt32();
                var (keyType, valueType) = GetDictionaryGenericArgs(shortTypeName);
                var dictEntries = new List<object>(Math.Max(0, count));
                for (var i = 0; i < count; i++) {
                    _ = reader.ReadByte();
                    var key = ReadFieldPayload(ref reader, references, types, keyType);
                    var value = ReadFieldPayload(ref reader, references, types, valueType);
                    dictEntries.Add(new KeyValuePair<object, object>(key, value));
                }
                return dictEntries;
            }

            if (IsListLikeType(shortTypeName) || shortTypeName == "Array" || shortTypeName == "System.Array" || shortTypeName.EndsWith("[]", StringComparison.Ordinal)) {
                var count = reader.ReadInt32();
                var elementType = GetElementTypeForCollection(shortTypeName, expectedTypeHint);
                var list = new List<object>(Math.Max(0, count));
                for (var i = 0; i < count; i++) {
                    list.Add(ReadFieldPayload(ref reader, references, types, elementType));
                }
                return list;
            }

            return ReadUnknownExternalObject(ref reader);
        }

        private static object ReadUnknownExternalObject(ref SpanReader reader)
        {
            var start = reader.Position;
            try {
                if (reader.Remaining >= 3) {
                    var maybeString = ReadStringFieldPayload(ref reader);
                    if (maybeString is StringValue sv && NormalizeGuid(sv.Value).HasValue()) return sv;
                }
            }
            catch {
                reader.Position = start;
            }

            try {
                var payload = reader.ReadSpan(reader.Remaining, out var payloadOffset);
                var guid = FindGuidInBytes(payload, out var relativeOffset, out var length);
                return guid.HasValue()
                    ? new StringValue(guid, payloadOffset + relativeOffset, length)
                    : null;
            }
            catch {
                reader.Position = start;
                reader.SkipRemaining();
                return null;
            }
        }

        private static string FindGuidInBytes(ReadOnlySpan<byte> payload, out int relativeOffset, out int length)
        {
            relativeOffset = -1;
            length = 0;
            if (payload.Length < 32) return null;
            for (var i = 0; i <= payload.Length - 32; i++) {
                if (TryReadHexGuid(payload.Slice(i, 32), out var guid32)) {
                    relativeOffset = i;
                    length = 32;
                    return guid32;
                }
            }
            for (var i = 0; i <= payload.Length - 36; i++) {
                if (TryReadHyphenGuid(payload.Slice(i, 36), out var guid36)) {
                    relativeOffset = i;
                    length = 36;
                    return guid36;
                }
            }
            return null;
        }

        private static bool TryReadHexGuid(ReadOnlySpan<byte> span, out string guid)
        {
            guid = null;
            for (var i = 0; i < span.Length; i++) {
                if (!IsHexAscii(span[i])) return false;
            }
            guid = Encoding.ASCII.GetString(span);
            return true;
        }

        private static bool TryReadHyphenGuid(ReadOnlySpan<byte> span, out string guid)
        {
            guid = null;
            for (var i = 0; i < span.Length; i++) {
                var b = span[i];
                if (i == 8 || i == 13 || i == 18 || i == 23) {
                    if (b != (byte)'-') return false;
                    continue;
                }
                if (!IsHexAscii(b)) return false;
            }
            guid = Encoding.ASCII.GetString(span);
            return true;
        }

        private static bool IsHexAscii(byte b)
        {
            return (b >= (byte)'0' && b <= (byte)'9')
                || (b >= (byte)'a' && b <= (byte)'f')
                || (b >= (byte)'A' && b <= (byte)'F');
        }

        private static object ReadFieldPayload(
            ref SpanReader reader,
            IDictionary<uint, object> references,
            IReadOnlyDictionary<ushort, TypeDef> types,
            string declaredTypeName)
        {
            var shortType = ShortTypeName(declaredTypeName);

            if (shortType.StartsWith("System.String", StringComparison.Ordinal)) {
                return ReadStringFieldPayload(ref reader);
            }

            if (shortType.StartsWith("System.Nullable`1[[", StringComparison.Ordinal)) {
                var nullableInner = shortType.Substring("System.Nullable`1[[".Length);
                var end = nullableInner.IndexOf("]]", StringComparison.Ordinal);
                if (end >= 0) nullableInner = nullableInner.Substring(0, end);
                return ReadNullablePayload(ref reader, references, types, ShortTypeName(nullableInner));
            }

            if (TryReadPrimitivePayload(ref reader, shortType, out var primitive)) {
                return primitive;
            }

            if (shortType.Contains("StatType", StringComparison.Ordinal)) {
                try { return ReadEnumPayload(ref reader); } catch { }
            }

            return ReadObject(ref reader, references, types, shortType);
        }

        private static object ReadNullablePayload(
            ref SpanReader reader,
            IDictionary<uint, object> references,
            IReadOnlyDictionary<ushort, TypeDef> types,
            string innerType)
        {
            if (TryReadPrimitivePayload(ref reader, innerType, out var primitive, expectNullable: true)) {
                return primitive;
            }

            var pos = reader.Position;
            try {
                return ReadEnumPayload(ref reader, nullable: true);
            }
            catch {
                reader.Position = pos;
                return ReadObject(ref reader, references, types, innerType);
            }
        }

        private static bool TryReadPrimitivePayload(ref SpanReader reader, string expectedType, out object value, bool expectNullable = false)
        {
            value = null;
            var startPos = reader.Position;
            var startAbs = reader.AbsolutePosition;
            try {
                var typeId = reader.ReadUInt16();
                var valueOffset = startAbs + 2;
                if (expectNullable) {
                    var hasValue = reader.ReadByte();
                    if (hasValue == 0) return true;
                    typeId = (ushort)(typeId & ~NullableBit);
                    valueOffset++;
                }

                if (TryReadPrimitiveByTypeId(ref reader, typeId, out var primitive, out var primitiveLength)) {
                    value = new PrimitiveValue(primitive, valueOffset, primitiveLength);
                    return true;
                }
            }
            catch {
            }

            reader.Position = startPos;
            return false;
        }

        private static object ReadTypedPrimitivePayload(ref SpanReader reader)
        {
            var typeId = reader.ReadUInt16();
            return TryReadPrimitiveByTypeId(ref reader, typeId, out var value, out var primitiveLength)
                ? new PrimitiveValue(value, reader.AbsolutePosition - primitiveLength, primitiveLength)
                : null;
        }

        private static bool TryReadPrimitiveByTypeId(ref SpanReader reader, ushort typeId, out object value, out int length)
        {
            value = null;
            length = 0;
            switch (typeId) {
                case 0: value = reader.ReadByte(); length = 1; return true;
                case 1: value = reader.ReadSByte(); length = 1; return true;
                case 2: value = reader.ReadInt16(); length = 2; return true;
                case 3: value = reader.ReadUInt16(); length = 2; return true;
                case 4: value = reader.ReadInt32(); length = 4; return true;
                case 5: value = reader.ReadUInt32(); length = 4; return true;
                case 6: value = reader.ReadInt64(); length = 8; return true;
                case 7: value = reader.ReadUInt64(); length = 8; return true;
                case 8: value = reader.ReadSingle(); length = 4; return true;
                case 9: value = reader.ReadDouble(); length = 8; return true;
                case 10: value = (char)reader.ReadInt16(); length = 2; return true;
                case 11: value = reader.ReadByte() != 0; length = 1; return true;
                default: return false;
            }
        }

        private static string ReadEnumPayload(ref SpanReader reader, bool nullable = false)
        {
            var typeId = reader.ReadUInt16();
            if ((typeId & ~NullableBit) != 13) throw new InvalidOperationException("Expected enum payload.");
            if (nullable || (typeId & NullableBit) != 0) {
                var hasValue = reader.ReadByte();
                if (hasValue == 0) return null;
            }
            var length = Read7BitEncodedInt(ref reader);
            var payload = reader.ReadSpan(length, out _);
            return Encoding.UTF8.GetString(payload);
        }

        private static string ReadStringPayload(ref SpanReader reader)
        {
            var typeId = reader.ReadUInt16();
            if (typeId != 12) throw new InvalidOperationException("Expected string payload.");
            var hasValue = reader.ReadByte();
            if (hasValue == 0) return null;
            var length = Read7BitEncodedInt(ref reader);
            var payload = reader.ReadSpan(length, out _);
            return Encoding.UTF8.GetString(payload);
        }

        private static object ReadStringFieldPayload(ref SpanReader reader)
        {
            var typeId = reader.ReadUInt16();
            if (typeId != 12) throw new InvalidOperationException("Expected string payload.");
            var hasValue = reader.ReadByte();
            if (hasValue == 0) return null;
            var length = Read7BitEncodedInt(ref reader);
            var payload = reader.ReadSpan(length, out var payloadOffset);
            return new StringValue(Encoding.UTF8.GetString(payload), payloadOffset, length);
        }

        private static (byte Kind, uint ObjectId, ushort TypeId) ReadObjectHeader(ref SpanReader reader)
        {
            var kind = reader.ReadByte();
            return kind switch {
                ObjectKindNull => (kind, 0u, 0),
                ObjectKindRef => (kind, reader.ReadUInt32(), 0),
                ObjectKindObject => (kind, reader.ReadUInt32(), reader.ReadUInt16()),
                _ => throw new InvalidOperationException("Invalid object kind " + kind),
            };
        }

        private static int Read7BitEncodedInt(ref SpanReader reader)
        {
            var value = 0;
            var shift = 0;
            while (true) {
                var b = reader.ReadByte();
                value |= (b & 0x7F) << shift;
                if ((b & 0x80) == 0) return value;
                shift += 7;
                if (shift > 35) throw new InvalidOperationException("Invalid 7-bit integer payload.");
            }
        }

        private static bool IsListLikeType(string typeName)
        {
            return typeName.StartsWith("System.Collections.Generic.List`1[[", StringComparison.Ordinal)
                || typeName.StartsWith("System.Collections.Generic.HashSet`1[[", StringComparison.Ordinal)
                || typeName.StartsWith("System.Collections.Generic.Queue`1[[", StringComparison.Ordinal);
        }

        private static bool IsDictionaryType(string typeName)
        {
            return typeName.StartsWith("System.Collections.Generic.Dictionary`2[[", StringComparison.Ordinal);
        }

        private static string GetElementTypeForCollection(string typeName, string expectedTypeHint)
        {
            if (typeName.EndsWith("[]", StringComparison.Ordinal)) {
                return typeName.Substring(0, typeName.Length - 2);
            }

            if (IsListLikeType(typeName)) {
                var content = typeName.Substring(typeName.IndexOf("[[", StringComparison.Ordinal) + 2);
                var end = content.LastIndexOf("]]", StringComparison.Ordinal);
                if (end > 0) return content.Substring(0, end);
            }

            return expectedTypeHint.OrIfEmpty("System.Object");
        }

        private static (string KeyType, string ValueType) GetDictionaryGenericArgs(string dictionaryTypeName)
        {
            var start = dictionaryTypeName.IndexOf("[[", StringComparison.Ordinal);
            if (start < 0) return ("System.Object", "System.Object");
            var inner = dictionaryTypeName.Substring(start + 2);
            var split = inner.IndexOf("],[", StringComparison.Ordinal);
            if (split < 0) return ("System.Object", "System.Object");
            var key = inner.Substring(0, split);
            var value = inner.Substring(split + 3);
            var end = value.LastIndexOf("]]", StringComparison.Ordinal);
            if (end >= 0) value = value.Substring(0, end);
            return (key, value);
        }

        private static object ResolveRef(object value, IReadOnlyDictionary<uint, object> references)
        {
            while (value is ObjRef objRef) {
                if (!references.TryGetValue(objRef.Id, out var resolved)) return null;
                value = resolved;
            }
            return value;
        }

        private static object GetValue(Dictionary<string, object> obj, string key)
        {
            return obj.TryGetValue(key, out var value) ? value : null;
        }

        private static bool TypeEq(Dictionary<string, object> obj, string shortTypeName)
        {
            return TryGetType(obj, out var typeName) && TypeNameEq(typeName, shortTypeName);
        }

        private static bool TypeNameEq(string left, string right)
        {
            if (string.Equals(left, right, StringComparison.Ordinal)) return true;
            var leftShort = ShortTypeName(left);
            var rightShort = ShortTypeName(right);
            if (string.Equals(leftShort, rightShort, StringComparison.Ordinal)) return true;
            return string.Equals(GetSimpleTypeName(leftShort), GetSimpleTypeName(rightShort), StringComparison.Ordinal);
        }

        private static string GetSimpleTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return string.Empty;
            var lastDot = typeName.LastIndexOf('.');
            return lastDot >= 0 && lastDot + 1 < typeName.Length
                ? typeName.Substring(lastDot + 1)
                : typeName;
        }

        private static bool TryGetType(Dictionary<string, object> obj, out string typeName)
        {
            typeName = null;
            if (!obj.TryGetValue("$type", out var value)) return false;
            if (value is StringValue stringValue) {
                typeName = stringValue.Value;
                return !string.IsNullOrWhiteSpace(typeName);
            }
            typeName = value?.ToString();
            return !string.IsNullOrWhiteSpace(typeName);
        }

        private static int? AsInt(object value, IReadOnlyDictionary<uint, object> references)
        {
            value = ResolveRef(value, references);
            if (value is PrimitiveValue primitive) value = primitive.Value;
            if (value is int i) return i;
            if (value is long l && l <= int.MaxValue && l >= int.MinValue) return (int)l;
            if (value is short s) return s;
            if (value is byte b) return b;
            return null;
        }

        private static int? AsIntOffset(object value, IReadOnlyDictionary<uint, object> references)
        {
            value = ResolveRef(value, references);
            if (value is PrimitiveValue primitive) {
                if (primitive.Value is int
                    || primitive.Value is short
                    || primitive.Value is byte
                    || primitive.Value is long l && l <= int.MaxValue && l >= int.MinValue) {
                    return primitive.ValueOffset;
                }
            }
            return null;
        }

        private static float? AsFloat(object value, IReadOnlyDictionary<uint, object> references)
        {
            value = ResolveRef(value, references);
            if (value is PrimitiveValue primitive) value = primitive.Value;
            if (value is float f) return f;
            if (value is double d) return (float)d;
            return null;
        }

        private static bool? AsBool(object value, IReadOnlyDictionary<uint, object> references)
        {
            value = ResolveRef(value, references);
            if (value is PrimitiveValue primitive) value = primitive.Value;
            if (value is bool b) return b;
            return null;
        }

        private static string AsString(object value, IReadOnlyDictionary<uint, object> references)
        {
            value = ResolveRef(value, references);
            if (value is StringValue stringValue) return stringValue.Value;
            if (value is PrimitiveValue primitive) value = primitive.Value;
            return value as string;
        }

        private static string ExtractBlueprintGuid(object value, IReadOnlyDictionary<uint, object> references)
        {
            return ExtractBlueprintInfo(value, references).Guid;
        }

        private static (string Guid, int? Offset, int? Length) ExtractBlueprintInfo(object value, IReadOnlyDictionary<uint, object> references)
        {
            value = ResolveRef(value, references);
            if (value is StringValue rawString) {
                var normalized = NormalizeGuid(rawString.Value);
                return normalized.HasValue()
                    ? (normalized, rawString.ValueOffset, rawString.Length)
                    : (null, null, null);
            }
            if (value is string raw) {
                return (NormalizeGuid(raw), null, null);
            }

            if (value is not Dictionary<string, object> obj) return (null, null, null);
            var direct = NormalizeGuid(
                AsString(GetValue(obj, "m_Guid"), references)
                .OrIfEmpty(AsString(GetValue(obj, "m_AssetGuid"), references))
                .OrIfEmpty(AsString(GetValue(obj, "Guid"), references))
                .OrIfEmpty(AsString(GetValue(obj, "guid"), references))
                .OrIfEmpty(AsString(GetValue(obj, "AssetGuid"), references))
                .OrIfEmpty(AsString(GetValue(obj, "AssetId"), references))
                .OrIfEmpty(AsString(GetValue(obj, "Blueprint"), references))
                .OrIfEmpty(AsString(GetValue(obj, "deserializedGuid"), references)));
            if (direct.HasValue()) {
                var directRaw = ResolveRef(GetValue(obj, "m_Guid"), references)
                    ?? ResolveRef(GetValue(obj, "m_AssetGuid"), references)
                    ?? ResolveRef(GetValue(obj, "Guid"), references)
                    ?? ResolveRef(GetValue(obj, "guid"), references)
                    ?? ResolveRef(GetValue(obj, "AssetGuid"), references)
                    ?? ResolveRef(GetValue(obj, "AssetId"), references)
                    ?? ResolveRef(GetValue(obj, "Blueprint"), references)
                    ?? ResolveRef(GetValue(obj, "deserializedGuid"), references);
                if (directRaw is StringValue sv) return (direct, sv.ValueOffset, sv.Length);
                return (direct, null, null);
            }

            foreach (var kv in obj) {
                var nested = ResolveRef(kv.Value, references);
                if (nested is Dictionary<string, object> nestedObj) {
                    var nestedGuid = ExtractBlueprintInfo(nestedObj, references);
                    if (nestedGuid.Guid.HasValue()) return nestedGuid;
                }
                else if (nested is StringValue nestedStringValue) {
                    var nestedGuid = NormalizeGuid(nestedStringValue.Value);
                    if (nestedGuid.HasValue()) return (nestedGuid, nestedStringValue.ValueOffset, nestedStringValue.Length);
                }
                else if (nested is string nestedString) {
                    var nestedGuid = NormalizeGuid(nestedString);
                    if (nestedGuid.HasValue()) return (nestedGuid, null, null);
                }
            }

            return (null, null, null);
        }

        private static string NormalizeGuid(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var trimmed = value.Trim();
            if (trimmed.Length == 32 || trimmed.Length == 36) return trimmed;
            return null;
        }

        private static string ShortTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return string.Empty;
            var idx = typeName.IndexOf(", ", StringComparison.Ordinal);
            return idx > 0 ? typeName.Substring(0, idx) : typeName;
        }
    }
}
