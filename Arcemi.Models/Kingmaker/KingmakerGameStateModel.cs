using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameStateModel : IGameStateModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        public KingmakerGameStateModel(PlayerModel player)
        {
            Ref = player;
            QuestBook = new KingmakerGameStateQuestBookModel(player.QuestBook);
            A = Ref.GetAccessor();
            var dialog = A.Object<RefModel>("m_Dialog");
            var da = dialog.GetAccessor();
            Sections = new[] {
                GameDataModels.Object("Dialog cues", new IGameData[] {
                    GameDataModels.Message("Useful to add dialog cues. An example is that you have seen the dialog required to reach the 'true ending'."),
                    GameDataModels.RowList(da.ListValue<string>("ShownCues"), Res, Res.Blueprints.GetEntries(KingmakerBlueprintProvider.BookPage), "Cues",
                        searchPredicate: (o, t) => o.Name.ILike(t) || o.Properties.OfType<IGameDataText>().First().Value.IEq(t))
                }),
                //GameDataModels.Object("Answer checks", new IGameData[] {
                //    GameDataModels.RowList(da.List<Model>("AnswerChecks"), m => GameDataModels.Object(Res.Blueprints.GetNameOrBlueprint(m.GetAccessor().Value<string>("Key")), new IGameData[] {
                //        GameDataModels.Text("Value", m, x => x.GetAccessor().Value<string>("Value"), size: GameDataSize.Medium)
                //    }), itemName: "Answer")
                //})
            };
        }

        public PlayerModel Ref { get; }
        public bool IsSupported => true;
        public IGameStateQuestBook QuestBook { get; }
        public ModelDataAccessor A { get; }
        public IReadOnlyList<IGameDataObject> Sections { get; } = Array.Empty<IGameDataObject>();
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