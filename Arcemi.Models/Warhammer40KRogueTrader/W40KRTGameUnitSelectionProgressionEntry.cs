using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitSelectionProgressionEntry : IGameUnitSelectionProgressionEntry, IGameDataObject
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameUnitSelectionProgressionEntry(IGameUnitModel owner, UnitProgressionSelectionOfPartModel selection)
        {
            Owner = owner;
            Ref = selection;
            Name = NameWithout(Res.Blueprints.GetNameOrBlueprint(Ref.Selection), "Selection");
            if (W40KRTCharacterSelection.TryGet(Ref.Selection, out var sel)) {
                Options = new List<BlueprintOption>(sel.Options);
                Name = sel.Name;
            }
            else if (Ref.Selection.Eq(W40KRTArchetypes.Tier1.SelectionId)) { // Career Path
                Options = new List<BlueprintOption>(W40KRTArchetypes.Tier1.SelectionOptions);
            }
            else {
                Options = new List<BlueprintOption>();
            }
            if (Feature.HasValue() && !Options.Any(o => o.Id.Eq(Feature))) {
                ((List<BlueprintOption>)Options).Insert(0, new BlueprintOption(Feature, Res.Blueprints.TryGet(Feature, out var entry) ? NameWithout(entry.DisplayName, Name) : Feature + " (Unknown)"));
                IsReadOnly = true;
            }
            ((List<BlueprintOption>)Options).Sort((l, r) => StringComparer.CurrentCultureIgnoreCase.Compare(l.Name, r.Name));
            _Properties = new IGameData[] {
                GameDataModels.Text("Feature", Ref, x => Res.Blueprints.GetNameOrBlueprint(x.Feature), size: GameDataSize.Medium),
                GameDataModels.Integer("Level", Ref, x => W40KRTArchetypes.ActualLevel(x.Path, x.Level), size: GameDataSize.Small)
            };
        }

        string IGameDataObject.Name => string.Concat(Res.Blueprints.GetNameOrBlueprint(Ref.Path), " > ", Res.Blueprints.GetNameOrBlueprint(Ref.Selection));
        private readonly IReadOnlyList<IGameData> _Properties;
        IReadOnlyList<IGameData> IGameDataObject.Properties => _Properties;
        bool IGameDataObject.IsCollapsable => false;
        Model IGameDataObject.Ref => Ref;

        public string Name { get; }
        public int Level => W40KRTArchetypes.ActualLevel(Ref.Path, Ref.Level);
        public bool IsCreation => Ref.Level == 0;

        public string Feature
        {
            get => Ref.Selection.Eq(W40KRTForgeWorldAugments.Selection)
                ? W40KRTForgeWorldAugments.CanonicalChoice(Ref.Feature) : Ref.Feature;
            set {
                if (Ref.Selection.Eq(W40KRTArchetypes.Tier1.SelectionId)) {
                    if (Ref.Feature.Eq(value)) return;
                    var type = W40KRTArchetypes.Tier1.Get(value);
                    if (type is null) return;

                    Owner.Progression.CurrentLevel = 1;
                    var tier = W40KRTArchetypes.Tier1;
                    var oldFeats = tier.Types.SelectMany(t => t.KeystoneAbilityIds
                        .Concat(t.AutomaticFeats.Select(f => f.Id))
                        .Append(t.CareerPathId).Append(t.SelectionId)).ToHashSet(StringComparer.Ordinal);
                    foreach (var feat in Owner.Feats.Where(f => oldFeats.Contains(f.Blueprint)).ToArray()) {
                        Owner.Feats.Remove(feat);
                    }
                    Ref.Feature = value;
                    W40KRTProgressionFacts.AddSelectionRank(Owner, Ref);
                    Owner.Feats.AddByBlueprint(type.CareerPathId);
                    foreach (var id in W40KRTProgressionFacts.GetAutomaticFeatures(type.CareerPathId, 1)
                        ?? type.KeystoneAbilityIds) {
                        W40KRTProgressionFacts.AddPathRank(Owner, id, type.CareerPathId, type.CareerPathId, 1);
                    }
                    foreach (var grant in type.AutomaticFeats.Where(f => f.Level == 1)) {
                        if (!Owner.Feats.Any(f => f.Blueprint.Eq(grant.Id))) {
                            Owner.Feats.AddByBlueprint(grant.Id);
                        }
                    }
                }
                else {
                    if (Options.FirstOrDefault(o => o.Id.Eq(value)) is W40KRTCharacterSelectionOption option) {
                        option.Select(this);
                    }
                }
            }
        }
        public IReadOnlyList<BlueprintOption> Options { get; }
        public bool IsReadOnly { get; }

        public IGameUnitModel Owner { get; }
        public UnitProgressionSelectionOfPartModel Ref { get; }

        private string NameWithout(string name, string suffix)
        {
            if (name == null) return name;

            suffix = " " + suffix;
            if (name.IEnd(suffix)) return name.Remove(name.Length - suffix.Length);

            return name;
        }
    }
}