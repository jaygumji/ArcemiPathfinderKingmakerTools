using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameUnitDollModel : IGameUnitDollModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;

        public WotrGameUnitDollModel(DollDataModel doll, string name)
        {
            Ref = doll;
            Name = name;
        }

        public string Name { get; }
        public DollDataModel Ref { get; }
        public string Gender { get => Ref.Gender; set => Ref.Gender = value; }
        public IReadOnlyList<GenderOption> GenderOptions => GenderOption.Get(Gender, out _);

        public string Race { get => Ref.RacePreset; set => Ref.RacePreset = value; }
        public IReadOnlyList<BlueprintOption> RaceOptions => BlueprintOption.GetOptions(Res, BlueprintTypeId.RaceVisualPreset, Race, out _);

        public int ClothesPrimaryIndex { get => Ref.ClothesPrimaryIndex; set => Ref.ClothesPrimaryIndex = value; }
        public int ClothesSecondaryIndex { get => Ref.ClothesSecondaryIndex; set => Ref.ClothesSecondaryIndex = value; }

        public ListValueAccessor<string> EquipmentEntityIds => Ref.EquipmentEntityIds;

        public IReadOnlyList<GameKeyValueEntry<int>> EntityRampIdices => Ref.EntityRampIdices.Select(x => new GameKeyValueEntry<int>(x.Key, x.Value)).ToArray();
        public void RemoveEntityRampIdices(GameKeyValueEntry<int> item)
        {
            Ref.EntityRampIdices.Remove(item.Key);
        }
        public void AddEntityRampIdices(GameKeyValueEntry<int> item)
        {
            Ref.EntityRampIdices.Add(item.Key, item.Value);
        }
        public IReadOnlyList<GameKeyValueEntry<int>> EntitySecondaryRampIdices => Ref.EntitySecondaryRampIdices.Select(x => new GameKeyValueEntry<int>(x.Key, x.Value)).ToArray();
        public void RemoveEntitySecondaryRampIdices(GameKeyValueEntry<int> item)
        {
            Ref.EntitySecondaryRampIdices.Remove(item.Key);
        }
        public void AddEntitySecondaryRampIdices(GameKeyValueEntry<int> item)
        {
            Ref.EntitySecondaryRampIdices.Add(item.Key, item.Value);
        }

        public bool IsSupported => Ref is object;

        public string Export()
        {
            return Ref.Export();
        }

        public void Import(string code)
        {
            Ref.Import(code);
        }
    }
}