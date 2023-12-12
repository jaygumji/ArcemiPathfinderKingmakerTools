#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Models
{
    public class UnitModel : RefModel
    {
        public UnitModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public bool IsInGame { get => A.Value<bool>("m_IsInGame"); set => A.Value(value, "m_IsInGame"); }
        public bool IsRevealed { get => A.Value<bool>("m_IsRevealed"); set => A.Value(value, "m_IsRevealed"); }
        public bool Sleepless { get => A.Value<bool>(); set => A.Value(value); }
        public string UniqueId { get => A.Value<string>(); set => A.Value(value); }
    }
}
