﻿using System;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitAppearanceModel : IGameUnitAppearanceModel
    {
        public W40KRTGameUnitAppearanceModel(W40KRTGameUnitModel owner, UnitViewSettingsPartItemModel model)
        {
            Owner = owner;
            Ref = model;
            if (model is null) return;
            var dolls = new List<W40KRTGameUnitDollModel>();
            if (model.Doll is object) {
                dolls.Add(new W40KRTGameUnitDollModel(owner, model.Doll, "Main"));
            }
            Dolls = dolls;
        }

        public W40KRTGameUnitModel Owner { get; }
        public UnitViewSettingsPartItemModel Ref { get; }
        public IReadOnlyList<IGameUnitDollModel> Dolls { get; } = Array.Empty<IGameUnitDollModel>();
        public bool IsSupported => Ref is object && Owner.Type != UnitEntityType.Pet;

        public bool CanRestore => false;
        public bool CanDelete => false;
        public void Restore() { }
        public void Delete() { }
    }
}