using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitProgressionModel : IGameUnitProgressionModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public W40KRTGameUnitProgressionModel(IGameUnitModel owner, UnitProgressionPartItemModel model)
        {
            Owner = owner;
            Model = model;
            Selections = new GameModelCollection<IGameUnitSelectionProgressionEntry, UnitProgressionSelectionOfPartModel>(model?.Selections, s => new W40KRTGameUnitSelectionProgressionEntry(owner, s), s => s.Level == 0, new W40KRTGameUnitSelectionProgressionEntryWriter(owner));
            Data = GameDataModels.Object("All choices", new[] {
                GameDataModels.RowList(model?.Selections, s => GameDataModels.Object(
                    string.Concat(Res.Blueprints.GetNameOrBlueprint(s.Path), " > ", Res.Blueprints.GetNameOrBlueprint(s.Selection)), new IGameData[] {
                    GameDataModels.Text("Feature", s, x => Res.Blueprints.GetNameOrBlueprint(x.Feature), size: GameDataSize.Medium),
                    GameDataModels.Integer("Level", s, x => W40KRTArchetypes.ActualLevel(x.Path, x.Level), size: GameDataSize.Small),
                }), writer: new W40KRTGameUnitProgressionCollectionWriter(), nameSize: GameDataSize.Medium)
            }, isCollapsable: true);
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
            var value = Model.CharacterLevel - 1;
            Model.CharacterLevel = value;
            var oldSelections = Model.Selections.Where(x => x.Level > 0 && W40KRTArchetypes.ActualLevel(x.Path, x.Level) > value).ToArray();
            var featIds = new HashSet<string>(oldSelections.Select(x => x.Feature)
                .Concat(W40KRTArchetypes.GetBlueprintsHigherThan(value)), StringComparer.Ordinal);
            var feats = Owner.Feats.Where(f => featIds.Contains(f.Blueprint)).ToArray();

            foreach (var selection in oldSelections) {
                Model.Selections.Remove(selection);
                Logger.Current.Information($"Removed selection {Res.Blueprints.GetNameOrBlueprint(selection.Selection)}, Feature {selection.Feature}, Level {selection.Level}, Rank {selection.Rank}");
            }
            foreach (var feat in feats) {
                var match = (
                    from t in W40KRTArchetypes.All
                    from a in t.Types
                    where a.CareerPathId.Eq(feat.Blueprint)
                    select new { Tier = t, Archetype = a }).FirstOrDefault();
                if (match is null || match.Tier.Level > value) {
                    if (feat.Rank > 1) {
                        feat.Rank -= 1;
                        Logger.Current.Information($"Downranked path {feat.DisplayName} to {feat.Rank}");
                    }
                    else {
                        Owner.Feats.Remove(feat);
                        Logger.Current.Information("Removed feat " + feat.DisplayName);
                    }
                }
                else {
                    feat.Rank = value - match.Tier.Level + 1;
                    Logger.Current.Information($"Downranked path {feat.DisplayName} to {feat.Rank}");
                }
            }

        }

        public bool IsLevelReadOnly => false;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; } = Array.Empty<IGameUnitUltimateProgressionEntry>();
        public IGameModelCollection<IGameUnitSelectionProgressionEntry> Selections { get; }
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; } = Array.Empty<IGameUnitClassProgressionEntry>();

        public bool IsSupported => Model is object;

        public bool IsSelectionsRepairable => IsSupported && Selections.Count == 0 && Owner.Type.IsCharacter();

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
                option.Select(entry);

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
                for (var arankIndex = 0; arankIndex < afeat.Rank; arankIndex++) {
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