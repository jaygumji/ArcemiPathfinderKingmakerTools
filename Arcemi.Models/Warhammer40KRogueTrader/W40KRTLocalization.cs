using Newtonsoft.Json;
using System.Collections.Generic;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTLocalization
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("ownerGuid")]
        public string OwnerGuid { get; set; }

        [JsonProperty("languages")]
        public List<W40KRTLocalizationLanguage> Languages { get; set; } = new List<W40KRTLocalizationLanguage>();
    }

    public class W40KRTLocalizationLanguage
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}