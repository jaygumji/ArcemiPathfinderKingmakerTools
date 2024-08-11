using Newtonsoft.Json;
using System;
using System.IO;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTModificationArchiveOptions
    {
        public static W40KRTModificationArchiveOptions Rar { get; } = new W40KRTModificationArchiveOptions('\\', "WhRtModificationTemplate-release",
            f => SharpCompress.Archives.Rar.RarArchive.Open(f, new SharpCompress.Readers.ReaderOptions()));
        public static W40KRTModificationArchiveOptions Tar { get; } = new W40KRTModificationArchiveOptions('/', "WhRtModificationTemplate",
            f => SharpCompress.Archives.Tar.TarArchive.Open(f, new SharpCompress.Readers.ReaderOptions()));
        public W40KRTModificationArchiveOptions(char pathSeparator, string rootFolder, Func<FileInfo, SharpCompress.Archives.IArchive> getArchive)
        {
            PathSeparator = pathSeparator;
            RootFolder = rootFolder;
            GetArchive = getArchive;
        }

        public char PathSeparator { get; }
        public string RootFolder { get; }
        public Func<FileInfo, SharpCompress.Archives.IArchive> GetArchive { get; }
    }
    internal static class W40KRTModificationTemplateFile
    {
        internal static void LoadInto(string gameFolder, W40KRTBlueprintCachedData cacheInfo)
        {
            var archiveInfo = new FileInfo(Path.Combine(gameFolder, "WhRtModificationTemplate-release.rar"));
            if (archiveInfo.Exists) {
                Logger.Current.Information($"W40KRT => Attempting to load mod data from {archiveInfo.FullName}");
                LoadGameArchive(cacheInfo, W40KRTModificationArchiveOptions.Rar, archiveInfo);
                return;
            }
            var moddingInfo = new FileInfo(Path.Combine(gameFolder, "Modding", "WhRtModificationTemplate.tar"));
            if (moddingInfo.Exists) {
                Logger.Current.Information($"W40KRT => Attempting to load mod data from {moddingInfo.FullName}");
                LoadGameArchive(cacheInfo, W40KRTModificationArchiveOptions.Tar, moddingInfo);
                return;
            }
        }

        private static void LoadGameArchive(W40KRTBlueprintCachedData cacheInfo, W40KRTModificationArchiveOptions options, FileInfo archiveInfo)
        {
            if (cacheInfo.IsValidCache(archiveInfo.LastWriteTimeUtc)) {
                Logger.Current.Information("W40KRT => Mod data is uptodate");
                return;
            }

            var localizationPrefix = $@"{options.RootFolder}{options.PathSeparator}Strings{options.PathSeparator}Mechanics{options.PathSeparator}Blueprints{options.PathSeparator}";
            var blueprintPrefix = $@"{options.RootFolder}{options.PathSeparator}Blueprints{options.PathSeparator}";
            try {
                var serializer = new JsonSerializer();
                using (var archive = options.GetArchive(archiveInfo)) {
                    foreach (var entry in archive.Entries) {
                        if (entry.IsDirectory) continue;
                        var isLocalization = entry.Key.IStart(localizationPrefix);
                        var isBlueprint = entry.Key.IStart(blueprintPrefix);

                        if (!isLocalization && !isBlueprint) continue;

                        using (var stream = entry.OpenEntryStream())
                        using (var reader = new StreamReader(stream))
                        using (var jsonReader = new JsonTextReader(reader)) {
                            if (isLocalization) {
                                var localizationEntry = serializer.Deserialize<W40KRTLocalization>(jsonReader);
                                if (string.IsNullOrEmpty(localizationEntry.OwnerGuid)) continue;
                                if (!(localizationEntry.Languages?.Count > 0)) continue;
                                if (entry.Key.IEnd("DisplayName.json") || entry.Key.IEnd("DisplayName_.json") || entry.Key.IEnd("DisplayName_String.json")) {
                                    if (cacheInfo.DisplayNames.TryGetValue(localizationEntry.OwnerGuid, out var existing)) {
                                    }
                                    else {
                                        cacheInfo.DisplayNames.Add(localizationEntry.OwnerGuid, localizationEntry);
                                    }
                                }
                                else if (entry.Key.IEnd("Description.json") || entry.Key.IEnd("Description_.json") || entry.Key.IEnd("Description_String.json")) {
                                    if (cacheInfo.Descriptions.TryGetValue(localizationEntry.OwnerGuid, out var existing)) {
                                    }
                                    else {
                                        cacheInfo.Descriptions.Add(localizationEntry.OwnerGuid, localizationEntry);
                                    }
                                }
                                else {
                                    var key = entry.Key;
                                    key.ToString();
                                }
                            }
                            else if (isBlueprint) {
                                var blueprintAsset = serializer.Deserialize<W40KRTBlueprintAsset>(jsonReader);
                                if (string.IsNullOrEmpty(blueprintAsset?.AssetId)) continue;
                                if (cacheInfo.BlueprintAssets.TryGetValue(blueprintAsset.AssetId, out var existing)) {
                                    existing.ToString();
                                }
                                else {
                                    cacheInfo.BlueprintAssets.Add(blueprintAsset.AssetId, blueprintAsset);
                                }
                            }
                        }
                    }
                }
                cacheInfo.TimestampUtc = archiveInfo.LastWriteTimeUtc;
            }
            catch (Exception ex) {
                Logger.Current.Error("Unable to load localization cache", ex);
                return;
            }
        }
    }
}
