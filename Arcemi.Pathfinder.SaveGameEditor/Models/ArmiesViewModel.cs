using Arcemi.Pathfinder.Kingmaker;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class ArmiesViewModel
    {
        private readonly MainViewModel main;

        public ArmiesViewModel(MainViewModel main)
        {
            this.main = main;
        }

        public bool CanEdit => main.CanEdit;
        public IGameResourcesProvider Resources => main.Resources;

        public IEnumerable<PlayerArmyModel> Armies => main.Player?.GlobalMaps?.SelectMany(m => m.Armies).Where(a => a.Data.IsFactionCrusaders());
        public bool HasArmies => Armies?.Any() ?? false;

        public string FindLeaderPortrait(PlayerArmyModel unit)
        {
            if (string.IsNullOrEmpty(unit.Data.LeaderGuid)) return main.Resources.AppData.Portraits.GetUnknownUri();
            var leader = main.Player.LeadersManager?.Leaders?.FirstOrDefault(l => string.Equals(l.LeaderGuid, unit.Data.LeaderGuid));
            if (leader == null) return main.Resources.AppData.Portraits.GetUnknownUri();
            return leader.PortraitPath;
        }
    }
}
