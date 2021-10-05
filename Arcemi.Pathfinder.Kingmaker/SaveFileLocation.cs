using System;
using System.IO;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class SaveFileLocation
    {
        public SaveFileLocation(string directory, string name, ISaveFileIterator iterator = null)
            : this(Path.Combine(directory, name), iterator)
        {
        }
        public SaveFileLocation(string path, ISaveFileIterator iterator = null)
        {
            _iterator = iterator ?? FileSystemSaveFileIterator.Instance;
            Directory = Path.GetDirectoryName(path);
            Name = Path.GetFileNameWithoutExtension(path);
            Extension = Path.GetExtension(path);
            if (string.Equals(Extension, ".zks", StringComparison.OrdinalIgnoreCase)) {
                FileName = Name?.Replace(' ', '_');
            }
            else {
                Name += Extension;
                Extension = ".zks";
                FileName = Name?.Replace('.', '_')?.Replace(' ', '_');
            }
            if (!File.Exists(path)) {
                FileName = EnsureCorrectNewName(_iterator, Directory, FileName);
            }
            FilePath = Path.Combine(Directory, FileName + Extension);
            BackupFilePath = Path.ChangeExtension(FilePath, ".zks.bck");
            FileExists = File.Exists(FilePath);
            BackupExists = File.Exists(BackupFilePath);
        }

        public bool FileExists { get; }
        public bool BackupExists { get; }
        public string FilePath { get; }
        public string FileName { get; }
        public string Name { get; }

        private readonly ISaveFileIterator _iterator;

        public string Directory { get; }
        public string Extension { get; }
        public string BackupFilePath { get; }

        private static string EnsureCorrectNewName(ISaveFileIterator iterator, string directory, string name)
        {
            if (name?.StartsWith("Manual_", StringComparison.OrdinalIgnoreCase) ?? false) {
                return name;
            }

            var next = iterator.IterateAndFindLastIndex(directory) + 1;
            return string.Concat("Manual_", next, '_', name);
        }

        public SaveFileLocation Refresh()
        {
            return new SaveFileLocation(FilePath, _iterator);
        }
    }
}
