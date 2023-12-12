#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class StatsModel : RefModel
    {
        public StatsModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public CharacterAttributeModel HitPoints => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel TemporaryHitPoints => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel AC => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel AdditionalAttackBonus => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel AdditionalDamage => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel BaseAttackBonus => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel AttackOfOpportunityCount => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Speed => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Charisma => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel AdditionalCMB => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel AdditionalCMD => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Constitution => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Dexterity => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Intelligence => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SaveFortitude => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SaveReflex => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SaveWill => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillMobility => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillAthletics => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillKnowledgeArcana => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillLoreNature => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillPerception => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillThievery => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Strength => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Wisdom => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Initiative => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillPersuasion => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillStealth => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillUseMagicDevice => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillLoreReligion => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SkillKnowledgeWorld => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel CheckBluff => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel CheckDiplomacy => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel CheckIntimidate => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel SneakAttack => A.Object<CharacterAttributeModel>();
        public CharacterAttributeModel Reach => A.Object<CharacterAttributeModel>();

        public IEnumerable<CharacterAttributeModel> General
        {
            get {
                return new[] { HitPoints, TemporaryHitPoints, Speed };
            }
        }

        public IEnumerable<CharacterAttributeModel> Saves
        {
            get {
                return new[] { SaveReflex, SaveFortitude, SaveWill };
            }
        }

        public IEnumerable<CharacterAttributeModel> Combat
        {
            get {
                return new[] { BaseAttackBonus, AdditionalAttackBonus, AdditionalDamage, SneakAttack, AttackOfOpportunityCount, Reach, AC, Initiative };
            }
        }

        public IEnumerable<CharacterAttributeModel> Attributes
        {
            get {
                return new[] { Strength, Dexterity, Constitution,
                    Intelligence, Wisdom, Charisma };
            }
        }

        public IEnumerable<CharacterAttributeModel> Skills
        {
            get {
                return new[] { SkillAthletics, SkillMobility, SkillThievery,
                    SkillStealth, SkillKnowledgeArcana, SkillKnowledgeWorld,
                    SkillLoreNature, SkillLoreReligion, SkillPerception,
                    SkillPersuasion, SkillUseMagicDevice };
            }
        }

    }
}