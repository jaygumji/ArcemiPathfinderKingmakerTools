using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class BlueprintItemName : BlueprintName
    {
        public BlueprintItemName(BlueprintType type, string displayName, string original, IReadOnlyList<string> parts, int enhancement) : base(type, displayName, original)
        {
            Parts = parts;
            Enhancement = enhancement;
            SimpleName = string.Join(" ", parts);
            FirstPart = Parts[0];
            LastPart = Parts[Parts.Count - 1];
        }

        private IReadOnlyList<string> Parts { get; }
        private string FirstPart { get; }
        private string LastPart { get; }
        public string SimpleName { get; }
        public int Enhancement { get; }

        private bool OrigStart(string value) => Original.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        private bool P1Eq(string value) => FirstPart.Equals(value, StringComparison.OrdinalIgnoreCase);
        private bool P2Eq(string value) => Parts.Count > 1 && Parts[1].Equals(value, StringComparison.OrdinalIgnoreCase);
        private bool P3Eq(string value) => Parts.Count > 2 && Parts[2].Equals(value, StringComparison.OrdinalIgnoreCase);
        private bool P4Eq(string value) => Parts.Count > 3 && Parts[3].Equals(value, StringComparison.OrdinalIgnoreCase);
        private bool PLEq(string value) => LastPart.Equals(value, StringComparison.OrdinalIgnoreCase);

        public bool IsFinnean => OrigStart("Finnean");

        public bool IsSpecialWeapon()
        {
            if (!ReferenceEquals(Type, BlueprintTypes.ItemWeapon)) {
                return false;
            }
            if (Enhancement > 0) return false;

            return (P1Eq("Large") && P2Eq("Standard"))
                || P1Eq("Map")
                || P1Eq("Test")
                || P1Eq("Draft")
                || P1Eq("Trap")
                ;
        }

        public bool IsNaturalWeapon()
        {
            if (!ReferenceEquals(Type, BlueprintTypes.ItemWeapon)) {
                return false;
            }
            if (Enhancement > 0) return false;

            return P1Eq("Bite")
                || P1Eq("Claw")
                || P1Eq("Tail")
                || P1Eq("Talon")
                || P1Eq("Wing")
                || P1Eq("Gore")
                || P1Eq("Slam")
                || P1Eq("Sting")
                || P1Eq("Unarmed")
                || P1Eq("Tentacle")
                || P1Eq("Animated")
                || P1Eq("Bloodline")
                || P1Eq("Bloodrager")
                || P2Eq("Bite")
                || P2Eq("Gore")
                || P2Eq("Hoof")
                || P3Eq("Bite")
                || P3Eq("Claw")
                || P3Eq("Tail")
                || P3Eq("Wing")
                || P3Eq("Slam")
                || PLEq("Bite")
                || PLEq("Claw")
                || PLEq("Tail")
                || PLEq("Wing")
                || PLEq("Slam")
                ;
        }

        public bool IsComponent()
        {
            if (ReferenceEquals(Type, BlueprintTypes.ItemWeapon)) {
                return IsFinnean
                    || P1Eq("Weapon")
                    || PLEq("Weapon")
                    || PLEq("Shield")
                    || P3Eq("Shield");
            }
            if (ReferenceEquals(Type, BlueprintTypes.ItemArmor)) {
                return P1Eq("Shield")
                    || P1Eq("Buckler")
                    || PLEq("Armor")
                    || (P1Eq("Tower") && P2Eq("Shield"));
            }
            return false;
        }

        private static bool IsSequence(string value, int index, string sequence)
        {
            if (sequence.Length + index > value.Length) return false;
            for (var i = 0; i < sequence.Length; i++) {
                var s = sequence[i];
                var c = value[i + index];
                if (s != c) return false;
            }
            return true;
        }

        public static BlueprintName Detect(BlueprintType type, string name)
        {
            if (string.IsNullOrEmpty(name)) return new BlueprintName(type, name, name);
            var parts = new List<string>();
            var enhancement = 0;

            var len = name.Length;
            var st = 0;

            void AppendPart(int p, int inc = 0)
            {
                if (st < p) {
                    parts.Add(name.Substring(st, p - st));
                    st = p + inc;
                }
            }
            for (var i = 1; i < len; i++) {
                var c = name[i];
                var cp = name[i - 1];
                void Skip(int count)
                {
                    i += count;
                    st = i + 1;
                }

                if (c == '_') {
                    AppendPart(i, inc: 1);
                    continue;
                }

                if (c == 'd' && char.IsDigit(cp) && i + 1 < len) {
                    if (char.IsDigit(name[i + 1])) {
                        if (i + 2 < len && char.IsDigit(name[i + 2])) {
                            AppendPart(i += 3);
                        }
                        else {
                            AppendPart(i += 2);
                        }

                        continue;
                    }
                }
                if (c == 'x' && char.IsDigit(cp) && i + 1 < len) {
                    if (char.IsDigit(name[i + 1])) {
                        AppendPart(i += 2);
                        continue;
                    }
                }

                if (IsSequence(name, i, "Item")) {
                    AppendPart(i);
                    Skip(3);
                    continue;
                }
                if (IsSequence(name, i, "Plus") && i + 4 < len) {
                    var cd = name[i + 4];
                    if (char.IsDigit(cd)) {
                        enhancement = cd - 48;
                        AppendPart(i);
                        Skip(4);
                        continue;
                    }
                }

                if (char.IsUpper(c) && i + 1 < name.Length && char.IsLower(name[i + 1])) {
                    AppendPart(i);
                }
                else if (char.IsDigit(c) && char.IsLetter(cp)) {
                    AppendPart(i);
                }
            }
            AppendPart(len);

            if (parts.Count > 0) {
                if (parts[0].EndsWith("of", StringComparison.Ordinal)) {
                    parts[0] = parts[0].Substring(0, parts[0].Length - 2);
                    parts.Insert(1, "Of");
                }
            }

            var displayName = string.Join(" ", parts);
            if (enhancement > 0) {
                displayName = string.Concat(displayName, " +", enhancement);
            }
            return new BlueprintItemName(type, displayName, name, parts, enhancement);
        }
    }
}