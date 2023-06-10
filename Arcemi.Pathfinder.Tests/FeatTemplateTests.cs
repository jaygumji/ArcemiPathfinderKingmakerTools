using Arcemi.Pathfinder.Kingmaker;
using System.Linq;
using Xunit;

namespace Arcemi.Pathfinder.Tests
{
    public class FeatTemplateTests
    {
        [Fact]
        public void Dodge()
        {
            var res = new GameResources();
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(BlueprintTypes.Feature).Where(f => f.DisplayName == "Dodge").First();
            
            var template = res.GetFeatTemplate(feat.Id);
            Assert.NotNull(template);
        }

        [Fact]
        public void ShifterMultiattack()
        {
            var res = new GameResources();
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(BlueprintTypes.Feature).Where(f => f.DisplayName == "ShifterAC Bonus").First();

            var template = res.GetFeatTemplate(feat.Id);
            Assert.NotNull(template);
        }
    }
}
