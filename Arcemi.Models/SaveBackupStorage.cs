using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Arcemi.Models
{
    public sealed class SaveBackupStorage
    {
        private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        public SaveBackupStorage(string defaultFolder, string customFolder = null, bool allowSaveFolderFallback = false)
        {
            DefaultFolder = Path.GetFullPath(defaultFolder);
            Folder = string.IsNullOrWhiteSpace(customFolder) ? DefaultFolder : Path.GetFullPath(customFolder);
            AllowSaveFolderFallback = allowSaveFolderFallback;
        }

        public string DefaultFolder { get; }
        public string Folder { get; }
        public bool AllowSaveFolderFallback { get; }

        public string GetWritePath(string savePath) => GetBackupPath(Folder, savePath);
        public string GetFallbackPath(string savePath) => AllowSaveFolderFallback ? LegacyPath(savePath) : null;

        public string Find(string savePath)
        {
            return new[] { GetWritePath(savePath), GetBackupPath(DefaultFolder, savePath), LegacyPath(savePath) }
                .Distinct(PathComparer)
                .Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();
        }

        public bool MoveLegacyBackup(string savePath)
        {
            var legacy = LegacyPath(savePath);
            var destination = GetWritePath(savePath);
            if (!File.Exists(legacy) || File.Exists(destination)) return false;

            CopyAtomically(legacy, destination, overwrite: false);
            File.Delete(legacy);
            return true;
        }

        public void Restore(string savePath)
        {
            var backup = Find(savePath) ?? throw new FileNotFoundException("No backup was found for this save.");
            CopyAtomically(backup, savePath, overwrite: true);
        }

        private static string LegacyPath(string savePath) => Path.ChangeExtension(Path.GetFullPath(savePath), ".zks.bck");

        private static string GetBackupPath(string folder, string savePath)
        {
            var fullPath = Path.GetFullPath(savePath);
            var saveFolder = Path.GetDirectoryName(fullPath);
            if (OperatingSystem.IsWindows()) saveFolder = saveFolder.ToUpperInvariant();
            var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(saveFolder)));
            return Path.Combine(folder, key, Path.GetFileName(LegacyPath(fullPath)));
        }

        internal static void BackupBeforeOverwrite(string savePath, string backupPath, string fallbackPath)
        {
            backupPath ??= LegacyPath(savePath);
            try {
                CopyAtomically(savePath, backupPath, overwrite: true, useCurrentTime: true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
                if (fallbackPath != null && !PathComparer.Equals(Path.GetFullPath(backupPath), Path.GetFullPath(fallbackPath))) {
                    try {
                        CopyAtomically(savePath, fallbackPath, overwrite: true, useCurrentTime: true);
                        return;
                    }
                    catch (Exception fallbackError) when (fallbackError is IOException or UnauthorizedAccessException) {
                        throw new IOException($"Could not back up the save to '{backupPath}' or '{fallbackPath}'. The save was not overwritten.",
                            new AggregateException(ex, fallbackError));
                    }
                }

                throw new IOException($"Could not back up the save to '{backupPath}'. The save was not overwritten. Check the backup folder in Settings.", ex);
            }
        }

        private static void CopyAtomically(string source, string destination, bool overwrite, bool useCurrentTime = false)
        {
            destination = Path.GetFullPath(destination);
            if (PathComparer.Equals(Path.GetFullPath(source), destination)) {
                throw new IOException("The source and backup paths must be different.");
            }

            var folder = Path.GetDirectoryName(destination);
            Directory.CreateDirectory(folder);
            var temporary = Path.Combine(folder, Guid.NewGuid().ToString("N") + ".tmp");
            try {
                File.Copy(source, temporary);
                if (useCurrentTime) File.SetLastWriteTimeUtc(temporary, DateTime.UtcNow);
                File.Move(temporary, destination, overwrite);
            }
            finally {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }
    }
}
