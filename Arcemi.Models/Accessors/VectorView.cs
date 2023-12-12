#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;

namespace Arcemi.Models
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
        public Alignment Direction => AlignmentExtensions.Detect(X, Y);

        public string DirectionText => Direction.ToString();
        public string DisplayName => DirectionText.AsDisplayable();
    }
}