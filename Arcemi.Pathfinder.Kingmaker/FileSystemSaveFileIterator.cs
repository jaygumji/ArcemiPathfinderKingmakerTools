namespace Arcemi.Pathfinder.Kingmaker
{
    public class FileSystemSaveFileIterator : ISaveFileIterator
    {
        public static FileSystemSaveFileIterator Instance { get; } = new FileSystemSaveFileIterator();

        public int IterateAndFindLastIndex(string directory)
        {
            var ncur = 0;
            foreach (var filePath in System.IO.Directory.EnumerateFiles(directory, "Manual_*.zks")) {
                var fileName = System.IO.Path.GetFileName(filePath);
                var idxStart = 7;
                var idxEnd = fileName.IndexOf('_', idxStart);
                if (idxEnd < 0) continue;
                var nstr = fileName.Substring(idxStart, idxEnd - idxStart);
                if (int.TryParse(nstr, out var n)) {
                    if (n > ncur) {
                        ncur = n;
                    }
                }
            }
            return ncur;
        }
    }
}