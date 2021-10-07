using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class SaveFileType
    {
        public const string Manual = "Manual";
        public const string Auto = "Auto";
        public const string Quick = "Quick";
    }

    public class HeaderModel : RefModel
    {
        public HeaderModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string GameId { get => A.Value<string>(); set => A.Value(value); }
        public string Type { get => A.Value<string>(); set => A.Value(value); }
        public bool IsManual => string.Equals(Type, SaveFileType.Manual, StringComparison.Ordinal);
        public string PlayerCharacterName { get => A.Value<string>(); set => A.Value(value); }
        public string Name { get => A.Value<string>(); set => A.Value(value); }
        public string Area { get => A.Value<string>(); set => A.Value(value); }
        public string AreaDisplayName => A.Res.Blueprints.GetNameOrBlueprint(Area);
        public int QuickSaveNumber { get => A.Value<int>(); set => A.Value(value); }
        public TimeSpan GameSaveTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public TimeSpan GameTotalTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public DateTime SystemSaveTime { get => A.Value<DateTime>(); set => A.Value(value); }
    }
}
