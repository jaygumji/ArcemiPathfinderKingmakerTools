using System;
using System.IO;

namespace Arcemi.SaveGameEditor
{
    public static class LocalImageUrl
    {
        public static string ToApiUrl(string pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl)) return string.Empty;
            if (pathOrUrl.StartsWith("/", StringComparison.Ordinal) && !File.Exists(pathOrUrl)) return pathOrUrl;
            if (pathOrUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) return pathOrUrl;
            if (pathOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || pathOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) {
                return pathOrUrl;
            }

            var path = pathOrUrl;
            if (pathOrUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase)
                && Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var fileUri)
                && fileUri.IsFile) {
                path = fileUri.LocalPath;
            }

            if (!Path.IsPathRooted(path)) return pathOrUrl;
            return "/api/local-images/file?path=" + Uri.EscapeDataString(path);
        }
    }
}
