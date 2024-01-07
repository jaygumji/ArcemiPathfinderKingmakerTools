using Newtonsoft.Json;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTLocalizationEntry
    {
        public string Text { get; set; }
    }
    public class W40KRTLocalizationAsset
    {
        [JsonProperty("strings")]
        public Dictionary<string, W40KRTLocalizationEntry> Strings { get; set; }
    }
}