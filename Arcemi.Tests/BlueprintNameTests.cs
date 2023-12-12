using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using System.Collections.Generic;
using Xunit;

namespace Arcemi.Tests
{
    public class BlueprintNameTests
    {
        public static IEnumerable<object[]> NameDisplayTestData()
        {
            yield return new object[] { WotrBlueprintTypeProvider.ItemWeapon, "BloodlineDraconicClaw1d6", "Bloodline Draconic Claw 1d6" };
            yield return new object[] { WotrBlueprintTypeProvider.ItemWeapon, "AirElementalSlam_Medium", "Air Elemental Slam Medium" };
            yield return new object[] { WotrBlueprintTypeProvider.ItemWeapon, "CorrosiveRapierPlus1", "Corrosive Rapier +1" };
            yield return new object[] { WotrBlueprintTypeProvider.ItemWeapon, "RapierAgileFlamingPlus4", "Rapier Agile Flaming +4" };
            yield return new object[] { WotrBlueprintTypeProvider.ItemWeapon, "AthachClaw1d10", "Athach Claw 1d10" };
            yield return new object[] { WotrBlueprintTypeProvider.Item, "BezoarItem", "Bezoar" };
            yield return new object[] { WotrBlueprintTypeProvider.Item, "DominoGreen1x1", "Domino Green 1x1" };
        }

        [Theory]
        [MemberData(nameof(NameDisplayTestData))]
        public void NameDisplay(BlueprintType type, string name, string expectedDisplayName)
        {
            var target = BlueprintName.Detect(GameDefinition.Pathfinder_WrathOfTheRighteous.Resources.Blueprints.TypeProvider, "", type, name);
            Assert.Equal(expectedDisplayName, target.DisplayName);
        }
    }
}
