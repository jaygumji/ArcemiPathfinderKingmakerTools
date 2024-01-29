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
        public bool IsRanked => Model.IsRanked;
        public int Rank
        {
            get => Model.Rank; set {
                var fr = Model.Rank;
                Model.Rank = value;
                if (value < fr) {
                    for (var i = fr; i > value; i--) {
                        if (Model.RankToSource.Count == 0) continue;

                        var duplicate = Model.RankToSource.GroupBy(rs => rs.Level).Where(g => g.Count() > 1).SelectMany(g => g).FirstOrDefault();
                        if (duplicate != null) {
                            Model.RankToSource.Remove(duplicate);
                            continue;
                        }

                        Model.RankToSource.RemoveAt(Model.RankToSource.Count - 1);
                    }
                }
                else if (value > fr) {
                    for (var i = fr; i < value; i++) {
                        var rankToSource = Model.RankToSource.Insert(0);
                        rankToSource.Blueprint = Model.Blueprint;
                        rankToSource.Level = i;
                    }
                }
            }
        }
        public int SourceLevel => Model.SourceLevel;

        public string Export()
        {
            return Model.Export();
        }
    }
}