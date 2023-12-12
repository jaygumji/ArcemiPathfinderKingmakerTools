#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class ClassModel : RefModel
    {
        public ClassModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int Level { get => A.Value<int>(); set => A.Value(value); }
        public string CharacterClass => A.Value<string>();
        public IReadOnlyList<string> Archetypes => A.ListValue<string>();

        public bool IsMythic => GameDefinition.Pathfinder_WrathOfTheRighteous.Resources.IsMythicClass(CharacterClass);
        public bool IsMythicChampion => string.Equals(CharacterClass, "247aa787806d5da4f89cfc3dff0b217f", StringComparison.Ordinal);
    }
}