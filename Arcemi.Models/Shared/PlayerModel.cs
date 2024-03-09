#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class PlayerModel : RefModel
    {
        private TimeParts _gameTimeParts;

        public PlayerModel(ModelDataAccessor accessor) : base(accessor)
        {
            N.On(Kingdom, nameof(Kingdom.Disabled), nameof(IsKingdomEnabled));
        }

        public bool IsKingdomEnabled => !(Kingdom?.Disabled ?? true);

        public int Money { get => A.Value<int>(); set => A.Value(value); }
        public int Chapter { get => A.Value<int>(); set => A.Value(value); }
        public int RespecsUsed { get => A.Value<int>(); set => A.Value(value); }
        public TimeSpan GameTime { get => A.Value<TimeSpan>(); set => A.Value(value); }
        public TimeParts GameTimeParts => _gameTimeParts ?? (_gameTimeParts = new TimeParts(() => GameTime, v => GameTime = v));
        public TimeSpan RealTime { get => A.Value<TimeSpan>(); set => A.Value(value); }

        public PlayerDifficultyModel Difficulty => A.Object(factory: a => new PlayerDifficultyModel(a));
        public PlayerKingdomModel Kingdom => A.Object(factory: a => new PlayerKingdomModel(a));
        public PlayerLeadersManagerModel LeadersManager => A.Object("m_LeadersManager", a => new PlayerLeadersManagerModel(a));
        public PlayerCorruptionModel Corruption => A.Object(factory: a => new PlayerCorruptionModel(a));

        public IReadOnlyList<PlayerGlobalMapsModel> GlobalMaps => A.List("m_GlobalMaps", a => new PlayerGlobalMapsModel(a));

        public FlagsContainerModel UnlockableFlags => A.Object("m_UnlockableFlags", a => new FlagsContainerModel(a));
        public EtudesSystemModel EtudesSystem => A.Object(factory: a => new EtudesSystemModel(a));
        public QuestBookModel QuestBook => A.Object("m_QuestBook", a => new QuestBookModel(a));
        public VendorTablesModel SharedVendorTables => A.Object(factory: a => new VendorTablesModel(a));

        public InventoryModel SharedStash => A.Object<InventoryModel>();
        //public string MainCharacterId { get => A.Value<string>("m_MainCharacter"); set => A.Value(value, "m_MainCharacter"); }

        public SettingsListModel SettingsList => A.Object(factory: a => new SettingsListModel(a));

        public IGameTimeProvider GetGameTimeProvider()
        {
            return new StdGameTimeProvider(this);
        }
        private class StdGameTimeProvider : IGameTimeProvider {
            private readonly PlayerModel _ref;
            public StdGameTimeProvider(PlayerModel @ref) { _ref = @ref; }
            public TimeSpan Get() => _ref.GameTime;
        }
    }
}
