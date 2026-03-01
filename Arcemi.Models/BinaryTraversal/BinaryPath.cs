using System;
using System.Collections.Generic;
using System.Globalization;

namespace Arcemi.Models.BinaryTraversal
{
    public enum BinaryPathSegmentKind
    {
        Property,
        Index
    }

    public readonly struct BinaryPathSegment
    {
        public BinaryPathSegment(BinaryPathSegmentKind kind, string value)
        {
            Kind = kind;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public BinaryPathSegmentKind Kind { get; }
        public string Value { get; }
    }

    public sealed class BinaryPath
    {
        public BinaryPath(string value, IReadOnlyList<BinaryPathSegment> segments)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Segments = segments ?? throw new ArgumentNullException(nameof(segments));
        }

        public string Value { get; }
        public IReadOnlyList<BinaryPathSegment> Segments { get; }

        public static BinaryPath Parse(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("Path must not be empty.", nameof(path));
            }

            var segments = new List<BinaryPathSegment>();
            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts) {
                ParsePart(part, segments);
            }

            return new BinaryPath(path, segments);
        }

        private static void ParsePart(string part, List<BinaryPathSegment> segments)
        {
            var nameEnd = part.IndexOf('[');
            if (nameEnd < 0) {
                segments.Add(new BinaryPathSegment(BinaryPathSegmentKind.Property, part));
                return;
            }

            if (nameEnd > 0) {
                segments.Add(new BinaryPathSegment(BinaryPathSegmentKind.Property, part[..nameEnd]));
            }

            var cursor = nameEnd;
            while (cursor < part.Length) {
                if (part[cursor] != '[') {
                    throw new FormatException("Invalid path segment '" + part + "'.");
                }

                var close = part.IndexOf(']', cursor + 1);
                if (close < 0) {
                    throw new FormatException("Invalid path segment '" + part + "'.");
                }

                var indexText = part.Substring(cursor + 1, close - cursor - 1);
                if (!int.TryParse(indexText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)) {
                    throw new FormatException("Invalid index '" + indexText + "' in path segment '" + part + "'.");
                }

                segments.Add(new BinaryPathSegment(BinaryPathSegmentKind.Index, indexText));
                cursor = close + 1;
            }
        }
    }
}
