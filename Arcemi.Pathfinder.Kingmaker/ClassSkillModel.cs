#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Pathfinder.Kingmaker
{
    public class ClassSkillModel : RefModel
    {
        public ClassSkillModel(ModelDataAccessor accessor) : base(accessor)
        {
            N.On(nameof(Count), nameof(IsSet));
        }

        public int Count { get => A.Value<int>("m_Count"); set => A.Value(value, "m_Count"); }
        public bool IsSet { get => Count > 0; set => Count = value ? 1 : 0; }
    }
}