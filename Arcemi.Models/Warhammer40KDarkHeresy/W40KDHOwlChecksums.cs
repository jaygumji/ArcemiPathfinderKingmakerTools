using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal static class W40KDHOwlChecksums
    {
        private static readonly SaveEntry<byte[]> ChecksumsEntry = new("checksums", SaveGameFile.ReadBytes);

        public static bool CanUpdate(SaveGameFile archive, string entryName, byte[] original)
        {
            return archive.Contains(ChecksumsEntry)
                && FindChecksum(archive.Read(ChecksumsEntry), entryName, original, out _) >= 0;
        }

        public static void StageChanges(SaveGameFile archive, Dictionary<string, byte[]> changes)
        {
            var modified = changes.Keys.Where(name => name.EndsWith(".owl", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (modified.Length == 0) return;

            var manifest = archive.Read(ChecksumsEntry);
            foreach (var name in modified) {
                var original = archive.Read(new SaveEntry<byte[]>(name, SaveGameFile.ReadBytes));
                var offset = FindChecksum(manifest, name, original, out var oldLength);
                if (offset < 0) throw new InvalidDataException($"The checksum for '{name}' is missing or ambiguous.");

                var newLine = Line(name, DecodeGameDigest(MD5.HashData(changes[name])));
                var updated = new byte[manifest.Length - oldLength + newLine.Length];
                manifest.AsSpan(0, offset).CopyTo(updated);
                newLine.CopyTo(updated.AsSpan(offset));
                manifest.AsSpan(offset + oldLength).CopyTo(updated.AsSpan(offset + newLine.Length));
                manifest = updated;
            }
            changes.Add(ChecksumsEntry.Name, manifest);
        }

        private static int FindChecksum(ReadOnlySpan<byte> manifest, string entryName, byte[] payload, out int length)
        {
            var digest = MD5.HashData(payload);
            var gameLine = Line(entryName, DecodeGameDigest(digest));
            var offset = FindLine(manifest, gameLine);
            length = gameLine.Length;

            // Earlier editor versions wrote the .NET UTF-8 replacement behavior.
            var editorLine = Line(entryName, Encoding.UTF8.GetString(digest));
            if (gameLine.AsSpan().SequenceEqual(editorLine)) return offset;

            var editorOffset = FindLine(manifest, editorLine);
            if (offset == AmbiguousLine || editorOffset == AmbiguousLine || (offset >= 0 && editorOffset >= 0)) return AmbiguousLine;
            if (editorOffset < 0) return offset;

            length = editorLine.Length;
            return editorOffset;
        }

        private static byte[] Line(string entryName, string digest) => Encoding.UTF8.GetBytes(entryName + ": " + digest);

        private static string DecodeGameDigest(ReadOnlySpan<byte> digest)
        {
            // Unity Mono replaces an invalid 3- or 4-byte prefix as a pair where .NET replaces each byte.
            Span<byte> normalized = stackalloc byte[digest.Length * 2];
            var length = 0;
            for (var i = 0; i < digest.Length; i++) {
                if (i + 1 < digest.Length && HasInvalidSecondByte(digest[i], digest[i + 1])) {
                    "\uFFFD"u8.CopyTo(normalized[length..]);
                    length += "\uFFFD"u8.Length;
                    i++;
                }
                else {
                    normalized[length++] = digest[i];
                }
            }
            return Encoding.UTF8.GetString(normalized[..length]);
        }

        private static bool HasInvalidSecondByte(byte first, byte second) => first switch {
            0xe0 => second is >= 0x80 and <= 0x9f,
            0xed => second is >= 0xa0 and <= 0xbf,
            0xf0 => second is >= 0x80 and <= 0x8f,
            0xf4 => second is >= 0x90 and <= 0xbf,
            _ => false
        };

        private const int AmbiguousLine = -2;

        private static int FindLine(ReadOnlySpan<byte> manifest, ReadOnlySpan<byte> line)
        {
            var found = -1;
            for (var offset = 0; offset <= manifest.Length - line.Length;) {
                var relative = manifest[offset..].IndexOf(line);
                if (relative < 0) break;
                var start = offset + relative;
                var end = start + line.Length;
                var startsLine = start == 0 || manifest[start - 1] == (byte)'\n';
                var endsLine = end == manifest.Length || manifest[end] == (byte)'\n'
                    || (manifest[end] == (byte)'\r' && end + 1 < manifest.Length && manifest[end + 1] == (byte)'\n');
                if (startsLine && endsLine) {
                    if (found >= 0) return AmbiguousLine;
                    found = start;
                }
                offset = start + 1;
            }
            return found;
        }
    }
}
