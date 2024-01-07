using Newtonsoft.Json;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTBlueprintAsset
    {
        public string AssetId { get; set; }
        public W40KRTBlueprintAssetData Data { get; set; }
    }

    public class W40KRTBlueprintAssetData
    {
        [JsonProperty("m_DisplayName")]
        public W40KRTBlueprintAssetLocalization DisplayName { get; set; }

        [JsonProperty("m_Description")]
        public W40KRTBlueprintAssetLocalization Description { get; set; }
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