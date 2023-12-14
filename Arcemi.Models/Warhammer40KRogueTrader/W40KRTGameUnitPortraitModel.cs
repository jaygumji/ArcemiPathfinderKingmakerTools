using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameUnitPortraitModel : IGameUnitPortraitModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        private readonly string unitBlueprint;

        public W40KRTGameUnitPortraitModel(UnitUISettingsPartItemModel uiSettings, string unitBlueprint)
        {
            UiSettings = uiSettings;
            this.unitBlueprint = unitBlueprint;
        }

        public UnitUISettingsPartItemModel UiSettings { get; }

        public string Blueprint => UiSettings.m_Portrait;

        public string Path
        {
            get {
                if (!string.IsNullOrEmpty(UiSettings.m_CustomPortrait?.CustomPortraitId)) {
                    return Res.GetPortraitsUri(UiSettings.m_CustomPortrait.CustomPortraitId);
                }
                if (!string.IsNullOrEmpty(Blueprint)) {
                    return Res.GetPortraitsUri(Blueprint);
                }
                if (Res.TryGetPortraitsUri(unitBlueprint, out var uri)) {
                    return uri;
                }
                var name = Res.GetCharacterName(unitBlueprint);
                if (name.HasValue()) {
                    if (name.Eq("Abelard")) name = "Abeliard"; // Misspell in game resources
                    name = name + "_Portrait";
                    var entry = Res.Blueprints.GetEntries(BlueprintTypeId.Portrait).FirstOrDefault(e => e.Name.Original.Eq(name));
                    if (entry is object)
                        return Res.GetPortraitsUri(entry.Id);
                }
                var unitBlueprintEntry = Res.Blueprints.Get(unitBlueprint);
                if (unitBlueprintEntry?.Type == W40KRTBlueprintTypeProvider.Starship) {
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