using Arcemi.Pathfinder.Kingmaker;
using System.Collections.Generic;
using Xunit;

namespace Arcemi.Pathfinder.Tests
{
    public class BlueprintNameTests
    {
        public static IEnumerable<object[]> NameDisplayTestData()
        {
            yield return new object[] { BlueprintTypes.ItemWeapon, "BloodlineDraconicClaw1d6", "Bloodline Draconic Claw 1d6" };
            yield return new object[] { BlueprintTypes.ItemWeapon, "AirElementalSlam_Medium", "Air Elemental Slam Medium" };
            yield return new object[] { BlueprintTypes.ItemWeapon, "CorrosiveRapierPlus1", "Corrosive Rapier +1" };
            yield return new object[] { BlueprintTypes.ItemWeapon, "RapierAgileFlamingPlus4", "Rapier Agile Flaming +4" };
            yield return new object[] { BlueprintTypes.ItemWeapon, "AthachClaw1d10", "Athach Claw 1d10" };
            yield return new object[] { BlueprintTypes.Item, "BezoarItem", "Bezoar" };
            yield return new object[] { BlueprintTypes.Item, "DominoGreen1x1", "Domino Green 1x1" };
        }

        [Theory]
        [MemberData(nameof(NameDisplayTestData))]
        public void NameDisplay(BlueprintType type, string name, string expectedDisplayName)
        {
            var target = BlueprintName.Detect("", type, name);
            Assert.Equal(expectedDisplayName, target.DisplayName);
        }
    }
}
