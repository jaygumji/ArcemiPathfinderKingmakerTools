using System.Collections.Generic;
using System;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAppearanceModel : IGameUnitAppearanceModel
    {
        public KingmakerGameUnitAppearanceModel(DollDataModel model)
        {
            Ref = model;
        }

        public DollDataModel Ref { get; }
        public IReadOnlyList<IGameUnitDollModel> Dolls { get; } = Array.Empty<IGameUnitDollModel>();
        public bool IsSupported => false;
    }
}