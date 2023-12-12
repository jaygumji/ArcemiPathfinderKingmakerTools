#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Models
{
    public class PlayerDifficultyValuesModel : RefModel
    {
        public PlayerDifficultyValuesModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public bool DeathDoorCondition { get => A.Value<bool>(); set => A.Value(value); }
        public bool TrueDeath { get => A.Value<bool>(); set => A.Value(value); }
        public bool RemoveNegativeEffectsOnRest { get => A.Value<bool>(); set => A.Value(value); }
        public bool EncumbranceSlowdown { get => A.Value<bool>(); set => A.Value(value); }
        public double DamageToParty { get => A.Value<double>(); set => A.Value(value); }
        public string CritsOnParty { get => A.Value<string>(); set => A.Value(value); }
        public string AutoLevelup { get => A.Value<string>(); set => A.Value(value); }
        public string StatsAdjustmentsType { get => A.Value<string>(); set => A.Value(value); }
        public string EnemyDifficulty { get => A.Value<string>(); set => A.Value(value); }
    }
}
