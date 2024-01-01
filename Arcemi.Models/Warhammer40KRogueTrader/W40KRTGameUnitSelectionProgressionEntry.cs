using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitSelectionProgressionEntry : IGameUnitSelectionProgressionEntry
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
            if (!Options.Any(o => o.Id.Eq(Feature))) {
                ((List<BlueprintOption>)Options).Insert(0, new BlueprintOption(Feature, Res.Blueprints.TryGet(Feature, out var entry) ? NameWithout(entry.DisplayName, Name) : Feature + " (Unknown)"));
                IsReadOnly = true;
            }
            ((List<BlueprintOption>)Options).Sort((l, r) => StringComparer.CurrentCultureIgnoreCase.Compare(l.Name, r.Name));
        }

        public string Name { get; }

        public string Feature
        {
            get => Ref.Feature;
            set {
                if (Ref.Selection.Eq(W40KRTArchetypes.Tier1.SelectionId)) {
                    var oldValue = Ref.Feature;
                    Ref.Feature = value;

                    var oldType = W40KRTArchetypes.Tier1.Get(oldValue);
                    if (oldType is null) return;

                    var type = W40KRTArchetypes.Tier1.Get(value);
                    if (type is null) return;

                    var feats = Owner.Feats.Where(f => W40KRTArchetypes.Tier1.Has(f.Blueprint)).ToArray();
                    foreach (var feat in feats) {
                        Owner.Feats.Remove(feat);
                    }
                    Owner.Feats.AddByBlueprint(type.SelectionId);
                    Owner.Feats.AddByBlueprint(type.CareerPathId);
                    foreach (var keystoneAbilityId in type.KeystoneAbilityIds) {
                        Owner.Feats.AddByBlueprint(keystoneAbilityId);
                    }
                    foreach (var autoFeat in type.AutomaticFeats.Where(x => x.Level <= 1)) {
                        Owner.Feats.AddByBlueprint(autoFeat.Id);
                    }

                    var part = ((W40KRTGameUnitProgressionModel)Owner.Progression).Model;
                    var oldSelections = part.Selections.Where(x => x.Path.Eq(Ref.Path)).ToArray();
                    foreach (var selection in oldSelections) {
                        part.Selections.Remove(selection);
                    }
                }
                else {
                    var option = Options.FirstOrDefault(o => o.Id.Eq(value));
                    if (option is null) {
                        Ref.Feature = value;
                        return;
                    }
                    ((W40KRTCharacterSelectionOption)option).Select(this);
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