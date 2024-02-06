using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameStateModel : IGameStateModel
    {
        public KingmakerGameStateModel(PlayerModel player)
        {
            Ref = player;
            QuestBook = new KingmakerGameStateQuestBookModel(player.QuestBook);
        }

        public PlayerModel Ref { get; }
        public bool IsSupported => true;
        public IGameStateQuestBook QuestBook { get; }
    }

    internal class KingmakerGameStateQuestBookModel : IGameStateQuestBook
    {
        public KingmakerGameStateQuestBookModel(QuestBookModel @ref)
        {
            Ref = @ref;
            Entries = @ref.GetAccessor().List<RefModel>("PersistentQuests").Select(q => new KingmakerGameStateQuestEntry(q)).ToArray();
        }

        public QuestBookModel Ref { get; }
        public IReadOnlyList<IGameStateQuestEntry> Entries { get; }
    }

    internal class KingmakerGameStateQuestEntry : IGameStateQuestEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameStateQuestEntry(RefModel @ref)
        {
            Ref = @ref;
            A = @ref.GetAccessor();
            StateOptions = new[] {
                new DataOption("None"),
                new DataOption("Started"),
                new DataOption("Completed"),
                new DataOption("Failed"),
            };
            Objectives = A.List<RefModel>("PersistentObjectives").Select(o => new KingmakerGameStateQuestObjectiveEntry(o, StateOptions)).ToArray();
        }

        public RefModel Ref { get; }
        public ModelDataAccessor A { get; }

        public string Name => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => A.Value<string>();
        public string State { get => A.Value<string>("m_State"); set => A.Value(value, "m_State"); }
        public bool IsCompleted => State.Eq("Completed");
        public IReadOnlyList<DataOption> StateOptions { get; }
        public IReadOnlyList<IGameStateQuestObjectiveEntry> Objectives { get; }
    }

    internal class KingmakerGameStateQuestObjectiveEntry : IGameStateQuestObjectiveEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameStateQuestObjectiveEntry(RefModel @ref, IReadOnlyList<DataOption> stateOptions)
        {
            Ref = @ref;
            A = @ref.GetAccessor();
            StateOptions = stateOptions;
        }

        public RefModel Ref { get; }
        public ModelDataAccessor A { get; }

        public string Name => Res.Blueprints.GetNameOrBlueprint(Blueprint);
        public string Blueprint => A.Value<string>();
        public string State { get => A.Value<string>("m_State"); set => A.Value(value, "m_State"); }
        public IReadOnlyList<DataOption> StateOptions { get; }
    }

}