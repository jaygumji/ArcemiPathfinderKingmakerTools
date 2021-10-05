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
        public string PlayerCharacterName { get => A.Value<string>(); set => A.Value(value); }
        public string Name { get => A.Value<string>(); set => A.Value(value); }
        public int QuickSaveNumber { get => A.Value<int>(); set => A.Value(value); }
        public TimeSpan GameSaveTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
    }
}
