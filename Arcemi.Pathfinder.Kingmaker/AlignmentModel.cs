#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
namespace Arcemi.Pathfinder.Kingmaker
{
    public class AlignmentModel : RefModel
    {
        public AlignmentModel(ModelDataAccessor accessor) : base(accessor)
        {
            Vector = new VectorView(accessor);
        }

        public VectorView Vector { get; }
    }

    public class VectorView
    {
        private int _x;
        private int _y;
        private ModelDataAccessor A { get; }

        public VectorView(ModelDataAccessor a)
        {
            A = a;
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

        public string Value { get => A.Value<string>("m_Vector"); set => A.Value(value, "m_Vector"); }
    }
}