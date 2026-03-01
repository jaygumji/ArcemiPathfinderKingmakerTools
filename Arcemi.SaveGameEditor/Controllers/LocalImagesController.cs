using Arcemi.SaveGameEditor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor.Controllers
{
    [ApiController]
    [Route("api/local-images")]
    public class LocalImagesController : ControllerBase
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) {
            ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif"
        };

        private readonly EditorConfiguration _config;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

        public LocalImagesController(EditorConfiguration config)
        {
            _config = config;
        }

        [HttpGet("file")]
        public async Task<IActionResult> FileByPath([FromQuery] string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return BadRequest();
            try {
                await _config.InitializeAsync();
            }
            catch {
                // Fallback to static roots when configuration is unavailable.
            }

            var normalizedPath = DecodeLocalPath(path);
            if (string.IsNullOrWhiteSpace(normalizedPath)) return BadRequest();
            if (!Path.IsPathRooted(normalizedPath)) return BadRequest();

            var fullPath = Path.GetFullPath(normalizedPath);
            var extension = Path.GetExtension(fullPath);
            if (!AllowedExtensions.Contains(extension)) return BadRequest();
            if (!System.IO.File.Exists(fullPath)) return NotFound();
            if (!IsPathAllowed(fullPath)) return Forbid();

            if (!_contentTypeProvider.TryGetContentType(fullPath, out var contentType)) {
                contentType = "application/octet-stream";
            }

            Response.Headers.CacheControl = "public,max-age=300";
            return PhysicalFile(fullPath, contentType);
        }

        private bool IsPathAllowed(string fullPath)
        {
            var roots = new List<string> {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
            };
            if (_config?.Instance?.Games != null) {
                foreach (var game in _config.Instance.Games) {
                    if (!string.IsNullOrWhiteSpace(game.AppDataFolder)) {
                        roots.Add(game.AppDataFolder);
                        roots.Add(Path.Combine(game.AppDataFolder, "Portraits"));
                    }
                }
            }

            return roots
                .Where(Directory.Exists)
                .Select(Path.GetFullPath)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Any(root => IsPathUnderRoot(fullPath, root));
        }

        private static bool IsPathUnderRoot(string filePath, string rootPath)
        {
            var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var normalizedRoot = rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                                 + Path.DirectorySeparatorChar;
            return filePath.StartsWith(normalizedRoot, comparison);
        }

        private static string DecodeLocalPath(string path)
        {
            if (path.StartsWith("file://", StringComparison.OrdinalIgnoreCase)
                && Uri.TryCreate(path, UriKind.Absolute, out var uri)
                && uri.IsFile) {
                return uri.LocalPath;
            }

            return Uri.UnescapeDataString(path);
        }
    }
}
