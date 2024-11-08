using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTBlueprintCachedData
    {
        public W40KRTBlueprintCachedData()
        {
        }

        public DateTime? TimestampUtc { get; set; }
        public Dictionary<string, W40KRTLocalization> DisplayNames { get; set; } = new Dictionary<string, W40KRTLocalization>();
        public Dictionary<string, W40KRTLocalization> Descriptions { get; set; } = new Dictionary<string, W40KRTLocalization>();
        public Dictionary<string, W40KRTBlueprintAsset> BlueprintAssets { get; set; } = new Dictionary<string, W40KRTBlueprintAsset>();

        public bool IsValidCache(DateTime timestampUtc)
        {
            return TimestampUtc.HasValue && TimestampUtc == timestampUtc;
        }

        private const string FileName = "cache.blueprints.json";
        internal static async Task<W40KRTBlueprintCachedData> LoadAsync(string workingDirectory)
        {
            try {
                if (workingDirectory.HasValue()) {
                    var path = Path.Combine(workingDirectory, FileName);
                    if (File.Exists(path)) {
                        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (var streamReader = new StreamReader(fileStream)) {
                            var json = await streamReader.ReadToEndAsync();
                            var cacheData = JsonConvert.DeserializeObject<W40KRTBlueprintCachedData>(json);
                            if (cacheData is object) {
                                return cacheData;
                            }
                            Logger.Current.Warning("Detected corrupt cache (null value), reseted cache");
                        }
                    }
                }
            }
            catch (Exception ex) {
                Logger.Current.Warning("Detected corrupt cache, reseted cache", ex);
            }
            return new W40KRTBlueprintCachedData();
        }

        public async Task SaveAsync(string workingDirectory)
        {
            if (!workingDirectory.HasValue()) return;
            try {
                if (!Directory.Exists(workingDirectory)) {
                    Directory.CreateDirectory(workingDirectory);
                }
                var path = Path.Combine(workingDirectory, FileName);
                var backupPath = Path.Combine(workingDirectory, FileName + ".bck");
                if (File.Exists(path)) {
                    if (File.Exists(backupPath)) {
                        File.Delete(backupPath);
                    }
                    File.Move(path, backupPath);
                }
                try {
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
                    using (var streamWriter = new StreamWriter(fileStream)) {
                        var json = JsonConvert.SerializeObject(this);
                        await streamWriter.WriteAsync(json);
                    }
                }
                catch (Exception ex) {
                    Logger.Current.Error("Could not save cache info file", ex);
                    if (File.Exists(backupPath)) {
                        File.Move(backupPath, path);
                    }
                    throw;
                }
            }
            catch (Exception ex) {
                Logger.Current.Warning("Unable to write cache, using memory cache only", ex);
            }
        }
    }
}