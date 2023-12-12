using Arcemi.Models;
using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace Arcemi.SaveGameEditor.Models
{
    public class GamesForSavedGamesView
    {
        public static IReadOnlyList<GamesForSavedGamesView> Create(EditorConfiguration config)
        {
            if (config.Instance is null) return Array.Empty<GamesForSavedGamesView>();
            return SupportedGames.All.Select(d => new GamesForSavedGamesView(d, config.Instance.GetGame(d))).ToArray();
        }

        public GameDefinition Definition { get; }
        public EditorGameConfiguration Config { get; }

        public GamesForSavedGamesView(GameDefinition definition, EditorGameConfiguration config)
        {
            Definition = definition;
            Config = config;
        }

        private IEnumerable<SaveFileGroup> _saveGroups;
        public IEnumerable<SaveFileGroup> SaveGroups
        {
            get {
                if (!Config.ValidateAppDataFolder()) return Array.Empty<SaveFileGroup>();
                if (_saveGroups == null) {
                    _saveGroups = SaveFileGroup.All(Config.GetSaveGamesFolder(), Definition.Resources);
                    var g = _saveGroups.FirstOrDefault();
                    if (g != null) g.IsExpanded = true;
                }
                return _saveGroups;
            }
        }
    }
}
