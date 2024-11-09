using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGamePartyModel : IGamePartyModel
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGamePartyModel(PlayerModel player, HeaderModel header)
        {
            Player = player;
            var A = player.GetAccessor();
            var difficulty = A.Object<RefModel>("MinDifficultyController")?.GetAccessor().Object<RefModel>("MinDifficulty")?.GetAccessor();
            Data = GameDataModels.Object(new IGameData[] {
                GameDataModels.Object("Party resources", new IGameData[] {
                    A.Object<W40KRTScrapResourceEntry>("Scrap"),
                    A.Object<W40KRTProfitFactorResourceEntry>("ProfitFactor"),
                    new W40KRTNavigatorResourceEntry(player.GetAccessor().Object<RefModel>("WarpTravelState")),
                    new W40KRTRespecsResourceEntry(player),
                }),
                GameDataModels.Object("Reputations", A.List<KeyValuePairModel<int>>("FractionsReputation").Where(x => !x.Key.IEq("None")).Select(x => new W40KRTGamePartyFactionResourceEntry(x)).ToArray()),
                GameDataModels.Object("DLC Rewards", new[] {
                    GameDataModels.RowList(header.GetAccessor().List<RefModel>("m_DlcRewards"), x => GameDataModels.Object(Res.Blueprints.GetNameOrBlueprint(x.GetAccessor().Value<string>("guid")), new IGameData[] {
                    }), writer: new W40KRTDlcRewardsCollectionWriter(player))
                }, isCollapsable: true),
                GameDataModels.Object("Difficulty", new IGameData[] {
                    GameDataModels.Options("Preset", W40KRTGameDifficulty.Presets, difficulty, x => x.Value<string>("GameDifficulty"), (x, v) => x.Value(v, "GameDifficulty"), GameDataSize.Medium),
                    GameDataModels.Boolean("Only one save", difficulty, x => x.Value<bool>("OnlyOneSave"), (x, v) => x.Value(v, "OnlyOneSave")),
                    GameDataModels.Separator("Progression", GameDataSeparatorType.Header),
                    GameDataModels.Boolean("Respec allowed", difficulty, x => x.Value<bool>("RespecAllowed"), (x, v) => x.Value(v, "RespecAllowed")),
                    GameDataModels.Options("Auto level up", W40KRTGameDifficulty.AutoLevelUp, difficulty, x => x.Value<string>("AutoLevelUp"), (x, v) => x.Value(v, "AutoLevelUp"), GameDataSize.Medium),
                    GameDataModels.Separator("Combat", GameDataSeparatorType.Header),
                    GameDataModels.Options("Capacity", W40KRTGameDifficulty.CombatEncountersCapacity, difficulty, x => x.Value<string>("CombatEncountersCapacity"), (x, v) => x.Value(v, "CombatEncountersCapacity"), GameDataSize.Medium),
                    GameDataModels.Integer("Cover hit bonus half mod (%)", difficulty, x => x.Value<int>("CoverHitBonusHalfModifier"), (x, v) => x.Value(v, "CoverHitBonusHalfModifier"), minValue: int.MinValue),
                    GameDataModels.Integer("Cover hit bonus full mod (%)", difficulty, x => x.Value<int>("CoverHitBonusFullModifier"), (x, v) => x.Value(v, "CoverHitBonusFullModifier"), minValue: int.MinValue),
                    GameDataModels.Integer("NPC characteristic mod (%)", difficulty, x => x.Value<int>("NPCAttributesBaseValuePercentModifier"), (x, v) => x.Value(v, "NPCAttributesBaseValuePercentModifier"), minValue: int.MinValue),
                    GameDataModels.Integer("Ally resolve mod (%)", difficulty, x => x.Value<int>("AllyResolveModifier"), (x, v) => x.Value(v, "AllyResolveModifier"), minValue: int.MinValue),
                    GameDataModels.Boolean("Additional AI behaviors", difficulty, x => x.Value<bool>("AdditionalAIBehaviors"), (x, v) => x.Value(v, "AdditionalAIBehaviors")),
                    GameDataModels.Separator("Party", GameDataSeparatorType.Header),
                    GameDataModels.Integer("Min damage", difficulty, x => x.Value<int>("MinPartyDamage"), (x, v) => x.Value(v, "MinPartyDamage"), minValue: int.MinValue),
                    GameDataModels.Integer("Min damage (%)", difficulty, x => x.Value<int>("MinPartyDamageFraction"), (x, v) => x.Value(v, "MinPartyDamageFraction"), minValue: int.MinValue),
                    GameDataModels.Integer("Min damage armor (%)", difficulty, x => x.Value<int>("PartyDamageDealtAfterArmorReductionPercentModifier"), (x, v) => x.Value(v, "PartyDamageDealtAfterArmorReductionPercentModifier"), minValue: int.MinValue),
                    GameDataModels.Options("Hard CC max rounds", W40KRTGameDifficulty.HardCrowdControlOnPartyMaxDurationRounds, difficulty, x => x.Value<string>("HardCrowdControlOnPartyMaxDurationRounds"), (x, v) => x.Value(v, "HardCrowdControlOnPartyMaxDurationRounds"), GameDataSize.Medium),
                    GameDataModels.Integer("Momentum mod (%)", difficulty, x => x.Value<int>("PartyMomentumPercentModifier"), (x, v) => x.Value(v, "PartyMomentumPercentModifier"), minValue: int.MinValue),
                    GameDataModels.Integer("Skill test mod (%)", difficulty, x => x.Value<int>("SkillCheckModifier"), (x, v) => x.Value(v, "SkillCheckModifier"), minValue: int.MinValue),
                    GameDataModels.Separator("Injuries", GameDataSeparatorType.Header),
                    GameDataModels.Integer("Damage threshold (%)", difficulty, x => x.Value<int>("WoundDamagePerTurnThresholdHPFraction"), (x, v) => x.Value(v, "WoundDamagePerTurnThresholdHPFraction"), minValue: int.MinValue),
                    GameDataModels.Integer("Old wound delay rounds", difficulty, x => x.Value<int>("OldWoundDelayRounds"), (x, v) => x.Value(v, "OldWoundDelayRounds"), minValue: int.MinValue),
                    GameDataModels.Integer("Injury to trauma stacks", difficulty, x => x.Value<int>("WoundStacksForTrauma"), (x, v) => x.Value(v, "WoundStacksForTrauma"), minValue: int.MinValue),
                    GameDataModels.Separator("Party starship", GameDataSeparatorType.Header),
                    GameDataModels.Options("Space combat difficulty", W40KRTGameDifficulty.SpaceCombat, difficulty, x => x.Value<string>("SpaceCombatDifficulty"), (x, v) => x.Value(v, "SpaceCombatDifficulty"), GameDataSize.Medium),
                    GameDataModels.Integer("Min damage", difficulty, x => x.Value<int>("MinPartyStarshipDamage"), (x, v) => x.Value(v, "MinPartyStarshipDamage"), minValue: int.MinValue),
                    GameDataModels.Integer("Min damage (%)", difficulty, x => x.Value<int>("MinPartyStarshipDamageFraction"), (x, v) => x.Value(v, "MinPartyStarshipDamageFraction"), minValue: int.MinValue),
                    GameDataModels.Separator("Enemy", GameDataSeparatorType.Header),
                    GameDataModels.Integer("Dodge mod (%)", difficulty, x => x.Value<int>("EnemyDodgePercentModifier"), (x, v) => x.Value(v, "EnemyDodgePercentModifier"), minValue: int.MinValue),
                    GameDataModels.Integer("Wound mod (%)", difficulty, x => x.Value<int>("EnemyHitPointsPercentModifier"), (x, v) => x.Value(v, "EnemyHitPointsPercentModifier"), minValue: int.MinValue),
                }, isCollapsable: true)
            });
        }

        public PlayerModel Player { get; }
        public IGameDataObject Data { get; }
        public bool IsSupported => true;
    }
}