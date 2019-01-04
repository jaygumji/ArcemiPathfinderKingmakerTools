#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class TextIdentifier
    {
        private readonly string _value;

        public TextIdentifier(string value)
        {
            _value = value;
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public bool Equals(string value)
        {
            return string.Equals(value, _value, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(TextIdentifier tid)
        {
            return string.Equals(tid._value, _value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is TextIdentifier tid) {
                return Equals(tid);
            }
            if (obj is string str) {
                return Equals(str);
            }
            return false;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}