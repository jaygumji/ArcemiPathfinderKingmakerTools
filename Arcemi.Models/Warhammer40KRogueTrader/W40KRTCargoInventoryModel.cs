using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTCargoInventoryModel : IGameInventoryModel
    {
        public W40KRTCargoInventoryModel(RefModel cargoState)
        {
            Ref = cargoState;
            if (IsSupported) {
                A = Ref?.GetAccessor();
                Sections = A.List<W40KRTCargoEntityModel>("m_CargoEntities");
            }
        }

        public RefModel Ref { get; }
        private ModelDataAccessor A { get; }

        public IReadOnlyList<IGameItemSection> Sections { get; }

        public bool IsSupported => false; // Ref is object;
    }
}