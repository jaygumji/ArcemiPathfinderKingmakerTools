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
            var saveFileProvider = MockSaveFileProvider.FromPartyJson(fileText, mainCharacterId);
            var target = new CharacterViewModel(saveFileProvider);

            var cls = saveFileProvider.PlayerEntity.Descriptor.Progression.Classes.First();
            Assert.Equal(7, cls.Level);
            target.DowngradeClass(saveFileProvider.PlayerEntity, cls);
            Assert.Equal(6, cls.Level);
        }
    }
}
