using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class BlueprintTypes
    {
        public static BlueprintType UnitAsksList { get; } = new BlueprintType("", "Kingmaker.Visual.Sound.BlueprintUnitAsksList");
        public static BlueprintType TimeOfDaySettings { get; } = new BlueprintType("", "Kingmaker.Visual.LightSelector.BlueprintTimeOfDaySettings");
        public static BlueprintType HitSystemRoot { get; } = new BlueprintType("", "Kingmaker.Visual.HitSystem.HitSystemRoot");
        public static BlueprintType KingmakerEquipmentEntity { get; } = new BlueprintType("", "Kingmaker.Visual.CharacterSystem.KingmakerEquipmentEntity");
        public static BlueprintType Tutorial { get; } = new BlueprintType("", "Kingmaker.Tutorial.BlueprintTutorial");
        public static BlueprintType CampingEncounter { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.BlueprintCampingEncounter");
        public static BlueprintType RandomEncounter { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.BlueprintRandomEncounter");
        public static BlueprintType SpawnableObject { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.BlueprintSpawnableObject");
        public static BlueprintType RandomEncountersRoot { get; } = new BlueprintType("", "Kingmaker.RandomEncounters.Settings.RandomEncountersRoot");
        public static BlueprintType ClockworkScenario { get; } = new BlueprintType("", "Kingmaker.QA.Clockwork.BlueprintClockworkScenario");
        public static BlueprintType ClockworkScenarioPart { get; } = new BlueprintType("", "Kingmaker.QA.Clockwork.BlueprintClockworkScenarioPart");
        public static BlueprintType ArbiterInstruction { get; } = new BlueprintType("", "Kingmaker.QA.Arbiter.BlueprintArbiterInstruction");
        public static BlueprintType InteractionRoot { get; } = new BlueprintType("", "Kingmaker.Interaction.BlueprintInteractionRoot");
        public static BlueprintType UISound { get; } = new BlueprintType("", "Kingmaker.UI.BlueprintUISound");
        public static BlueprintType MultiEntrance { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintMultiEntrance");
        public static BlueprintType MultiEntranceEntry { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintMultiEntranceEntry");
        public static BlueprintType GlobalMap { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMap");
        public static BlueprintType GlobalMapEdge { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMapEdge");
        public static BlueprintType GlobalMapPoint { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMapPoint");
        public static BlueprintType GlobalMapPointVariation { get; } = new BlueprintType("", "Kingmaker.Globalmap.Blueprints.BlueprintGlobalMapPointVariation");
        public static BlueprintType PartyFormation { get; } = new BlueprintType("", "Kingmaker.Formations.BlueprintPartyFormation");
        public static BlueprintType FollowersFormation { get; } = new BlueprintType("", "Kingmaker.Formations.FollowersFormation");
        public static BlueprintType UnitUpgrader { get; } = new BlueprintType("", "Kingmaker.EntitySystem.Persistence.Versioning.BlueprintUnitUpgrader");
        public static BlueprintType DungeonLocalizedStrings { get; } = new BlueprintType("", "Kingmaker.Dungeon.Blueprints.BlueprintDungeonLocalizedStrings");
        public static BlueprintType Answer { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswer");
        public static BlueprintType AnswersList { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintAnswersList");
        public static BlueprintType BookPage { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintBookPage");
        public static BlueprintType Check { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintCheck");
        public static BlueprintType Cue { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintCue");
        public static BlueprintType CueSequence { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintCueSequence");
        public static BlueprintType Dialog { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintDialog");
        public static BlueprintType DialogExperienceModifierTable { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintDialogExperienceModifierTable");
        public static BlueprintType MythicInfo { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintMythicInfo");
        public static BlueprintType MythicsSettings { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintMythicsSettings");
        public static BlueprintType SequenceExit { get; } = new BlueprintType("", "Kingmaker.DialogSystem.Blueprints.BlueprintSequenceExit");
        public static BlueprintType Dlc { get; } = new BlueprintType("", "Kingmaker.DLC.BlueprintDlc");
        public static BlueprintType DlcReward { get; } = new BlueprintType("", "Kingmaker.DLC.BlueprintDlcReward");
        public static BlueprintType SettlementBlueprintArea { get; } = new BlueprintType("", "Kingmaker.Crusade.SettlementBlueprintArea");
        public static BlueprintType GlobalMagicSpell { get; } = new BlueprintType("", "Kingmaker.Crusade.GlobalMagic.BlueprintGlobalMagicSpell");
        public static BlueprintType Ingredient { get; } = new BlueprintType("Ingredient", BlueprintTypeCategory.Item, "Kingmaker.Craft.BlueprintIngredient");
        public static BlueprintType CraftRoot { get; } = new BlueprintType("", "Kingmaker.Craft.CraftRoot");
        public static BlueprintType CorruptionRoot { get; } = new BlueprintType("", "Kingmaker.Corruption.BlueprintCorruptionRoot");
        public static BlueprintType CookingRecipe { get; } = new BlueprintType("", "Kingmaker.Controllers.Rest.Cooking.BlueprintCookingRecipe");
        public static BlueprintType AbilityResource { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintAbilityResource");
        public static BlueprintType CompanionStory { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintCompanionStory");
        public static BlueprintType ComponentList { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintComponentList");
        public static BlueprintType ControllableProjectile { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintControllableProjectile");
        public static BlueprintType Faction { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintFaction");
        public static BlueprintType Portrait { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintPortrait");
        public static BlueprintType Projectile { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintProjectile");
        public static BlueprintType ProjectileTrajectory { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintProjectileTrajectory");
        public static BlueprintType SummonPool { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintSummonPool");
        public static BlueprintType TrapSettings { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintTrapSettings");
        public static BlueprintType TrapSettingsRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintTrapSettingsRoot");
        public static BlueprintType Unit { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnit");
        public static BlueprintType UnitTemplate { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnitTemplate");
        public static BlueprintType UnitType { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnitType");
        public static BlueprintType UnlockableFlag { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintUnlockableFlag");
        public static BlueprintType RomanceCounter { get; } = new BlueprintType("", "Kingmaker.Blueprints.BlueprintRomanceCounter");
        public static BlueprintType Root { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.BlueprintRoot");
        public static BlueprintType ConsoleRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.ConsoleRoot");
        public static BlueprintType CutscenesRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.CutscenesRoot");
        public static BlueprintType FormationsRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.FormationsRoot");
        public static BlueprintType CastsGroup { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.Fx.CastsGroup");
        public static BlueprintType FxRoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Root.Fx.FxRoot");
        public static BlueprintType Quest { get; } = new BlueprintType("", "Kingmaker.Blueprints.Quests.BlueprintQuest");
        public static BlueprintType QuestGroups { get; } = new BlueprintType("", "Kingmaker.Blueprints.Quests.BlueprintQuestGroups");
        public static BlueprintType QuestObjective { get; } = new BlueprintType("", "Kingmaker.Blueprints.Quests.BlueprintQuestObjective");
        public static BlueprintType Loot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Loot.BlueprintLoot");
        public static BlueprintType UnitLoot { get; } = new BlueprintType("", "Kingmaker.Blueprints.Loot.BlueprintUnitLoot");
        public static BlueprintType TrashLootSettings { get; } = new BlueprintType("", "Kingmaker.Blueprints.Loot.TrashLootSettings");
        public static BlueprintType HiddenItem { get; } = new BlueprintType("Hidden", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintHiddenItem");
        public static BlueprintType Item { get; } = new BlueprintType("Other", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItem");
        public static BlueprintType ItemKey { get; } = new BlueprintType("Key", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItemKey");
        public static BlueprintType ItemNote { get; } = new BlueprintType("Note", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItemNote");
        public static BlueprintType ItemThiefTool { get; } = new BlueprintType("Thief Tool", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.BlueprintItemThiefTool");
        public static BlueprintType ItemsList { get; } = new BlueprintType("Items List", "Kingmaker.Blueprints.Items.BlueprintItemsList");
        public static BlueprintType SharedVendorTable { get; } = new BlueprintType("Shared Vendor Table", "Kingmaker.Blueprints.Items.BlueprintSharedVendorTable");
        public static BlueprintType CategoryDefaults { get; } = new BlueprintType("Category Defaults", "Kingmaker.Blueprints.Items.Weapons.BlueprintCategoryDefaults");
        public static BlueprintType ItemWeapon { get; } = new BlueprintType("Weapon", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Weapons.BlueprintItemWeapon");
        public static BlueprintType WeaponType { get; } = new BlueprintType("Weapon Type", "Kingmaker.Blueprints.Items.Weapons.BlueprintWeaponType");
        public static BlueprintType ItemShield { get; } = new BlueprintType("Shield", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Shields.BlueprintItemShield");
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
        public static BlueprintType ArmorEnchantment { get; } = new BlueprintType("Armor Enchantment", "Kingmaker.Blueprints.Items.Ecnchantments.BlueprintArmorEnchantment");
        public static BlueprintType EquipmentEnchantment { get; } = new BlueprintType("Equipment Enchantment", "Kingmaker.Blueprints.Items.Ecnchantments.BlueprintEquipmentEnchantment");
        public static BlueprintType WeaponEnchantment { get; } = new BlueprintType("Weapon Enchantment", "Kingmaker.Blueprints.Items.Ecnchantments.BlueprintWeaponEnchantment");
        public static BlueprintType ArmorType { get; } = new BlueprintType("Armor Type", "Kingmaker.Blueprints.Items.Armors.BlueprintArmorType");
        public static BlueprintType ItemArmor { get; } = new BlueprintType("Armor", BlueprintTypeCategory.Item, "Kingmaker.Blueprints.Items.Armors.BlueprintItemArmor");
        public static BlueprintType ShieldType { get; } = new BlueprintType("Shield Type", "Kingmaker.Blueprints.Items.Armors.BlueprintShieldType");
        public static BlueprintType Footprint { get; } = new BlueprintType("Footprint", "Kingmaker.Blueprints.Footrprints.BlueprintFootprint");
        public static BlueprintType FootprintType { get; } = new BlueprintType("Footprint Type", "Kingmaker.Blueprints.Footrprints.BlueprintFootprintType");
        public static BlueprintType UnitFact { get; } = new BlueprintType("Unit Fact", "Kingmaker.Blueprints.Facts.BlueprintUnitFact");
        public static BlueprintType EncyclopediaChapter { get; } = new BlueprintType("", "Kingmaker.Blueprints.Encyclopedia.BlueprintEncyclopediaChapter");
        public static BlueprintType EncyclopediaPage { get; } = new BlueprintType("", "Kingmaker.Blueprints.Encyclopedia.BlueprintEncyclopediaPage");
        public static BlueprintType CreditsGroup { get; } = new BlueprintType("", "Kingmaker.Blueprints.Credits.BlueprintCreditsGroup");
        public static BlueprintType CreditsRoles { get; } = new BlueprintType("", "Kingmaker.Blueprints.Credits.BlueprintCreditsRoles");
        public static BlueprintType CreditsTeams { get; } = new BlueprintType("", "Kingmaker.Blueprints.Credits.BlueprintCreditsTeams");
        public static BlueprintType GamePadTexts { get; } = new BlueprintType("", "Kingmaker.Blueprints.Console.GamePadTexts");
        public static BlueprintType Archetype { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintArchetype");
        public static BlueprintType CharacterClass { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintCharacterClass");
        public static BlueprintType CharacterClassGroup { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintCharacterClassGroup");
        public static BlueprintType ClassAdditionalVisualSettings { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintClassAdditionalVisualSettings");
        public static BlueprintType ClassAdditionalVisualSettingsProgression { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintClassAdditionalVisualSettingsProgression");
        public static BlueprintType Feature { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintFeature");
        public static BlueprintType FeatureReplaceSpellbook { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintFeatureReplaceSpellbook");
        public static BlueprintType FeatureSelectMythicSpellbook { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintFeatureSelectMythicSpellbook");
        public static BlueprintType LevelUpPlanFeaturesList { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintLevelUpPlanFeaturesList");
        public static BlueprintType Progression { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintProgression");
        public static BlueprintType Race { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintRace");
        public static BlueprintType StatProgression { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.BlueprintStatProgression");
        public static BlueprintType SpellList { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Spells.BlueprintSpellList");
        public static BlueprintType Spellbook { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Spells.BlueprintSpellbook");
        public static BlueprintType SpellsTable { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Spells.BlueprintSpellsTable");
        public static BlueprintType FeatureSelection { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Selection.BlueprintFeatureSelection");
        public static BlueprintType ParametrizedFeature { get; } = new BlueprintType("", "Kingmaker.Blueprints.Classes.Selection.BlueprintParametrizedFeature");
        public static BlueprintType RaceVisualPreset { get; } = new BlueprintType("", "Kingmaker.Blueprints.CharGen.BlueprintRaceVisualPreset");
        public static BlueprintType Area { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintArea");
        public static BlueprintType AreaEnterPoint { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaEnterPoint");
        public static BlueprintType AreaMechanics { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaMechanics");
        public static BlueprintType AreaPart { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaPart");
        public static BlueprintType AreaPreset { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaPreset");
        public static BlueprintType AreaTransition { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintAreaTransition");
        public static BlueprintType DynamicMapObject { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintDynamicMapObject");
        public static BlueprintType LogicConnector { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintLogicConnector");
        public static BlueprintType ScriptZone { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintScriptZone");
        public static BlueprintType SettlementAreaPreset { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintSettlementAreaPreset");
        public static BlueprintType Trap { get; } = new BlueprintType("", "Kingmaker.Blueprints.Area.BlueprintTrap");
        public static BlueprintType BarkBanter { get; } = new BlueprintType("", "Kingmaker.BarkBanters.BlueprintBarkBanter");
        public static BlueprintType RaceGenderDistribution { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Customization.RaceGenderDistribution");
        public static BlueprintType RandomParameters { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Customization.RandomParameters");
        public static BlueprintType UnitCustomizationPreset { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Customization.UnitCustomizationPreset");
        public static BlueprintType Buff { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff");
        public static BlueprintType ActivatableAbility { get; } = new BlueprintType("", "Kingmaker.UnitLogic.ActivatableAbilities.BlueprintActivatableAbility");
        public static BlueprintType UnitProperty { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Mechanics.Properties.BlueprintUnitProperty");
        public static BlueprintType Ability { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAbility");
        public static BlueprintType AbilityAreaEffect { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAbilityAreaEffect");
        public static BlueprintType AreaEffectPitVisualSettings { get; } = new BlueprintType("", "Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAreaEffectPitVisualSettings");
        public static BlueprintType Settlement { get; } = new BlueprintType("", "Kingmaker.Kingdom.BlueprintSettlement");
        public static BlueprintType KingdomUIRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.KingdomUIRoot");
        public static BlueprintType KingdomMoraleFlag { get; } = new BlueprintType("", "Kingmaker.Kingdom.Flags.BlueprintKingdomMoraleFlag");
        public static BlueprintType SettlementBuilding { get; } = new BlueprintType("", "Kingmaker.Kingdom.Settlements.BlueprintSettlementBuilding");
        public static BlueprintType LeadersRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.LeadersRoot");
        public static BlueprintType ArmyRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.ArmyRoot");
        public static BlueprintType ArmyPresetList { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintArmyPresetList");
        public static BlueprintType CrusadeEvent { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintCrusadeEvent");
        public static BlueprintType CrusadeEventTimeline { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintCrusadeEventTimeline");
        public static BlueprintType KingdomBuff { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomBuff");
        public static BlueprintType KingdomDeck { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomDeck");
        public static BlueprintType KingdomEvent { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomEvent");
        public static BlueprintType KingdomEventTimeline { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomEventTimeline");
        public static BlueprintType KingdomProject { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintKingdomProject");
        public static BlueprintType Region { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.BlueprintRegion");
        public static BlueprintType KingdomRoot { get; } = new BlueprintType("", "Kingmaker.Kingdom.Blueprints.KingdomRoot");
        public static BlueprintType ArmyLeader { get; } = new BlueprintType("", "Kingmaker.Armies.BlueprintArmyLeader");
        public static BlueprintType LeaderProgression { get; } = new BlueprintType("", "Kingmaker.Armies.BlueprintLeaderProgression");
        public static BlueprintType LeaderSkillsList { get; } = new BlueprintType("", "Kingmaker.Armies.BlueprintLeaderSkillsList");
        public static BlueprintType TacticalCombatAiAttack { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.BlueprintTacticalCombatAiAttack");
        public static BlueprintType TacticalCombatAiCastSpell { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.BlueprintTacticalCombatAiCastSpell");
        public static BlueprintType TacticalCombatBrain { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.BlueprintTacticalCombatBrain");
        public static BlueprintType ArmyHealthConsideration { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.Considerations.ArmyHealthConsideration");
        public static BlueprintType TacticalCombatCanAttackThisTurnConsideration { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.Considerations.TacticalCombatCanAttackThisTurnConsideration");
        public static BlueprintType TacticalCombatTagConsideration { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Brain.Considerations.TacticalCombatTagConsideration");
        public static BlueprintType TacticalCombatArea { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Blueprints.BlueprintTacticalCombatArea");
        public static BlueprintType TacticalCombatObstaclesMap { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Blueprints.BlueprintTacticalCombatObstaclesMap");
        public static BlueprintType TacticalCombatRoot { get; } = new BlueprintType("", "Kingmaker.Armies.TacticalCombat.Blueprints.BlueprintTacticalCombatRoot");
        public static BlueprintType ArmyPreset { get; } = new BlueprintType("", "Kingmaker.Armies.Blueprints.BlueprintArmyPreset");
        public static BlueprintType LeaderSkill { get; } = new BlueprintType("", "Kingmaker.Armies.Blueprints.BlueprintLeaderSkill");
        public static BlueprintType MoraleRoot { get; } = new BlueprintType("", "Kingmaker.Armies.Blueprints.MoraleRoot");
        public static BlueprintType Etude { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Etudes.BlueprintEtude");
        public static BlueprintType EtudeConflictingGroup { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Etudes.BlueprintEtudeConflictingGroup");
        public static BlueprintType Cutscene { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Cutscenes.Cutscene");
        public static BlueprintType Gate { get; } = new BlueprintType("", "Kingmaker.AreaLogic.Cutscenes.Gate");
        public static BlueprintType AchievementData { get; } = new BlueprintType("", "Kingmaker.Achievements.Blueprints.AchievementData");
        public static BlueprintType AiAttack { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiAttack");
        public static BlueprintType AiCastSpell { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiCastSpell");
        public static BlueprintType AiFollow { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiFollow");
        public static BlueprintType AiSwitchWeapon { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiSwitchWeapon");
        public static BlueprintType AiTouch { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintAiTouch");
        public static BlueprintType Brain { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.BlueprintBrain");
        public static BlueprintType CustomAiConsiderationsRoot { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.CustomAiConsiderationsRoot");
        public static BlueprintType ActiveCommandConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ActiveCommandConsideration");
        public static BlueprintType AlignmentConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.AlignmentConsideration");
        public static BlueprintType ArmorTypeConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ArmorTypeConsideration");
        public static BlueprintType BuffConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.BuffConsideration");
        public static BlueprintType BuffNotFromCasterConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.BuffNotFromCasterConsideration");
        public static BlueprintType BuffsAroundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.BuffsAroundConsideration");
        public static BlueprintType CanMakeFullAttackConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CanMakeFullAttackConsideration");
        public static BlueprintType CanUseSpellCombatConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CanUseSpellCombatConsideration");
        public static BlueprintType CasterClassConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CasterClassConsideration");
        public static BlueprintType CommandCooldownConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.CommandCooldownConsideration");
        public static BlueprintType ComplexConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ComplexConsideration");
        public static BlueprintType ConditionConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ConditionConsideration");
        public static BlueprintType DistanceConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.DistanceConsideration");
        public static BlueprintType DistanceRangeConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.DistanceRangeConsideration");
        public static BlueprintType FactConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.FactConsideration");
        public static BlueprintType HasAutoCastConsideraion { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HasAutoCastConsideraion");
        public static BlueprintType HasManualTargetConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HasManualTargetConsideration");
        public static BlueprintType HealthAroundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HealthAroundConsideration");
        public static BlueprintType HealthConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HealthConsideration");
        public static BlueprintType HitThisRoundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.HitThisRoundConsideration");
        public static BlueprintType InRangeConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.InRangeConsideration");
        public static BlueprintType LastTargetConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.LastTargetConsideration");
        public static BlueprintType LifeStateConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.LifeStateConsideration");
        public static BlueprintType LineOfSightConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.LineOfSightConsideration");
        public static BlueprintType ManualTargetConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ManualTargetConsideration");
        public static BlueprintType NotImpatientConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.NotImpatientConsideration");
        public static BlueprintType RandomConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.RandomConsideration");
        public static BlueprintType StatConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.StatConsideration");
        public static BlueprintType SwarmTargetsConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.SwarmTargetsConsideration");
        public static BlueprintType TargetClassConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.TargetClassConsideration");
        public static BlueprintType TargetMainCharacter { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.TargetMainCharacter");
        public static BlueprintType TargetSelfConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.TargetSelfConsideration");
        public static BlueprintType ThreatedByConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.ThreatedByConsideration");
        public static BlueprintType UnitsAroundConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.UnitsAroundConsideration");
        public static BlueprintType UnitsThreateningConsideration { get; } = new BlueprintType("", "Kingmaker.AI.Blueprints.Considerations.UnitsThreateningConsideration");

        private static readonly Dictionary<string, BlueprintType> Lookup = new Dictionary<string, BlueprintType>(StringComparer.Ordinal) {
            {UnitAsksList.FullName, UnitAsksList},
            {TimeOfDaySettings.FullName, TimeOfDaySettings},
            {HitSystemRoot.FullName, HitSystemRoot},
            {KingmakerEquipmentEntity.FullName, KingmakerEquipmentEntity},
            {Tutorial.FullName, Tutorial},
            {CampingEncounter.FullName, CampingEncounter},
            {RandomEncounter.FullName, RandomEncounter},
            {SpawnableObject.FullName, SpawnableObject},
            {RandomEncountersRoot.FullName, RandomEncountersRoot},
            {ClockworkScenario.FullName, ClockworkScenario},
            {ClockworkScenarioPart.FullName, ClockworkScenarioPart},
            {ArbiterInstruction.FullName, ArbiterInstruction},
            {InteractionRoot.FullName, InteractionRoot},
            {UISound.FullName, UISound},
            {MultiEntrance.FullName, MultiEntrance},
            {MultiEntranceEntry.FullName, MultiEntranceEntry},
            {GlobalMap.FullName, GlobalMap},
            {GlobalMapEdge.FullName, GlobalMapEdge},
            {GlobalMapPoint.FullName, GlobalMapPoint},
            {GlobalMapPointVariation.FullName, GlobalMapPointVariation},
            {PartyFormation.FullName, PartyFormation},
            {FollowersFormation.FullName, FollowersFormation},
            {UnitUpgrader.FullName, UnitUpgrader},
            {DungeonLocalizedStrings.FullName, DungeonLocalizedStrings},
            {Answer.FullName, Answer},
            {AnswersList.FullName, AnswersList},
            {BookPage.FullName, BookPage},
            {Check.FullName, Check},
            {Cue.FullName, Cue},
            {CueSequence.FullName, CueSequence},
            {Dialog.FullName, Dialog},
            {DialogExperienceModifierTable.FullName, DialogExperienceModifierTable},
            {MythicInfo.FullName, MythicInfo},
            {MythicsSettings.FullName, MythicsSettings},
            {SequenceExit.FullName, SequenceExit},
            {Dlc.FullName, Dlc},
            {DlcReward.FullName, DlcReward},
            {SettlementBlueprintArea.FullName, SettlementBlueprintArea},
            {GlobalMagicSpell.FullName, GlobalMagicSpell},
            {Ingredient.FullName, Ingredient},
            {CraftRoot.FullName, CraftRoot},
            {CorruptionRoot.FullName, CorruptionRoot},
            {CookingRecipe.FullName, CookingRecipe},
            {AbilityResource.FullName, AbilityResource},
            {CompanionStory.FullName, CompanionStory},
            {ComponentList.FullName, ComponentList},
            {ControllableProjectile.FullName, ControllableProjectile},
            {Faction.FullName, Faction},
            {Portrait.FullName, Portrait},
            {Projectile.FullName, Projectile},
            {ProjectileTrajectory.FullName, ProjectileTrajectory},
            {SummonPool.FullName, SummonPool},
            {TrapSettings.FullName, TrapSettings},
            {TrapSettingsRoot.FullName, TrapSettingsRoot},
            {Unit.FullName, Unit},
            {UnitTemplate.FullName, UnitTemplate},
            {UnitType.FullName, UnitType},
            {UnlockableFlag.FullName, UnlockableFlag},
            {RomanceCounter.FullName, RomanceCounter},
            {Root.FullName, Root},
            {ConsoleRoot.FullName, ConsoleRoot},
            {CutscenesRoot.FullName, CutscenesRoot},
            {FormationsRoot.FullName, FormationsRoot},
            {CastsGroup.FullName, CastsGroup},
            {FxRoot.FullName, FxRoot},
            {Quest.FullName, Quest},
            {QuestGroups.FullName, QuestGroups},
            {QuestObjective.FullName, QuestObjective},
            {Loot.FullName, Loot},
            {UnitLoot.FullName, UnitLoot},
            {TrashLootSettings.FullName, TrashLootSettings},
            {HiddenItem.FullName, HiddenItem},
            {Item.FullName, Item},
            {ItemKey.FullName, ItemKey},
            {ItemNote.FullName, ItemNote},
            {ItemThiefTool.FullName, ItemThiefTool},
            {ItemsList.FullName, ItemsList},
            {SharedVendorTable.FullName, SharedVendorTable},
            {CategoryDefaults.FullName, CategoryDefaults},
            {ItemWeapon.FullName, ItemWeapon},
            {WeaponType.FullName, WeaponType},
            {ItemShield.FullName, ItemShield},
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
            {ArmorEnchantment.FullName, ArmorEnchantment},
            {EquipmentEnchantment.FullName, EquipmentEnchantment},
            {WeaponEnchantment.FullName, WeaponEnchantment},
            {ArmorType.FullName, ArmorType},
            {ItemArmor.FullName, ItemArmor},
            {ShieldType.FullName, ShieldType},
            {Footprint.FullName, Footprint},
            {FootprintType.FullName, FootprintType},
            {UnitFact.FullName, UnitFact},
            {EncyclopediaChapter.FullName, EncyclopediaChapter},
            {EncyclopediaPage.FullName, EncyclopediaPage},
            {CreditsGroup.FullName, CreditsGroup},
            {CreditsRoles.FullName, CreditsRoles},
            {CreditsTeams.FullName, CreditsTeams},
            {GamePadTexts.FullName, GamePadTexts},
            {Archetype.FullName, Archetype},
            {CharacterClass.FullName, CharacterClass},
            {CharacterClassGroup.FullName, CharacterClassGroup},
            {ClassAdditionalVisualSettings.FullName, ClassAdditionalVisualSettings},
            {ClassAdditionalVisualSettingsProgression.FullName, ClassAdditionalVisualSettingsProgression},
            {Feature.FullName, Feature},
            {FeatureReplaceSpellbook.FullName, FeatureReplaceSpellbook},
            {FeatureSelectMythicSpellbook.FullName, FeatureSelectMythicSpellbook},
            {LevelUpPlanFeaturesList.FullName, LevelUpPlanFeaturesList},
            {Progression.FullName, Progression},
            {Race.FullName, Race},
            {StatProgression.FullName, StatProgression},
            {SpellList.FullName, SpellList},
            {Spellbook.FullName, Spellbook},
            {SpellsTable.FullName, SpellsTable},
            {FeatureSelection.FullName, FeatureSelection},
            {ParametrizedFeature.FullName, ParametrizedFeature},
            {RaceVisualPreset.FullName, RaceVisualPreset},
            {Area.FullName, Area},
            {AreaEnterPoint.FullName, AreaEnterPoint},
            {AreaMechanics.FullName, AreaMechanics},
            {AreaPart.FullName, AreaPart},
            {AreaPreset.FullName, AreaPreset},
            {AreaTransition.FullName, AreaTransition},
            {DynamicMapObject.FullName, DynamicMapObject},
            {LogicConnector.FullName, LogicConnector},
            {ScriptZone.FullName, ScriptZone},
            {SettlementAreaPreset.FullName, SettlementAreaPreset},
            {Trap.FullName, Trap},
            {BarkBanter.FullName, BarkBanter},
            {RaceGenderDistribution.FullName, RaceGenderDistribution},
            {RandomParameters.FullName, RandomParameters},
            {UnitCustomizationPreset.FullName, UnitCustomizationPreset},
            {Buff.FullName, Buff},
            {ActivatableAbility.FullName, ActivatableAbility},
            {UnitProperty.FullName, UnitProperty},
            {Ability.FullName, Ability},
            {AbilityAreaEffect.FullName, AbilityAreaEffect},
            {AreaEffectPitVisualSettings.FullName, AreaEffectPitVisualSettings},
            {Settlement.FullName, Settlement},
            {KingdomUIRoot.FullName, KingdomUIRoot},
            {KingdomMoraleFlag.FullName, KingdomMoraleFlag},
            {SettlementBuilding.FullName, SettlementBuilding},
            {LeadersRoot.FullName, LeadersRoot},
            {ArmyRoot.FullName, ArmyRoot},
            {ArmyPresetList.FullName, ArmyPresetList},
            {CrusadeEvent.FullName, CrusadeEvent},
            {CrusadeEventTimeline.FullName, CrusadeEventTimeline},
            {KingdomBuff.FullName, KingdomBuff},
            {KingdomDeck.FullName, KingdomDeck},
            {KingdomEvent.FullName, KingdomEvent},
            {KingdomEventTimeline.FullName, KingdomEventTimeline},
            {KingdomProject.FullName, KingdomProject},
            {Region.FullName, Region},
            {KingdomRoot.FullName, KingdomRoot},
            {ArmyLeader.FullName, ArmyLeader},
            {LeaderProgression.FullName, LeaderProgression},
            {LeaderSkillsList.FullName, LeaderSkillsList},
            {TacticalCombatAiAttack.FullName, TacticalCombatAiAttack},
            {TacticalCombatAiCastSpell.FullName, TacticalCombatAiCastSpell},
            {TacticalCombatBrain.FullName, TacticalCombatBrain},
            {ArmyHealthConsideration.FullName, ArmyHealthConsideration},
            {TacticalCombatCanAttackThisTurnConsideration.FullName, TacticalCombatCanAttackThisTurnConsideration},
            {TacticalCombatTagConsideration.FullName, TacticalCombatTagConsideration},
            {TacticalCombatArea.FullName, TacticalCombatArea},
            {TacticalCombatObstaclesMap.FullName, TacticalCombatObstaclesMap},
            {TacticalCombatRoot.FullName, TacticalCombatRoot},
            {ArmyPreset.FullName, ArmyPreset},
            {LeaderSkill.FullName, LeaderSkill},
            {MoraleRoot.FullName, MoraleRoot},
            {Etude.FullName, Etude},
            {EtudeConflictingGroup.FullName, EtudeConflictingGroup},
            {Cutscene.FullName, Cutscene},
            {Gate.FullName, Gate},
            {AchievementData.FullName, AchievementData},
            {AiAttack.FullName, AiAttack},
            {AiCastSpell.FullName, AiCastSpell},
            {AiFollow.FullName, AiFollow},
            {AiSwitchWeapon.FullName, AiSwitchWeapon},
            {AiTouch.FullName, AiTouch},
            {Brain.FullName, Brain},
            {CustomAiConsiderationsRoot.FullName, CustomAiConsiderationsRoot},
            {ActiveCommandConsideration.FullName, ActiveCommandConsideration},
            {AlignmentConsideration.FullName, AlignmentConsideration},
            {ArmorTypeConsideration.FullName, ArmorTypeConsideration},
            {BuffConsideration.FullName, BuffConsideration},
            {BuffNotFromCasterConsideration.FullName, BuffNotFromCasterConsideration},
            {BuffsAroundConsideration.FullName, BuffsAroundConsideration},
            {CanMakeFullAttackConsideration.FullName, CanMakeFullAttackConsideration},
            {CanUseSpellCombatConsideration.FullName, CanUseSpellCombatConsideration},
            {CasterClassConsideration.FullName, CasterClassConsideration},
            {CommandCooldownConsideration.FullName, CommandCooldownConsideration},
            {ComplexConsideration.FullName, ComplexConsideration},
            {ConditionConsideration.FullName, ConditionConsideration},
            {DistanceConsideration.FullName, DistanceConsideration},
            {DistanceRangeConsideration.FullName, DistanceRangeConsideration},
            {FactConsideration.FullName, FactConsideration},
            {HasAutoCastConsideraion.FullName, HasAutoCastConsideraion},
            {HasManualTargetConsideration.FullName, HasManualTargetConsideration},
            {HealthAroundConsideration.FullName, HealthAroundConsideration},
            {HealthConsideration.FullName, HealthConsideration},
            {HitThisRoundConsideration.FullName, HitThisRoundConsideration},
            {InRangeConsideration.FullName, InRangeConsideration},
            {LastTargetConsideration.FullName, LastTargetConsideration},
            {LifeStateConsideration.FullName, LifeStateConsideration},
            {LineOfSightConsideration.FullName, LineOfSightConsideration},
            {ManualTargetConsideration.FullName, ManualTargetConsideration},
            {NotImpatientConsideration.FullName, NotImpatientConsideration},
            {RandomConsideration.FullName, RandomConsideration},
            {StatConsideration.FullName, StatConsideration},
            {SwarmTargetsConsideration.FullName, SwarmTargetsConsideration},
            {TargetClassConsideration.FullName, TargetClassConsideration},
            {TargetMainCharacter.FullName, TargetMainCharacter},
            {TargetSelfConsideration.FullName, TargetSelfConsideration},
            {ThreatedByConsideration.FullName, ThreatedByConsideration},
            {UnitsAroundConsideration.FullName, UnitsAroundConsideration},
            {UnitsThreateningConsideration.FullName, UnitsThreateningConsideration}
        };

        public static BlueprintType Resolve(string typeFullName)
        {
            return Lookup.TryGetValue(typeFullName, out var type) ? type : new BlueprintType(typeFullName, "<Unknown>");
        }
    }
}
