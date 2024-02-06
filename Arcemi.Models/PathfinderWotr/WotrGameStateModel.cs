using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameStateModel : IGameStateModel
    {
        public WotrGameStateModel(PlayerModel player)
        {
            Player = player;
            QuestBook = new WotrGameStateQuestBookModel(player.QuestBook);
        }

        public PlayerModel Player { get; }

        public bool IsSupported => true;
        public IGameStateQuestBook QuestBook { get; }
    }

    internal class WotrGameStateQuestBookModel : IGameStateQuestBook
    {
        public WotrGameStateQuestBookModel(QuestBookModel @ref)
        {
            Ref = @ref;
            Entries = @ref.Facts.Items.OfType<QuestFactItemModel>().Select(q => new WotrGameStateQuestEntry(q)).ToArray();
        }

        public QuestBookModel Ref { get; }
        public IReadOnlyList<IGameStateQuestEntry> Entries { get; }
    }

    internal class WotrGameStateQuestEntry : IGameStateQuestEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameStateQuestEntry(QuestFactItemModel @ref)
        {
            Ref = @ref;
            StateOptions = new[] {
                new DataOption("None"),
                new DataOption("Started"),
                new DataOption("Completed"),
                new DataOption("Failed"),
            };
            Objectives = @ref.PersistentObjectives.Select(o => new WotrGameStateQuestObjectiveEntry(o, StateOptions)).ToArray();
        }

        public QuestFactItemModel Ref { get; }

        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Blueprint);
        public string State { get => Ref.State; set => Ref.State = value; }
        public bool IsCompleted => Ref.State.Eq("Completed");
        public IReadOnlyList<DataOption> StateOptions { get; }
        public IReadOnlyList<IGameStateQuestObjectiveEntry> Objectives { get; }
    }

    internal class WotrGameStateQuestObjectiveEntry : IGameStateQuestObjectiveEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public WotrGameStateQuestObjectiveEntry(PersistentObjectiveModel @ref, IReadOnlyList<DataOption> stateOptions)
        {
            Ref = @ref;
            StateOptions = stateOptions;
        }

        public PersistentObjectiveModel Ref { get; }
        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Blueprint);
        public string State { get => Ref.State; set => Ref.State = value; }
        public IReadOnlyList<DataOption> StateOptions { get; }
    }
}