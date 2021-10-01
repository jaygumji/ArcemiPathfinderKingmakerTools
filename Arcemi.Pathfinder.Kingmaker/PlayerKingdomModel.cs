#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerKingdomModel : RefModel
    {
        public PlayerKingdomModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name
        {
            get => KingdomName;
            set {
                KingdomName = value;
                KingdomNameIsSet = true;
            }
        }

        public bool KingdomNameIsSet { get => A.Value<bool>(); set => A.Value(value); }
        public string KingdomName { get => A.Value<string>(); set => A.Value(value); }
        public string UnrestOnLastRavenVisit { get => A.Value<string>(); set => A.Value(value); }
        public int LastRavenVisitDay { get => A.Value<int>(); set => A.Value(value); }
        public int BPOnLastRavenVisit { get => A.Value<int>(); set => A.Value(value); }
        public int CurrentTurn { get => A.Value<int>(); set => A.Value(value); }
        public int BP { get => A.Value<int>(); set => A.Value(value); }
        public int BPPerTurn { get => A.Value<int>(); set => A.Value(value); }
        public int BPPrevious { get => A.Value<int>(); set => A.Value(value); }
        public int CurrentDay { get => A.Value<int>(); set => A.Value(value); }
        public int BPPerRegion { get => A.Value<int>(); set => A.Value(value); }
        public int BPPerUpgrade { get => A.Value<int>(); set => A.Value(value); }
        public int StartDay { get => A.Value<int>(); set => A.Value(value); }
        public string Alignment { get => A.Value<string>(); set => A.Value(value); }
        public string Unrest { get => A.Value<string>(); set => A.Value(value); }
        public bool Disabled { get => A.Value<bool>(); set => A.Value(value); }
        public PlayerKingdomMoraleStateModel MoraleState => A.Object(factory: a => new PlayerKingdomMoraleStateModel(a));

        public IReadOnlyList<string> AvailableNPCLeaders => A.ListValue<string>();

        public PlayerKingdomStatsModel Stats => A.Object(factory: a => new PlayerKingdomStatsModel(a));

        public IReadOnlyList<PlayerKingdomEventModel> ActiveEvents => A.List<PlayerKingdomEventModel>();
        public IReadOnlyList<PlayerKingdomEventHistoryModel> EventHistory => A.List<PlayerKingdomEventHistoryModel>();
        public IReadOnlyList<PlayerKingdomEventHistoryModel> FinishedEvents => A.List<PlayerKingdomEventHistoryModel>();

        public IReadOnlyList<PlayerKingdomLeaderModel> Leaders => A.List<PlayerKingdomLeaderModel>("m_Leaders");
        public PlayerKingdomResources Resources => A.Object(factory: a => new PlayerKingdomResources(a));
    }
}
