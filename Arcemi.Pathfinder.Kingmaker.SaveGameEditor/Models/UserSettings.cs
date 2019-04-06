#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models
{
    public class UserSettings
    {
        public string OpenPath { get; set; }

        public void Save(string path)
        {
            JsonUtilities.Serialize(path, this);
        }

        public void Save()
        {
            Save(GetDefaultPath());
        }

        public static UserSettings From(string path)
        {
            if (!File.Exists(path)) {
                return new UserSettings();
            }
            return JsonUtilities.Deserialize<UserSettings>(path)
                ?? new UserSettings();
        }

        public static UserSettings FromDefaultPath()
        {
            return From(GetDefaultPath());
        }

        private static string GetDefaultPath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var settingsPath = Path.Combine(appDataDir, "Arcemi", "Pathfinder_Kingmaker", "SaveGameEditor", "userSettings.json");
            return settingsPath;
        }

    }
}
