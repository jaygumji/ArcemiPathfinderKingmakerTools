using Arcemi.Pathfinder.Kingmaker;
using Arcemi.Pathfinder.SaveGameEditor.Models;
using Arcemi.Pathfinder.Tests.Mocks;
using System.Linq;
using Xunit;

namespace Arcemi.Pathfinder.Tests
{
    public class CharacterViewModelTests
    {
        [Fact]
        public void DowngradeLevelTest()
        {
            var fileText = Res.Get("party_Downgrade_LevelCheck.json");
            const string mainCharacterId = "34a0b9c5-923e-47ad-8e85-bcc923a80ac9";
            var resources = new GameResources();
            resources.SetStaticBlueprints(
                new BlueprintMetadataEntry {Guid = "b79e92dd495edd64e90fb483c504b8df", Name = "KineticistProgression", TypeFullName = BlueprintTypes.Progression.FullName}
            );

            var saveFileProvider = MockSaveFileProvider.FromPartyJson(fileText, mainCharacterId, resources);
            var target = new CharacterLevelManipulator(saveFileProvider.PlayerEntity, resources);

            var cls = saveFileProvider.PlayerEntity.Descriptor.Progression.Classes.First();
            Assert.Equal(7, cls.Level);
            target.DowngradeClass(cls);
            Assert.Equal(6, cls.Level);
        }
    }
}
