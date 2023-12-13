#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Models
{
    public class HoldingSlotModel : RefModel
    {
        private const string TypeHandSlot = "Kingmaker.Items.Slots.HandSlot, Assembly-CSharp";
        private const string TypeArmorSlot = "Kingmaker.Items.Slots.ArmorSlot, Assembly-CSharp";
        private const string TypeWeaponSlot = "Kingmaker.Items.Slots.WeaponSlot, Assembly-CSharp";
        private const string TypeUsableSlot = "Kingmaker.Items.Slots.UsableSlot, Assembly-CSharp";

        public HoldingSlotModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Type { get => A.Value<string>("$type"); }
        public bool IsActive { get => A.Value<bool>("m_Active"); set => A.Value(value, "m_Active"); }
        public ItemModel Item => A.Object<ItemModel>("m_Item");
        public string ItemRef => A.Value<string>("m_ItemRef");
        public CharacterModel Owner => A.Object<CharacterModel>();
    }
}