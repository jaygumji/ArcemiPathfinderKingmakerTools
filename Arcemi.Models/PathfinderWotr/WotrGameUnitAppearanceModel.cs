using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitAppearanceModel : IGameUnitAppearanceModel
    {
        public WotrGameUnitAppearanceModel(IGameUnitModel unit, UnitDollDataPartItemModel model)
        {
            Unit = unit;
            Ref = model;
            if (model is null) return;
            if (model.Default is object) {
                _dolls.Add(new WotrGameUnitDollModel(model.Default, "Main"));
            }
            if (model.Special?.KitsunePolymorph is object) {
                _dolls.Add(new WotrGameUnitDollModel(model.Special.KitsunePolymorph, "Kitsune polymorph"));
            }
        }

        public IGameUnitModel Unit { get; }
        public UnitDollDataPartItemModel Ref { get; private set; }

        private readonly List<IGameUnitDollModel> _dolls = new List<IGameUnitDollModel>();
        public IReadOnlyList<IGameUnitDollModel> Dolls => _dolls;

        public bool CanRestore => Ref is null;
        public void Restore()
        {
            if (!CanRestore) return;

            if (Ref is null) {
                Ref = (UnitDollDataPartItemModel)Unit.Ref.Parts.Items.Add((r, o) => {
                    o.Add("$type", UnitDollDataPartItemModel.TypeRef);
                    JObject InitDoll()
                    {
                        return new JObject {
                            {"EquipmentEntityIds", new JArray()},
                            {"EntityRampIdices", new JObject()},
                            {"EntitySecondaryRampIdices", new JObject()},
                            {"Gender", "Female"},
                            {"RacePreset", "58181bf151eb0c0408f82546541dcc03"},
                            {"ClothesPrimaryIndex", 1},
                            {"ClothesSecondaryIndex", 1},
                        };
                    }

                    o.Add("Default", InitDoll());
                    const string kitsuneRace = "fd188bb7bb0002e49863aec93bfb9d99";
                    if (Unit.Ref.Descriptor.Progression.Race.Eq(kitsuneRace)) {
                        o.Add("Special", new JObject {
                            {"KitsunePolymorph", InitDoll()}
                        });
                    }
                });

                _dolls.Add(new WotrGameUnitDollModel(Ref.Default, "Main"));
                if (Ref.Special?.KitsunePolymorph is object) {
                    _dolls.Add(new WotrGameUnitDollModel(Ref.Special.KitsunePolymorph, "Kitsune polymorph"));
                }
            }
        }

        public bool CanDelete => Ref is object && Dolls.Count > 0
            && (Unit.Type == UnitEntityType.Companion || Unit.Type == UnitEntityType.Pet || Unit.Type == UnitEntityType.Other);
        public void Delete()
        {
            if (Unit.Ref.Parts.Items.Remove(Ref)) {
                Ref = null;
                _dolls.Clear();
            }
        }

        public bool IsSupported => true;
    }
}