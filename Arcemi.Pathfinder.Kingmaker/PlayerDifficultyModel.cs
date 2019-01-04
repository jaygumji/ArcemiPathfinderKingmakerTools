#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
namespace Arcemi.Pathfinder.Kingmaker
{
    public class PlayerDifficultyModel : RefModel
    {
        public PlayerDifficultyModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string BlueprintOnStartGame { get => A.Value<string>(); set => A.Value(value); }

        public PlayerDifficultyValuesModel MinDifficulty => A.Object(factory: a => new PlayerDifficultyValuesModel(a));
    }
}
