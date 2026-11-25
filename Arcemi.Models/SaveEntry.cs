using System;
using System.IO;

namespace Arcemi.Models
{
    public sealed class SaveEntry<T>
    {
        public SaveEntry(string name, Func<Stream, T> read)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentNullException.ThrowIfNull(read);
            Name = name;
            Read = read;
        }

        public string Name { get; }
        internal Func<Stream, T> Read { get; }
    }
}
