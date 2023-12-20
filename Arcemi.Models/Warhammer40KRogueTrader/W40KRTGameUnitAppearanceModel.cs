using System;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitAppearanceModel : IGameUnitAppearanceModel
    {
        public W40KRTGameUnitAppearanceModel(UnitViewSettingsPartItemModel model)
        {
            Ref = model;
            if (model is null) return;
            var dolls = new List<W40KRTGameUnitDollModel>();
            if (model.Doll is object) {
                dolls.Add(new W40KRTGameUnitDollModel(model.Doll, "Main"));
            }
            Dolls = dolls;
        }

        public UnitViewSettingsPartItemModel Ref { get; }
        public IReadOnlyList<IGameUnitDollModel> Dolls { get; } = Array.Empty<IGameUnitDollModel>();
        public bool IsSupported => Ref is object;
    }
}