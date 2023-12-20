using System;
using System.Collections.Generic;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitAppearanceModel : IGameUnitAppearanceModel
    {
        public WotrGameUnitAppearanceModel(UnitDollDataPartItemModel model)
        {
            Ref = model;
            if (model is null) return;
            var dolls = new List<WotrGameUnitDollModel>();
            if (model.Default is object) {
                dolls.Add(new WotrGameUnitDollModel(model.Default, "Main"));
            }
            if (model.Special?.KitsunePolymorph is object) {
                dolls.Add(new WotrGameUnitDollModel(model.Special.KitsunePolymorph, "Kitsune polymorph"));
            }
            Dolls = dolls;
        }

        public UnitDollDataPartItemModel Ref { get; }
        public IReadOnlyList<IGameUnitDollModel> Dolls { get; } = Array.Empty<IGameUnitDollModel>();
        public bool IsSupported => true;
    }
}