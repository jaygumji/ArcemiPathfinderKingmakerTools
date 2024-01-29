namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitPortraitModel : IGameUnitPortraitModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public bool IsSupported => Unit?.Descriptor?.UISettings is object;
        public string Blueprint => Unit.Descriptor?.UISettings?.Portrait;
        public string Path
        {
            get {
                if (!string.IsNullOrEmpty(UISettings?.CustomPortrait?.CustomPortraitId)) {
                    return Res.GetPortraitsUri(UISettings?.CustomPortrait.CustomPortraitId);
                }
                if (!string.IsNullOrEmpty(UISettings?.Portrait)) {
                    return Res.GetPortraitsUri(UISettings?.Portrait);
                }
                if (Res.TryGetPortraitsUri(Unit?.Descriptor?.Blueprint, out var uri)) {
                    return uri;
                }
                var portraitId = Res.GetCharacterPotraitIdentifier(Unit?.Descriptor?.Blueprint);
                return Res.GetPortraitsUri(portraitId);
            }
        }

        public UnitEntityModel Unit { get; }
        public UISettingsModel UISettings => Unit?.Descriptor?.UISettings;

        public KingmakerGameUnitPortraitModel(UnitEntityModel unit)
        {
            Unit = unit;
        }

        public void Set(Portrait portrait)
        {
            var accessor = UISettings.GetAccessor();
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