#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class StringExtensions
    {
        public static string AsDisplayable(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var len = value.Length;

            var b = new StringBuilder();
            var c = value[0];
            b.Append(char.IsUpper(c) ? c : char.ToUpper(c));

            for (var i = 1; i < len; i++) {
                c = value[i];
                var cp = value[i - 1];

                if (c == '_') {
                    b.Append(' ');
                    continue;
                }

                if (c == 'd' && char.IsDigit(cp) && i + 1 < len) {
                    var cd = value[i + 1];
                    if (char.IsDigit(cd)) {
                        b.Append(c);
                        b.Append(cd);
                        i++;
                        continue;
                    }
                }
                if (char.IsUpper(c) && i + 1 < value.Length && char.IsLower(value[i + 1])) {
                    b.Append(' ');
                }
                else if (char.IsDigit(c) && char.IsLetter(cp)) {
                    b.Append(' ');
                }

                b.Append(c);
            }

            return b.ToString();
        }

        public static string OrIfEmpty(this string value, string otherwise)
        {
            return !string.IsNullOrEmpty(value) ? value : otherwise;
        }

        public static string CutAt(this string value, int maxLength)
        {
            if (value == null) return value;
            if (value.Length <= maxLength) return value;
            return value.Substring(0, maxLength - 3) + "...";
        }

        public static IReadOnlyList<string> ToComponents(this string value)
        {
            if (string.IsNullOrEmpty(value)) return new string[0];
            var list = new List<string>();
            var start = 0;
            for (var i = 1; i < value.Length; i++) {
                var c = value[i];
                if (i != 0 && char.IsUpper(c)) {
                    var component = value.Substring(start, i - start);
                    list.Add(component);
                    start = i;
                }
            }
            var lastComponent = value.Substring(start);
            list.Add(lastComponent);

            return list;
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool Eq(this string value, string other)
        {
            return string.Equals(value, other, System.StringComparison.Ordinal);
        }

        public static bool IEq(this string value, string other)
        {
            return string.Equals(value, other, System.StringComparison.OrdinalIgnoreCase);
        }

        public static bool ILike(this string value, string other)
        {
            return value?.IndexOf(other, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IStart(this string value, string other)
        {
            return value?.StartsWith(other, System.StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
