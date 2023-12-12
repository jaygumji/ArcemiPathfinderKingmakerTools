using Arcemi.Models;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.SaveGameEditor.Models
{
    public class ArmiesViewModel
    {
        private readonly IEditFileSession _session;
        private readonly IGameResourcesProvider _resources;

        public ArmiesViewModel(IEditFileSession session, IGameResourcesProvider resources)
        {
            _session = session;
            _resources = resources;
        }

        public bool CanEdit => _session.CanEdit;
        public IEnumerable<PlayerArmyModel> Armies => _session.Player?.GlobalMaps?.SelectMany(m => m.Armies).Where(a => a.Data.IsFactionCrusaders());
        public bool HasArmies => Armies?.Any() ?? false;

        public string FindLeaderPortrait(PlayerArmyModel unit)
        {
            if (string.IsNullOrEmpty(unit.Data.LeaderGuid)) return _resources.AppData.Portraits.GetUnknownUri();
            var leader = _session.Game.Management.Members.FirstOrDefault(l => l.UniqueId.Eq(unit.Data.LeaderGuid));
            if (leader == null) return _resources.AppData.Portraits.GetUnknownUri();
            return leader.PortraitPath;
        }
    }
}
