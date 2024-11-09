using System.Collections.Generic;
using System.Linq;
using System;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameModelCollectionBuffWriter : GameModelCollectionWriter<IGameUnitBuffEntry, FactItemModel>
    {
        private static IGameResourcesProvider Res => GameDefinition.Pathfinder_Kingmaker.Resources;

        private readonly IGameTimeProvider _gameTimeProvider;

        public KingmakerGameModelCollectionBuffWriter(IGameTimeProvider gameTimeProvider)
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