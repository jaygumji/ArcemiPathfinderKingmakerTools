using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameModelCollectionAbilityWriter : GameModelCollectionWriter<IGameUnitAbilityEntry, FactItemModel>
    {
        private static IGameResourcesProvider Res => GameDefinition.Pathfinder_Kingmaker.Resources;

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            AbilityFactItemModel.Prepare(args.References, args.Obj);
            args.Obj.Add(nameof(AbilityFactItemModel.Blueprint), args.Blueprint);
        }

        public override bool IsAddEnabled => false;

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitAbilityEntry> current)
        {
            var currentIds = new HashSet<string>(current.Select(x => x.Blueprint), StringComparer.Ordinal);
            return Res.Blueprints.GetEntries(BlueprintTypeId.Ability)
                .Where(e => !currentIds.Contains(e.Id))
                .ToArray();
        }
    }
}