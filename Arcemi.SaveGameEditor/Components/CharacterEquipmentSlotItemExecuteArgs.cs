using Arcemi.Models;

namespace Arcemi.SaveGameEditor.Components
{
    public class CharacterEquipmentSlotItemExecuteArgs
    {
        public CharacterEquipmentSlotItemExecuteArgs(ItemModel item, bool isEdit = false)
        {
            Item = item;
            IsEdit = isEdit;
        }

        public ItemModel Item { get; }
        public bool IsEdit { get; }
    }
}
