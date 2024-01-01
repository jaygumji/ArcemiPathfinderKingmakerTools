using System.Collections.Generic;
using System;

namespace Arcemi.Models
{
    public interface IGameData
    {
    }

    public interface IGameDataValue : IGameData
    {
        string Label { get; }
        bool IsReadOnly { get; }
        GameDataSize Size { get; }
    }
    public enum GameDataSize
    {
        Small,
        Medium,
        Large,
        Row
    }

    public enum GameDataListMode
    {
        Full,
        Rows
    }
    public interface IGameDataList : IGameData
    {
        string ItemName { get; }
        GameDataListMode Mode { get; }
        IGameModelCollection<IGameDataObject> Entries { get; }
    }
    public interface IGameDataObject : IGameData
    {
        string Name { get; }
        IReadOnlyList<IGameData> Properties { get; }
        bool IsCollapsable { get; }
        Model Ref { get; }
    }

    public interface IGameDataOptions : IGameDataValue
    {
        IReadOnlyList<DataOption> Options { get; }
        string Value { get; set; }
    }

    public interface IGameDataText : IGameDataValue
    {
        string Value { get; set; }
    }

    public interface IGameDataBoolean : IGameDataValue
    {
        bool Value { get; set; }
    }
    public interface IGameDataTime : IGameDataValue
    {
        TimeParts Value { get; }
    }
    public interface IGameDataInteger : IGameDataValue
    {
        int Value { get; set; }
        int MinValue { get; }
        int MaxValue { get; }
        int Modifiers { get; }
    }
    public static class GameDataModels
    {
        private class GameDataBoolean<T> : IGameDataBoolean
        {
            private readonly Func<T, bool> getter;
            private readonly Action<T, bool> setter;

            public GameDataBoolean(string label, T instance, Func<T, bool> getter, Action<T, bool> setter, GameDataSize size)
            {
                Label = label;
                Size = size;
                Instance = instance;
                this.getter = getter;
                this.setter = setter;
                IsReadOnly = this.setter is null;
            }

            public bool Value { get => getter(Instance); set => setter?.Invoke(Instance, value); }

            public string Label { get; }
            public GameDataSize Size { get; }
            public T Instance { get; }
            public bool IsReadOnly { get; }
        }
        public static IGameDataBoolean Boolean<T>(string label, T instance, Func<T, bool> getter, Action<T, bool> setter = null, GameDataSize size = GameDataSize.Small)
        {
            return new GameDataBoolean<T>(label, instance, getter, setter, size);
        }

        private class GameDataTime : IGameDataTime
        {
            public GameDataTime(string label, TimeParts time, GameDataSize size, bool isReadOnly)
            {
                Label = label;
                Value = time;
                Size = size;
                IsReadOnly = isReadOnly;
            }

            public TimeParts Value { get; }
            public GameDataSize Size { get; }
            public string Label { get; }
            public bool IsReadOnly { get; }
        }
        public static IGameDataTime Time(string label, TimeParts time, GameDataSize size = GameDataSize.Large, bool isReadOnly = false)
        {
            return new GameDataTime(label, time, size, isReadOnly);
        }

        private class GameDataText<T> : IGameDataText
        {
            private readonly Func<T, string> getter;
            private readonly Action<T, string> setter;

            public GameDataText(string label, T instance, Func<T, string> getter, Action<T, string> setter, GameDataSize size)
            {
                Label = label;
                Instance = instance;
                this.getter = getter;
                this.setter = setter;
                Size = size;
                IsReadOnly = this.setter is null;
            }

            public string Value { get => getter(Instance); set => setter?.Invoke(Instance, value); }

            public string Label { get; }
            public T Instance { get; }
            public GameDataSize Size { get; }
            public bool IsReadOnly { get; }
        }
        public static IGameDataText Text<T>(string label, T instance, Func<T, string> getter, Action<T, string> setter = null, GameDataSize size = GameDataSize.Large)
        {
            return new GameDataText<T>(label, instance, getter, setter, size);
        }

        private class GameDataOptions<T> : IGameDataOptions
        {
            private readonly Func<T, string> getter;
            private readonly Action<T, string> setter;

            public GameDataOptions(string label, T instance, Func<T, string> getter, Action<T, string> setter, GameDataSize size, IReadOnlyList<DataOption> options)
            {
                Label = label;
                Instance = instance;
                this.getter = getter;
                this.setter = setter;
                Size = size;
                Options = options;
                IsReadOnly = this.setter is null;
            }

            public string Value { get => getter(Instance); set => setter?.Invoke(Instance, value); }

            public string Label { get; }
            public T Instance { get; }
            public GameDataSize Size { get; }
            public bool IsReadOnly { get; }
            public IReadOnlyList<DataOption> Options { get; }
        }
        public static IGameDataOptions BlueprintOptions<T>(string label, IEnumerable<IBlueprintMetadataEntry> options, T instance, Func<T, string> getter, Action<T, string> setter = null, GameDataSize size = GameDataSize.Large)
        {
            return new GameDataOptions<T>(label, instance, getter, setter, size, DataOption.Get(options, getter(instance), out _));
        }

        private class GameDataObject : IGameDataObject
        {
            public GameDataObject(string name, IReadOnlyList<IGameData> properties, Model @ref, bool isCollapsable)
            {
                Name = name;
                Properties = properties;
                Ref = @ref;
                IsCollapsable = isCollapsable;
            }

            public string Name { get; }
            public IReadOnlyList<IGameData> Properties { get; }
            public Model Ref { get; }
            public bool IsCollapsable { get; }
        }
        public static IGameDataObject Object(IReadOnlyList<IGameData> properties, Model @ref = null, bool isCollapsable = false) => Object(null, properties, @ref, isCollapsable);
        public static IGameDataObject Object(string name, IReadOnlyList<IGameData> properties, Model @ref = null, bool isCollapsable = false)
        {
            return new GameDataObject(name, properties, @ref, isCollapsable);
        }

        private class GameDataList : IGameDataList
        {
            public GameDataList(string itemName, IGameModelCollection<IGameDataObject> entries, GameDataListMode mode)
            {
                ItemName = itemName;
                Entries = entries;
                Mode = mode;
            }
            public string ItemName { get; }
            public IGameModelCollection<IGameDataObject> Entries { get; }
            public GameDataListMode Mode { get; }
        }
        public static IGameDataList List(string itemName, IGameModelCollection<IGameDataObject> entries, GameDataListMode mode = GameDataListMode.Full)
        {
            return new GameDataList(itemName, entries, mode);
        }
        public static IGameDataList List<T>(string itemName, ListAccessor<T> accessor, Func<T, IGameDataObject> factory, Func<T, bool> predicate = null, GameModelCollectionWriter<IGameDataObject, T> writer = null, GameDataListMode mode = GameDataListMode.Full)
            where T : Model
        {
            var entries = new GameModelCollection<IGameDataObject, T>(accessor, factory, predicate, writer);
            return new GameDataList(itemName, entries, mode);
        }

        private class GameDataInteger<T> : IGameDataInteger
        {
            private readonly Func<T, int> getter;
            private readonly Action<T, int> setter;

            public GameDataInteger(string label, T instance, Func<T, int> getter, Action<T, int> setter, int minValue, int maxValue, int modifiers, GameDataSize size)
            {
                Label = label;
                Instance = instance;
                this.getter = getter;
                this.setter = setter;
                IsReadOnly = this.setter is null;
                MinValue = minValue;
                MaxValue = maxValue;
                Modifiers = modifiers;
                Size = size;
            }

            public int Value { get => getter(Instance); set => setter?.Invoke(Instance, value); }

            public string Label { get; }
            public T Instance { get; }
            public bool IsReadOnly { get; }
            public int MinValue { get; }
            public int MaxValue { get; }
            public int Modifiers { get; }
            public GameDataSize Size { get; }
        }
        public static IGameDataInteger Integer<T>(string label, T instance, Func<T, int> getter, Action<T, int> setter = null, int minValue = 1, int maxValue = int.MaxValue, int modifiers = 0, GameDataSize size = GameDataSize.Medium)
        {
            return new GameDataInteger<T>(label, instance, getter, setter, minValue, maxValue, modifiers, size);
        }
    }
}
