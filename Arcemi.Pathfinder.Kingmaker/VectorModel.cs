#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Pathfinder.Kingmaker
{
    public class VectorModel : Model
    {
        public VectorModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public double X { get => A.Value<double>("x"); set => A.Value(value, "x"); }
        public double Y { get => A.Value<double>("y"); set => A.Value(value, "y"); }
    }
}