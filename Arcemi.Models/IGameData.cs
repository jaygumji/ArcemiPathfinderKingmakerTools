using System.Collections.Generic;
using System;
using System.Linq;

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
    public interface IGameDataAction : IGameData
    {
        string Label { get; }
        bool IsEnabled { get; }
        GameDataActionType Type { get; }
        GameDataSize Size { get; }
        void Invoke();
    }
    public interface IGameDataMessage : IGameData
    {
        string Text { get; }
        GameDataSize Size { get; }
        GameDataMessageType Type { get; }
    }
    public enum GameDataMessageType
    {
        Info,
        Warning,
        Error
    }
    public enum GameDataActionType
    {
        Normal,
        Add,
        Destroy
    }
    public enum GameDataSize
    {
        Small,
        Medium,
        Large,
        Row
    }
    public interface IGameDataList : IGameData
    {
        string ItemName { get; }
        Func<IGameDataObject, string, bool> SearchPredicate { get; }
        IGameModelCollection<IGameDataObject> Entries { get; }
    }
    public interface IGameDataRowList : IGameDataList
    {
        GameDataSize NameSize { get; }
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
    public interface IGameDataDouble : IGameDataValue
    {
        double Value { get; set; }
        double MinValue { get; }
        double MaxValue { get; }
        double Modifiers { get; }
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
        public static IGameDataOptions Options<T>(string label, IReadOnlyList<DataOption> options, T instance, Func<T, string> getter, Action<T, string> setter = null, GameDataSize size = GameDataSize.Large)
        {
            return new GameDataOptions<T>(label, instance, getter, setter, size, options);
        }
        public static IGameDataOptions Options<T>(string label, IEnumerable<string> options, T instance, Func<T, string> getter, Action<T, string> setter = null, GameDataSize size = GameDataSize.Large)
        {
            return new GameDataOptions<T>(label, instance, getter, setter, size, options.Select(o => new DataOption(o)).ToArray());
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
            public GameDataList(string itemName, IGameModelCollection<IGameDataObject> entries, Func<IGameDataObject, string, bool> searchPredicate)
            {
                ItemName = itemName;
                SearchPredicate = searchPredicate;
                Entries = entries;
            }
            public string ItemName { get; }
            public Func<IGameDataObject, string, bool> SearchPredicate { get; }
            public IGameModelCollection<IGameDataObject> Entries { get; }
        }
        public static IGameDataList List(string itemName, IGameModelCollection<IGameDataObject> entries, Func<IGameDataObject, string, bool> searchPredicate = null)
        {
            return new GameDataList(itemName, entries, searchPredicate);
        }
        public static IGameDataList List<T>(string itemName, ListAccessor<T> accessor, Func<T, IGameDataObject> factory, Func<T, bool> predicate = null, GameModelCollectionWriter<IGameDataObject, T> writer = null, Func<IGameDataObject, string, bool> searchPredicate = null)
            where T : Model
        {
            var entries = new GameModelCollection<IGameDataObject, T>(accessor, factory, predicate, writer);
            return new GameDataList(itemName, entries, searchPredicate);
        }

        private class GameDataRowList : GameDataList, IGameDataRowList
        {
            public GameDataRowList(string itemName, IGameModelCollection<IGameDataObject> entries, Func<IGameDataObject, string, bool> searchPredicate, GameDataSize nameSize)
                : base(itemName, entries, searchPredicate)
            {
                NameSize = nameSize;
            }

            public GameDataSize NameSize { get; }
        }
        public static IGameDataRowList RowList(IGameModelCollection<IGameDataObject> entries, string itemName = null, Func<IGameDataObject, string, bool> searchPredicate = null, GameDataSize nameSize = GameDataSize.Medium)
        {
            return new GameDataRowList(itemName, entries, searchPredicate, nameSize);
        }
        public static IGameDataRowList RowList<T>(ListAccessor<T> accessor, Func<T, IGameDataObject> factory, Func<T, bool> predicate = null, GameModelCollectionWriter<IGameDataObject, T> writer = null, string itemName = null, Func<IGameDataObject, string, bool> searchPredicate = null, GameDataSize nameSize = GameDataSize.Medium)
            where T : Model
        {
            var entries = new GameModelCollection<IGameDataObject, T>(accessor, factory, predicate, writer);
            return new GameDataRowList(itemName, entries, searchPredicate, nameSize);
        }
        public static IGameDataRowList RowList(ListValueAccessor<string> blueprints, IGameResourcesProvider res, IReadOnlyList<IBlueprintMetadataEntry> availableEntries, string itemName = null, Func<IGameDataObject, string, bool> searchPredicate = null, GameDataSize nameSize = GameDataSize.Medium)
        {
            return RowList(GameModelCollectionBlueprintListWriter.CreateCollection(blueprints, res, availableEntries), itemName, searchPredicate, nameSize);
        }
        
        private class GameDataInteger<T> : IGameDataInteger
            where T : class
        {
            private readonly Func<T, int> getter;
            private readonly Action<T, int> setter;

            public GameDataInteger(string label, T instance, Func<T, int> getter, Action<T, int> setter, int minValue, int maxValue, int modifiers, GameDataSize size)
            {
                Label = label;
                Instance = instance;
                this.getter = getter;
                this.setter = setter;
                IsReadOnly = instance is null || this.setter is null;
                MinValue = minValue;
                MaxValue = maxValue;
                Modifiers = modifiers;
                Size = size;
            }

            public int Value { get => Instance is null ? default : getter(Instance); set => setter?.Invoke(Instance, value); }

            public string Label { get; }
            public T Instance { get; }
            public bool IsReadOnly { get; }
            public int MinValue { get; }
            public int MaxValue { get; }
            public int Modifiers { get; }
            public GameDataSize Size { get; }
        }
        public static IGameDataInteger Integer<T>(string label, T instance, Func<T, int> getter, Action<T, int> setter = null, int minValue = 1, int maxValue = int.MaxValue, int modifiers = 0, GameDataSize size = GameDataSize.Medium)
            where T : class
        {
            return new GameDataInteger<T>(label, instance, getter, setter, minValue, maxValue, modifiers, size);
        }

        private class GameDataDouble<T> : IGameDataDouble
            where T : class
        {
            private readonly Func<T, double> getter;
            private readonly Action<T, double> setter;

            public GameDataDouble(string label, T instance, Func<T, double> getter, Action<T, double> setter, double minValue, double maxValue, double modifiers, GameDataSize size)
            {
                Label = label;
                Instance = instance;
                this.getter = getter;
                this.setter = setter;
                IsReadOnly = instance is null || this.setter is null;
                MinValue = minValue;
                MaxValue = maxValue;
                Modifiers = modifiers;
                Size = size;
            }

            public double Value { get => Instance is null ? default : getter(Instance); set => setter?.Invoke(Instance, value); }

            public string Label { get; }
            public T Instance { get; }
            public bool IsReadOnly { get; }
            public double MinValue { get; }
            public double MaxValue { get; }
            public double Modifiers { get; }
            public GameDataSize Size { get; }
        }
        public static IGameDataDouble Double<T>(string label, T instance, Func<T, double> getter, Action<T, double> setter = null, double minValue = 1, double maxValue = double.MaxValue, double modifiers = 0, GameDataSize size = GameDataSize.Medium)
            where T : class
        {
            return new GameDataDouble<T>(label, instance, getter, setter, minValue, maxValue, modifiers, size);
        }

        private class GameDataAction : IGameDataAction
        {
            private readonly Action action;
            private readonly Func<bool> _isEnabled;

            public GameDataAction(string label, Action action, Func<bool> isEnabled, GameDataActionType type, GameDataSize size)
            {
                Label = label;
                this.action = action;
                _isEnabled = isEnabled;
                Type = type;
                Size = size;
            }

            public string Label { get; }
            public bool IsEnabled => _isEnabled();
            public GameDataActionType Type { get; }
            public GameDataSize Size { get; }
            public void Invoke()
            {
                action?.Invoke();
            }
        }
        public static IGameDataAction Action(string label, Action action, bool isEnabled = true, GameDataActionType type = GameDataActionType.Normal, GameDataSize size = GameDataSize.Medium)
        {
            return new GameDataAction(label, action, () => isEnabled, type, size);
        }
        public static IGameDataAction Action(string label, Action action, Func<bool> isEnabled, GameDataActionType type = GameDataActionType.Normal, GameDataSize size = GameDataSize.Medium)
        {
            return new GameDataAction(label, action, isEnabled, type, size);
        }
        public static IGameDataAction Action(Action action, bool isEnabled = true, GameDataActionType type = GameDataActionType.Normal, GameDataSize size = GameDataSize.Small)
        {
            return new GameDataAction(null, action, () => isEnabled, type, size);
        }
        public static IGameDataAction Action(Action action, Func<bool> isEnabled, GameDataActionType type = GameDataActionType.Normal, GameDataSize size = GameDataSize.Small)
        {
            return new GameDataAction(null, action, isEnabled, type, size);
        }

        private class GameDataMessage : IGameDataMessage
        {
            public GameDataMessage(string text, GameDataMessageType type, GameDataSize size)
            {
                Text = text;
                Type = type;
                Size = size;
            }

            public string Text { get; }
            public GameDataMessageType Type { get; }
            public GameDataSize Size { get; }
        }
        public static IGameDataMessage Message(string text, GameDataMessageType type = GameDataMessageType.Info, GameDataSize size = GameDataSize.Row)
        {
            return new GameDataMessage(text, type, size);
        }
    }
}
