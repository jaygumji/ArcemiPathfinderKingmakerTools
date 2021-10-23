using Arcemi.Pathfinder.Kingmaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Arcemi.Pathfinder.Tests
{
    public class JTokenExtensionTests
    {
        [Fact]
        public void CloneDollDataTest()
        {
            var targetJson = Res.Get("DollData.json");
            var expectedJson = Res.Get("DollDataClone.json");
            var target = JObject.Parse(targetJson);
            var copy = target.Export(deep: true, incSys: false);
            var copyJson = copy.ToString(Formatting.None);
            Assert.Equal(expectedJson, copyJson);
        }

        [Fact]
        public void ImportDollDataTest()
        {
            var srcJson = Res.Get("DollDataClone.json");
            var expectedJson = JObject.Parse(Res.Get("DollData.json")).ToString(Formatting.None);
            var srcObj = JObject.Parse(srcJson);
            var destObj = new JObject {
                {"$id", "8"},
                {"EquipmentEntityIds", new JArray { "InvalidArrayItem" } },
                {"EntityRampIdices", new JObject {
                    {"InvalidDictionaryKey", -1}
                }}
            };
            srcObj.ImportTo(destObj, ImportOptions.CodeScripts);
            var json = destObj.ToString(Formatting.None);
            Assert.Equal(expectedJson, json);
        }
    }
}
