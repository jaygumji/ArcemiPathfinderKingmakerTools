using System.Collections.Generic;
using System.Linq;
using System;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameModelBuffCollectionWriter : GameModelCollectionWriter<IGameUnitBuffEntry, FactItemModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            W40KRTBuffFactItemModel.Prepare(args.References, args.Obj);
            args.Obj.Add(nameof(W40KRTBuffFactItemModel.Blueprint), args.Blueprint);
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