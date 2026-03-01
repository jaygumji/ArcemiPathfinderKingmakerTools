using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arcemi.Tests
{
    [Collection("SharedGameResources")]
    public class ArmyViewModelTests
    {
        private static ArmyEditorViewModel CreateVm(string resourceName)
        {
            var json = Res.Get(resourceName);
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            var refs = new References();
            var obj = JObject.Parse(json);
            refs.VisitTree(null, obj);
            var accessor = new ModelDataAccessor(obj, refs);
            var army = new PlayerArmyModel(accessor);
            return new ArmyEditorViewModel(res, army);
        }

        public class Expect
        {
            public int ArmyCount;
            public int FirstAvailablePositionDefault;
            public int FirstAvailablePositionLarge;
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { CreateVm("Army5FirstSlots.json"), new Expect {
                ArmyCount = 5,
                FirstAvailablePositionDefault = 7,
                FirstAvailablePositionLarge = 5
            }};
            yield return new object[] { CreateVm("Army1SlotHole.json"), new Expect {
                ArmyCount = 5,
                FirstAvailablePositionDefault = 9,
                FirstAvailablePositionLarge = 4
            }};
            yield return new object[] { CreateVm("ArmyAll5ToLeft.json"), new Expect {
                ArmyCount = 5,
                FirstAvailablePositionDefault = 9,
                FirstAvailablePositionLarge = 3
            }};
            yield return new object[] { CreateVm("Army2Default2Large.json"), new Expect {
                ArmyCount = 4,
                FirstAvailablePositionDefault = 0,
                FirstAvailablePositionLarge = 3
            }};
            yield return new object[] { CreateVm("ArmyNoLargeSpace.json"), new Expect {
                ArmyCount = 5,
                FirstAvailablePositionDefault = 7,
                FirstAvailablePositionLarge = -1
            }};
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void FirstAvailablePositionSizeDefault(ArmyEditorViewModel vm, Expect expect)
        {
            var index = vm.FindFirstAvailablePosition(ArmyUnitSize.Default);
            Assert.Equal(expect.FirstAvailablePositionDefault, index);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void FirstAvailablePositionSizeLarge(ArmyEditorViewModel vm, Expect expect)
        {
            var index = vm.FindFirstAvailablePosition(ArmyUnitSize.Large);
            Assert.Equal(expect.FirstAvailablePositionLarge, index);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void AddDefaultArmyUnit(ArmyEditorViewModel vm, Expect expect)
        {
            var blueprint = "41d36c6c26d04e14a9125a1319565f96";

            vm.AddArmyUnit(blueprint, ArmyUnitSize.Default, 10);
            Assert.Equal(expect.ArmyCount + 1, vm.Army.Data.Squads.Count);
            Assert.Equal(blueprint, vm.Army.Data.Squads[expect.ArmyCount].Unit);

            for (var i = 0; i < vm.Army.Data.SquadsPosition.Count; i++) {
                var army = vm.Army.Data.SquadsPosition[i];
                if (i == expect.FirstAvailablePositionDefault) {
                    Assert.Equal(blueprint, army.Unit);
                }
                else {
                    Assert.NotEqual(blueprint, army?.Unit);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void AddLargeArmyUnit(ArmyEditorViewModel vm, Expect expect)
        {
            var blueprint = "41d36c6c26d04e14a9125a1319565f96";

            var firstAvailableLargePosition = vm.FindFirstAvailablePosition(ArmyUnitSize.Large);
            var hasSpace = firstAvailableLargePosition != -1;

            var added = vm.AddArmyUnit(blueprint, ArmyUnitSize.Large, 10);
            Assert.Equal(hasSpace, added);
            if (added) {
                Assert.Equal(expect.ArmyCount + 1, vm.Army.Data.Squads.Count);
                Assert.Equal(blueprint, vm.Army.Data.Squads[expect.ArmyCount].Unit);
            }

            var positions = hasSpace
                ? (IList<int>)new[] {
                    firstAvailableLargePosition,
                    firstAvailableLargePosition + 1,
                    firstAvailableLargePosition + 7,
                    firstAvailableLargePosition + 8
                }
                : new int[0];
            for (var i = 0; i < vm.Army.Data.SquadsPosition.Count; i++) {
                var army = vm.Army.Data.SquadsPosition[i];
                if (added && positions.Contains(i)) {
                    Assert.Equal(blueprint, army?.Unit);
                }
                else {
                    Assert.NotEqual(blueprint, army?.Unit);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void RemoveFirstDefaultArmyUnit(ArmyEditorViewModel vm, Expect expect)
        {
            var au = vm.Units.FirstOrDefault(u => DetectSizeFromPosition(vm, u) == ArmyUnitSize.Default);
            if (au == null) return;
            vm.RemoveArmyUnit(au);
            Assert.Equal(expect.ArmyCount - 1, vm.Army.Data.Squads.Count);
            Assert.All(vm.Army.Data.Squads, s => Assert.NotEqual(s.Unit, au.Squad.Unit));

            for (var i = 0; i < vm.Army.Data.SquadsPosition.Count; i++) {
                var army = vm.Army.Data.SquadsPosition[i];
                Assert.NotEqual(au.Squad.Unit, army?.Unit);
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void RemoveFirstLargeArmyUnit(ArmyEditorViewModel vm, Expect expect)
        {
            var au = vm.Units.FirstOrDefault(u => DetectSizeFromPosition(vm, u) == ArmyUnitSize.Large);
            if (au == null) return;
            vm.RemoveArmyUnit(au);
            Assert.Equal(expect.ArmyCount - 1, vm.Army.Data.Squads.Count);
            Assert.All(vm.Army.Data.Squads, s => Assert.NotEqual(s.Unit, au.Squad.Unit));

            for (var i = 0; i < vm.Army.Data.SquadsPosition.Count; i++) {
                var army = vm.Army.Data.SquadsPosition[i];
                Assert.NotEqual(au.Squad.Unit, army?.Unit);
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void RemoveLastDefaultArmyUnit(ArmyEditorViewModel vm, Expect expect)
        {
            var au = vm.Units.LastOrDefault(u => DetectSizeFromPosition(vm, u) == ArmyUnitSize.Default);
            if (au == null) return;
            vm.RemoveArmyUnit(au);
            Assert.Equal(expect.ArmyCount - 1, vm.Army.Data.Squads.Count);
            Assert.All(vm.Army.Data.Squads, s => Assert.NotEqual(s.Unit, au.Squad.Unit));

            for (var i = 0; i < vm.Army.Data.SquadsPosition.Count; i++) {
                var army = vm.Army.Data.SquadsPosition[i];
                Assert.NotEqual(au.Squad.Unit, army?.Unit);
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void RemoveLastLargeArmyUnit(ArmyEditorViewModel vm, Expect expect)
        {
            var au = vm.Units.LastOrDefault(u => DetectSizeFromPosition(vm, u) == ArmyUnitSize.Large);
            if (au == null) return;
            vm.RemoveArmyUnit(au);
            Assert.Equal(expect.ArmyCount - 1, vm.Army.Data.Squads.Count);
            Assert.All(vm.Army.Data.Squads, s => Assert.NotEqual(s.Unit, au.Squad.Unit));

            for (var i = 0; i < vm.Army.Data.SquadsPosition.Count; i++) {
                var army = vm.Army.Data.SquadsPosition[i];
                Assert.NotEqual(au.Squad.Unit, army?.Unit);
            }
        }

        private static ArmyUnitSize DetectSizeFromPosition(ArmyEditorViewModel vm, ArmyEditorUnitViewModel unit)
        {
            var count = vm.Army.Data.SquadsPosition.Count(x => x == unit.Squad);
            if (count == 4) return ArmyUnitSize.Large;
            if (count == 1) return ArmyUnitSize.Default;
            return ArmyUnitSize.Unknown;
        }
    }
}
