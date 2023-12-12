#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion

namespace Arcemi.Models
{
    public class UISettingsModel : RefModel
    {
        public UISettingsModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListValueAccessor<string> m_AlreadyAutomaticallyAdded => A.ListValue<string>("m_AlreadyAutomaticallyAdded");
        public string Portrait { get => A.Value<string>("m_Portrait"); set => A.Value(value, "m_Portrait"); }
        public CustomPortraitModel CustomPortrait => A.Object("m_CustomPortrait", a => new CustomPortraitModel(a));
    }
}