#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class CharacterAttributeModel : RefModel
    {
        public CharacterAttributeModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Name
        {
            get {
                if (Type == null) return null;
                if (Type.StartsWith("Skill", StringComparison.Ordinal)) {
                    return Type.Remove(0, 5).AsDisplayable();
                }
                return Type.AsDisplayable();
            }
        }
        public string Type { get => A.Value<string>(); set => A.Value(value); }
        public int BaseValue { get => A.Value<int>("m_BaseValue"); set => A.Value(value, "m_BaseValue"); }
        public int PermanentValue { get => A.Value<int>(); set => A.Value(value); }
        public int PairedValue { get => PermanentValue; set => PermanentValue = BaseValue = value; }
        public string ModifiersSum
        {
            get {
                var sum = PersistentModifiers.Sum(x => x.ModValue);
                if (sum < 0) return sum.ToString();
                if (sum > 0) return "+" + sum;
                return "";
            }
        }
        public string ModifiersDescription
        {
            get {
                if (PersistentModifiers?.Count > 0) {
                    var descriptions = PersistentModifiers
                        .Where(m => !string.IsNullOrEmpty(m.ModDescriptor))
                        .Select(m => m.ModDescriptor);
                    return string.Join(Environment.NewLine, descriptions);
                }
                return null;
            }
        }
        public ClassSkillModel ClassSkill => A.Object<ClassSkillModel>();
        public IReadOnlyList<PersistentModifierModel> PersistentModifiers => A.List<PersistentModifierModel>("PersistentModifierList");
    }
}