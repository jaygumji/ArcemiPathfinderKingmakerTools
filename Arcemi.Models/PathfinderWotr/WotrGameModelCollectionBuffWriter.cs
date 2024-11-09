using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameModelCollectionBuffWriter : GameModelCollectionWriter<IGameUnitBuffEntry, FactItemModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        private readonly IGameTimeProvider _gameTimeProvider;

        public WotrGameModelCollectionBuffWriter(IGameTimeProvider gameTimeProvider)
        {
            _gameTimeProvider = gameTimeProvider;
        }

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            BuffFactItemModel.Prepare(args.References, args.Obj, _gameTimeProvider);
            args.Obj.Add(nameof(BuffFactItemModel.Blueprint), args.Blueprint);
        }

        public override bool IsAddEnabled => false;

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitBuffEntry> current)
        {
            var currentIds = new HashSet<string>(current.Select(x => x.Blueprint), StringComparer.Ordinal);
            return Res.Blueprints.GetEntries(BlueprintTypeId.Ability)
                .Where(e => !currentIds.Contains(e.Id))
                .ToArray();
        }
    }
}