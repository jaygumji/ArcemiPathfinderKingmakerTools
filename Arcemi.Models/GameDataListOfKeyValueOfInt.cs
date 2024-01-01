using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class GameDataListOfKeyValueOfInt : IGameDataList
    {
        public GameDataListOfKeyValueOfInt(IGameResourcesProvider res, ListAccessor<KeyValuePairModel<int>> @ref, string itemName, params BlueprintType[] types)
        {
            ItemName = itemName;
            Entries = new GameModelCollection<IGameDataObject, KeyValuePairModel<int>>(@ref,
                a => GameDataModels.Object(res.Blueprints.GetNameOrBlueprint(a.Key), new IGameData[] {new GameDataListOfKeyValueOfIntEntry(a) }, a),
                writer: new GameDataListOfKeyValueOfIntCollectionWriter(res, types));
        }

        public string ItemName { get; }
        public IGameModelCollection<IGameDataObject> Entries { get; }
        public GameDataListMode Mode => GameDataListMode.Rows;
    }

    public class GameDataListOfKeyValueOfIntCollectionWriter : GameModelCollectionWriter<IGameDataObject, KeyValuePairModel<int>>
    {
        private readonly IGameResourcesProvider Res;
        public GameDataListOfKeyValueOfIntCollectionWriter(IGameResourcesProvider res, params BlueprintType[] types)
        {
            Res = res;
            Types = types;
        }

        public IReadOnlyList<BlueprintType> Types { get; }

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            args.Obj.Add("Key", args.Blueprint);
            args.Obj.Add("Value", 1);
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameDataObject> current)
        {
            var currentBlueprints = new HashSet<string>(current.Select(e => ((KeyValuePairModel<int>)e.Ref).Key), StringComparer.Ordinal);
            return Res.Blueprints.GetEntries(Types).Where(e => !currentBlueprints.Contains(e.Id)).ToArray();
        }
    }

    internal class GameDataListOfKeyValueOfIntEntry : IGameDataInteger
    {
        public GameDataListOfKeyValueOfIntEntry(KeyValuePairModel<int> @ref)
        {
            Ref = @ref;
        }

        public KeyValuePairModel<int> Ref { get; }
        public string Label => "Count";
        public int Value { get => Ref.Value; set => Ref.Value = value; }
        public int MinValue => 1;
        public int MaxValue => int.MaxValue;
        public int Modifiers => 0;
        public bool IsReadOnly => false;

        public GameDataSize Size => GameDataSize.Medium;
    }
}