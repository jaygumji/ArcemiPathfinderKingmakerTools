#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Arcemi.Models
{
    public sealed class SaveGameFile : IDisposable
    {
        private ZipArchive _archive;
        private Dictionary<string, ZipArchiveEntry> _entries;

        public SaveGameFile(string path) : this(OpenFile(path))
        {
        }

        private SaveGameFile(Stream stream)
        {
            ZipArchive archive = null;
            try {
                archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
                var entries = archive.Entries.ToDictionary(entry => entry.FullName, StringComparer.OrdinalIgnoreCase);
                _archive = archive;
                _entries = entries;
            }
            catch {
                archive?.Dispose();
                stream.Dispose();
                throw;
            }
        }

        internal static SaveGameFile Snapshot(string path)
        {
            using var source = OpenFile(path);
            var snapshot = new FileStream(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zks"),
                FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, bufferSize: 81920, FileOptions.DeleteOnClose);
            try {
                source.CopyTo(snapshot);
                snapshot.Position = 0;
                return new SaveGameFile(snapshot);
            }
            catch {
                snapshot.Dispose();
                throw;
            }
        }

        private static FileStream OpenFile(string path) =>
            new(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);

        public static HeaderModel ReadHeader(string path)
        {
            using var file = new SaveGameFile(path);
            return file.Contains(SaveGameDocument.HeaderEntry)
                ? file.Read(SaveGameDocument.HeaderEntry).GetRoot<HeaderModel>()
                : null;
        }

        public bool Contains<T>(SaveEntry<T> entry) => _entries.ContainsKey(entry.Name);

        public T Read<T>(SaveEntry<T> entry)
        {
            if (!_entries.TryGetValue(entry.Name, out var member)) {
                throw new InvalidDataException($"The save is missing '{entry.Name}'.");
            }
            using var stream = member.Open();
            return entry.Read(stream);
        }

        internal void Save(string destination, string backupPath, string fallbackBackupPath, IReadOnlyDictionary<string, byte[]> changes)
        {
            var fullDestination = Path.GetFullPath(destination);
            var temporary = Path.Combine(Path.GetDirectoryName(fullDestination), Guid.NewGuid().ToString("N") + ".tmp");
            try {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (var output = new ZipArchive(stream, ZipArchiveMode.Create)) {
                    foreach (var entry in _archive.Entries) {
                        var copy = output.CreateEntry(entry.FullName);
                        copy.LastWriteTime = entry.LastWriteTime;
                        copy.ExternalAttributes = entry.ExternalAttributes;
                        using var target = copy.Open();
                        if (changes.TryGetValue(entry.FullName, out var replacement)) {
                            target.Write(replacement);
                        }
                        else {
                            using var source = entry.Open();
                            source.CopyTo(target);
                        }
                    }
                }

                using var staged = Snapshot(temporary);
                staged.Read(SaveGameDocument.HeaderEntry);
                if (staged._entries.Count != _entries.Count) {
                    throw new InvalidDataException("The staged save has a different entry count.");
                }
                foreach (var change in changes) {
                    var entry = new SaveEntry<byte[]>(change.Key, ReadBytes);
                    if (!staged.Read(entry).AsSpan().SequenceEqual(change.Value)) {
                        throw new InvalidDataException($"Could not verify the staged save entry '{change.Key}'.");
                    }
                }

                if (File.Exists(fullDestination)) {
                    SaveBackupStorage.BackupBeforeOverwrite(fullDestination, backupPath, fallbackBackupPath);
                    File.Replace(temporary, fullDestination, destinationBackupFileName: null);
                }
                else {
                    File.Move(temporary, fullDestination);
                }

                (_archive, staged._archive) = (staged._archive, _archive);
                (_entries, staged._entries) = (staged._entries, _entries);
            }
            finally {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }

        internal static JsonPartSaveGameFile ReadJson(Stream stream)
        {
            using var text = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            using var json = new JsonTextReader(text);
            return new JsonPartSaveGameFile(JObject.Load(json));
        }

        internal static byte[] ReadBytes(Stream stream)
        {
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return buffer.ToArray();
        }

        public void Dispose() => _archive.Dispose();
    }
}
