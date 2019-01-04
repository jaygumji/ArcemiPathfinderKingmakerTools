#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class StringExtensions
    {
        public static string AsDisplayable(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var b = new StringBuilder();
            var c = value[0];
            b.Append(char.IsUpper(c) ? c : char.ToUpper(c));

            for (var i = 1; i < value.Length; i++) {
                c = value[i];

                if (char.IsUpper(c) && i + 1 < value.Length && char.IsLower(value[i + 1])) {
                    b.Append(" ");
                }
                b.Append(c);
            }

            return b.ToString();
        }

        public static string OrIfEmpty(this string value, string otherwise)
        {
            return !string.IsNullOrEmpty(value) ? value : otherwise;
        }
    }
}
