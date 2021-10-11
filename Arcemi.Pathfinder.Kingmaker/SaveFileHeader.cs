using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class SaveFileGroup
    {
        public SaveFileGroup(IEnumerable<SaveFileHeader> headers)
        {
            Headers = headers.OrderByDescending(h => h.Header.SystemSaveTime).ToArray();
        }

        public IReadOnlyList<SaveFileHeader> Headers { get; }
        public string PlayerCharacterName => Headers.First().Header.PlayerCharacterName;
        public DateTime LastSystemSaveTime => Headers.First().Header.SystemSaveTime;
        public bool IsExpanded { get; set; }
        public void ToggleExpansion()
        {
            IsExpanded = !IsExpanded;
        }
        public static IEnumerable<SaveFileGroup> All(string directory, IGameResourcesProvider res)
        {
            var files = Directory.EnumerateFiles(directory, "*.zks");
            return files
                .Select(f => new SaveFileHeader(f, res))
                .Where(h => h.Exception == null)
                .GroupBy(h => h.Header.GameId)
                .Select(g => new SaveFileGroup(g))
                .OrderByDescending(g => g.LastSystemSaveTime)
                .ToArray();
        }
    }
    public class SaveFileHeader
    {
        public SaveFileHeader(string filePath, IGameResourcesProvider res)
        {
            Location = new SaveFileLocation(filePath);
            try {
                Header = SaveGameFile.ReadHeader(filePath, res);
            }
            catch (Exception e) {
                Exception = e;
            }
        }

        public SaveFileHeader(SaveFileLocation location, HeaderModel header)
        {
            Location = location;
            Header = header;
        }

        public SaveFileLocation Location { get; }
        public HeaderModel Header { get; }
        public Exception Exception { get; }
    }
}
