using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class GameModelCollectionBlueprintListWriter : GameModelCollectionWriter<IGameDataObject>
    {
        public static IGameModelCollection<IGameDataObject> CreateCollection(ListValueAccessor<string> accessor, IGameResourcesProvider res, IReadOnlyList<IBlueprintMetadataEntry> availableEntries)
        {
            if (accessor is null) return GameModelCollection<IGameDataObject>.Empty;
            var writer = new GameModelCollectionBlueprintListWriter(accessor, res, availableEntries);
            return new GameModelCollection<IGameDataObject>(accessor.Select(b => writer.Create(b)), writer);
        }

        private readonly ListValueAccessor<string> accessor;
        private readonly IGameResourcesProvider res;
        private readonly IReadOnlyList<IBlueprintMetadataEntry> availableEntries;

        public GameModelCollectionBlueprintListWriter(ListValueAccessor<string> accessor, IGameResourcesProvider res, IReadOnlyList<IBlueprintMetadataEntry> availableEntries)
        {
            this.accessor = accessor;
            this.res = res;
            this.availableEntries = availableEntries;
        }

        public IGameDataObject Create(string blueprint)
        {
            return GameDataModels.Object(res.Blueprints.GetNameOrBlueprint(blueprint), new IGameData[] {
                GameDataModels.Text("Blueprint", new object(), _ => blueprint)
            });
        }

        public override IGameDataObject Add(AddCollectionItemArgs args)
        {
            accessor.Add(args.Blueprint);
            return Create(args.Blueprint);
        }

        public override IGameDataObject Insert(int index, AddCollectionItemArgs args)
        {
            accessor.Insert(index, args.Blueprint);
            return Create(args.Blueprint);
        }

        public override void Remove(RemoveCollectionItemArgs<IGameDataObject> args)
        {
            accessor.Remove(((IGameDataText)args.GameModel.Properties.First()).Value);
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameDataObject> current)
        {
            var hashset = new HashSet<string>(current.Select(x => ((IGameDataText)x.Properties.First()).Value), StringComparer.Ordinal);
            return availableEntries.Where(e => !hashset.Contains(e.Id)).ToArray();
        }
    }
}
