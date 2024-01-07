using Arcemi.Models;

namespace Arcemi.SaveGameEditor.Components
{
    public class CharacterEquipmentSlotItemExecuteArgs
    {
        public CharacterEquipmentSlotItemExecuteArgs(IGameItemEntry item, bool isEdit = false)
        {
            Item = item;
            IsEdit = isEdit;
        }

        public IGameItemEntry Item { get; }
        public bool IsEdit { get; }
    }
}
