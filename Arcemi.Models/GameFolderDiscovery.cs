using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Arcemi.Models
{
    public sealed class GameFolderDiscovery
    {
        private const string Company = "Owlcat Games";
        private static readonly Regex VdfPair = new(@"""(?<key>[^""\r\n]+)""\s+""(?<value>(?:\\.|[^""\\\r\n])*)""");
        private readonly string _home;
        private readonly OSPlatform _platform;
        private readonly string _configHome;
        private readonly string[] _steamLibraries;

        public GameFolderDiscovery(string home, OSPlatform platform, string configHome = null, string dataHome = null,
            IEnumerable<string> additionalSteamRoots = null)
        {
            _home = home;
            _platform = platform;
            _configHome = Path.IsPathFullyQualified(configHome ?? "") ? configHome : Path.Combine(home, ".config");
            var localData = Path.IsPathFullyQualified(dataHome ?? "") ? dataHome : Path.Combine(home, ".local", "share");
            var roots = (additionalSteamRoots ?? Array.Empty<string>()).Concat(DefaultSteamRoots(localData))
                .Where(Directory.Exists).Distinct();
            _steamLibraries = roots.SelectMany(root => ReadVdf(Path.Combine(root, "steamapps", "libraryfolders.vdf"))
                .Where(entry => entry.Key.Equals("path", StringComparison.OrdinalIgnoreCase) || int.TryParse(entry.Key, out _))
                .Select(entry => entry.Value).Where(Path.IsPathFullyQualified).Prepend(root))
                .Where(Directory.Exists).Distinct().ToArray();
        }

        public static GameFolderDiscovery ForCurrentUser()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (OperatingSystem.IsWindows()) {
                string steamPath = null;
                try {
                    using var steam = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
                    steamPath = steam?.GetValue("SteamPath") as string;
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException or IOException) {
                    // Conventional Steam paths are available even if the registry cannot be read.
                }
                return new GameFolderDiscovery(home, OSPlatform.Windows, additionalSteamRoots: new[] { steamPath });
            }
            var platform = OperatingSystem.IsMacOS() ? OSPlatform.OSX : OSPlatform.Linux;
            return new GameFolderDiscovery(home, platform, Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"),
                Environment.GetEnvironmentVariable("XDG_DATA_HOME"));
        }

        public string FindSaveDataFolder(GameDefinition game)
        {
            if (game.AppDataFolderName is null) return null;
            return SaveDataCandidates(game).Distinct()
                .Where(folder => Directory.Exists(Path.Combine(folder, "Saved Games")))
                .OrderByDescending(LatestSave)
                .FirstOrDefault();
        }

        public string FindGameFolder(GameDefinition game)
        {
            if (game.WindowsGameFolderName is null) return null;
            foreach (var folder in SteamGameCandidates(game)) {
                var resolved = ResolveGameFolder(folder, game);
                if (resolved is not null) return resolved;
            }

            if (_platform == OSPlatform.OSX && game.MacBundleName is not null) {
                foreach (var applications in new[] { Path.Combine(_home, "Applications"), "/Applications" }) {
                    var folder = ResolveGameFolder(Path.Combine(applications, game.MacBundleName), game);
                    if (folder is not null) return folder;
                }
            }
            return null;
        }

        private IEnumerable<string> SteamGameCandidates(GameDefinition game)
        {
            foreach (var library in _steamLibraries) {
                var steamApps = Path.Combine(library, "steamapps");
                var installName = ReadVdf(Path.Combine(steamApps, $"appmanifest_{game.SteamAppId}.acf"))
                    .FirstOrDefault(entry => entry.Key.Equals("installdir", StringComparison.OrdinalIgnoreCase)).Value;
                if (!string.IsNullOrWhiteSpace(installName) && Path.GetFileName(installName) == installName
                    && installName is not "." and not "..") {
                    yield return Path.Combine(steamApps, "common", installName);
                }
            }

            foreach (var library in _steamLibraries) {
                yield return Path.Combine(library, "steamapps", "common", game.WindowsGameFolderName);
            }
        }

        public static string ResolveGameFolder(string selection, GameDefinition game)
        {
            if (string.IsNullOrWhiteSpace(selection) || game.WindowsGameFolderName is null) return null;
            var path = selection.Trim();
            if (File.Exists(path)) path = Path.GetDirectoryName(path);
            if (!Directory.Exists(path)) return null;

            if (HasBlueprintData(path)) return path;
            if (Path.GetFileName(Path.TrimEndingDirectorySeparator(path)).Equals("Bundles", StringComparison.OrdinalIgnoreCase)
                && HasBlueprintData(Path.GetDirectoryName(Path.TrimEndingDirectorySeparator(path)))) {
                return Path.GetDirectoryName(Path.TrimEndingDirectorySeparator(path));
            }

            for (var directory = new DirectoryInfo(path); directory is not null; directory = directory.Parent) {
                if (directory.Extension.Equals(".app", StringComparison.OrdinalIgnoreCase)) {
                    return ResolveBundle(directory.FullName);
                }
            }

            return game.MacBundleName is null ? null : ResolveBundle(Path.Combine(path, game.MacBundleName));
        }

        public static bool HasBlueprintData(string folder)
            => !string.IsNullOrWhiteSpace(folder) && File.Exists(Path.Combine(folder, "Bundles", "cheatdata.json"));

        private static string ResolveBundle(string bundle)
        {
            var resources = Path.Combine(bundle, "Contents", "Resources");
            var data = Path.Combine(resources, "Data");
            return new[] { bundle, resources, data, Path.Combine(data, "StreamingAssets") }.FirstOrDefault(HasBlueprintData);
        }

        private IEnumerable<string> DefaultSteamRoots(string localData)
        {
            if (_platform == OSPlatform.Windows) {
                foreach (var root in new[] { "Program Files (x86)", "Program Files", "Programs", "Games" }) {
                    yield return Path.Combine(Path.GetPathRoot(_home) ?? _home, root, "Steam");
                }
            }
            else if (_platform == OSPlatform.OSX) {
                yield return Path.Combine(_home, "Library", "Application Support", "Steam");
            }
            else {
                yield return Path.Combine(localData, "Steam");
                yield return Path.Combine(_home, ".local", "share", "Steam");
                yield return Path.Combine(_home, ".steam", "steam");
                yield return Path.Combine(_home, ".steam", "root");
                yield return Path.Combine(_home, ".var", "app", "com.valvesoftware.Steam", ".local", "share", "Steam");
            }
        }

        private IEnumerable<string> SaveDataCandidates(GameDefinition game)
        {
            var name = game.AppDataFolderName;
            if (_platform == OSPlatform.Windows) {
                yield return Path.Combine(_home, "AppData", "LocalLow", Company, name);
            }
            else if (_platform == OSPlatform.OSX) {
                var support = Path.Combine(_home, "Library", "Application Support");
                yield return Path.Combine(support, Company, name);
                yield return Path.Combine(support, $"unity.{Company}.{name}");
            }
            else {
                yield return Path.Combine(_configHome, "unity3d", Company, name);
                yield return Path.Combine(_home, ".var", "app", "com.valvesoftware.Steam", "config", "unity3d", Company, name);
                foreach (var library in _steamLibraries) {
                    var users = Path.Combine(library, "steamapps", "compatdata", game.SteamAppId, "pfx", "drive_c", "users");
                    yield return Path.Combine(users, "steamuser", "AppData", "LocalLow", Company, name);
                }
            }
        }

        private static DateTime LatestSave(string appData)
        {
            try {
                return Directory.EnumerateFiles(Path.Combine(appData, "Saved Games"))
                    .Where(file => Path.GetExtension(file).Equals(".zks", StringComparison.OrdinalIgnoreCase))
                    .Select(File.GetLastWriteTimeUtc).DefaultIfEmpty(DateTime.MinValue).Max();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
                return DateTime.MinValue;
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> ReadVdf(string file)
        {
            string text;
            try {
                text = File.ReadAllText(file);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
                yield break;
            }

            foreach (Match match in VdfPair.Matches(text)) {
                var value = match.Groups["value"].Value.Replace(@"\\", @"\").Replace("\\\"", "\"");
                yield return new KeyValuePair<string, string>(match.Groups["key"].Value, value);
            }
        }
    }
}
