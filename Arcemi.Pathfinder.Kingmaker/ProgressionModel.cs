#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ProgressionModel : RefModel
    {
        public ProgressionModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public IReadOnlyList<ClassModel> Classes => A.List(factory: a => new ClassModel(a));
        public FeaturesModel Features => A.Object(factory: a => new FeaturesModel(a));
        public int CharacterLevel { get => A.Value<int>(); set => A.Value(value); }
        public int Experience { get => A.Value<int>(); set => A.Value(value); }
        public string Race { get => A.Value<string>("m_Race"); set => A.Value(value, "m_Race"); }
        public string RaceName => Mappings.GetRaceName(Race);
    }
}