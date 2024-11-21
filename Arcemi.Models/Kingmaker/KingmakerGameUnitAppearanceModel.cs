using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAppearanceModel : IGameUnitAppearanceModel
    {
        public KingmakerGameUnitAppearanceModel(UnitEntityModel @ref)
        {
            var doll = @ref.Descriptor.GetAccessor().Object("Doll", a => new KingmakerGameUnitDollModel(a, "Main"));
            Dolls = doll is object
                ? new IGameUnitDollModel[] { doll }
                : Array.Empty<IGameUnitDollModel>();
            Ref = @ref;
        }

        public IReadOnlyList<IGameUnitDollModel> Dolls { get; }
        public bool IsSupported => Ref is object;

        public bool CanRestore => false;
        public bool CanDelete => false;
        public void Restore() { }
        public void Delete() { }

        public UnitEntityModel Ref { get; }
    }

    internal class KingmakerGameUnitDollModel : RefModel, IGameUnitDollModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;

        public KingmakerGameUnitDollModel(ModelDataAccessor accessor, string name) : base(accessor)
        {
            Name = name;
        }

        public string Name { get; }
        public string Gender { get => A.Value<string>(); set => A.Value(value); }
        public string RacePreset { get => A.Value<string>(); set => A.Value(value); }
        public int ClothesPrimaryIndex { get => A.Value<int>(); set => A.Value(value); }
        public int ClothesSecondaryIndex { get => A.Value<int>(); set => A.Value(value); }
        public ListAccessor<KeyValuePairModel<int>> EntityRampIdices => A.List<KeyValuePairModel<int>>();
        public ListAccessor<KeyValuePairModel<int>> EntitySecondaryRampIdices => A.List<KeyValuePairModel<int>>();

        public IReadOnlyList<GenderOption> GenderOptions => GenderOption.Get(Gender, out _);

        string IGameUnitDollModel.Race { get => RacePreset; set => RacePreset = value; }
        public IReadOnlyList<BlueprintOption> RaceOptions => BlueprintOption.GetOptions(Res, BlueprintTypeId.RaceVisualPreset, RacePreset, out _);

        public ListValueAccessor<string> EquipmentEntityIds => A.ListValue<string>();

        IReadOnlyList<GameKeyValueEntry<int>> IGameUnitDollModel.EntityRampIdices => EntityRampIdices.Select(x => new GameKeyValueEntry<int>(x.Key, x.Value)).ToArray();
        public void RemoveEntityRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = EntityRampIdices.FirstOrDefault(x => x.Key.Eq(item.Key));
            if (actItem is null) return;
            EntityRampIdices.Remove(actItem);
        }
        public void AddEntityRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = EntityRampIdices.Add();
            actItem.Key = item.Key;
            actItem.Value = item.Value;
        }
        IReadOnlyList<GameKeyValueEntry<int>> IGameUnitDollModel.EntitySecondaryRampIdices => EntitySecondaryRampIdices.Select(x => new GameKeyValueEntry<int>(x.Key, x.Value)).ToArray();
        public void RemoveEntitySecondaryRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = EntitySecondaryRampIdices.FirstOrDefault(x => x.Key.Eq(item.Key));
            if (actItem is null) return;
            EntitySecondaryRampIdices.Remove(actItem);
        }
        public void AddEntitySecondaryRampIdices(GameKeyValueEntry<int> item)
        {
            var actItem = EntitySecondaryRampIdices.Add();
            actItem.Key = item.Key;
            actItem.Value = item.Value;
        }

        public bool IsSupported => true;
    }
}