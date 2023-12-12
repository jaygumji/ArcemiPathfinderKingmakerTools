using Arcemi.Models.PathfinderWotr;
using System.IO;

namespace Arcemi.Models
{
    public class GameDefinition
    {
        public static GameDefinition NotSet { get; } = new GameDefinition("NOTSET", "Not Set", null, new EmptyBlueprintTypeProvider());
        public static GameDefinition Warhammer40K_RogueTrader { get; } = new GameDefinition("W40KRT", "Warhammer 40K Rogue Trader", @"LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader", new Warhammer40KRogueTrader.W40KRTBlueprintTypeProvider());
        public static GameDefinition Pathfinder_WrathOfTheRighteous { get; } = new GameDefinition("PATHWOTR", "Pathfinder Wrath of the Righteous", @"LocalLow\Owlcat Games\Pathfinder Wrath Of The Righteous", new PathfinderWotr.WotrBlueprintTypeProvider());
        public static GameDefinition Pathfinder_Kingmaker { get; } = new GameDefinition("PATHKING", "Pathfinder Kingmaker", @"LocalLow\Owlcat Games\Pathfinder Kingmaker", new EmptyBlueprintTypeProvider());

        private GameDefinition(string id, string name, string windowsRelativeAppDataPath, IBlueprintTypeProvider blueprintTypeProvider)
        {
            Id = id;
            Name = name;
            WindowsRelativeAppDataPath = windowsRelativeAppDataPath;
            Resources = new GameResources(this, blueprintTypeProvider);
        }

        public string Id { get; }
        public string Name { get; }
        public string WindowsRelativeAppDataPath { get; }
        public GameResources Resources { get; }
    }
}