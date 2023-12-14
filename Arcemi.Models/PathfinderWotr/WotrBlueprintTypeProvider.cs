using System;
using System.Collections.Generic;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrBlueprintTypeProvider : IBlueprintTypeProvider
    {
        public BlueprintType Get(BlueprintTypeId id)
        {
            return LookupId[id];
        }

        public BlueprintType Get(string fullName)
        {
            return LookupFullName.TryGetValue(fullName, out var type) ? type : new BlueprintType("<Unknown>", fullName);
        }

        public static BlueprintType Ability { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAbility");
        public static BlueprintType AbilityAreaEffect { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAbilityAreaEffect");
        public static BlueprintType AbilityResource { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintAbilityResource");
        public static BlueprintType AchievementData { get; } = new BlueprintType("", "Kingmaker.Achievements.Blueprints.AchievementData");
        public static BlueprintType ActivatableAbility { get; } = new BlueprintType("", "Kingmaker.UnitLogic.ActivatableAbilities.BlueprintActivatableAbility");
        public static BlueprintType ActiveCommandConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ActiveCommandConsideration");
        public static BlueprintType AiAttack { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiAttack");
        public static BlueprintType AiCastSpell { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiCastSpell");
        public static BlueprintType AiFollow { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiFollow");
        public static BlueprintType AiRoam { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiRoam");
        public static BlueprintType AiSwitchWeapon { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiSwitchWeapon");
        public static BlueprintType AiTouch { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiTouch");
        public static BlueprintType AlignmentConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.AlignmentConsideration");
        public static BlueprintType Answer { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer");
        public static BlueprintType AnswersList { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList");
        public static BlueprintType ArbiterInstruction { get; } = new BlueprintType("", "Kingmaker.QA.Arbiter.BlueprintArbiterInstruction");
        public static BlueprintType ArbiterRoot { get; } = new BlueprintType("", "Kingmaker.QA.Arbiter.BlueprintArbiterRoot");
        public static BlueprintType Archetype { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintArchetype");
        public static BlueprintType Area { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintArea");
        public static BlueprintType AreaEffectPitVisualSettings { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAreaEffectPitVisualSettings");
        public static BlueprintType AreaEnterPoint { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaEnterPoint");
        public static BlueprintType AreaMechanics { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaMechanics");
        public static BlueprintType AreaPart { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaPart");
        public static BlueprintType AreaPreset { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaPreset");
        public static BlueprintType AreaTransition { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaTransition");
        public static BlueprintType ArmorEnchantment { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.Ecnchantments.BlueprintArmorEnchantment");
        public static BlueprintType ArmorType { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.Armors.BlueprintArmorType");
        public static BlueprintType ArmorTypeConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ArmorTypeConsideration");
        public static BlueprintType ArmyHealthConsideration { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.Considerations.ArmyHealthConsideration");
        public static BlueprintType ArmyLeader { get; } = new BlueprintType("", "Kingmaker.Armies.BlueprintArmyLeader");
        public static BlueprintType ArmyPreset { get; } = new BlueprintType("", "Kingmaker.Armies.Blueprints.BlueprintArmyPreset");
        public static BlueprintType ArmyPresetList { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintArmyPresetList");
        public static BlueprintType ArmyRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.ArmyRoot");
        public static BlueprintType BarkBanter { get; } = new BlueprintType("", "Kingmaker.BarkBanters.BlueprintBarkBanter");
        public static BlueprintType BookPage { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintBookPage");
        public static BlueprintType Brain { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintBrain");
        public static BlueprintType Buff { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff");
        public static BlueprintType BuffConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.BuffConsideration");
        public static BlueprintType BuffNotFromCasterConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.BuffNotFromCasterConsideration");
        public static BlueprintType BuffsAroundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.BuffsAroundConsideration");
        public static BlueprintType Campaign { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.BlueprintCampaign");
        public static BlueprintType CampingEncounter { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.BlueprintCampingEncounter");
        public static BlueprintType CanMakeFullAttackConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CanMakeFullAttackConsideration");
        public static BlueprintType CanUseSpellCombatConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CanUseSpellCombatConsideration");
        public static BlueprintType CasterClassConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CasterClassConsideration");
        public static BlueprintType CastsGroup { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.Fx.CastsGroup");
        public static BlueprintType CategoryDefaults { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.Weapons.BlueprintCategoryDefaults");
        public static BlueprintType CharacterClass { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintCharacterClass");
        public static BlueprintType CharacterClassGroup { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintCharacterClassGroup");
        public static BlueprintType Check { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintCheck");
        public static BlueprintType ClassAdditionalVisualSettings { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintClassAdditionalVisualSettings");
        public static BlueprintType ClassAdditionalVisualSettingsProgression { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintClassAdditionalVisualSettingsProgression");
        public static BlueprintType ClockworkScenario { get; } = new BlueprintType("", "Kingmaker.QA.Clockwork.BlueprintClockworkScenario");
        public static BlueprintType ClockworkScenarioPart { get; } = new BlueprintType("", "Kingmaker.QA.Clockwork.BlueprintClockworkScenarioPart");
        public static BlueprintType CommandCooldownConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CommandCooldownConsideration");
        public static BlueprintType CompanionStory { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintCompanionStory");
        public static BlueprintType ComplexConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ComplexConsideration");
        public static BlueprintType ComponentList { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintComponentList");
        public static BlueprintType ConditionConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ConditionConsideration");
        public static BlueprintType ConsoleRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.ConsoleRoot");
        public static BlueprintType ControllableProjectile { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintControllableProjectile");
        public static BlueprintType CookingRecipe { get; } = new BlueprintType("", "Kingmaker.Controllers.Rest.Cooking.BlueprintCookingRecipe");
        public static BlueprintType CorruptionRoot { get; } = new BlueprintType("", "Kingmaker.Corruption.BlueprintCorruptionRoot");
        public static BlueprintType CraftRoot { get; } = new BlueprintType("", "Kingmaker.Craft.CraftRoot");
        public static BlueprintType CreditsGroup { get; } = new BlueprintType("", "Kingmaker.Blueprints.Credits.BlueprintCreditsGroup");
        public static BlueprintType CreditsRoles { get; } = new BlueprintType("", "Kingmaker.Blueprints.Credits.BlueprintCreditsRoles");
        public static BlueprintType CreditsTeams { get; } = new BlueprintType("", "Kingmaker.Blueprints.Credits.BlueprintCreditsTeams");
        public static BlueprintType CrusadeEvent { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintCrusadeEvent");
        public static BlueprintType CrusadeEventTimeline { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintCrusadeEventTimeline");
        public static BlueprintType Cue { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintCue");
        public static BlueprintType CueSequence { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintCueSequence");
        public static BlueprintType CustomAiConsiderationsRoot { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.CustomAiConsiderationsRoot");
        public static BlueprintType Cutscene { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Cutscenes.Cutscene");
        public static BlueprintType CutscenesRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.CutscenesRoot");
        public static BlueprintType Dialog { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintDialog");
        public static BlueprintType DialogExperienceModifierTable { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintDialogExperienceModifierTable");
        public static BlueprintType DirectionConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.DirectionConsideration");
        public static BlueprintType DistanceConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.DistanceConsideration");
        public static BlueprintType DistanceRangeConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.DistanceRangeConsideration");
        public static BlueprintType Dlc { get; } = new BlueprintType("", "Kingmaker.DLC.BlueprintDlc");
        public static BlueprintType DlcReward { get; } = new BlueprintType("", "Kingmaker.DLC.BlueprintDlcReward");
        public static BlueprintType DlcRewardCampaign { get; } = new BlueprintType("", "Kingmaker.DLC.BlueprintDlcRewardCampaign");
        public static BlueprintType DungeonArmy { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonArmy");
        public static BlueprintType DungeonBoon { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonBoon");
        public static BlueprintType DungeonCampaign { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonCampaign");
        public static BlueprintType DungeonDifficultyCurve { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonDifficultyCurve");
        public static BlueprintType DungeonExpedition { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonExpedition");
        public static BlueprintType DungeonIsland { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonIsland");
        public static BlueprintType DungeonIslandReward { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonIslandReward");
        public static BlueprintType DungeonIslandRewardGold { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonIslandRewardGold");
        public static BlueprintType DungeonIslandRewardLoot { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonIslandRewardLoot");
        public static BlueprintType DungeonIslandRewardObject { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonIslandRewardObject");
        public static BlueprintType DungeonIslandRewardUnit { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonIslandRewardUnit");
        public static BlueprintType DungeonLocalizedStrings { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonLocalizedStrings");
        public static BlueprintType DungeonLoot { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonLoot");
        public static BlueprintType DungeonLootBudget { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonLootBudget");
        public static BlueprintType DungeonMap { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonMap");
        public static BlueprintType DungeonModificator { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonModificator");
        public static BlueprintType DungeonRoot { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonRoot");
        public static BlueprintType DungeonTheme { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonTheme");
        public static BlueprintType DungeonTier { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonTier");
        public static BlueprintType DungeonTrapSpellList { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonTrapSpellList");
        public static BlueprintType DynamicMapObject { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintDynamicMapObject");
        public static BlueprintType EncyclopediaChapter { get; } = new BlueprintType("", "Kingmaker.Blueprints.Encyclopedia.BlueprintEncyclopediaChapter");
        public static BlueprintType EncyclopediaPage { get; } = new BlueprintType("", "Kingmaker.Blueprints.Encyclopedia.BlueprintEncyclopediaPage");
        public static BlueprintType EquipmentEnchantment { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.Ecnchantments.BlueprintEquipmentEnchantment");
        public static BlueprintType Etude { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Etudes.BlueprintEtude");
        public static BlueprintType EtudeConflictingGroup { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Etudes.BlueprintEtudeConflictingGroup");
        public static BlueprintType FactConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.FactConsideration");
        public static BlueprintType Faction { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintFaction");
        public static BlueprintType Feature { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintFeature");
        public static BlueprintType FeatureReplaceSpellbook { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintFeatureReplaceSpellbook");
        public static BlueprintType FeatureSelection { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Selection.BlueprintFeatureSelection");
        public static BlueprintType FeatureSelectMythicSpellbook { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintFeatureSelectMythicSpellbook");
        public static BlueprintType FollowersFormation { get; } = new BlueprintType("", "Kingmaker.Formations.FollowersFormation");
        public static BlueprintType Footprint { get; } = new BlueprintType("", "Kingmaker.Blueprints.Footrprints.BlueprintFootprint");
        public static BlueprintType FootprintType { get; } = new BlueprintType("", "Kingmaker.Blueprints.Footrprints.BlueprintFootprintType");
        public static BlueprintType FormationsRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.FormationsRoot");
        public static BlueprintType FxRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.Fx.FxRoot");
        public static BlueprintType GamePadTexts { get; } = new BlueprintType("", "Kingmaker.Blueprints.Console.GamePadTexts");
        public static BlueprintType Gate { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Cutscenes.Gate");
        public static BlueprintType GlobalMagicSpell { get; } = new BlueprintType("", "Kingmaker.Crusade.GlobalMagic.BlueprintGlobalMagicSpell");
        public static BlueprintType GlobalMap { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMap");
        public static BlueprintType GlobalMapEdge { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMapEdge");
        public static BlueprintType GlobalMapPoint { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMapPoint");
        public static BlueprintType GlobalMapPointVariation { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMapPointVariation");
        public static BlueprintType HasAutoCastConsideraion { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HasAutoCastConsideraion");
        public static BlueprintType HasManualTargetConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HasManualTargetConsideration");
        public static BlueprintType HealthAroundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HealthAroundConsideration");
        public static BlueprintType HealthConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HealthConsideration");
        public static BlueprintType HiddenItem { get; } = new BlueprintType("Hidden", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintHiddenItem");
        public static BlueprintType HitSystemRoot { get; } = new BlueprintType("", "Kingmaker.Visual.HitSystem.HitSystemRoot");
        public static BlueprintType HitThisRoundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HitThisRoundConsideration");
        public static BlueprintType Ingredient { get; } = new BlueprintType("Ingredient", BlueprintTypeCategory.Item, "Kingmaker.Craft.BlueprintIngredient");
        public static BlueprintType InRangeConsideration { get; } = new BlueprintType("In Range Consideration", "Kingmaker.AI.Blueprints.Considerations.InRangeConsideration");
        public static BlueprintType InteractionRoot { get; } = new BlueprintType("Interaction Root", "Kingmaker.Interaction.BlueprintInteractionRoot");
        public static BlueprintType Item { get; } = new BlueprintType("Other", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItem");
        public static BlueprintType ItemArmor { get; } = new BlueprintType("Armor", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Armors.BlueprintItemArmor");
        public static BlueprintType ItemEquipmentBelt { get; } = new BlueprintType("Belt", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentBelt");
        public static BlueprintType ItemEquipmentFeet { get; } = new BlueprintType("Feet", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentFeet");
        public static BlueprintType ItemEquipmentGlasses { get; } = new BlueprintType("Glasses", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentGlasses");
        public static BlueprintType ItemEquipmentGloves { get; } = new BlueprintType("Gloves", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentGloves");
        public static BlueprintType ItemEquipmentHead { get; } = new BlueprintType("Head", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentHead");
        public static BlueprintType ItemEquipmentNeck { get; } = new BlueprintType("Neck", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentNeck");
        public static BlueprintType ItemEquipmentRing { get; } = new BlueprintType("Ring", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentRing");
        public static BlueprintType ItemEquipmentShirt { get; } = new BlueprintType("Shirt", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentShirt");
        public static BlueprintType ItemEquipmentShoulders { get; } = new BlueprintType("Shoulders", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentShoulders");
        public static BlueprintType ItemEquipmentUsable { get; } = new BlueprintType("Usable", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentUsable");
        public static BlueprintType ItemEquipmentWrist { get; } = new BlueprintType("Wrist", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Equipment.BlueprintItemEquipmentWrist");
        public static BlueprintType ItemKey { get; } = new BlueprintType("Key", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItemKey");
        public static BlueprintType ItemNote { get; } = new BlueprintType("Note", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItemNote");
        public static BlueprintType ItemShield { get; } = new BlueprintType("Shield", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Shields.BlueprintItemShield");
        public static BlueprintType ItemsList { get; } = new BlueprintType("Items List", "Kingmaker.Blueprints.Items.BlueprintItemsList");
        public static BlueprintType ItemThiefTool { get; } = new BlueprintType("Thief Tool", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItemThiefTool");
        public static BlueprintType ItemWeapon { get; } = new BlueprintType("Weapon", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Weapons.BlueprintItemWeapon");
        public static BlueprintType KingdomBuff { get; } = new BlueprintType("Buff", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomBuff");
        public static BlueprintType KingdomDeck { get; } = new BlueprintType("Deck", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomDeck");
        public static BlueprintType KingdomEvent { get; } = new BlueprintType("Event", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomEvent");
        public static BlueprintType KingdomEventTimeline { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomEventTimeline");
        public static BlueprintType KingdomMoraleFlag { get; } = new BlueprintType("", "Kingmaker.Kingdom.Flags.BlueprintKingdomMoraleFlag");
        public static BlueprintType KingdomProject { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomProject");
        public static BlueprintType KingdomRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.KingdomRoot");
        public static BlueprintType KingdomUIRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.KingdomUIRoot");
        public static BlueprintType KingmakerEquipmentEntity { get; } = new BlueprintType("", "Kingmaker.Visual.CharacterSystem.KingmakerEquipmentEntity");
        public static BlueprintType LastTargetConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.LastTargetConsideration");
        public static BlueprintType LeaderProgression { get; } = new BlueprintType("", "Kingmaker.Armies.BlueprintLeaderProgression");
        public static BlueprintType LeaderSkill { get; } = new BlueprintType("", "Kingmaker.Armies.Blueprints.BlueprintLeaderSkill");
        public static BlueprintType LeaderSkillsList { get; } = new BlueprintType("", "Kingmaker.Armies.BlueprintLeaderSkillsList");
        public static BlueprintType LeadersRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.LeadersRoot");
        public static BlueprintType LevelUpPlanFeaturesList { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintLevelUpPlanFeaturesList");
        public static BlueprintType LifeStateConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.LifeStateConsideration");
        public static BlueprintType LineOfSightConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.LineOfSightConsideration");
        public static BlueprintType LoadingScreenSpriteList { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintLoadingScreenSpriteList");
        public static BlueprintType LogicConnector { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintLogicConnector");
        public static BlueprintType Loot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Loot.BlueprintLoot");
        public static BlueprintType ManualTargetConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ManualTargetConsideration");
        public static BlueprintType MoraleRoot { get; } = new BlueprintType("", "Kingmaker.Armies.Blueprints.MoraleRoot");
        public static BlueprintType MultiEntrance { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintMultiEntrance");
        public static BlueprintType MultiEntranceEntry { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintMultiEntranceEntry");
        public static BlueprintType MythicInfo { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintMythicInfo");
        public static BlueprintType MythicsSettings { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintMythicsSettings");
        public static BlueprintType NotImpatientConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.NotImpatientConsideration");
        public static BlueprintType ParametrizedFeature { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Selection.BlueprintParametrizedFeature");
        public static BlueprintType PartyFormation { get; } = new BlueprintType("", "Kingmaker.Formations.BlueprintPartyFormation");
        public static BlueprintType PersonageLimits { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintPersonageLimits");
        public static BlueprintType PhotoModeRoot { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintPhotoModeRoot");
        public static BlueprintType Portrait { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintPortrait");
        public static BlueprintType Progression { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintProgression");
        public static BlueprintType Projectile { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintProjectile");
        public static BlueprintType ProjectileTrajectory { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintProjectileTrajectory");
        public static BlueprintType Quest { get; } = new BlueprintType("", "Kingmaker.Blueprints.Quests.BlueprintQuest");
        public static BlueprintType QuestGroups { get; } = new BlueprintType("", "Kingmaker.Blueprints.Quests.BlueprintQuestGroups");
        public static BlueprintType QuestObjective { get; } = new BlueprintType("", "Kingmaker.Blueprints.Quests.BlueprintQuestObjective");
        public static BlueprintType Race { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintRace");
        public static BlueprintType RaceGenderDistribution { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Customization.RaceGenderDistribution");
        public static BlueprintType RaceVisualPreset { get; } = new BlueprintType("", "Kingmaker.Blueprints.CharGen.BlueprintRaceVisualPreset");
        public static BlueprintType RandomConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.RandomConsideration");
        public static BlueprintType RandomEncounter { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.BlueprintRandomEncounter");
        public static BlueprintType RandomEncountersRoot { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.RandomEncountersRoot");
        public static BlueprintType Region { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintRegion");
        public static BlueprintType RomanceCounter { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintRomanceCounter");
        public static BlueprintType Root { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.BlueprintRoot");
        public static BlueprintType ScriptZone { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintScriptZone");
        public static BlueprintType SequenceExit { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintSequenceExit");
        public static BlueprintType Settlement { get; } = new BlueprintType("", "Kingmaker.Kingdom.BlueprintSettlement");
        public static BlueprintType SettlementAreaPreset { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintSettlementAreaPreset");
        public static BlueprintType SettlementBlueprintArea { get; } = new BlueprintType("", "Kingmaker.Crusade.SettlementBlueprintArea");
        public static BlueprintType SettlementBuilding { get; } = new BlueprintType("", "Kingmaker.Kingdom.Settlements.BlueprintSettlementBuilding");
        public static BlueprintType SettlementBuildList { get; } = new BlueprintType("", "Kingmaker.Kingdom.AI.SettlementBuildList");
        public static BlueprintType SharedVendorTable { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.BlueprintSharedVendorTable");
        public static BlueprintType ShieldType { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.Armors.BlueprintShieldType");
        public static BlueprintType SpawnableObject { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.BlueprintSpawnableObject");
        public static BlueprintType Spellbook { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Spells.BlueprintSpellbook");
        public static BlueprintType SpellList { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Spells.BlueprintSpellList");
        public static BlueprintType SpellSchoolRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.SpellSchoolRoot");
        public static BlueprintType SpellsTable { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Spells.BlueprintSpellsTable");
        public static BlueprintType StatConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.StatConsideration");
        public static BlueprintType StatProgression { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintStatProgression");
        public static BlueprintType SummonPool { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintSummonPool");
        public static BlueprintType SwarmTargetsConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.SwarmTargetsConsideration");
        public static BlueprintType TacticalCombatAiAttack { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.BlueprintTacticalCombatAiAttack");
        public static BlueprintType TacticalCombatAiCastSpell { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.BlueprintTacticalCombatAiCastSpell");
        public static BlueprintType TacticalCombatArea { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Blueprints.BlueprintTacticalCombatArea");
        public static BlueprintType TacticalCombatBrain { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.BlueprintTacticalCombatBrain");
        public static BlueprintType TacticalCombatCanAttackThisTurnConsideration { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.Considerations.TacticalCombatCanAttackThisTurnConsideration");
        public static BlueprintType TacticalCombatObstaclesMap { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Blueprints.BlueprintTacticalCombatObstaclesMap");
        public static BlueprintType TacticalCombatRoot { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Blueprints.BlueprintTacticalCombatRoot");
        public static BlueprintType TacticalCombatTagConsideration { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.Considerations.TacticalCombatTagConsideration");
        public static BlueprintType TargetClassConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.TargetClassConsideration");
        public static BlueprintType TargetMainCharacter { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.TargetMainCharacter");
        public static BlueprintType TargetSelfConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.TargetSelfConsideration");
        public static BlueprintType ThreatedByConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ThreatedByConsideration");
        public static BlueprintType TimeOfDaySettings { get; } = new BlueprintType("", "Kingmaker.Visual.LightSelector.BlueprintTimeOfDaySettings");
        public static BlueprintType Trap { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintTrap");
        public static BlueprintType TrapSettings { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintTrapSettings");
        public static BlueprintType TrapSettingsRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintTrapSettingsRoot");
        public static BlueprintType TrashLootSettings { get; } = new BlueprintType("", "Kingmaker.Blueprints.Loot.TrashLootSettings");
        public static BlueprintType Tutorial { get; } = new BlueprintType("", "Kingmaker.Tutorial.BlueprintTutorial");
        public static BlueprintType UIInteractionTypeSprites { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUIInteractionTypeSprites");
        public static BlueprintType UISound { get; } = new BlueprintType("", "Kingmaker.UI.BlueprintUISound");
        public static BlueprintType Unit { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnit");
        public static BlueprintType UnitAnimationActionSubstitution { get; } = new BlueprintType("", "Kingmaker.Blueprints.UnitAnimationActionSubstitution");
        public static BlueprintType UnitAsksList { get; } = new BlueprintType("", "Kingmaker.Visual.Sound.BlueprintUnitAsksList");
        public static BlueprintType UnitConditionConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.UnitConditionConsideration");
        public static BlueprintType UnitCustomizationPreset { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Customization.UnitCustomizationPreset");
        public static BlueprintType UnitFact { get; } = new BlueprintType("", "Kingmaker.Blueprints.Facts.BlueprintUnitFact");
        public static BlueprintType UnitLoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Loot.BlueprintUnitLoot");
        public static BlueprintType UnitProperty { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Mechanics.Properties.BlueprintUnitProperty");
        public static BlueprintType UnitsAroundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.UnitsAroundConsideration");
        public static BlueprintType UnitsThreateningConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.UnitsThreateningConsideration");
        public static BlueprintType UnitTemplate { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnitTemplate");
        public static BlueprintType UnitType { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnitType");
        public static BlueprintType UnitUpgrader { get; } = new BlueprintType("", "Kingmaker.EntitySystem.Persistence.Versioning.BlueprintUnitUpgrader");
        public static BlueprintType UnlockableFlag { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnlockableFlag");
        public static BlueprintType WeaponEnchantment { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.Ecnchantments.BlueprintWeaponEnchantment");
        public static BlueprintType WeaponType { get; } = new BlueprintType("", "Kingmaker.Blueprints.Items.Weapons.BlueprintWeaponType");

        private static readonly Dictionary<string, BlueprintType> LookupFullName = new Dictionary<string, BlueprintType>(StringComparer.Ordinal) {
            {Ability.FullName, Ability},
            {AbilityAreaEffect.FullName, AbilityAreaEffect},
            {AbilityResource.FullName, AbilityResource},
            {AchievementData.FullName, AchievementData},
            {ActivatableAbility.FullName, ActivatableAbility},
            {ActiveCommandConsideration.FullName, ActiveCommandConsideration},
            {AiAttack.FullName, AiAttack},
            {AiCastSpell.FullName, AiCastSpell},
            {AiFollow.FullName, AiFollow},
            {AiRoam.FullName, AiRoam},
            {AiSwitchWeapon.FullName, AiSwitchWeapon},
            {AiTouch.FullName, AiTouch},
            {AlignmentConsideration.FullName, AlignmentConsideration},
            {Answer.FullName, Answer},
            {AnswersList.FullName, AnswersList},
            {ArbiterInstruction.FullName, ArbiterInstruction},
            {ArbiterRoot.FullName, ArbiterRoot},
            {Archetype.FullName, Archetype},
            {Area.FullName, Area},
            {AreaEffectPitVisualSettings.FullName, AreaEffectPitVisualSettings},
            {AreaEnterPoint.FullName, AreaEnterPoint},
            {AreaMechanics.FullName, AreaMechanics},
            {AreaPart.FullName, AreaPart},
            {AreaPreset.FullName, AreaPreset},
            {AreaTransition.FullName, AreaTransition},
            {ArmorEnchantment.FullName, ArmorEnchantment},
            {ArmorType.FullName, ArmorType},
            {ArmorTypeConsideration.FullName, ArmorTypeConsideration},
            {ArmyHealthConsideration.FullName, ArmyHealthConsideration},
            {ArmyLeader.FullName, ArmyLeader},
            {ArmyPreset.FullName, ArmyPreset},
            {ArmyPresetList.FullName, ArmyPresetList},
            {ArmyRoot.FullName, ArmyRoot},
            {BarkBanter.FullName, BarkBanter},
            {BookPage.FullName, BookPage},
            {Brain.FullName, Brain},
            {Buff.FullName, Buff},
            {BuffConsideration.FullName, BuffConsideration},
            {BuffNotFromCasterConsideration.FullName, BuffNotFromCasterConsideration},
            {BuffsAroundConsideration.FullName, BuffsAroundConsideration},
            {Campaign.FullName, Campaign},
            {CampingEncounter.FullName, CampingEncounter},
            {CanMakeFullAttackConsideration.FullName, CanMakeFullAttackConsideration},
            {CanUseSpellCombatConsideration.FullName, CanUseSpellCombatConsideration},
            {CasterClassConsideration.FullName, CasterClassConsideration},
            {CastsGroup.FullName, CastsGroup},
            {CategoryDefaults.FullName, CategoryDefaults},
            {CharacterClass.FullName, CharacterClass},
            {CharacterClassGroup.FullName, CharacterClassGroup},
            {Check.FullName, Check},
            {ClassAdditionalVisualSettings.FullName, ClassAdditionalVisualSettings},
            {ClassAdditionalVisualSettingsProgression.FullName, ClassAdditionalVisualSettingsProgression},
            {ClockworkScenario.FullName, ClockworkScenario},
            {ClockworkScenarioPart.FullName, ClockworkScenarioPart},
            {CommandCooldownConsideration.FullName, CommandCooldownConsideration},
            {CompanionStory.FullName, CompanionStory},
            {ComplexConsideration.FullName, ComplexConsideration},
            {ComponentList.FullName, ComponentList},
            {ConditionConsideration.FullName, ConditionConsideration},
            {ConsoleRoot.FullName, ConsoleRoot},
            {ControllableProjectile.FullName, ControllableProjectile},
            {CookingRecipe.FullName, CookingRecipe},
            {CorruptionRoot.FullName, CorruptionRoot},
            {CraftRoot.FullName, CraftRoot},
            {CreditsGroup.FullName, CreditsGroup},
            {CreditsRoles.FullName, CreditsRoles},
            {CreditsTeams.FullName, CreditsTeams},
            {CrusadeEvent.FullName, CrusadeEvent},
            {CrusadeEventTimeline.FullName, CrusadeEventTimeline},
            {Cue.FullName, Cue},
            {CueSequence.FullName, CueSequence},
            {CustomAiConsiderationsRoot.FullName, CustomAiConsiderationsRoot},
            {Cutscene.FullName, Cutscene},
            {CutscenesRoot.FullName, CutscenesRoot},
            {Dialog.FullName, Dialog},
            {DialogExperienceModifierTable.FullName, DialogExperienceModifierTable},
            {DirectionConsideration.FullName, DirectionConsideration},
            {DistanceConsideration.FullName, DistanceConsideration},
            {DistanceRangeConsideration.FullName, DistanceRangeConsideration},
            {Dlc.FullName, Dlc},
            {DlcReward.FullName, DlcReward},
            {DlcRewardCampaign.FullName, DlcRewardCampaign},
            {DungeonArmy.FullName, DungeonArmy},
            {DungeonBoon.FullName, DungeonBoon},
            {DungeonCampaign.FullName, DungeonCampaign},
            {DungeonDifficultyCurve.FullName, DungeonDifficultyCurve},
            {DungeonExpedition.FullName, DungeonExpedition},
            {DungeonIsland.FullName, DungeonIsland},
            {DungeonIslandReward.FullName, DungeonIslandReward},
            {DungeonIslandRewardGold.FullName, DungeonIslandRewardGold},
            {DungeonIslandRewardLoot.FullName, DungeonIslandRewardLoot},
            {DungeonIslandRewardObject.FullName, DungeonIslandRewardObject},
            {DungeonIslandRewardUnit.FullName, DungeonIslandRewardUnit},
            {DungeonLocalizedStrings.FullName, DungeonLocalizedStrings},
            {DungeonLoot.FullName, DungeonLoot},
            {DungeonLootBudget.FullName, DungeonLootBudget},
            {DungeonMap.FullName, DungeonMap},
            {DungeonModificator.FullName, DungeonModificator},
            {DungeonRoot.FullName, DungeonRoot},
            {DungeonTheme.FullName, DungeonTheme},
            {DungeonTier.FullName, DungeonTier},
            {DungeonTrapSpellList.FullName, DungeonTrapSpellList},
            {DynamicMapObject.FullName, DynamicMapObject},
            {EncyclopediaChapter.FullName, EncyclopediaChapter},
            {EncyclopediaPage.FullName, EncyclopediaPage},
            {EquipmentEnchantment.FullName, EquipmentEnchantment},
            {Etude.FullName, Etude},
            {EtudeConflictingGroup.FullName, EtudeConflictingGroup},
            {FactConsideration.FullName, FactConsideration},
            {Faction.FullName, Faction},
            {Feature.FullName, Feature},
            {FeatureReplaceSpellbook.FullName, FeatureReplaceSpellbook},
            {FeatureSelection.FullName, FeatureSelection},
            {FeatureSelectMythicSpellbook.FullName, FeatureSelectMythicSpellbook},
            {FollowersFormation.FullName, FollowersFormation},
            {Footprint.FullName, Footprint},
            {FootprintType.FullName, FootprintType},
            {FormationsRoot.FullName, FormationsRoot},
            {FxRoot.FullName, FxRoot},
            {GamePadTexts.FullName, GamePadTexts},
            {Gate.FullName, Gate},
            {GlobalMagicSpell.FullName, GlobalMagicSpell},
            {GlobalMap.FullName, GlobalMap},
            {GlobalMapEdge.FullName, GlobalMapEdge},
            {GlobalMapPoint.FullName, GlobalMapPoint},
            {GlobalMapPointVariation.FullName, GlobalMapPointVariation},
            {HasAutoCastConsideraion.FullName, HasAutoCastConsideraion},
            {HasManualTargetConsideration.FullName, HasManualTargetConsideration},
            {HealthAroundConsideration.FullName, HealthAroundConsideration},
            {HealthConsideration.FullName, HealthConsideration},
            {HiddenItem.FullName, HiddenItem},
            {HitSystemRoot.FullName, HitSystemRoot},
            {HitThisRoundConsideration.FullName, HitThisRoundConsideration},
            {Ingredient.FullName, Ingredient},
            {InRangeConsideration.FullName, InRangeConsideration},
            {InteractionRoot.FullName, InteractionRoot},
            {Item.FullName, Item},
            {ItemArmor.FullName, ItemArmor},
            {ItemEquipmentBelt.FullName, ItemEquipmentBelt},
            {ItemEquipmentFeet.FullName, ItemEquipmentFeet},
            {ItemEquipmentGlasses.FullName, ItemEquipmentGlasses},
            {ItemEquipmentGloves.FullName, ItemEquipmentGloves},
            {ItemEquipmentHead.FullName, ItemEquipmentHead},
            {ItemEquipmentNeck.FullName, ItemEquipmentNeck},
            {ItemEquipmentRing.FullName, ItemEquipmentRing},
            {ItemEquipmentShirt.FullName, ItemEquipmentShirt},
            {ItemEquipmentShoulders.FullName, ItemEquipmentShoulders},
            {ItemEquipmentUsable.FullName, ItemEquipmentUsable},
            {ItemEquipmentWrist.FullName, ItemEquipmentWrist},
            {ItemKey.FullName, ItemKey},
            {ItemNote.FullName, ItemNote},
            {ItemShield.FullName, ItemShield},
            {ItemsList.FullName, ItemsList},
            {ItemThiefTool.FullName, ItemThiefTool},
            {ItemWeapon.FullName, ItemWeapon},
            {KingdomBuff.FullName, KingdomBuff},
            {KingdomDeck.FullName, KingdomDeck},
            {KingdomEvent.FullName, KingdomEvent},
            {KingdomEventTimeline.FullName, KingdomEventTimeline},
            {KingdomMoraleFlag.FullName, KingdomMoraleFlag},
            {KingdomProject.FullName, KingdomProject},
            {KingdomRoot.FullName, KingdomRoot},
            {KingdomUIRoot.FullName, KingdomUIRoot},
            {KingmakerEquipmentEntity.FullName, KingmakerEquipmentEntity},
            {LastTargetConsideration.FullName, LastTargetConsideration},
            {LeaderProgression.FullName, LeaderProgression},
            {LeaderSkill.FullName, LeaderSkill},
            {LeaderSkillsList.FullName, LeaderSkillsList},
            {LeadersRoot.FullName, LeadersRoot},
            {LevelUpPlanFeaturesList.FullName, LevelUpPlanFeaturesList},
            {LifeStateConsideration.FullName, LifeStateConsideration},
            {LineOfSightConsideration.FullName, LineOfSightConsideration},
            {LoadingScreenSpriteList.FullName, LoadingScreenSpriteList},
            {LogicConnector.FullName, LogicConnector},
            {Loot.FullName, Loot},
            {ManualTargetConsideration.FullName, ManualTargetConsideration},
            {MoraleRoot.FullName, MoraleRoot},
            {MultiEntrance.FullName, MultiEntrance},
            {MultiEntranceEntry.FullName, MultiEntranceEntry},
            {MythicInfo.FullName, MythicInfo},
            {MythicsSettings.FullName, MythicsSettings},
            {NotImpatientConsideration.FullName, NotImpatientConsideration},
            {ParametrizedFeature.FullName, ParametrizedFeature},
            {PartyFormation.FullName, PartyFormation},
            {PersonageLimits.FullName, PersonageLimits},
            {PhotoModeRoot.FullName, PhotoModeRoot},
            {Portrait.FullName, Portrait},
            {Progression.FullName, Progression},
            {Projectile.FullName, Projectile},
            {ProjectileTrajectory.FullName, ProjectileTrajectory},
            {Quest.FullName, Quest},
            {QuestGroups.FullName, QuestGroups},
            {QuestObjective.FullName, QuestObjective},
            {Race.FullName, Race},
            {RaceGenderDistribution.FullName, RaceGenderDistribution},
            {RaceVisualPreset.FullName, RaceVisualPreset},
            {RandomConsideration.FullName, RandomConsideration},
            {RandomEncounter.FullName, RandomEncounter},
            {RandomEncountersRoot.FullName, RandomEncountersRoot},
            {Region.FullName, Region},
            {RomanceCounter.FullName, RomanceCounter},
            {Root.FullName, Root},
            {ScriptZone.FullName, ScriptZone},
            {SequenceExit.FullName, SequenceExit},
            {Settlement.FullName, Settlement},
            {SettlementAreaPreset.FullName, SettlementAreaPreset},
            {SettlementBlueprintArea.FullName, SettlementBlueprintArea},
            {SettlementBuilding.FullName, SettlementBuilding},
            {SettlementBuildList.FullName, SettlementBuildList},
            {SharedVendorTable.FullName, SharedVendorTable},
            {ShieldType.FullName, ShieldType},
            {SpawnableObject.FullName, SpawnableObject},
            {Spellbook.FullName, Spellbook},
            {SpellList.FullName, SpellList},
            {SpellSchoolRoot.FullName, SpellSchoolRoot},
            {SpellsTable.FullName, SpellsTable},
            {StatConsideration.FullName, StatConsideration},
            {StatProgression.FullName, StatProgression},
            {SummonPool.FullName, SummonPool},
            {SwarmTargetsConsideration.FullName, SwarmTargetsConsideration},
            {TacticalCombatAiAttack.FullName, TacticalCombatAiAttack},
            {TacticalCombatAiCastSpell.FullName, TacticalCombatAiCastSpell},
            {TacticalCombatArea.FullName, TacticalCombatArea},
            {TacticalCombatBrain.FullName, TacticalCombatBrain},
            {TacticalCombatCanAttackThisTurnConsideration.FullName, TacticalCombatCanAttackThisTurnConsideration},
            {TacticalCombatObstaclesMap.FullName, TacticalCombatObstaclesMap},
            {TacticalCombatRoot.FullName, TacticalCombatRoot},
            {TacticalCombatTagConsideration.FullName, TacticalCombatTagConsideration},
            {TargetClassConsideration.FullName, TargetClassConsideration},
            {TargetMainCharacter.FullName, TargetMainCharacter},
            {TargetSelfConsideration.FullName, TargetSelfConsideration},
            {ThreatedByConsideration.FullName, ThreatedByConsideration},
            {TimeOfDaySettings.FullName, TimeOfDaySettings},
            {Trap.FullName, Trap},
            {TrapSettings.FullName, TrapSettings},
            {TrapSettingsRoot.FullName, TrapSettingsRoot},
            {TrashLootSettings.FullName, TrashLootSettings},
            {Tutorial.FullName, Tutorial},
            {UIInteractionTypeSprites.FullName, UIInteractionTypeSprites},
            {UISound.FullName, UISound},
            {Unit.FullName, Unit},
            {UnitAnimationActionSubstitution.FullName, UnitAnimationActionSubstitution},
            {UnitAsksList.FullName, UnitAsksList},
            {UnitConditionConsideration.FullName, UnitConditionConsideration},
            {UnitCustomizationPreset.FullName, UnitCustomizationPreset},
            {UnitFact.FullName, UnitFact},
            {UnitLoot.FullName, UnitLoot},
            {UnitProperty.FullName, UnitProperty},
            {UnitsAroundConsideration.FullName, UnitsAroundConsideration},
            {UnitsThreateningConsideration.FullName, UnitsThreateningConsideration},
            {UnitTemplate.FullName, UnitTemplate},
            {UnitType.FullName, UnitType},
            {UnitUpgrader.FullName, UnitUpgrader},
            {UnlockableFlag.FullName, UnlockableFlag},
            {WeaponEnchantment.FullName, WeaponEnchantment},
            {WeaponType.FullName, WeaponType},
        };

        private static readonly Dictionary<BlueprintTypeId, BlueprintType> LookupId = new Dictionary<BlueprintTypeId, BlueprintType> {
            {BlueprintTypeId.UnitAsksList, UnitAsksList},
            {BlueprintTypeId.Portrait, Portrait},
            {BlueprintTypeId.Ability, Ability},
            {BlueprintTypeId.Item, Item},
            {BlueprintTypeId.ItemEquipmentHead, ItemEquipmentHead},
            {BlueprintTypeId.ItemEquipmentNeck, ItemEquipmentNeck},
            {BlueprintTypeId.ItemEquipmentGlasses, ItemEquipmentGlasses},
            {BlueprintTypeId.ItemArmor, ItemArmor},
            {BlueprintTypeId.ItemEquipmentShoulders, ItemEquipmentShoulders},
            {BlueprintTypeId.ItemEquipmentShirt, ItemEquipmentShirt},
            {BlueprintTypeId.ItemEquipmentRing, ItemEquipmentRing},
            {BlueprintTypeId.ItemEquipmentBelt, ItemEquipmentBelt},
            {BlueprintTypeId.ItemEquipmentGloves, ItemEquipmentGloves},
            {BlueprintTypeId.ItemEquipmentWrist, ItemEquipmentWrist},
            {BlueprintTypeId.ItemEquipmentFeet, ItemEquipmentFeet},
            {BlueprintTypeId.ItemWeapon, ItemWeapon},
            {BlueprintTypeId.ItemShield, ItemShield},
            {BlueprintTypeId.ItemEquipmentUsable, ItemEquipmentUsable},
            {BlueprintTypeId.Feature, Feature},
            {BlueprintTypeId.MemberSkill, LeaderSkill},
            {BlueprintTypeId.Etude, Etude},
            {BlueprintTypeId.UnlockableFlag, UnlockableFlag},
            {BlueprintTypeId.RaceVisualPreset, RaceVisualPreset},
            {BlueprintTypeId.Unit, Unit},
            {BlueprintTypeId.AbilityResource, AbilityResource},
        };
    }
}
