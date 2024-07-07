using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitDollModel : IGameUnitDollModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;

        public W40KRTGameUnitDollModel(W40KRTGameUnitModel unit, W40KRTDollModel doll, string name)
        {
            Unit = unit;
            Ref = doll;
            Name = name;
        }

        public string Name { get; }
        public W40KRTGameUnitModel Unit { get; }
        public W40KRTDollModel Ref { get; }
        public string Gender
        {
            get => Ref.Gender;
            set {
                Ref.Gender = value;
                if (Unit?.RefDescriptor is object) {
                    Unit.RefDescriptor.CustomGender = value;
                }
            }
        }
        public IReadOnlyList<GenderOption> GenderOptions => GenderOption.Get(Gender, out _);

        public string Race { get => Ref.RacePreset; set => Ref.RacePreset = value; }
        public IReadOnlyList<BlueprintOption> RaceOptions { get; } = new[] {
            new BlueprintOption("58181bf151eb0c0408f82546541dcc03", "Human"),
            new BlueprintOption("10d74847c1492bf428b462f948e69d4f", "Human Large"),
            new BlueprintOption("e03b9c63971878743b8f53bdf14673ee", "Human Thin"),
        };

        public int ClothesPrimaryIndex { get => Ref.ClothesPrimaryIndex; set => Ref.ClothesPrimaryIndex = value; }
        public int ClothesSecondaryIndex { get => Ref.ClothesSecondaryIndex; set => Ref.ClothesSecondaryIndex = value; }

        public ListValueAccessor<string> EquipmentEntityIds => Ref.EquipmentEntityIds;
        public IReadOnlyList<GameKeyValueEntry<int>> EntityRampIdices => Ref.EntityRampIdices.Select(x => new GameKeyValueEntry<int>(x.Key, x.Value)).ToArray();
        public void RemoveEntityRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = Ref.EntityRampIdices.FirstOrDefault(x => x.Key.Eq(item.Key));
            Ref.EntityRampIdices.Remove(actItem);
        }
        public void AddEntityRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = Ref.EntityRampIdices.Add();
            actItem.Key = item.Key;
            actItem.Value = item.Value;
        }
        public IReadOnlyList<GameKeyValueEntry<int>> EntitySecondaryRampIdices => Ref.EntitySecondaryRampIdices.Select(x => new GameKeyValueEntry<int>(x.Key, x.Value)).ToArray();
        public void RemoveEntitySecondaryRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = Ref.EntitySecondaryRampIdices.FirstOrDefault(x => x.Key.Eq(item.Key));
            Ref.EntitySecondaryRampIdices.Remove(actItem);
        }
        public void AddEntitySecondaryRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = Ref.EntitySecondaryRampIdices.Add();
            actItem.Key = item.Key;
            actItem.Value = item.Value;
        }

        public bool IsSupported => Ref is object;

        public string Export() => Ref.Export();

        public void Import(string code)
        {
            Ref.Import(code);
            if (Unit?.RefDescriptor is object) {
                Unit.RefDescriptor.CustomGender = Ref.Gender;
            }
        }
    }
}