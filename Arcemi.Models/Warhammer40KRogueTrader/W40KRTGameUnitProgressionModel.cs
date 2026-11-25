using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitProgressionModel : IGameUnitProgressionModel
    {
        private readonly W40KRTUnitMediator _mediator;

        public W40KRTGameUnitProgressionModel(IGameUnitModel owner, UnitProgressionPartItemModel model, W40KRTUnitMediator mediator)
        {
            Owner = owner;
            Model = model;
            _mediator = mediator;
            Selections = new GameModelCollection<IGameUnitSelectionProgressionEntry, UnitProgressionSelectionOfPartModel>(model?.Selections, s => new W40KRTGameUnitSelectionProgressionEntry(owner, s), writer: new W40KRTGameUnitSelectionProgressionEntryWriter(owner));

            if (model is null) return;
            var dataProperties = new List<IGameData>();
            var respecs = model.GetAccessor().Object<Model>("RespecInfo");
            if (respecs is object) {
                dataProperties.Add(GameDataModels.Object("Respecs", new IGameData[] {
                    GameDataModels.Integer("Count", respecs, r => r.GetAccessor().Value<int>("m_RespecCount"), (r, v) => r.GetAccessor().Value(v, "m_RespecCount"), 0, size: GameDataSize.Small),
                    GameDataModels.Boolean("Extra", respecs, r => r.GetAccessor().Value<bool>("m_HasExtraRespec"), (r, v) => r.GetAccessor().Value(v, "m_HasExtraRespec"))
                }, isCollapsable: true));
            }
            dataProperties.Add(GameDataModels.Object("All choices", new[] {
                GameDataModels.RowList(Selections.Project(x => (IGameDataObject)x), nameSize: GameDataSize.Medium)
            }, isCollapsable: true));
            Data = GameDataModels.Object(dataProperties);
        }

        public IGameUnitModel Owner { get; }
        public UnitProgressionPartItemModel Model { get; }
        public IGameDataObject Data { get; }

        public int Experience { get => Model.Experience; set => Model.Experience = value; }
        public int CurrentLevel
        {
            get => Model.CharacterLevel;
            set {
                if (value <= 0 || value > Model.CharacterLevel) return;
                var steps = Model.CharacterLevel - value;
                for (var i = 0; i < steps; i++) {
                    // Downgrade one level at a time, easier to calculate
                    DowngradeLevel();
                }
            }
        }

        private void DowngradeLevel()
        {
            var level = Model.CharacterLevel;
            var oldSelections = Selections.OfType<W40KRTGameUnitSelectionProgressionEntry>()
                .Where(x => !x.IsCreation && x.Level >= level).Reverse().ToArray();
            var careerPaths = (from tier in W40KRTArchetypes.All
                               from path in tier.Types
                               from feat in Owner.Feats
                               where feat.Blueprint.Eq(path.CareerPathId)
                               select (tier, path, feat)).ToArray();

            foreach (var selection in oldSelections) {
                W40KRTProgressionFacts.RemoveSelectionRank(Owner, selection.Ref);
                Selections.Remove(selection);
            }

            foreach (var (tier, path, feat) in careerPaths) {
                var pathRank = feat.Rank;
                if (pathRank <= 0 || tier.Level + pathRank - 1 != level) continue;
                var automatic = W40KRTProgressionFacts.GetAutomaticFeatures(path.CareerPathId, pathRank)
                    ?? tier.AutomaticFeats.Concat(path.AutomaticFeats)
                        .Where(f => f.Level == pathRank && pathRank > 1).Select(f => f.Id)
                        .Concat(pathRank == 1 ? path.KeystoneAbilityIds : Array.Empty<string>());
                W40KRTProgressionFacts.RemovePathRank(Owner, path.CareerPathId, pathRank, automatic);
                if (pathRank == 1) {
                    Owner.Feats.Remove(feat);
                    foreach (var marker in Owner.Feats.Where(f => f.Blueprint.Eq(path.SelectionId)).ToArray()) {
                        Owner.Feats.Remove(marker);
                    }
                }
                else {
                    feat.Rank--;
                }
                path.Downgrade?.Invoke(new W40KRTArchetypeDowngradeArguments(Owner, _mediator, tier, path, level - 1));
            }

            Model.CharacterLevel = level - 1;
        }

        public bool IsLevelReadOnly => false;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; } = Array.Empty<IGameUnitUltimateProgressionEntry>();
        public IGameModelCollection<IGameUnitSelectionProgressionEntry> Selections { get; }
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; } = Array.Empty<IGameUnitClassProgressionEntry>();

        public bool IsSupported => Model is object;

        public bool IsSelectionsRepairable => IsSupported && Selections.Count == 0 && Owner.Type.IsCharacter() && Owner.Type != UnitEntityType.Pet;

        public void RepairSelections()
        {
            int index = 0;
            void Insert(W40KRTCharacterSelection sel, int optionIndex = 0, W40KRTCharacterSelectionOption option = null)
            {
                if (option is null) {
                    var optionIds = new HashSet<string>(sel.Options.Select(o => o.Id), StringComparer.Ordinal);
                    var feat = Owner.Feats.FirstOrDefault(f => optionIds.Contains(f.Blueprint));
                    option = feat is object ? sel.Options.First(x => x.Id.Eq(feat.Blueprint)) : sel.Options.Skip(optionIndex).First();
                }
                var entry = Selections.InsertByBlueprint(index++, sel.SelectionId, option.Id);
                if (!Owner.Feats.Any(f => f.Blueprint.Eq(option.Id))) {
                    W40KRTProgressionFacts.AddSelectionRank(Owner, ((W40KRTGameUnitSelectionProgressionEntry)entry).Ref);
                }

                if (option.SubSelectionId.HasValue() && W40KRTCharacterSelection.TryGet(option.SubSelectionId, out var subsel)) {
                    Insert(subsel);
                }
            }
            Insert(W40KRTCharacterSelection.Homeworld);
            Insert(W40KRTCharacterSelection.Occupation, 4);
            Insert(W40KRTCharacterSelection.DarkestHour);
            Insert(W40KRTCharacterSelection.MomentOfTriumph);

            var careerPathSelectionIds = new HashSet<string>(W40KRTArchetypes.Tier1.Types.Select(t => t.SelectionId), StringComparer.Ordinal);
            var cfeat = Owner.Feats.FirstOrDefault(f => careerPathSelectionIds.Contains(f.Blueprint));
            var cfeature = cfeat is object ? cfeat.Blueprint : W40KRTArchetypes.Tier1.Types.First().SelectionId;
            Selections.InsertByBlueprint(index++, W40KRTArchetypes.Tier1.SelectionId, cfeature);

            var attrSelOptions = new[] {
                W40KRTCharacterSelectionOption.AllAttributes[0],
                W40KRTCharacterSelectionOption.AllAttributes[0],
                W40KRTCharacterSelectionOption.AllAttributes[1],
                W40KRTCharacterSelectionOption.AllAttributes[1],
                W40KRTCharacterSelectionOption.AllAttributes[2],
                W40KRTCharacterSelectionOption.AllAttributes[2],
            };
            var attrFeatIds = new HashSet<string>(W40KRTCharacterSelectionOption.AllAttributes.Select(a => a.Id), StringComparer.Ordinal);
            var afeats = Owner.Feats.Where(f => attrFeatIds.Contains(f.Blueprint)).ToArray();
            var aindex = 0;
            foreach (var afeat in afeats) {
                for (var arankIndex = 0; arankIndex < afeat.Rank && aindex < attrSelOptions.Length; arankIndex++) {
                    attrSelOptions[aindex++] = W40KRTCharacterSelectionOption.AllAttributes.First(a => a.Id.Eq(afeat.Blueprint));
                }
            }

            Insert(W40KRTCharacterSelection.Attribute1, option: attrSelOptions[0]);
            Insert(W40KRTCharacterSelection.Attribute2, option: attrSelOptions[1]);
            Insert(W40KRTCharacterSelection.Attribute3, option: attrSelOptions[2]);
            Insert(W40KRTCharacterSelection.Attribute4, option: attrSelOptions[3]);
            Insert(W40KRTCharacterSelection.Attribute5, option: attrSelOptions[4]);
            Insert(W40KRTCharacterSelection.Attribute6, option: attrSelOptions[5]);
        }
    }
}