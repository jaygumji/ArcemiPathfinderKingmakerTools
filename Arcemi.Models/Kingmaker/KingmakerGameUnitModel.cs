using System.Collections.Generic;
using System.Linq;
using System;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitModel : IGameUnitModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public bool IsSupported => Ref.Descriptor is object;
        public UnitEntityModel Ref { get; }
        public string UniqueId => Ref.UniqueId;
        public string Name => CustomName.OrIfEmpty(DefaultName);
        public string DefaultName => Res.GetCharacterName(Ref.Descriptor.Blueprint);
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
        public IGameUnitSpellCasterModel SpellCaster { get; }

        public IGameModelCollection<IGameUnitFeatEntry> Feats { get; }
        public IGameModelCollection<IGameUnitAbilityEntry> Abilities { get; }
        public IGameModelCollection<IGameUnitBuffEntry> Buffs { get; }
        public IGameModelCollection<IGameUnitBuffEntry> UniqueBuffs { get; } = GameModelCollection<IGameUnitBuffEntry>.Empty;
        public IReadOnlyList<IGameDataObject> Sections { get; } = Array.Empty<IGameDataObject>();

        public UnitWearinessPartItemModel Weariness { get; }

        public KingmakerGameUnitModel(UnitEntityModel unit, IGameTimeProvider gameTimeProvider)
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
            Appearance = new KingmakerGameUnitAppearanceModel(unit);
            Body = new KingmakerGameUnitBodyModel(unit);
            SpellCaster = new KingmakerGameUnitSpellCasterModel(unit);

            Feats = new GameModelCollection<IGameUnitFeatEntry, FactItemModel>(Ref.Descriptor.Progression.Features.Facts, x => new KingmakerGameUnitFeatEntry(x), x => x is FeatureFactItemModel feat,
                new KingmakerGameModelCollectionFeatWriter(unit));
            Abilities = new GameModelCollection<IGameUnitAbilityEntry, FactItemModel>(
                Ref.Descriptor.GetAccessor().Object<RefModel>("Abilities").GetAccessor().List("m_Facts", FactItemModel.Factory), x => new KingmakerGameUnitAbilityEntry(x), x => x is AbilityFactItemModel feat,
                new KingmakerGameModelCollectionAbilityWriter());
            Buffs = new GameModelCollection<IGameUnitBuffEntry, FactItemModel>(
                Ref.Descriptor.GetAccessor().Object<RefModel>("Buffs").GetAccessor().List("m_Facts", FactItemModel.Factory), x => new KingmakerGameUnitBuffEntry(x, gameTimeProvider), x => x is BuffFactItemModel feat,
                new KingmakerGameModelCollectionBuffWriter());

            var parts = Ref.Descriptor.GetAccessor().Object<KingmakerPartsContainerModel>("m_Parts");
            Weariness = (UnitWearinessPartItemModel)parts.Items.FirstOrDefault(x => x.Value is UnitWearinessPartItemModel)?.Value;
            Sections = new[] {
                GameDataModels.Object("Misc", new IGameData[] {
                    GameDataModels.RowList(unit.Descriptor.Resources.PersistantResources, x => GameDataModels.Object(Res.Blueprints.GetNameOrBlueprint(x.Blueprint), new IGameData[] {
                        GameDataModels.Integer("Amount", x, item => item.Amount, (item, val) => item.Amount = val, minValue: 0)
                    })),
                    GameDataModels.Object("Weariness", new IGameData[] {
                        GameDataModels.Message("To easily remove weariness from your party, Use the button on the party overview. Set extra hours to a negative value to make the character last longer until needing rest"),
                        GameDataModels.Integer("Stacks", Weariness, w => w.WearinessStacks, (w,v) => w.WearinessStacks = v, minValue: int.MinValue),
                        GameDataModels.Double("Extra hours", Weariness, w => w.ExtraWearinessHours, (w, v) => w.ExtraWearinessHours = v, minValue: 0),
                        GameDataModels.Time("Tick", new TimeParts(() => Weariness?.LastStackTime ?? TimeSpan.Zero, v => { if (Weariness is object) Weariness.LastStackTime = v; })),
                        GameDataModels.Time("Debuff applied", new TimeParts(() => Weariness?.LastBuffApplyTime ?? TimeSpan.Zero, v => { if (Weariness is object) Weariness.LastBuffApplyTime = v; }))
                    })
                })
            };
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

    public class KingmakerPartsContainerModel : RefModel
    {
        public KingmakerPartsContainerModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListAccessor<KeyValuePairObjectModel<PartItemModel>> Items => A.List("m_Parts", a => new KeyValuePairObjectModel<PartItemModel>(a, PartItemModel.Factory), createIfNull: true);
    }

}