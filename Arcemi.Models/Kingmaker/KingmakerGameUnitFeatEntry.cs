using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitFeatEntry : IGameUnitFeatEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameUnitFeatEntry(FactItemModel model)
        {
            Model = (FeatureFactItemModel)model;
            if (Res.Blueprints.TryGet(model.Blueprint, out var blueprint)) {
                Tooltip = string.Concat(blueprint.Name.Original, ", ", blueprint.Id, ", ", blueprint.Type.FullName);
            }
        }

        public FeatureFactItemModel Model { get; }

        public string DisplayName => Model.DisplayName(Res);
        public string Tooltip { get; }
        public string Blueprint => Model.Blueprint;
        public string Category => Model.Param?.WeaponCategory ?? Model.Param?.SpellSchool;
        public bool IsRanked => true;
        public int Rank
        {
            get => Model.GetAccessor().Value<int>();
            set => Model.GetAccessor().Value(value);
        }
        public int SourceLevel => Model.SourceLevel;

        public string Export()
        {
            return Model.Export();
        }
    }
}