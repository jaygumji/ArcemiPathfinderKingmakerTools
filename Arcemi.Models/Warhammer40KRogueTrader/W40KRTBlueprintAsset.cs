using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTBlueprintAsset
    {
        public string AssetId { get; set; }
        public W40KRTBlueprintAssetData Data { get; set; }
    }

    public class W40KRTBlueprintAssetData
    {
        public W40KRTRankEntry[] RankEntries { get; set; }
        public string[] FactGrants { get; set; }

        [JsonProperty("Components")]
        private JArray Components
        {
            set
            {
                FactGrants = value?.OfType<JObject>()
                    .Where(c => ((string)c["$type"])?.EndsWith(", AddFacts", StringComparison.Ordinal) == true)
                    .SelectMany(c => c["m_Facts"]?.Values<string>() ?? Array.Empty<string>()).ToArray();
                if (FactGrants?.Length == 0) FactGrants = null;
            }
        }

        [JsonProperty("m_DisplayName")]
        public W40KRTBlueprintAssetLocalization DisplayName { get; set; }

        [JsonProperty("m_Description")]
        public W40KRTBlueprintAssetLocalization Description { get; set; }
    }

    public class W40KRTRankEntry
    {
        [JsonProperty("m_Features")]
        public string[] Features { get; set; }
    }

    public class W40KRTBlueprintAssetLocalization
    {
        public W40KRTBlueprintAssetDataShared Shared { get; set; }
    }

    public class W40KRTBlueprintAssetDataShared
    {
        [JsonProperty("assetguid")]
        public string AssetGuid { get; set; }

        [JsonProperty("stringkey")]
        public string StringKey { get; set; }
    }
}