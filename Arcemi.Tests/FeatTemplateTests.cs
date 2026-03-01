//using Arcemi.Models;
//using Arcemi.Models.PathfinderWotr;
//using System.Linq;
//using Xunit;

//namespace Arcemi.Tests
//{
//    [Collection("SharedGameResources")]
//    public class FeatTemplateTests
//    {
//        [Fact]
//        public void Dodge()
//        {
//            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
//            res.LoadFeatTemplates();

//            var blueprint = res.FeatTemplates.First().Blueprint;
//            res.Blueprints.SetEntries(new[] {
//                new BlueprintMetadataEntry { Guid = blueprint, Name = "Dodge", TypeFullName = WotrBlueprintProvider.Feature.FullName }
//            });

//            var template = res.GetFeatTemplate(blueprint);
//            Assert.NotNull(template);
//        }

//        [Fact]
//        public void ShifterMultiattack()
//        {
//            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
//            res.LoadFeatTemplates();

//            var blueprint = res.FeatTemplates.Skip(1).First().Blueprint;
//            res.Blueprints.SetEntries(new[] {
//                new BlueprintMetadataEntry { Guid = blueprint, Name = "ShifterAC Bonus", TypeFullName = WotrBlueprintProvider.Feature.FullName }
//            });

//            var template = res.GetFeatTemplate(blueprint);
//            Assert.NotNull(template);
//        }
//    }
//}
