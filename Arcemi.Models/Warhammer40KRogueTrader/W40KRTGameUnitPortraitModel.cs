using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitPortraitModel : IGameUnitPortraitModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public W40KRTGameUnitPortraitModel(IGameUnitModel owner, UnitUISettingsPartItemModel uiSettings)
        {
            Owner = owner;
            UiSettings = uiSettings;
        }

        public IGameUnitModel Owner { get; }
        public UnitUISettingsPartItemModel UiSettings { get; }

        public string Blueprint => UiSettings?.m_Portrait;

        private static readonly IReadOnlyDictionary<string, string> Reroutes = new Dictionary<string, string>(StringComparer.Ordinal) {
            {"fffed8b281aa45fc9926d62d7aaf94c9", "64c01932603e419585d9bfa92b8ba367" }, // Master_ArbitesCyberMastiff_PetUnit
            {"dbdd1762ef204564b5255af113609602", "9170af4bd2a7480bba619b444d1fb12c" }, // Master_ServoskullSwarm_PetUnit
            {"65636e47aafd47399c479ebf49153985", "28676476a9e14601b2e17de5fa65f65f" }, // Master_PsykerPsyberRaven_PetUnit
            {"7a9448a35ad449bcbcb9af8bed810134", "5164ccb798dd434a81ad05cabba43263" }, // Master_CyberEagle_PetUnit
            {"df761ea549574f888876390dfd2292aa", "" }, // Master_ExperimentalServitor_PetUnit
        };

        public string Path
        {
            get {
                if (UiSettings is object) {
                    if (!string.IsNullOrEmpty(UiSettings.m_CustomPortrait?.CustomPortraitId)) {
                        return Res.GetPortraitsUri(UiSettings.m_CustomPortrait.CustomPortraitId);
                    }
                    if (!string.IsNullOrEmpty(Blueprint)) {
                        return Res.GetPortraitsUri(Blueprint);
                    }
                }

                var unitBlueprint = Owner.Blueprint;
                if (string.IsNullOrEmpty(unitBlueprint)) {
                    return Res.AppData.Portraits.GetUnknownUri();
                }
                Logger.Current.Information($"Attempting to resolve portrait from unit '{unitBlueprint}'");
                if (Res.TryGetPortraitsUri(unitBlueprint, out var uri)) {
                    return uri;
                }
                if (Reroutes.TryGetValue(unitBlueprint, out var reroute)) {
                    Logger.Current.Information($"Rerouted '{unitBlueprint}' to '{reroute}'");
                    return Res.GetPortraitsUri(reroute);
                }
                var name = Res.GetCharacterName(unitBlueprint);
                if (name.HasValue()) {
                    if (name.Eq("Abelard")) name = "Abeliard"; // Misspell in game resources
                    name = name + "_Portrait";
                    var entry = Res.Blueprints.GetEntries(BlueprintTypeId.Portrait).FirstOrDefault(e => e.Name.Original.Eq(name));
                    if (entry is object)
                        return Res.GetPortraitsUri(entry.Id);
                }
                if (Res.Blueprints.TryGet(unitBlueprint, out var unitBlueprintEntry) && unitBlueprintEntry?.Type == W40KRTBlueprintProvider.Starship) {
                    // Use a ship portrait to differentiate from other companions
                    return Res.GetPortraitsUri("ebadc55b015a41c689a60b60b1c4d5c7"); // Imperial Frigate Sword
                }
                var portraitId = Res.GetCharacterPotraitIdentifier(unitBlueprint);
                return Res.GetPortraitsUri(portraitId);
            }
        }

        public bool IsSupported => UiSettings is object;

        public void Set(Portrait portrait)
        {
            var accessor = UiSettings.GetAccessor();
            if (portrait.IsCompanion) {
                accessor.Value(null, "m_Portrait");
                accessor.SetObjectToNull("m_CustomPortrait");
            }
            else if (portrait.IsCustom) {
                accessor.Value(null, "m_Portrait");
                var c = accessor.Object("m_CustomPortrait", a => new CustomPortraitModel(a), createIfNull: true);
                c.CustomPortraitId = portrait.Key;
            }
            else {
                accessor.Value(portrait.Key, "m_Portrait");
                accessor.SetObjectToNull("m_CustomPortrait");
            }
        }
    }
}