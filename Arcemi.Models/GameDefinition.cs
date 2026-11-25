namespace Arcemi.Models
{
    public class GameDefinition
    {
        public static GameDefinition NotSet { get; } = new GameDefinition("NOTSET", "Not Set", null, null, new EmptyBlueprintTypeProvider());
        public static GameDefinition Warhammer40K_RogueTrader { get; } = new GameDefinition("W40KRT", "Warhammer 40K Rogue Trader",
            "Warhammer 40000 Rogue Trader",
            "Warhammer 40,000 Rogue Trader",
            new Warhammer40KRogueTrader.W40KRTBlueprintProvider(), steamAppId: "2186680", macBundleName: "WH40KRT.app");
        public static GameDefinition Warhammer40K_DarkHeresy { get; } = new GameDefinition("W40KDH", "Warhammer 40K Dark Heresy",
            "WHDH",
            "Warhammer 40,000 Dark Heresy Playtest",
            new EmptyBlueprintTypeProvider(), steamAppId: "3989130");
        public static GameDefinition Pathfinder_WrathOfTheRighteous { get; } = new GameDefinition("PATHWOTR", "Pathfinder Wrath of the Righteous",
            "Pathfinder Wrath Of The Righteous",
            "Pathfinder Wrath Of The Righteous",
            new PathfinderWotr.WotrBlueprintProvider(), steamAppId: "1184370", macBundleName: "Wrath.app");
        public static GameDefinition Pathfinder_Kingmaker { get; } = new GameDefinition("PATHKING", "Pathfinder Kingmaker",
            "Pathfinder Kingmaker",
            "Pathfinder Kingmaker",
            new Kingmaker.KingmakerBlueprintProvider(), isReadingGameFolder: false, steamAppId: "640820");

        private GameDefinition(string id, string name, string appDataFolderName, string windowsGameFolderName, BlueprintProvider blueprintTypeProvider, bool isReadingGameFolder = true, string steamAppId = null, string macBundleName = null)
        {
            Id = id;
            Name = name;
            AppDataFolderName = appDataFolderName;
            SteamAppId = steamAppId;
            MacBundleName = macBundleName;
            WindowsGameFolderName = windowsGameFolderName;
            IsReadingGameFolder = isReadingGameFolder;
            Resources = new GameResources(this, blueprintTypeProvider);
        }

        public string Id { get; }
        public string Name { get; }
        public string AppDataFolderName { get; }
        public string SteamAppId { get; }
        public string MacBundleName { get; }
        public string WindowsRelativeAppDataPath => AppDataFolderName is null ? null : $@"LocalLow\Owlcat Games\{AppDataFolderName}";
        public string WindowsGameFolderName { get; }
        public bool IsReadingGameFolder { get; }
        public GameResources Resources { get; }
    }
}
