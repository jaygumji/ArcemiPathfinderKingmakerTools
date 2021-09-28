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
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class SaveGameFile
    {
        public string Filepath { get; private set; }
        public string WorkingPath { get; }

        private readonly Dictionary<string, string> _lookup;

        public SaveGameFile(string path)
        {
            Filepath = path;
            WorkingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            _lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Extract();
        }

        private void Extract()
        {
            if (!Directory.Exists(WorkingPath)) {
                Directory.CreateDirectory(WorkingPath);
            }

            using (var stream = new FileStream(Filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read)) {
                    foreach (var entry in archive.Entries) {
                        using (var entryStream = entry.Open()) {
                            var writePath = Path.Combine(WorkingPath, entry.Name);
                            _lookup.Add(entry.Name, writePath);

                            using (var writeStream = new FileStream(writePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read)) {
                                entryStream.CopyTo(writeStream);
                            }
                        }
                    }
                }
            }
        }

        public JsonPartSaveGameFile GetParty()
        {
            return new JsonPartSaveGameFile(GetWorkingPath("party.json"), GetJson("party.json"));
        }

        public JsonPartSaveGameFile GetPlayer()
        {
            return new JsonPartSaveGameFile(GetWorkingPath("player.json"), GetJson("player.json"));
        }

        public JsonPartSaveGameFile GetHeader()
        {
            return new JsonPartSaveGameFile(GetWorkingPath("header.json"), GetJson("header.json"));
        }

        public void Close()
        {
            foreach (var file in Directory.EnumerateFiles(WorkingPath)) {
                File.Delete(file);
            }
            Directory.Delete(WorkingPath);
        }

        private string GetWorkingPath(string name)
        {
            if (!_lookup.TryGetValue(name, out var path)) {
                throw new ArgumentException($"No entries with the name {name} exists.");
            }
            return path;
        }

        public string GetText(string name)
        {
            return File.ReadAllText(GetWorkingPath(name), Encoding.UTF8);
        }

        public void Save(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create)) {
                    foreach (var file in Directory.EnumerateFiles(WorkingPath)) {
                        var entryName = Path.GetFileName(file);
                        archive.CreateEntryFromFile(file, entryName);
                    }
                }
            }
            Filepath = filePath;
        }

        public JObject GetJson(string name)
        {
            using (var stream = new FileStream(GetWorkingPath(name), FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var streamReader = new StreamReader(stream, Encoding.UTF8)) {
                    using (var reader = new JsonTextReader(streamReader)) {
                        return JObject.Load(reader);
                    }
                }
            }
        }

    }
}
