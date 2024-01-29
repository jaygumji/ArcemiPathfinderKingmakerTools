using System.Collections.Generic;
using System.Linq;
using System;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitModel : IGameUnitModel
    {
        public bool IsSupported => Ref.Descriptor is object;
        public UnitEntityModel Ref { get; }
        public string UniqueId => Ref.UniqueId;
        public string Name => CustomName.OrIfEmpty(DefaultName);
        public string DefaultName => GameDefinition.Pathfinder_Kingmaker.Resources.GetCharacterName(Ref.Descriptor.Blueprint);
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

        public KingmakerGameUnitModel(UnitEntityModel unit)
        {
            Ref = unit;
            if (unit.Descriptor is null) return;
            Portrait = new KingmakerGameUnitPortraitModel(unit);
            Companion = new KingmakerGameUnitCompanionModel(this, unit);
            Alignment = new KingmakerGameUnitAlignmentModel(unit);
            Asks = new KingmakerGameUnitAsksModel(unit);
            Race = new KingmakerGameUnitRaceModel(unit);
            Progression = new KingmakerGameUnitProgressionModel(unit);
            Stats = new KingmakerGameUnitStatsModel(unit);
            Appearance = new KingmakerGameUnitAppearanceModel(unit.Descriptor.GetAccessor().Object<DollDataModel>("Doll"));
            Body = new KingmakerGameUnitBodyModel(unit);

            Feats = new GameModelCollection<IGameUnitFeatEntry, FactItemModel>(Ref.Descriptor.Progression.Features.Facts, x => new KingmakerGameUnitFeatEntry(x), x => x is FeatureFactItemModel feat,
                new KingmakerGameModelCollectionFeatWriter());
            Abilities = new GameModelCollection<IGameUnitAbilityEntry, FactItemModel>(
                Ref.Descriptor.GetAccessor().Object<RefModel>("Abilities").GetAccessor().List("m_Facts", FactItemModel.Factory), x => new KingmakerGameUnitAbilityEntry(x), x => x is AbilityFactItemModel feat,
                new KingmakerGameModelCollectionAbilityWriter());
            Buffs = new GameModelCollection<IGameUnitBuffEntry, FactItemModel>(
                Ref.Descriptor.GetAccessor().Object<RefModel>("Buffs").GetAccessor().List("m_Facts", FactItemModel.Factory), x => new KingmakerGameUnitBuffEntry(x), x => x is BuffFactItemModel feat,
                new KingmakerGameModelCollectionBuffWriter());
        }

        public UnitEntityType Type
        {
            get {
                var blueprint = Ref.Descriptor?.Blueprint;
                if (blueprint.Eq("4391e8b9afbb0cf43aeba700c089f56d")) return UnitEntityType.Player;
                if (blueprint.Eq("baaff53a675a84f4983f1e2113b24552")) return UnitEntityType.Mercenary;
                //if (Ref.Parts.Items.OfType<UnitPetPartItemModel>().Any()) return UnitEntityType.Pet;
                return UnitEntityType.Companion;
            }
        }

        public void ReplacePartyMemberWith(IGameUnitModel unit)
        {
            unit.Companion.State = CompanionPartState.InParty;
            Companion.State = CompanionPartState.Remote;
        }

        public void AddToRetinue()
        {
            Companion.State = CompanionPartState.Remote;
        }
    }
}