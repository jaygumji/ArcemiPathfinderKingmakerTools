using System.Linq;
using System;
using System.Collections.Generic;
using Arcemi.Models.Kingmaker;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitModel : IGameUnitModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
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
        public IGameUnitSpellCasterModel SpellCaster { get; }

        public IGameModelCollection<IGameUnitFeatEntry> Feats { get; }
        public IGameModelCollection<IGameUnitAbilityEntry> Abilities { get; }
        public IGameModelCollection<IGameUnitBuffEntry> Buffs { get; }
        public IGameModelCollection<IGameUnitBuffEntry> UniqueBuffs { get; } = GameModelCollection<IGameUnitBuffEntry>.Empty;
        public IReadOnlyList<IGameDataObject> Sections { get; } = Array.Empty<IGameDataObject>();

        public UnitWearinessPartItemModel Weariness { get; }

        public WotrGameUnitModel(UnitEntityModel unit, IGameTimeProvider gameTimeProvider)
        {
            Ref = unit;
            if (unit.Descriptor is null) return;
            Portrait = new WotrGameUnitPortraitModel(unit);
            Companion = new WotrGameUnitCompanionModel(this, unit);
            Alignment = new WotrGameUnitAlignmentModel(unit);
            Asks = new WotrGameUnitAsksModel(unit);
            Race = new WotrGameUnitRaceModel(unit);
            Progression = new WotrGameUnitProgressionModel(this);
            Stats = new WotrGameUnitStatsModel(unit);
            Appearance = new WotrGameUnitAppearanceModel(this, unit.Parts.Items.OfType<UnitDollDataPartItemModel>().FirstOrDefault());
            Body = new WotrGameUnitBodyModel(unit);
            SpellCaster = new WotrGameUnitSpellCasterModel(unit);
            Feats = new GameModelCollection<IGameUnitFeatEntry, FactItemModel>(Ref.Facts.Items, x => new WotrGameUnitFeatEntry(x), x => x is FeatureFactItemModel feat
                && x.Context?.ParentContext?.SourceItemRef == null, new WotrGameModelCollectionFeatWriter());
            Abilities = new GameModelCollection<IGameUnitAbilityEntry, FactItemModel>(Ref.Facts.Items, x => new WotrGameUnitAbilityEntry(x), x => x is AbilityFactItemModel || x is ActivatableAbilityFactItemModel,
                new WotrGameModelCollectionAbilityWriter());

            bool IsBuff(FactItemModel fact) {
                if (fact is BuffFactItemModel) return true;
                if (Res.Blueprints.TryGet(fact.Blueprint, out var blueprint)) {
                    if (blueprint.Type == WotrBlueprintProvider.Buff) {
                        return true;
                    }
                }
                return false;
            }
            Buffs = new GameModelCollection<IGameUnitBuffEntry, FactItemModel>(Ref.Facts.Items, x => new WotrGameUnitBuffEntry(x, gameTimeProvider), IsBuff,
                new WotrGameModelCollectionBuffWriter(gameTimeProvider));

            var uniqueBuffs = Ref.Parts.Items.OfType<BuffUniquePartItemModel>().FirstOrDefault();
            if (uniqueBuffs?.Buffs?.Count > 0) {
                UniqueBuffs = new GameModelCollection<IGameUnitBuffEntry, BuffFactItemModel>(uniqueBuffs.Buffs, x => new WotrGameUnitBuffEntry(x, gameTimeProvider));
            }
            Weariness = Ref.Parts.Items.OfType<UnitWearinessPartItemModel>().FirstOrDefault();
            Sections = new[] {
                GameDataModels.Object("Misc", new IGameData[] {
                    GameDataModels.RowList(unit.Descriptor.Resources.PersistantResources, x => GameDataModels.Object(Res.Blueprints.GetNameOrBlueprint(x.Blueprint), new IGameData[] {
                        GameDataModels.Integer("Amount", x, item => item.Amount, (item, val) => item.Amount = val, minValue: 0),
                        GameDataModels.Integer("Retain", x, item => item.RetainCount, (item, val) => item.RetainCount = val, minValue: 0),
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
                if (Ref.Parts.Items.OfType<UnitPetPartItemModel>().Any()) return UnitEntityType.Pet;
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
