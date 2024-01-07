using System.Linq;
using System;
using System.Collections.Generic;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitModel : IGameUnitModel
    {
        public bool IsSupported => Ref.Descriptor is object;
        public UnitEntityModel Ref { get; }
        public string UniqueId => Ref.UniqueId;
        public string Name => CustomName ?? DefaultName;
        public string DefaultName => GameDefinition.Pathfinder_WrathOfTheRighteous.Resources.GetCharacterName(Ref.Descriptor.Blueprint);
        public string CustomName { get => Ref.Descriptor?.CustomName; set => Ref.Descriptor.CustomName = value; }

        public IGameUnitPortraitModel Portrait { get; }
        public IGameUnitCompanionModel Companion { get; }
        public IGameUnitAlignmentModel Alignment { get; }
        public IGameUnitAsksModel Asks { get; }
        public IGameUnitRaceModel Race { get; }
        public IGameUnitProgressionModel Progression { get; }
        public IGameUnitStatsModel Stats { get; }
        public IGameUnitAppearanceModel Appearance { get; }
        public IGameUnitBodyModel Body { get; }
        public IGameDataObject Overview { get; }

        public IGameModelCollection<IGameUnitFeatEntry> Feats { get; }
        public IGameModelCollection<IGameUnitAbilityEntry> Abilities { get; }
        public IGameModelCollection<IGameUnitBuffEntry> Buffs { get; }
        public IReadOnlyList<IGameDataObject> Sections { get; } = Array.Empty<IGameDataObject>();

        public WotrGameUnitModel(UnitEntityModel unit)
        {
            Ref = unit;
            if (unit.Descriptor is null) return;
            Portrait = new WotrGameUnitPortraitModel(unit);
            Companion = new WotrGameUnitCompanionModel(this, unit);
            Alignment = new WotrGameUnitAlignmentModel(unit);
            Asks = new WotrGameUnitAsksModel(unit);
            Race = new WotrGameUnitRaceModel(unit);
            Progression = new WotrGameUnitProgressionModel(unit);
            Stats = new WotrGameUnitStatsModel(unit);
            Appearance = new WotrGameUnitAppearanceModel(unit.Parts.Items.OfType<UnitDollDataPartItemModel>().FirstOrDefault());
            Body = new WotrGameUnitBodyModel(unit);
            Feats = new GameModelCollection<IGameUnitFeatEntry, FactItemModel>(Ref.Facts.Items, x => new WotrGameUnitFeatEntry(x), x => x is FeatureFactItemModel feat
                && x.Context?.ParentContext?.SourceItemRef == null, new WotrGameModelCollectionFeatWriter());
            Abilities = new GameModelCollection<IGameUnitAbilityEntry, FactItemModel>(Ref.Facts.Items, x => new WotrGameUnitAbilityEntry(x), x => x is AbilityFactItemModel feat,
                new WotrGameModelCollectionAbilityWriter());
            Buffs = new GameModelCollection<IGameUnitBuffEntry, FactItemModel>(Ref.Facts.Items, x => new WotrGameUnitBuffEntry(x), x => x is BuffFactItemModel feat,
                new WotrGameModelCollectionBuffWriter());
        }

        public UnitEntityType Type
        {
            get {
                var blueprint = Ref.Descriptor?.Blueprint;
                if (blueprint.Eq("4391e8b9afbb0cf43aeba700c089f56d")) return UnitEntityType.Player;
                if (blueprint.Eq("baaff53a675a84f4983f1e2113b24552")) return UnitEntityType.Mercenary;
                if (Ref.Parts.Items.OfType<UnitPetPartItemModel>().Any()) return UnitEntityType.Pet;
                return UnitEntityType.Companion;
            }
        }
    }
}
