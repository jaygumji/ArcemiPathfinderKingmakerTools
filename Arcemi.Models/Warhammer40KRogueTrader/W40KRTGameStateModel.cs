using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameStateModel : IGameStateModel
    {
        public W40KRTGameStateModel(PlayerModel player)
        {
            Player = player;
            QuestBook = new W40KRTGameStateQuestBookModel(player.QuestBook);
        }

        public PlayerModel Player { get; }
        public bool IsSupported => false;
        public IGameStateQuestBook QuestBook { get; }
    }

    internal class W40KRTGameStateQuestBookModel : IGameStateQuestBook
    {
        public W40KRTGameStateQuestBookModel(QuestBookModel @ref)
        {
            Ref = @ref;
            Entries = @ref.Facts.Items.OfType<QuestFactItemModel>().Select(q => new W40KRTGameStateQuestEntry(q)).ToArray();
        }

        public QuestBookModel Ref { get; }
        public IReadOnlyList<IGameStateQuestEntry> Entries { get; }
    }

    internal class W40KRTGameStateQuestEntry : IGameStateQuestEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameStateQuestEntry(QuestFactItemModel @ref)
        {
            Ref = @ref;
            StateOptions = new[] {
                new DataOption("None"),
                new DataOption("Started"),
                new DataOption("Completed"),
                new DataOption("Failed"),
            };
            Objectives = @ref.PersistentObjectives.Select(o => new W40KRTGameStateQuestObjectiveEntry(o, StateOptions)).ToArray();
        }

        public QuestFactItemModel Ref { get; }

        public string Name => Res.Blueprints.GetNameOrBlueprint(Ref.Blueprint);
        public string State { get => Ref.State; set => Ref.State = value; }
        public bool IsCompleted => Ref.State.Eq("Completed");
        public IReadOnlyList<DataOption> StateOptions { get; }
        public IReadOnlyList<IGameStateQuestObjectiveEntry> Objectives { get; }
    }

    internal class W40KRTGameStateQuestObjectiveEntry : IGameStateQuestObjectiveEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameStateQuestObjectiveEntry(PersistentObjectiveModel @ref, IReadOnlyList<DataOption> stateOptions)
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