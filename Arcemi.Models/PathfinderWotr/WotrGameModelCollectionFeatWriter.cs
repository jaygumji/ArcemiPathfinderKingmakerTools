using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameModelCollectionFeatWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            FeatureFactItemModel.Prepare(args.References, args.Obj);
            args.Obj.Add("Blueprint", args.Blueprint);
            if (args.Blueprint.HasValue() && WotrPredefinedFeats.Instance.TryGet(args.Blueprint, out var spec)) {
                spec.ApplyOn(args.Obj);
            }
        }

        public override void AfterAdd(AfterAddCollectionItemArgs<IGameUnitFeatEntry, FactItemModel> args)
        {
            if (args.Blueprint.HasValue()) {
                if (WotrPredefinedFeats.Instance.Is(args.Blueprint)) return;

                var template = Res.GetFeatTemplate(args.Blueprint);
                if (template is object) {
                    args.Model.Import(template);
                }
            }
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitFeatEntry> current)
        {
            var hashset = new HashSet<string>(current.Select(x => {
                var wfeat = (WotrGameUnitFeatEntry)x;
                return FeatSpec.CreateIdentifier(wfeat.Blueprint, wfeat.Model.Param?.SpellSchool, wfeat.Model.Param?.WeaponCategory);
            }), StringComparer.Ordinal);

            var entries = WotrPredefinedFeats.Instance.Combine(Res.Blueprints.GetEntries(BlueprintTypeId.Feature));
            return entries.Where(x => !hashset.Contains(x.Id)).ToArray();
        }
    }
    public class WotrPredefinedFeats : FeatSpecCollection
    {
        public static WotrPredefinedFeats Instance { get; } = new WotrPredefinedFeats();
        private WotrPredefinedFeats() { }

        private FeatSpec FocusMythic(string name) => WeaponNew("Weapon Focus Mythic", "74eb201774bccb9428ba5ac8440bf990", name);
        private FeatSpec SpecializationMythic(string name) => WeaponNew("Weapon Specialization Mythic", "d84ac5b1931bc504a98bfefaa419e34f", name);
        private FeatSpec TricksterImprovedImprovedCritical(string name) => WeaponNew("Trickster Improved Improved Critical", "56f94badbba018b4b8277ce6e2e79e72", name);
        private FeatSpec TricksterImprovedImprovedImprovedCritical(string name) => WeaponNew("Trickster Improved Improved Improved Critical", "006a966007802a0478c9e21007207aac", name);
        private FeatSpec TricksterImprovedImprovedImprovedImprovedCritical(string name) => WeaponNew("Trickster Improved Improved Improved Improved Critical", "319c882ab3cc51544ad2f3f43633d5b1", name);
        private FeatSpec ImprovedCriticalMythic(string name) => WeaponNew("Improved Critical Mythic", "8bc0190a4ec04bd489eec290aeaa6d07", name);

        protected override IEnumerable<FeatSpec> Melee(IEnumerable<FeatSpec> feats, string name)
        {
            return feats.Concat(new[] {
                Focus(name), FocusGreater(name), FocusMythic(name),
                Specialization(name), SpecializationGreater(name), SpecializationMythic(name), MasteryParametrized(name),
                ImprovedCritical(name), ImprovedCriticalMythic(name),
                TricksterImprovedImprovedCritical(name), TricksterImprovedImprovedImprovedCritical(name), TricksterImprovedImprovedImprovedImprovedCritical(name),
                SlashingGrace(name), FencingGrace(name)
            });
        }

        protected override IEnumerable<FeatSpec> Ranged(IEnumerable<FeatSpec> feats, string name)
        {
            return feats.Concat(new[] {
                Focus(name), FocusGreater(name), FocusMythic(name),
                Specialization(name), SpecializationGreater(name), SpecializationMythic(name), MasteryParametrized(name),
                ImprovedCritical(name), ImprovedCriticalMythic(name),
                TricksterImprovedImprovedCritical(name), TricksterImprovedImprovedImprovedCritical(name), TricksterImprovedImprovedImprovedImprovedCritical(name),
                PointBlankMaster(name)
            });
        }

        private FeatSpec ExpandedArsenalSchool(string name) => SpellNew("Expanded Arsenal School", "f137089c48364014aa3ec3b92ccaf2e2", name);
        private FeatSpec SchoolMasteryMythic(string name) => SpellNew("School Mastery Mythic", "ac830015569352b458efcdfae00a948c", name);
        private FeatSpec SpellFocusMythic(string name) => SpellNew("Spell Focus Mythic", "41fa2470ab50ff441b4cfbb2fc725109", name);

        protected override IEnumerable<FeatSpec> SpellSchool(IEnumerable<FeatSpec> feats, string name)
        {
            return feats.Concat(new[] {
                ExpandedArsenalSchool(name),
                SchoolMasteryMythic(name),
                SpellFocusMythic(name),
                SpellFocus(name),
                SpellFocusGreater(name),
            });
        }

        protected override void OnDefine(FeatSpecDefineArgs args)
        {
            args
                .Melee("Bardiche")
                .Melee("BastardSword")
                .Melee("Battleaxe")
                .Melee("Bite")
                .Melee("Claw")
                .Melee("Club")
                .Melee("Dagger")
                .Melee("DoubleAxe")
                .Melee("DoubleSword")
                .Melee("DuelingSword")
                .Melee("Urgrosh")
                .Melee("DwarvenWaraxe")
                .Melee("EarthBreaker")
                .Melee("ElvenCurveBlade")
                .Melee("Estoc")
                .Melee("Falcata")
                .Melee("Falchion")
                .Melee("Fauchard")
                .Melee("Flail")
                .Melee("Glaive")
                .Melee("Gore")
                .Melee("Greataxe")
                .Melee("Greatclub")
                .Melee("Greatsword")
                .Melee("Handaxe")
                .Melee("HeavyFlail")
                .Melee("HeavyMace")
                .Melee("HeavyPick")
                .Melee("Hoof")
                .Melee("HookedHammer")
                .Melee("IncorporealTouch")
                .Melee("Kama")
                .Melee("Kukri")
                .Melee("LightHammer")
                .Melee("LightMace")
                .Melee("LightPick")
                .Melee("LightShield")
                .Melee("Longspear")
                .Melee("Longsword")
                .Melee("Nunchaku")
                .Melee("PunchingDagger")
                .Melee("Quarterstaff")
                .Melee("Rapier")
                .Melee("Sai")
                .Melee("Scimitar")
                .Melee("Scythe")
                .Melee("Shortspear")
                .Melee("Shortsword")
                .Melee("Sickle")
                .Melee("Slam")
                .Melee("Spear")
                .Melee("Spike")
                .Melee("Starknife")
                .Melee("Sting")
                .Melee("Tail")
                .Melee("Talon")
                .Melee("Tentacle")
                .Melee("Tongi")
                .Melee("Trident")
                .Melee("Warhammer")
                .Melee("Unarmed")
                .Melee("Wing")

                // Ranged
                .Ranged("Bomb")
                .Ranged("Dart")
                .Ranged("HeavyCrossbow")
                .Ranged("Javelin")
                .Ranged("LightCrossbow")
                .Ranged("Longbow")
                .Ranged("Ray")
                .Ranged("Shortbow")
                .Ranged("SlingStaff")
                .Ranged("ThrowingAxe")
                .Ranged("Touch")

                // Spell Schools
                .SpellSchool("Abjuration")
                .SpellSchool("Conjuration")
                .SpellSchool("Divination")
                .SpellSchool("Enchantment")
                .SpellSchool("Evocation")
                .SpellSchool("Illusion")
                .SpellSchool("Necromancy")
                .SpellSchool("Transmutation");
        }
    }

}