using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitModel : Model, IGameUnitModel
    {
        public string UniqueId => Ref.UniqueId;
        public string Name => CustomName ?? DefaultName;
        public string DefaultName => GameDefinition.Warhammer40K_RogueTrader.Resources.GetCharacterName(Blueprint);
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

        public IGameModelCollection<IGameUnitFeatEntry> Feats { get; }
        public IGameModelCollection<IGameUnitAbilityEntry> Abilities { get; }
        public IGameModelCollection<IGameUnitBuffEntry> Buffs { get; }

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
                    Progression = new W40KRTGameUnitProgressionModel(progression);
                    Race = new W40KRTGameUnitRaceModel(progression);
                }
                else if (part is UnitBodyPartItemModel body) {
                }
                else if (part is UnitAsksPartItemModel asks) {
                    Asks = new W40KRTGameUnitAsksModel(asks);
                }
                else if (part is UnitUISettingsPartItemModel uiSettings) {
                    Portrait = new W40KRTGameUnitPortraitModel(uiSettings, Blueprint);
                }
                else if (part is UnitCompanionPartItemModel companion) {
                    Companion = new W40KRTGameUnitCompanionModel(companion);
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
            if (Companion is null) Companion = new W40KRTGameUnitCompanionModel(null);
            if (Alignment is null) Alignment = new W40KRTGameUnitAlignmentModel(null);
            if (Asks is null) Asks = new W40KRTGameUnitAsksModel(null);
            if (Race is null) Race = new W40KRTGameUnitRaceModel(null);
            if (Progression is null) Progression = new W40KRTGameUnitProgressionModel(null);
            if (Stats is null) Stats = new W40KRTGameUnitStatsModel(null);

            Feats = new GameModelCollection<IGameUnitFeatEntry, FactItemModel>(Ref.Facts.Items, x => new W40KRTGameUnitFeatEntry(x), x => x is W40KRTFeatFactItemModel, new W40KRTGameModelFeatCollectionWriter());
            Abilities = new GameModelCollection<IGameUnitAbilityEntry, FactItemModel>(Ref.Facts.Items, x => new W40KRTGameUnitAbilityEntry(x), x => x is W40KRTAbilityFactItemModel, new W40KRTGameModelAbilityCollectionWriter());
            Buffs = new GameModelCollection<IGameUnitBuffEntry, FactItemModel>(Ref.Facts.Items, x => new W40KRTGameUnitBuffEntry(x), x => x is W40KRTBuffFactItemModel, new W40KRTGameModelBuffCollectionWriter());
        }

        private string Blueprint => A.Value<string>();

        public UnitEntityType Type
        {
            get {
                if (Blueprint.Eq("3a849d3674644c0085d5099ccf6813df")) return UnitEntityType.Player;
                if (Blueprint.Eq("1793b8c1fd824805908bee8ed0e95041")) return UnitEntityType.Starship;
                if (Ref.Parts.Items.OfType<UnitPetPartItemModel>().Any()) return UnitEntityType.Pet;
                return UnitEntityType.Companion;
            }
        }
    }
}
