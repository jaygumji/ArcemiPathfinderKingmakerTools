using Arcemi.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitModel : Model, IGameUnitModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public string UniqueId => Ref.UniqueId;
        public string Name => CustomName ?? DefaultName;
        public string DefaultName => Res.GetCharacterName(Blueprint);
        public string CustomName { get => RefDescriptor?.CustomName; set => RefDescriptor.CustomName = value; }
        public bool IsSupported => RefDescriptor is object;

        public UnitEntityModel Ref { get; }
        public UnitDescriptionPartItemModel RefDescriptor { get; }
        public PartItemModel RefInventory { get; }

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
        public IReadOnlyList<IGameDataObject> Sections { get; }

        public W40KRTGameUnitModel(UnitEntityModel unit)
            : base(unit.GetAccessor())
        {
            Ref = unit;
            foreach (var part in unit.Parts.Container) {
                var type = part.GetAccessor().TypeValue();
                if (part is UnitDescriptionPartItemModel descriptor) {
                    RefDescriptor = descriptor;
                }
                else if (part is UnitProgressionPartItemModel progression) {
                    Progression = new W40KRTGameUnitProgressionModel(this, progression);
                    Race = new W40KRTGameUnitRaceModel(progression);
                }
                else if (part is UnitBodyPartItemModel body) {
                    Body = new W40KRTGameUnitBodyModel(this, body);
                }
                else if (part is UnitViewSettingsPartItemModel viewSettings) {
                    Appearance = new W40KRTGameUnitAppearanceModel(viewSettings);
                }
                else if (part is UnitAsksPartItemModel asks) {
                    Asks = new W40KRTGameUnitAsksModel(asks);
                }
                else if (part is UnitUISettingsPartItemModel uiSettings) {
                    Portrait = new W40KRTGameUnitPortraitModel(uiSettings, Blueprint);
                }
                else if (part is UnitCompanionPartItemModel companion) {
                    Companion = new W40KRTGameUnitCompanionModel(this, companion);
                }
                else if (part is UnitAlignmentPartItemModel alignment) {
                    Alignment = new W40KRTGameUnitAlignmentModel(alignment);
                }
                else if (part is StatsContainerPartItemModel stats) {
                    Stats = new W40KRTGameUnitStatsModel(stats);
                }
                else if (type.Eq("Kingmaker.UnitLogic.Parts.PartInventory, Code")) {
                    RefInventory = part;
                }
            }
            if (Portrait is null) Portrait = new W40KRTGameUnitPortraitModel(null, null);
            if (Companion is null) Companion = new W40KRTGameUnitCompanionModel(this, null);
            if (Alignment is null) Alignment = new W40KRTGameUnitAlignmentModel(null);
            if (Asks is null) Asks = new W40KRTGameUnitAsksModel(null);
            if (Race is null) Race = new W40KRTGameUnitRaceModel(null);
            if (Progression is null) Progression = new W40KRTGameUnitProgressionModel(this, null);
            if (Stats is null) Stats = new W40KRTGameUnitStatsModel(null);
            if (Appearance is null) Appearance = new W40KRTGameUnitAppearanceModel(null);
            if (Body is null) Body = new W40KRTGameUnitBodyModel(this, null);

            SpellCaster = new W40KRTGameUnitSpellCasterModel(this, Ref);
            Feats = new GameModelCollection<IGameUnitFeatEntry, FactItemModel>(Ref.Facts.Items, x => new W40KRTGameUnitFeatEntry(x), x => x is W40KRTFeatFactItemModel, new W40KRTGameModelFeatCollectionWriter(Ref.UniqueId));
            Abilities = new GameModelCollection<IGameUnitAbilityEntry, FactItemModel>(Ref.Facts.Items, x => new W40KRTGameUnitAbilityEntry(x), x => x is W40KRTAbilityFactItemModel, new W40KRTGameModelAbilityCollectionWriter());
            Buffs = new GameModelCollection<IGameUnitBuffEntry, FactItemModel>(Ref.Facts.Items, x => new W40KRTGameUnitBuffEntry(x), x => x is W40KRTBuffFactItemModel, new W40KRTGameModelBuffCollectionWriter());

            if (Type == UnitEntityType.Starship) {
                var starshipTypes = Res.Blueprints.GetEntries(W40KRTBlueprintProvider.Starship)
                    .Where(x => x.Name.Original.IEnd("Player_Starship"));

                Overview = GameDataModels.Object("Starship", new IGameData[] {
                    GameDataModels.BlueprintOptions("Type", starshipTypes, this, x => x.Blueprint, (x, v) => x.Blueprint = v)
                });
            }
            else if (Type.IsCharacter()) {
                var soulMarks = Ref.Facts.Items.OfType<W40KRTSoulMarkFactItemModel>().ToArray();
                var heretical = soulMarks.FirstOrDefault(sm => sm.Blueprint.Eq("175d1fd853b24f188a4078306ca066ad"));
                var dogmatic = soulMarks.FirstOrDefault(sm => sm.Blueprint.Eq("1aa7cb5ae17c4ed19aa2596b6bcca9d3"));
                var iconoclast = soulMarks.FirstOrDefault(sm => sm.Blueprint.Eq("676567cf7bb8459abded7ee617d1625e"));
                var biographyProperties = new List<IGameData>();
                if (iconoclast is object) biographyProperties.Add(GameDataModels.Integer("Iconoclast", iconoclast, i => i.Rank - 1, (i, v) => i.Rank = v + 1, 0, int.MaxValue));
                if (dogmatic is object) biographyProperties.Add(GameDataModels.Integer("Dogmatic", dogmatic, i => i.Rank - 1, (i, v) => i.Rank = v + 1, 0, int.MaxValue));
                if (heretical is object) biographyProperties.Add(GameDataModels.Integer("Heretical", heretical, i => i.Rank - 1, (i, v) => i.Rank = v + 1, 0, int.MaxValue));

                if (iconoclast is object) biographyProperties.Add(GameDataModels.Object("Iconoclast", new[] {
                    GameDataModels.RowList(iconoclast.Sources, x => GameDataModels.Object(x.DisplayName(Res), new[] {
                        GameDataModels.Integer("Rank", x, sm => sm.PathRank, (sm, v) => sm.PathRank = v)
                    }, x), x => x.PathRank > 0, new W40KRTSoulMarkSourceCollectionWriter(), itemName: "Choice")
                }, isCollapsable: true));
                if (dogmatic is object) biographyProperties.Add(GameDataModels.Object("Dogmatic", new[] {
                    GameDataModels.RowList(dogmatic.Sources, x => GameDataModels.Object(x.DisplayName(Res), new[] {
                        GameDataModels.Integer("Rank", x, sm => sm.PathRank, (sm, v) => sm.PathRank = v)
                    }, x), x => x.PathRank > 0, new W40KRTSoulMarkSourceCollectionWriter(), itemName: "Choice")
                }, isCollapsable: true));
                if (heretical is object) biographyProperties.Add(GameDataModels.Object("Heretical", new[] {
                    GameDataModels.RowList(heretical.Sources, x => GameDataModels.Object(x.DisplayName(Res), new[] {
                        GameDataModels.Integer("Rank", x, sm => sm.PathRank, (sm, v) => sm.PathRank = v)
                    }, x), x => x.PathRank > 0, new W40KRTSoulMarkSourceCollectionWriter(), itemName: "Choice")
                }, isCollapsable: true));

                biographyProperties.Add(GameDataModels.Object("Soul Mark", new[] {
                    GameDataModels.RowList(Ref.Facts.Items, x => GameDataModels.Object(x.DisplayName(Res), new[] {
                        GameDataModels.Integer("Rank", x, sm => sm.GetAccessor().Value<int>("Rank"))
                    }, x), x => x is W40KRTSoulMarkFactItemModel, new W40KRTGameUnitSoulMarkCollectionWriter(unit.UniqueId), itemName: "Soul Mark")
                }, isCollapsable: true));

                Sections = new IGameDataObject[] {
                    GameDataModels.Object("Biography", biographyProperties)
                };
            }
        }

        private string Blueprint { get => A.Value<string>(); set => A.Value(value); }

        public UnitEntityType Type
        {
            get {
                if (A.TypeValue().Eq("Kingmaker.EntitySystem.Entities.UnitEntity, Code")) {
                    if (Blueprint.Eq("3a849d3674644c0085d5099ccf6813df")) return UnitEntityType.Player;
                    if (Blueprint.Eq("baaff53a675a84f4983f1e2113b24552")) return UnitEntityType.Mercenary;
                    if (Ref.Parts.Items.OfType<UnitPetPartItemModel>().Any()) return UnitEntityType.Pet;
                    return UnitEntityType.Companion;
                }
                if (A.TypeValue().Eq("Kingmaker.EntitySystem.Entities.StarshipEntity, Code")) return UnitEntityType.Starship;
                if (A.TypeValue().Eq("Kingmaker.EntitySystem.Entities.AreaEffectEntity, Code")) return UnitEntityType.Other;
                if (A.TypeValue().Eq("Kingmaker.Mechanics.Entities.LightweightUnitEntity, Code")) return UnitEntityType.Other;
                return UnitEntityType.Other;
            }
        }

        public void ReplacePartyMemberWith(IGameUnitModel unit)
        {
            unit.Companion.State = CompanionPartState.InParty;
            Companion.State = CompanionPartState.Remote;

            var sourcePos = A.Object<Vector3Model>("m_Position");
            var targetPos = unit.Ref.GetAccessor().Object<Vector3Model>("m_Position");

            targetPos.X = sourcePos.X;
            targetPos.Y = sourcePos.Y;
            targetPos.Z = sourcePos.Z;

            A.Value(false, "m_IsInGame");
            unit.Ref.GetAccessor().Value(true, "m_IsInGame");
        }

        public void AddToRetinue()
        {
            Companion.State = CompanionPartState.Remote;
            var combatGroup = Ref.Parts.Container.OfType<CombatGroupPartItemModel>().FirstOrDefault();
            const string controllableId = "<directly-controllable-unit>";
            if (combatGroup is object && !combatGroup.ControlId.Eq(controllableId)) {
                combatGroup.ControlId = controllableId;
            }
        }
    }
}
