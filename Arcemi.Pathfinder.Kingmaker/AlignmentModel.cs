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
            N.On(Vector, "X", "Vector");
            N.On(Vector, "Y", "Vector");
        }

        public VectorModel Vector => A.Object<VectorModel>();

    }
}