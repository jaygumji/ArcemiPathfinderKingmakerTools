using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class Enchantments
    {
        public const string TricksterKnowledgeArcanaTier1Feature = "c7bb946de7454df4380c489a8350ba38";
        public const string TricksterKnowledgeArcanaTier2Feature = "7bbd9f681440a294382b527a554e419d";
        public const string TricksterKnowledgeArcanaTier3Feature = "5e26c673173e423881e318d2f0ae84f0";

        public static bool IsLevel(string blueprint)
        {
            return Weapon.Level.Levels.Any(l => l.Blueprint.Eq(blueprint))
                || Armor.Level.Levels.Any(l => l.Blueprint.Eq(blueprint))
                || Shield.Level.Levels.Any(l => l.Blueprint.Eq(blueprint));
        }

        public static bool IsMapped(string blueprint)
        {
            return Weapon.All.Any(e => e.Blueprint.Eq(blueprint))
                || Armor.All.Any(e => e.Blueprint.Eq(blueprint));
        }

        public static class Weapon
        {
            public static class Components
            {
                public const string EnergyDamageDice = "$WeaponEnergyDamageDice$077d2129-bb6a-4092-ae10-554f046d1f4f";
                public const string EnergyBurst = "$WeaponEnergyBurst$08f38b38-870f-4d8e-9a7e-b78d643aeaf0";
                public const string EnergyBurst2 = "$WeaponEnergyBurst$bbb58f7b-2854-4daf-aeff-16f5e81ca683";
                public const string ExtraAttack = "$WeaponExtraAttack$35d7d1ec-c494-4d26-ab01-cafaa995d77d";
                public const string DamageAgainstAlignment = "$WeaponDamageAgainstAlignment$3ac2acd7-5e33-4bca-a82e-e0f9805d79ce";
                public const string CriticalEdgeIncrease = "$WeaponCriticalEdgeIncrease$7b9ff2fb-26d6-438e-bb56-ce32ba91c212";
                public const string Level = "$WeaponEnhancementBonus$f1459788-04d5-4128-ad25-dace4b8dee42";
                public const string Reality = "$WeaponReality$7e3ed999-10f0-4de4-a256-40e79a586e63";
                public const string Material = "$WeaponMaterial$ce7e2254-e6b0-492e-8e8a-b9a96ab0f4b7";
                public const string DamageStatReplacementIncrease = "$WeaponDamageStatReplacementIncrease$1aa4eb5e-6d3d-46c5-945c-21b6bff0cda0";
                public const string ConditionalEnhancementBonus = "$WeaponConditionalEnhancementBonus$c3420665-b0ee-4913-85da-cf1c29966347";
                public const string ConditionalDamageDice = "$WeaponConditionalDamageDice$ee0200bb-90c1-4625-855c-dab0de0d4557";
                public const string DamageMultiplierStatReplacement = "$WeaponDamageMultiplierStatReplacement$0c0776ac-6832-4bc3-bba6-9be70b963566";
            }
            public static EnchantmentSpec Adamantine { get; } = new EnchantmentSpec("Adamantine", "ab39e7d59dd12f4429ffef5dca88dc7b");
            public static EnchantmentSpec Agile { get; } = new EnchantmentSpec("Agile", "a36ad92c51789b44fa8a1c5c116a1328", Components.DamageStatReplacementIncrease);
            public static EnchantmentSpec Anarchic { get; } = new EnchantmentSpec("Anarchic", "57315bc1e1f62a741be0efde688087e9", Components.DamageAgainstAlignment);
            public static EnchantmentSpec Axiomatic { get; } = new EnchantmentSpec("Axiomatic", "0ca43051edefcad4b9b2240aa36dc8d4", Components.DamageAgainstAlignment);
            public static EnchantmentSpec BaneAberration { get; } = new EnchantmentSpec("Bane Aberration", "ee71cc8848219c24b8418a628cc3e2fa", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneAnimal { get; } = new EnchantmentSpec("Bane Animal", "78cf9fabe95d3934688ea898c154d904", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneConstruct { get; } = new EnchantmentSpec("Bane Construct", "73d30862f33cc754bb5a5f3240162ae6", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneDragon { get; } = new EnchantmentSpec("Bane Dragon", "e5cb46a0a658b0a41854447bea32d2ee", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneEverything { get; } = new EnchantmentSpec("Bane Everything", "1a93ab9c46e48f3488178733be29342a", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneFey { get; } = new EnchantmentSpec("Bane Fey", "b6948040cdb601242884744a543050d4", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneHumanoidGiant { get; } = new EnchantmentSpec("Bane Humanoid Giant", "dcecb5f2ffacfd44ead0ed4f8846445d", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneHumanoidGiant2d6 { get; } = new EnchantmentSpec("Bane Humanoid Giant 2d6", "1e25d1f515c867d40b9c0642e0b40ec2", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneHumanoidReptilian { get; } = new EnchantmentSpec("Bane Humanoid Reptilian", "c4b9cce255d1d6641a6105a255934e2e", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneLiving { get; } = new EnchantmentSpec("Bane Living", "e1d6f5e3cd3855b43a0cb42f6c747e1c", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneLongshankEnchant { get; } = new EnchantmentSpec("Bane Longshank Enchant", "92a1f5db1a03c5b468828c25dd375806", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneLycanthrope { get; } = new EnchantmentSpec("Bane Lycanthrope", "188efcfcd9938d44e9561c87794d17a8", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneMagicalBeast { get; } = new EnchantmentSpec("Bane Magical Beast", "97d477424832c5144a9413c64d818659", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneMonstrousHumanoid { get; } = new EnchantmentSpec("Bane Monstrous Humanoid", "c5f84a79ad154c84e8d2e9fe0dd49350", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneOrcGoblin { get; } = new EnchantmentSpec("Bane Orc Goblin", "0391d8eae25f39a48bcc6c2fc8bf4e12", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneOrcGoblin1d6 { get; } = new EnchantmentSpec("Bane Orc Goblin 1d6", "ab0108b67cfc2a849926a79ece0fdddc", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneOutsiderChaotic { get; } = new EnchantmentSpec("Bane Outsider Chaotic", "234177d5807909f44b8c91ed3c9bf7ac", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneOutsiderEvil { get; } = new EnchantmentSpec("Bane Outsider Evil", "20ba9055c6ae1e44ca270c03feacc53b", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneOutsiderGood { get; } = new EnchantmentSpec("Bane Outsider Good", "a876de94b916b7249a77d090cb9be4f3", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneOutsiderLawful { get; } = new EnchantmentSpec("Bane Outsider Lawful", "3a6f564c8ea2d1941a45b19fa16e59f5", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneOutsiderNeutral { get; } = new EnchantmentSpec("Bane Outsider Neutral", "4e30e79c500e5af4b86a205cc20436f2", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BanePlant { get; } = new EnchantmentSpec("Bane Plant", "0b761b6ed6375114d8d01525d44be5a9", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneUndead { get; } = new EnchantmentSpec("Bane Undead", "eebb4d3f20b8caa43af1fed8f2773328", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneVermin { get; } = new EnchantmentSpec("Bane Vermin", "c3428441c00354c4fabe27629c6c64dd", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec BaneVermin1d8 { get; } = new EnchantmentSpec("Bane Vermin 1d8", "e535fc007d0d7e74da45021d4607e607", Components.ConditionalEnhancementBonus, Components.ConditionalDamageDice);
            public static EnchantmentSpec Bleed { get; } = new EnchantmentSpec("Bleed", "ac0108944bfaa7e48aa74f407e3944e3", "$AddInitiatorAttackWithWeaponTrigger$68fcacc1-f262-466e-a20d-63e32aff65c5");
            public static EnchantmentSpec Bliz { get; } = new EnchantmentSpec("The Bliz", "f1891b0dd34740d4aa97957e8f9240b5", "$AddInitiatorAttackWithWeaponTrigger$c3bcf325-3c41-4082-8584-e0ded444b3eb");
            public static EnchantmentSpec Brass { get; } = new EnchantmentSpec("Brass", "5e0e5de297c229f42b00c5b1738b50fa", Components.EnergyDamageDice);
            public static EnchantmentSpec BrilliantEnergy { get; } = new EnchantmentSpec("Brilliant Energy", "66e9e299c9002ea4bb65b6f300e43770", "$BrilliantEnergy$cec1a7bf-c950-4d57-a917-8252e4443484", "$MissAgainstFactOwner$8521181c-1ce7-4ccf-a856-bfdaafcad246");
            public static EnchantmentSpec ColdIron { get; } = new EnchantmentSpec("Cold Iron", "e5990dc76d2a613409916071c898eee8", Components.Material);
            public static EnchantmentSpec Corrosive { get; } = new EnchantmentSpec("Corrosive", "633b38ff1d11de64a91d490c683ab1c8", Components.EnergyDamageDice);
            public static EnchantmentSpec Corrosive2d6 { get; } = new EnchantmentSpec("Corrosive 2d6", "2becfef47bec13940b9ee71f1b14d2dd", Components.EnergyDamageDice);
            public static EnchantmentSpec CorrosiveBurst { get; } = new EnchantmentSpec("Corrosive Burst", "0cf34703e67e37b40905845ca14b1380", Components.EnergyBurst);
            public static EnchantmentSpec Cruel { get; } = new EnchantmentSpec("Cruel", "629c383ffb407224398bb71d1bd95d14", "$AddInitiatorAttackWithWeaponTrigger$f798bc3d-4a60-47fd-ac63-ccf55e5ba994", "$AddInitiatorAttackWithWeaponTrigger$db5a4e45-dc87-45d4-b98f-fc6452a4ef7b");
            //public static EnchantmentSpec CrystalDagger { get; } = new EnchantmentSpec("Crystal Dagger", "ac3b2efb35ed4c04d8be2db24f013847", new RuntimeComponentSpec("$AddUnitFactEquipment$53aa5b64-e439-402c-8a4c-d24763b0d274", addedFact: new FactFeatureSpec("b5a8f5bb6503c9846bdea9428d3d442a", "$IncreaseSpellSchoolDC$1a6713a4-08c1-4ea9-a50c-2f3f7473dc43")));
            public static EnchantmentSpec CrystalDagger { get; } = new EnchantmentSpec("Crystal Dagger", "ac3b2efb35ed4c04d8be2db24f013847", "$AddUnitFactEquipment$53aa5b64-e439-402c-8a4c-d24763b0d274");
            public static EnchantmentSpec DeathPact { get; } = new EnchantmentSpec("Death Pact", "0433071606d94ed3b4006fa1b6348272", "$AdditionalDiceOnAttack$c744a5ce-2534-4a81-99bd-9c29a534a9b0", "$AddInitiatorAttackWithWeaponTrigger$7b1fa38a-782e-4a96-8006-18df48d8a813");
            public static EnchantmentSpec Devitalizer { get; } = new EnchantmentSpec("Devitalizer", "577af31035b34142a2235083c09220be", "$AddInitiatorAttackWithWeaponTrigger$7685cb0b-eafc-46f1-afa4-826232f02358");
            public static EnchantmentSpec Disruption { get; } = new EnchantmentSpec("Disruption", "0f20d79b7049c0f4ca54ca3d1ea44baa", "$AddInitiatorAttackWithWeaponTrigger$279973fd-a35c-430b-9188-fb4ba0e71921");
            public static EnchantmentSpec ElderCorrosive { get; } = new EnchantmentSpec("Elder Corrosive", "c7fa5c82d5bb4baf8458dd30981908d1", Components.EnergyDamageDice, "$IgnoreResistanceForDamageFromEnchantment$cc8b00cc-137f-4673-8baf-b4ff0f937be0");
            public static EnchantmentSpec ElderCorrosiveBurst { get; } = new EnchantmentSpec("Elder Corrosive Burst", "ea30c02bc5814d8fb600e66dc9d3d520", Components.EnergyBurst, "$IgnoreResistanceForDamageFromEnchantment$1dd5ce43-d39c-44c5-a2b7-cb9f02523a71");
            public static EnchantmentSpec ExplosiveFervor { get; } = new EnchantmentSpec("Explosive Fervor", "ec6c3df98da7413fa21ffa5680802762", "$AddInitiatorAttackWithWeaponTrigger$c113bc17-da7d-4e1c-923c-2a074174a00f");
            public static EnchantmentSpec FierySpellWeaver { get; } = new EnchantmentSpec("Fiery Spell Weaver", "e2dac84c193848bb937e115192d4e720", "$AddUnitFeatureEquipment$f88721fe-5cb4-44d0-8b6e-f716162a884f");
            public static EnchantmentSpec Flaming { get; } = new EnchantmentSpec("Flaming", "30f90becaaac51f41bf56641966c4121", Components.EnergyDamageDice);
            public static EnchantmentSpec FlamingBurst { get; } = new EnchantmentSpec("Flaming Burst", "3f032a3cd54e57649a0cdad0434bf221", Components.EnergyBurst);
            public static EnchantmentSpec Frost { get; } = new EnchantmentSpec("Frost", "421e54078b7719d40915ce0672511d0b", Components.EnergyDamageDice);
            public static EnchantmentSpec Frost2d8 { get; } = new EnchantmentSpec("Frost 2d8", "83e7559124cb78a4c9d61360d3a4c3c2", Components.EnergyDamageDice);
            public static EnchantmentSpec Furious { get; } = new EnchantmentSpec("Furious", "b606a3f5daa76cc40add055613970d2a", Components.ConditionalEnhancementBonus);
            public static EnchantmentSpec Furyborn { get; } = new EnchantmentSpec("Furyborn", "091e2f6b2fad84a45ae76b8aac3c55c3", "$IncreaseWeaponEnhancementBonusOnTargetFocus$b303ff25-7b48-4c37-bd59-f3f059882670");
            public static EnchantmentSpec GhostTouch { get; } = new EnchantmentSpec("Ghost touch", "47857e1a5a3ec1a46adf6491b1423b4f", Components.Reality);
            public static EnchantmentSpec GoreFeaster { get; } = new EnchantmentSpec("Gore Feaster", "d8891abf13224fc39c0b94f8e2549200", "$AddUnitFeatureEquipment$f88721fe-5cb4-44d0-8b6e-f716162a884f", "$AddInitiatorAttackWithWeaponTrigger$749e9f16-5df6-4ab8-bbb4-5e24830c4bda");
            public static EnchantmentSpec GreatclubOfSacredCinders { get; } = new EnchantmentSpec("Greatclub Of Sacred Cinders", "e6716f81a0e3495a9442237b3ce9f380", "$AddInitiatorAttackWithWeaponTrigger$190fd062-d0a0-477d-ba55-a3ec66ca577a", "$AddUnitFeatureEquipment$c733097d-bd78-444c-99b9-e021840880ce");
            public static EnchantmentSpec Holy { get; } = new EnchantmentSpec("Holy", "28a9964d81fedae44bae3ca45710c140", Components.DamageAgainstAlignment);
            public static EnchantmentSpec Ice2d6 { get; } = new EnchantmentSpec("Ice 2d6", "00049f6046b20394091b29702c6e9617", Components.EnergyDamageDice);
            public static EnchantmentSpec IcyBurst { get; } = new EnchantmentSpec("Icy Burst", "564a6924b246d254c920a7c44bf2a58b", Components.EnergyBurst2);
            public static EnchantmentSpec Keen { get; } = new EnchantmentSpec("Keen", "102a9c8c9b7a75e4fb5844e79deaf4c0", Components.CriticalEdgeIncrease);
            public static EnchantmentSpec MageSlayer { get; } = new EnchantmentSpec("Mage Slayer", "2bd82e00f02d4f2fb1e21e19c325b550", "$AddInitiatorAttackWithWeaponTrigger$cce5676f-54ca-4dd8-aabc-3da3915ab6e8");
            public static EnchantmentSpec Masterwork { get; } = new EnchantmentSpec("Masterwork", "6b38844e2bffbac48b63036b66e735be", "$WeaponMasterwork$b064bd6b-a1f3-45e0-9aa4-7a4c11504d0d");
            public static EnchantmentSpec Mithral { get; } = new EnchantmentSpec("Mithral", "0ae8fc9f2e255584faf4d14835224875");
            public static EnchantmentSpec Necrotic { get; } = new EnchantmentSpec("Necrotic", "bad4134798e182c4487819dce9b43003", "$WeaponConditionalDamageDice$c2afe5d3-8604-4e69-b4da-e55814f39f37");
            public static EnchantmentSpec Nullifying { get; } = new EnchantmentSpec("Nullifying", "efbe3a35fc7349845ac9f96b4c63312e", "$AddInitiatorAttackWithWeaponTrigger$e6d4066b-c989-4dfa-ae85-43c9523b43f2", "$AddInitiatorAttackWithWeaponTrigger$b8023010-de95-4cd2-b357-04d6f8f303a6");
            public static EnchantmentSpec Oversized { get; } = new EnchantmentSpec("Oversized", "d8e1ebc1062d8cc42abff78783856b0d", "$WeaponOversized$4e351706-9aad-4c2b-8d6e-445037d37be9");
            public static EnchantmentSpec Radiant { get; } = new EnchantmentSpec("Radiant", "5ac5c88157f7dde48a2a5b24caf40131", Components.EnergyDamageDice);
            public static EnchantmentSpec Sacrificial { get; } = new EnchantmentSpec("Sacrificial", "b7f029a31452b26408bc75d715227993", Components.ConditionalEnhancementBonus);
            public static EnchantmentSpec Shock { get; } = new EnchantmentSpec("Shock", "7bda5277d36ad114f9f9fd21d0dab658", Components.EnergyDamageDice);
            public static EnchantmentSpec Shock2d6 { get; } = new EnchantmentSpec("Shock 2d6", "b1de8528121b80844bd7cf09d9e1cf00", Components.EnergyDamageDice);
            public static EnchantmentSpec ShockingBurst { get; } = new EnchantmentSpec("Shocking Burst", "914d7ee77fb09d846924ca08bccee0ff", Components.EnergyBurst);
            public static EnchantmentSpec Speed { get; } = new EnchantmentSpec("Speed", "f1c0c50108025d546b2554674ea1c006", Components.ExtraAttack);
            public static EnchantmentSpec StrengthComposite { get; } = new EnchantmentSpec("Strength Composite", "c3209eb058d471548928a200d70765e0", Components.DamageMultiplierStatReplacement);
            public static EnchantmentSpec StrengthThrown { get; } = new EnchantmentSpec("Strength Thrown", "c4d213911e9616949937e1520c80aaf3", Components.DamageMultiplierStatReplacement);
            public static EnchantmentSpec Thundering { get; } = new EnchantmentSpec("Thundering", "690e762f7704e1f4aa1ac69ef0ce6a96", Components.EnergyDamageDice);
            public static EnchantmentSpec ThunderingBurst { get; } = new EnchantmentSpec("Thundering Burst", "83bd616525288b34a8f34976b2759ea1", Components.EnergyBurst);
            public static EnchantmentSpec Ultrasound { get; } = new EnchantmentSpec("Ultrasound", "582849db96824254ebcc68f0b7484e51", Components.EnergyDamageDice);
            public static EnchantmentSpec Unholy { get; } = new EnchantmentSpec("Unholy", "d05753b8df780fc4bb55b318f06af453", Components.DamageAgainstAlignment);
            public static EnchantmentSpec Vicious { get; } = new EnchantmentSpec("Vicious", "a1455a289da208144981e4b1ef92cc56", "$AddInitiatorAttackRollTrigger$08ff0ec7-9e2f-4047-938f-4d0f73bfbb38", "$AddUnitFeatureEquipment$52dea738-a3a2-48de-b61d-4ce6e9b068c5", "$AddInitiatorAttackRollTrigger$b82361b4-8c30-489c-bc1e-24ed3f1a500a");

            public static IReadOnlyList<EnchantmentSpec> All { get; } = new[] {
                Adamantine,
                Agile,
                Anarchic,
                Axiomatic,
                BaneAberration,
                BaneAnimal,
                BaneConstruct,
                BaneDragon,
                BaneEverything,
                BaneFey,
                BaneHumanoidGiant,
                BaneHumanoidGiant2d6,
                BaneHumanoidReptilian,
                BaneLiving,
                BaneLongshankEnchant,
                BaneLycanthrope,
                BaneMagicalBeast,
                BaneMonstrousHumanoid,
                BaneOrcGoblin,
                BaneOrcGoblin1d6,
                BaneOutsiderChaotic,
                BaneOutsiderEvil,
                BaneOutsiderGood,
                BaneOutsiderLawful,
                BaneOutsiderNeutral,
                BanePlant,
                BaneUndead,
                BaneVermin,
                BaneVermin1d8,
                Bleed,
                Bliz,
                Brass,
                BrilliantEnergy,
                ColdIron,
                Corrosive,
                Corrosive2d6,
                CorrosiveBurst,
                Cruel,
                CrystalDagger,
                DeathPact,
                Devitalizer,
                Disruption,
                ElderCorrosive,
                ElderCorrosiveBurst,
                ExplosiveFervor,
                FierySpellWeaver,
                Flaming,
                FlamingBurst,
                Frost,
                Frost2d8,
                Furious,
                Furyborn,
                GhostTouch,
                GoreFeaster,
                GreatclubOfSacredCinders,
                Holy,
                Ice2d6,
                IcyBurst,
                Keen,
                MageSlayer,
                Mithral,
                Necrotic,
                Nullifying,
                Oversized,
                Radiant,
                Sacrificial,
                Shock,
                Shock2d6,
                ShockingBurst,
                Speed,
                StrengthComposite,
                StrengthThrown,
                Thundering,
                ThunderingBurst,
                Ultrasound,
                Unholy,
                Vicious
            };

            public static EnchantmentLevel Level { get; } = new EnchantmentLevel(
                new EnchantmentSpec("Level 1", "d42fc23b92c640846ac137dc26e000d4", Components.Level),
                new EnchantmentSpec("Level 2", "eb2faccc4c9487d43b3575d7e77ff3f5", Components.Level),
                new EnchantmentSpec("Level 3", "80bb8a737579e35498177e1e3c75899b", Components.Level),
                new EnchantmentSpec("Level 4", "783d7d496da6ac44f9511011fc5f1979", Components.Level),
                new EnchantmentSpec("Level 5", "bdba267e951851449af552aa9f9e3992", Components.Level),
                new EnchantmentSpec("Level 6", "0326d02d2e24d254a9ef626cc7a3850f", Components.Level)
            );
        }

        public static class Equipment
        {
            public static class Components
            {
                public const string StatBonus = "$AddStatBonusEquipment$666b4f87-5f4e-4833-a7c8-0b087b41efdf";
            }
            public static EnchantmentSpec Constitution4 { get; } = new EnchantmentSpec("Constitution 4", "4b9573fa2b0e74043a4682740e9ae138", Components.StatBonus);
        }

        public static class Armor
        {
            public static class Components
            {
                public const string BonusAC1 = "$ArmorEnhancementBonus$5a486a8c-d492-4a6f-a4c5-d0dcff3699e9";
                public const string BonusAC2 = "$ArmorEnhancementBonus$6d181b1b-f120-4420-8f60-dee4abbeb9f2";
                public const string BonusAC3 = "$ArmorEnhancementBonus$3f3183ee-32cf-422a-93ac-7e868aeb81cf";
                public const string BonusAC4 = "$ArmorEnhancementBonus$4a0d34c2-145f-4439-94cf-db83c3ce40ef";
                public const string BonusAC5 = "$ArmorEnhancementBonus$7f092aa2-3fb6-406c-9f15-6c453cb6e4f4";

                public const string Stats1 = "$AdvanceArmorStats$e260771a-2fbe-4887-a404-c350595e312f";
                public const string Stats2 = "$AdvanceArmorStats$ca2bdcea-5159-4376-bd99-71c422364011";
                public const string Stats3 = "$AdvanceArmorStats$f787b7e4-5fef-434f-bdb5-64c67478736b";
                public const string Stats4 = "$AdvanceArmorStats$8655dc3a-fe68-48d3-9b07-19d000a169e1";
                public const string Stats5 = "$AdvanceArmorStats$8de57ec6-0c32-4f18-b2cc-60c0507b35a6";

                public const string Mithral = "$MithralEnchantment$454f5e24-f45a-413b-a43c-4d43e97a5f3a";
                public const string MithralStats = "$AdvanceArmorStats$d1822197-d530-4b06-ad17-4d0d00d814dc";
                public const string FactEquipment = "$AddFactEquipment$f68e9844-d2e9-45b8-a1ce-46e0ee16369a";
            }

            public static EnchantmentSpec AcidResistance10 { get; } = new EnchantmentSpec("Acid Resistance 10", "dd0e096412423d646929d9b945fd6d4c", Components.FactEquipment);
            public static EnchantmentSpec AcidResistance15 { get; } = new EnchantmentSpec("Acid Resistance 15", "09e0be00530efec4693a913d6a7efe23", Components.FactEquipment);
            public static EnchantmentSpec AcidResistance20 { get; } = new EnchantmentSpec("Acid Resistance 20", "1346633e0ff138148a9a925e330314b5", Components.FactEquipment);
            public static EnchantmentSpec AcidResistance30 { get; } = new EnchantmentSpec("Acid Resistance 30", "e6fa2f59c7f1bb14ebfc429f17d0a4c6", Components.FactEquipment);
            public static EnchantmentSpec AdamantineArmorHeavy { get; } = new EnchantmentSpec("Adamantine Armor Heavy", "933456ff83c454146a8bf434e39b1f93", Components.FactEquipment, "$AdvanceArmorStats$f7609921-9ca8-4eeb-b32e-b7e45ebb9e9a");
            public static EnchantmentSpec AdamantineArmorLight { get; } = new EnchantmentSpec("Adamantine Armor Light", "5faa3aaee432ac444b101de2b7b0faf7", Components.FactEquipment, "$AdvanceArmorStats$a478b692-b697-493f-a9c2-6cc0c8cc640b");
            public static EnchantmentSpec AdamantineArmorMedium { get; } = new EnchantmentSpec("Adamantine Armor Medium", "aa25531ab5bb58941945662aa47b73e7", Components.FactEquipment, "$AdvanceArmorStats$e251b65b-f190-4424-8ac8-332666d32b3f");
            public static EnchantmentSpec AntiToxin { get; } = new EnchantmentSpec("Anti Toxin", "30c370f2385b56045814e2f37b34cc96", Components.FactEquipment);
            public static EnchantmentSpec ArrowCatcher { get; } = new EnchantmentSpec("Arrow Catcher", "2940acde07f54421b8bd137dc7b2a6fa", "$AddUnitFactEquipment$471000aa-2255-4bae-ac09-62afebca8290");
            public static EnchantmentSpec ColdResistance10 { get; } = new EnchantmentSpec("Cold Resistance 10", "c872314ecfab32949ad2e0eebd834919", Components.FactEquipment);
            public static EnchantmentSpec ColdResistance15 { get; } = new EnchantmentSpec("Cold Resistance 15", "581c22e55f03e4e4f9f9ea619d89af5f", Components.FactEquipment);
            public static EnchantmentSpec ColdResistance20 { get; } = new EnchantmentSpec("Cold Resistance 20", "510d87d2a949587469882061ee186522", Components.FactEquipment);
            public static EnchantmentSpec ColdResistance30 { get; } = new EnchantmentSpec("Cold Resistance 30", "7ef70c319ca74fe4cb5eddea792bb353", Components.FactEquipment);
            public static EnchantmentSpec ElectricityResistance10 { get; } = new EnchantmentSpec("Electricity Resistance 10", "1e4dcaf8ffa56c24788e392dae886166", Components.FactEquipment);
            public static EnchantmentSpec ElectricityResistance15 { get; } = new EnchantmentSpec("Electricity Resistance 15", "2ed92b92b5381ef488282eb506170322", Components.FactEquipment);
            public static EnchantmentSpec ElectricityResistance20 { get; } = new EnchantmentSpec("Electricity Resistance 20", "fcfd9515adbd07a43b490280c06203f9", Components.FactEquipment);
            public static EnchantmentSpec ElectricityResistance30 { get; } = new EnchantmentSpec("Electricity Resistance 30", "26b91513989a653458986fabce24ba95", Components.FactEquipment);
            public static EnchantmentSpec EnergyResistance10n10 { get; } = new EnchantmentSpec("Energy Resistance 10neg 10pos", "bc67a01b94164ea4a843028edfcbab01", Components.FactEquipment);
            public static EnchantmentSpec FireResistance10 { get; } = new EnchantmentSpec("Fire Resistance 10", "47f45701cc9545049b3745ef949d7446", Components.FactEquipment);
            public static EnchantmentSpec FireResistance15 { get; } = new EnchantmentSpec("Fire Resistance 15", "85c2f44721922e4409130791f913d4b4", Components.FactEquipment);
            public static EnchantmentSpec FireResistance20 { get; } = new EnchantmentSpec("Fire Resistance 20", "e7af6912cc308df4e9ee63c8824f2738", Components.FactEquipment);
            public static EnchantmentSpec FireResistance30 { get; } = new EnchantmentSpec("Fire Resistance 30", "0e98403449de8ce4c846361c6df30d1f", Components.FactEquipment);
            public static EnchantmentSpec Fortification25 { get; } = new EnchantmentSpec("Fortification 25", "1e69e9029c627914eb06608dad707b36", Components.FactEquipment);
            public static EnchantmentSpec Fortification50 { get; } = new EnchantmentSpec("Fortification 50", "62ec0b22425fb424c82fd52d7f4c02a5", Components.FactEquipment);
            public static EnchantmentSpec Fortification75 { get; } = new EnchantmentSpec("Fortification 75", "9b1538c732e06544bbd955fee570a2be", Components.FactEquipment);
            public static EnchantmentSpec NegativeEnergyResistance10 { get; } = new EnchantmentSpec("Negative Energy Resistance 10", "34504fb2cecda144aaff34929ba10202", Components.FactEquipment);
            public static EnchantmentSpec NegativeEnergyResistance20 { get; } = new EnchantmentSpec("Negative Energy Resistance 20", "1bd448c554f14fc44878bbc983605710", Components.FactEquipment);
            public static EnchantmentSpec NegativeEnergyResistance30 { get; } = new EnchantmentSpec("Negative Energy Resistance 30", "27e95849860301b4ab257f72df627149", Components.FactEquipment);
            public static EnchantmentSpec PositiveEnergyResistance30 { get; } = new EnchantmentSpec("Positive Energy Resistance 30", "80453601b93f0ef43b215087a484d517", Components.FactEquipment);
            public static EnchantmentSpec Mithral { get; } = new EnchantmentSpec("Mithral", "7b95a819181574a4799d93939aa99aff", Components.Mithral, Components.MithralStats);
            public static EnchantmentSpec ShadowArmor { get; } = new EnchantmentSpec("Shadow Armor", "d64d7aa52626bc24da3906dce17dbc7d", "$AddStatBonusEquipment$1d9322ee-f053-4154-931c-c5512a0c34be");
            public static EnchantmentSpec SingingSteel { get; } = new EnchantmentSpec("Singing Steel", "451601816a45311419b77b83f253b75b", Components.Mithral, Components.MithralStats, "$AddUnitFactEquipment$5242bea5-bb52-4e35-89f6-5f6698b76ea5");
            public static EnchantmentSpec SonicResistance10 { get; } = new EnchantmentSpec("Sonic Resistance 10", "6e2dfcafe4faf8941b1426a86a76c368", Components.FactEquipment);
            public static EnchantmentSpec SonicResistance30 { get; } = new EnchantmentSpec("Sonic Resistance 30", "8b940da1e47fb6843aacdeac9410ec41", Components.FactEquipment);
            public static EnchantmentSpec SpellResistance13 { get; } = new EnchantmentSpec("Spell Resistance 13", "4bc20fd0e137e1645a18f030b961ef3d", Components.FactEquipment);
            public static EnchantmentSpec SpellResistance15 { get; } = new EnchantmentSpec("Spell Resistance 15", "ad0f81f6377180d4292a2316efb950f2", Components.FactEquipment);
            public static EnchantmentSpec SpellResistance17 { get; } = new EnchantmentSpec("Spell Resistance 17", "49fe9e1969afd874181ed7613120c250", Components.FactEquipment);
            public static EnchantmentSpec SpellResistance19 { get; } = new EnchantmentSpec("Spell Resistance 19", "583938eaafc820f49ad94eca1e5a98ca", Components.FactEquipment);

            public static IReadOnlyList<EnchantmentSpec> All { get; } = new[] {
                AcidResistance10,
                AcidResistance15,
                AcidResistance20,
                AcidResistance30,
                AdamantineArmorHeavy,
                AdamantineArmorLight,
                AdamantineArmorMedium,
                ColdResistance10,
                ColdResistance15,
                ColdResistance20,
                ColdResistance30,
                ElectricityResistance10,
                ElectricityResistance15,
                ElectricityResistance20,
                ElectricityResistance30,
                EnergyResistance10n10,
                FireResistance10,
                FireResistance15,
                FireResistance20,
                FireResistance30,
                Fortification25,
                Fortification50,
                Fortification75,
                NegativeEnergyResistance10,
                NegativeEnergyResistance20,
                NegativeEnergyResistance30,
                PositiveEnergyResistance30,
                Mithral,
                SingingSteel,
                SonicResistance10,
                SonicResistance30,
                SpellResistance13,
                SpellResistance15,
                SpellResistance17,
                SpellResistance19
            };

            public static EnchantmentLevel Level { get; } = new EnchantmentLevel(
                new EnchantmentSpec("Level 1", "a9ea95c5e02f9b7468447bc1010fe152", Components.BonusAC1, Components.Stats1),
                new EnchantmentSpec("Level 2", "758b77a97640fd747abf149f5bf538d0", Components.BonusAC2, Components.Stats2),
                new EnchantmentSpec("Level 3", "9448d3026111d6d49b31fc85e7f3745a", Components.BonusAC3, Components.Stats3),
                new EnchantmentSpec("Level 4", "eaeb89df5be2b784c96181552414ae5a", Components.BonusAC4, Components.Stats4),
                new EnchantmentSpec("Level 5", "6628f9d77fd07b54c911cd8930c0d531", Components.BonusAC5, Components.Stats5),
                new EnchantmentSpec("Level 6", "de15272d1f4eb7244aa3af47dbb754ef", Components.BonusAC5, Components.Stats5)
            );
        }

        public static class Shield
        {
            public static class Components
            {
                public const string BonusAC1 = "$ArmorEnhancementBonus$bb160059-fd6e-47ec-94ef-966087f9cc72";
                public const string BonusAC2 = "$ArmorEnhancementBonus$66ec610e-e1eb-4298-ae47-13cd1238ff65";
                public const string BonusAC3 = "$ArmorEnhancementBonus$e7e0737e-1113-40e4-ae34-f0aa5521e19d";
                public const string BonusAC4 = "$ArmorEnhancementBonus$f2d70d6c-0969-4751-b3fe-8591d8a1da62";
                public const string BonusAC5 = "$ArmorEnhancementBonus$3b531e97-77d2-4289-952b-19d68e22b272";

                public const string Stats1 = "$AdvanceArmorStats$cfdf702d-673d-4c44-a00f-ec087b438418";
                public const string Stats2 = "$AdvanceArmorStats$9a0cf77a-6124-4adc-a3f4-6da4066baa0e";
                public const string Stats3 = "$AdvanceArmorStats$3db8a64a-d758-4202-8a99-ce362a312979";
                public const string Stats4 = "$AdvanceArmorStats$a903c095-8003-4239-ad87-4a245dec7fae";
                public const string Stats5 = "$AdvanceArmorStats$e517aa19-8ab6-4751-a4d3-e6ab75af94b8";
            }

            public static IReadOnlyList<EnchantmentSpec> All { get; } = new [] {
                Armor.Mithral
            };

            public static EnchantmentLevel Level { get; } = new EnchantmentLevel(
                new EnchantmentSpec("Level 1", "e90c252e08035294eba39bafce76c119", Components.BonusAC1, Components.Stats1),
                new EnchantmentSpec("Level 2", "7b9f2f78a83577d49927c78be0f7fbc1", Components.BonusAC2, Components.Stats2),
                new EnchantmentSpec("Level 3", "ac2e3a582b5faa74aab66e0a31c935a9", Components.BonusAC3, Components.Stats3),
                new EnchantmentSpec("Level 4", "a5d27d73859bd19469a6dde3b49750ff", Components.BonusAC4, Components.Stats4),
                new EnchantmentSpec("Level 5", "84d191a748edef84ba30c13b8ab83bd9", Components.BonusAC5, Components.Stats5)
            //new EnchantmentSpec("Level 6", "70c26c66adb96d74baec38fc8d20c139", "", "")
            );
        }
    }
}