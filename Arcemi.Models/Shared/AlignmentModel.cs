#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion

namespace Arcemi.Models
{
    public class AlignmentModel : RefModel
    {
        public AlignmentModel(ModelDataAccessor accessor) : base(accessor)
        {
            Vector = new VectorView(accessor, "m_Vector");
        }

        public VectorView Vector { get; }
        public ListAccessor<AlignmentHistoryModel> History => A.List("m_History", a => new AlignmentHistoryModel(a));
        public string LockedAlignmentMask { get => A.Value<string>("m_LockedAlignmentMask"); set => A.Value(value, "m_LockedAlignmentMask"); }
        public string TargetAlignment { get => A.Value<string>("m_TargetAlignment"); set => A.Value(value, "m_TargetAlignment"); }
    }
}