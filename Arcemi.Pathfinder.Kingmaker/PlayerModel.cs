#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerModel : RefModel
    {
        public PlayerModel(ModelDataAccessor accessor) : base(accessor)
        {
            N.On(Kingdom, nameof(Kingdom.Disabled), nameof(IsKingdomEnabled));
        }

        public bool IsKingdomEnabled => !(Kingdom?.Disabled ?? true);

        public int Money { get => A.Value<int>(); set => A.Value(value); }
        public int Chapter { get => A.Value<int>(); set => A.Value(value); }
        public TimeSpan GameTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public TimeSpan RealTime { get => A.Value<TimeSpan>(); set => A.Value(value); }

        public PlayerDifficultyModel Difficulty => A.Object(factory: a => new PlayerDifficultyModel(a));
        public PlayerKingdomModel Kingdom => A.Object(factory: a => new PlayerKingdomModel(a));
        public PlayerLeadersManagerModel LeadersManager => A.Object("m_LeadersManager", a => new PlayerLeadersManagerModel(a));
        public PlayerCorruptionModel Corruption => A.Object(factory: a => new PlayerCorruptionModel(a));

        public InventoryModel SharedStash => A.Object<InventoryModel>();
        public string MainCharacterId => A.Value<string>("m_MainCharacter");
    }
}
