using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using Arcemi.SaveGameEditor.Models;
using Arcemi.Tests.Mocks;
using System.Linq;
using Xunit;

namespace Arcemi.Tests
{
    public class CharacterViewModelTests
    {
        [Fact]
        public void DowngradeLevelTest()
        {
            var fileText = Res.Get("party_Downgrade_LevelCheck.json");
            const string mainCharacterId = "34a0b9c5-923e-47ad-8e85-bcc923a80ac9";
            var resources = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            resources.Blueprints.SetEntries( new[] {
                new BlueprintMetadataEntry {Guid = "b79e92dd495edd64e90fb483c504b8df", Name = "KineticistProgression", TypeFullName = WotrBlueprintProvider.Progression.FullName}
            });

            var saveFileProvider = MockEditFileSession.FromPartyJson(fileText, mainCharacterId, resources);

            var cls = saveFileProvider.Game.MainCharacter.Progression.Classes.First();
            Assert.Equal(7, cls.Level);
            cls.Level--;
            Assert.Equal(6, cls.Level);
        }
    }
}
