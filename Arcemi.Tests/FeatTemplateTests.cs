using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arcemi.Tests
{
    public class FeatTemplateTests
    {
        [Fact]
        public async Task Dodge()
        {
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            await res.LoadGameFolderAsync(null, @"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(WotrBlueprintProvider.Feature).Where(f => f.DisplayName == "Dodge").First();
            
            var template = res.GetFeatTemplate(feat.Id);
            Assert.NotNull(template);
        }

        [Fact]
        public async Task ShifterMultiattack()
        {
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            await res.LoadGameFolderAsync(null, @"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(WotrBlueprintProvider.Feature).Where(f => f.DisplayName == "ShifterAC Bonus").First();

            var template = res.GetFeatTemplate(feat.Id);
            Assert.NotNull(template);
        }
    }
}
