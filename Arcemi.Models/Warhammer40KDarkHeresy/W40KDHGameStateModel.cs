using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHGameStateModel : IGameStateModel
    {
        public W40KDHGameStateModel(W40KDHPlayerData player, IGameResourcesProvider resources)
        {
            var quests = player?.Quests;
            if (quests is not null) {
                QuestBook = new QuestBookModel(quests.Select(quest => new QuestEntryModel(
                    resources.Blueprints.GetNameOrBlueprint(quest.Blueprint),
                    quest.State.OrIfEmpty("Unknown"),
                    quest.Objectives.Select(objective => (IGameStateQuestObjectiveEntry)new ObjectiveModel(
                        resources.Blueprints.GetNameOrBlueprint(objective.Blueprint),
                        objective.State.OrIfEmpty("Unknown"))).ToArray())).ToArray());
            }

            var entries = new List<IGameData>();
            if (player?.Investigations is { } investigations) {
                if (investigations.Cases.HasValue) entries.Add(GameDataModels.Integer("Cases", investigations, x => x.Cases.Value));
                if (investigations.Clues.HasValue) entries.Add(GameDataModels.Integer("Clues", investigations, x => x.Clues.Value));
                if (investigations.Conclusions.HasValue) entries.Add(GameDataModels.Integer("Conclusions", investigations, x => x.Conclusions.Value));
            }
            Sections = entries.Count > 0 ? new[] { GameDataModels.Object("Investigations", entries) } : Array.Empty<IGameDataObject>();
        }

        public bool IsSupported => QuestBook is not null || Sections.Count > 0;
        public IGameStateQuestBook QuestBook { get; }
        public IReadOnlyList<IGameDataObject> Sections { get; }

        private sealed record QuestBookModel(IReadOnlyList<IGameStateQuestEntry> Entries) : IGameStateQuestBook;

        private sealed record QuestEntryModel(string Name, string State, IReadOnlyList<IGameStateQuestObjectiveEntry> Objectives) : IGameStateQuestEntry
        {
            string IGameStateQuestEntry.State { get => State; set => throw new NotSupportedException(); }
            public bool IsCompleted => State == "Completed";
            public bool IsReadOnly => true;
            public IReadOnlyList<DataOption> StateOptions => Array.Empty<DataOption>();
        }

        private sealed record ObjectiveModel(string Name, string State) : IGameStateQuestObjectiveEntry
        {
            string IGameStateQuestObjectiveEntry.State { get => State; set => throw new NotSupportedException(); }
            public bool IsReadOnly => true;
            public IReadOnlyList<DataOption> StateOptions => Array.Empty<DataOption>();
        }
    }
}
