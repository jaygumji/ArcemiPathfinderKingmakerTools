using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameUnitProgressionModel : IGameUnitProgressionModel
    {
        private IGameResourcesProvider Res => GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        private readonly IGameData[] _dataProperties;

        public WotrGameUnitProgressionModel(IGameUnitModel unit)
        {
            Unit = unit.Ref;
            Ultimates = new IGameUnitUltimateProgressionEntry[] {
                new WotrGameUnitProgressionMythicModel(unit.Ref)
            };
            Classes = unit.Ref.Descriptor.Progression.Classes.Select(c => new WotrGameUnitClassProgressionEntry(unit, c)).ToArray();
            Data = GameDataModels.Object(_dataProperties = new[] {
                GetAllChoices()
            });
        }

        private IGameData GetAllChoices()
        {
            var selections = new GameModelCollection<IGameDataObject>(
                from item in Unit.Descriptor.Progression.Selections
                from level in item.Value.ByLevel.Keys
                from selection in item.Value.ByLevel[level]
                orderby level
                select GameDataModels.Object(Res.Blueprints.GetNameOrBlueprint(item.Key), new IGameData[] {
                    GameDataModels.Text("Source", item, v => Res.Blueprints.GetNameOrBlueprint(v.Value.Source.Blueprint), size: GameDataSize.Medium),
                    GameDataModels.Text("Selection", selection, v => Res.Blueprints.GetNameOrBlueprint(v), size: GameDataSize.Small),
                    GameDataModels.Text("Level", level, v => v, size: GameDataSize.Small)
                }));

            var allChoices = GameDataModels.Object("All choices", new[] {
                GameDataModels.RowList(selections, nameSize: GameDataSize.Medium)
            }, isCollapsable: true);
            return allChoices;
        }

        public void RefreshSelections()
        {
            _dataProperties[0] = GetAllChoices();
        }

        public UnitEntityModel Unit { get; }
        public int Experience { get => Unit.Descriptor.Progression.Experience; set => Unit.Descriptor.Progression.Experience = value; }

        public int CurrentLevel { get => Unit.Descriptor.Progression.CurrentLevel; set { } }
        public bool IsLevelReadOnly => true;

        public IReadOnlyList<IGameUnitUltimateProgressionEntry> Ultimates { get; }
        public IReadOnlyList<IGameUnitClassProgressionEntry> Classes { get; }

        public IGameModelCollection<IGameUnitSelectionProgressionEntry> Selections { get; private set; } = GameModelCollection<IGameUnitSelectionProgressionEntry>.Empty;
        public IGameDataObject Data { get; }

        public bool IsSupported => true;

        public bool IsSelectionsRepairable => false;

        public void RepairSelections()
        {
        }
    }
}