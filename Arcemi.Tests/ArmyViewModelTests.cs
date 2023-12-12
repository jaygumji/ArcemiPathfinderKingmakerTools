using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using Arcemi.SaveGameEditor.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arcemi.Tests
{
    public class ArmyViewModelTests
    {
        private static ArmyViewModel CreateVm(string resourceName)
        {
            var json = Res.Get(resourceName);
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            var refs = new References();
            var obj = JObject.Parse(json);
            refs.VisitTree(null, obj);
            var accessor = new ModelDataAccessor(obj, refs);
            var army = new PlayerArmyModel(accessor);
            return new ArmyViewModel(res, army);
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
                FirstAvailablePositionDefault = 5,
                FirstAvailablePositionLarge = 5
            }};
            yield return new object[] { CreateVm("Army1SlotHole.json"), new Expect {
                ArmyCount = 5,
                FirstAvailablePositionDefault = 2,
                FirstAvailablePositionLarge = 4
            }};
            yield return new object[] { CreateVm("ArmyAll5ToLeft.json"), new Expect {
                ArmyCount = 5,
                FirstAvailablePositionDefault = 3,
                FirstAvailablePositionLarge = 3
            }};
            yield return new object[] { CreateVm("Army2Default2Large.json"), new Expect {
                ArmyCount = 4,
                FirstAvailablePositionDefault = 0,
                FirstAvailablePositionLarge = -1
            }};
            yield return new object[] { CreateVm("ArmyNoLargeSpace.json"), new Expect {
                ArmyCount = 5,
                FirstAvailablePositionDefault = 1,
                FirstAvailablePositionLarge = -1
            }};
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void FirstAvailablePositionSizeDefault(ArmyViewModel vm, Expect expect)
        {
            var index = vm.FindFirstAvailablePosition(ArmyUnitSize.Default);
            Assert.Equal(expect.FirstAvailablePositionDefault, index);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void FirstAvailablePositionSizeLarge(ArmyViewModel vm, Expect expect)
        {
            var index = vm.FindFirstAvailablePosition(ArmyUnitSize.Large);
            Assert.Equal(expect.FirstAvailablePositionLarge, index);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void AddDefaultArmyUnit(ArmyViewModel vm, Expect expect)
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
        public void AddLargeArmyUnit(ArmyViewModel vm, Expect expect)
        {
            var blueprint = "41d36c6c26d04e14a9125a1319565f96";

            var hasSpace = vm.FindFirstAvailablePosition(ArmyUnitSize.Large) != -1;

            var added = vm.AddArmyUnit(blueprint, ArmyUnitSize.Large, 10);
            Assert.Equal(hasSpace, added);
            if (added) {
                Assert.Equal(expect.ArmyCount + 1, vm.Army.Data.Squads.Count);
                Assert.Equal(blueprint, vm.Army.Data.Squads[expect.ArmyCount].Unit);
            }

            var positions = (IList<int>)new[] {
                expect.FirstAvailablePositionLarge,
                expect.FirstAvailablePositionLarge + 1,
                expect.FirstAvailablePositionLarge + 7,
                expect.FirstAvailablePositionLarge + 8
            };
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
        public void RemoveFirstDefaultArmyUnit(ArmyViewModel vm, Expect expect)
        {
            var au = vm.Units.FirstOrDefault(u => u.Mapping.Size == ArmyUnitSize.Default);
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
        public void RemoveFirstLargeArmyUnit(ArmyViewModel vm, Expect expect)
        {
            var au = vm.Units.FirstOrDefault(u => u.Mapping.Size == ArmyUnitSize.Large);
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
        public void RemoveLastDefaultArmyUnit(ArmyViewModel vm, Expect expect)
        {
            var au = vm.Units.LastOrDefault(u => u.Mapping.Size == ArmyUnitSize.Default);
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
        public void RemoveLastLargeArmyUnit(ArmyViewModel vm, Expect expect)
        {
            var au = vm.Units.LastOrDefault(u => u.Mapping.Size == ArmyUnitSize.Large);
            if (au == null) return;
            vm.RemoveArmyUnit(au);
            Assert.Equal(expect.ArmyCount - 1, vm.Army.Data.Squads.Count);
            Assert.All(vm.Army.Data.Squads, s => Assert.NotEqual(s.Unit, au.Squad.Unit));

            for (var i = 0; i < vm.Army.Data.SquadsPosition.Count; i++) {
                var army = vm.Army.Data.SquadsPosition[i];
                Assert.NotEqual(au.Squad.Unit, army?.Unit);
            }
        }
    }
}
