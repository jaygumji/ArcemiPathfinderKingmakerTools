#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class VectorView
    {
        private readonly PropertyAccessor<string> _accessor;
        private int _x;
        private int _y;

        public VectorView(ModelDataAccessor a, string name) : this(new ModelPropertyAccessor<string>(a, name))
        {
        }

        public VectorView(Func<string> getter, Action<string> setter) : this(new PropertyAccessor<string>(getter, setter))
        {
        }

        public VectorView(PropertyAccessor<string> accessor)
        {
            _accessor = accessor;
            var p = Value?.Split('|');
            if (p?.Length == 2) {
                _x = int.Parse(p[0]);
                _y = int.Parse(p[1]);
            }
        }

        private void Changed()
        {
            Value = $"{X}|{Y}";
        }

        public int X
        {
            get => _x;
            set {
                _x = value;
                Changed();
            }
        }

        public int Y
        {
            get => _y;
            set {
                _y = value;
                Changed();
            }
        }

        public string Value { get => _accessor.Get(); set => _accessor.Set(value); }
        public string Direction
        {
            get {
                if (Math.Pow(X, 2) + Math.Pow(Y, 2) < Math.Pow(33.3, 2)) {
                    return "TrueNeutral";
                }

                var angle = Math.Atan2(Y, X) * 180 / Math.PI;
                if (angle < 157.5 && angle >= 112.5) return "LawfulGood";
                if (angle < 112.5 && angle >= 67.5) return "NeutralGood";
                if (angle < 67.5 && angle >= 22.5) return "ChaoticGood";
                if (angle < 22.5 && angle >= -22.5) return "ChaoticNeutral";
                if (angle < -22.5 && angle >= -67.5) return "ChaoticEvil";
                if (angle < -67.5 && angle >= -112.5) return "NeutralEvil";
                if (angle < -112.5 && angle >= -157.5) return "LawfulEvil";
                return "LawfulNeutral";
            }
        }

        public string DisplayName => Direction.AsDisplayable();
    }
}