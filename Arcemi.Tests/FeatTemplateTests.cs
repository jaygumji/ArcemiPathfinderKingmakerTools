using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using System.Linq;
using Xunit;

namespace Arcemi.Tests
{
    public class FeatTemplateTests
    {
        [Fact]
        public void Dodge()
        {
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(WotrBlueprintTypeProvider.Feature).Where(f => f.DisplayName == "Dodge").First();
            
            var template = res.GetFeatTemplate(feat.Id);
            Assert.NotNull(template);
        }

        [Fact]
        public void ShifterMultiattack()
        {
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(WotrBlueprintTypeProvider.Feature).Where(f => f.DisplayName == "ShifterAC Bonus").First();

            var template = res.GetFeatTemplate(feat.Id);
            Assert.NotNull(template);
        }
    }
}
