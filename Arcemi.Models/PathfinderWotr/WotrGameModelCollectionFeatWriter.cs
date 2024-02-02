using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace Arcemi.Models.PathfinderWotr
{
    internal class WotrGameModelCollectionFeatWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            FeatureFactItemModel.Prepare(args.References, args.Obj);
            if (WotrPredefinedFeats.Lookup.TryGetValue(args.Blueprint, out var spec)) {
                spec.ApplyOn(args.Obj);
            }
        }

        public override void AfterAdd(AfterAddCollectionItemArgs<IGameUnitFeatEntry, FactItemModel> args)
        {
            if (WotrPredefinedFeats.Lookup.TryGetValue(args.Blueprint, out _)) return;

            var template = Res.GetFeatTemplate(args.Blueprint);
            if (template is object) {
                args.Model.Import(template);
            }
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitFeatEntry> current)
        {
            var hashset = new HashSet<string>(current.Select(x => {
                var wfeat = (WotrGameUnitFeatEntry)x;
                return FeatSpec.CreateIdentifier(wfeat.Blueprint, wfeat.Model.Param?.SpellSchool, wfeat.Model.Param?.WeaponCategory);
            }), StringComparer.Ordinal);

            foreach (var feat in WotrPredefinedFeats.All) hashset.Add(feat.Blueprint);

            return Res.Blueprints.GetEntries(BlueprintTypeId.Feature).Where(x => !hashset.Contains(x.Id))
                .Concat(WotrPredefinedFeats.All.Where(x => !hashset.Contains(x.Id)))
                .ToArray();
        }
    }
    public static class WotrPredefinedFeats
    {
        public static IReadOnlyDictionary<string, FeatSpec> Lookup { get; }
        public static IReadOnlyList<FeatSpec> All { get; }

        private static IEnumerable<FeatSpec> Define()
        {
            return Enumerable.Empty<FeatSpec>();
        }
        private static FeatSpec New(string name, string blueprint, string paraName, string paraValue)
            => FeatSpec.New(name, blueprint, PathfinderWotr.WotrBlueprintProvider.Feature, paraName, paraValue);
        private static IEnumerable<FeatSpec> Weapon(this IEnumerable<FeatSpec> feats, string name)
        {
            var displayName = name.AsDisplayable();
            const string paramKey = "WeaponCategory";
            FeatSpec Focus() => New("Weapon Focus " + displayName, "1e1f627d26ad36f43bbd26cc2bf8ac7e", paramKey, name);
            FeatSpec FocusGreater() => New("Weapon Focus Greater " + displayName, "09c9e82965fb4334b984a1e9df3bd088", paramKey, name);
            FeatSpec FocusMythic() => New("Weapon Focus Mythic " + displayName, "74eb201774bccb9428ba5ac8440bf990", paramKey, name);
            FeatSpec Specialization() => New("Weapon Specialization " + displayName, "31470b17e8446ae4ea0dacd6c5817d86", paramKey, name);
            FeatSpec SpecializationGreater() => New("Weapon Specialization Greater " + displayName, "7cf5edc65e785a24f9cf93af987d66b3", paramKey, name);
            FeatSpec SpecializationMythic() => New("Weapon Specialization Mythic " + displayName, "d84ac5b1931bc504a98bfefaa419e34f", paramKey, name);
            FeatSpec MasteryParametrized() => New("Weapon Mastery Parametrized " + displayName, "38ae5ac04463a8947b7c06a6c72dd6bb", paramKey, name);
            FeatSpec ImprovedCritical() => New("Improved Critical " + displayName, "f4201c85a991369408740c6888362e20", paramKey, name);
            FeatSpec TricksterImprovedImprovedCritical() => New("Trickster Improved Improved Critical " + displayName, "56f94badbba018b4b8277ce6e2e79e72", paramKey, name);
            FeatSpec TricksterImprovedImprovedImprovedCritical() => New("Trickster Improved Improved Improved Critical " + displayName, "006a966007802a0478c9e21007207aac", paramKey, name);
            FeatSpec TricksterImprovedImprovedImprovedImprovedCritical() => New("Trickster Improved Improved Improved Improved Critical " + displayName, "319c882ab3cc51544ad2f3f43633d5b1", paramKey, name);
            FeatSpec ImprovedCriticalMythic() => New("Improved Critical Mythic " + displayName, "8bc0190a4ec04bd489eec290aeaa6d07", paramKey, name);

            return feats.Concat(new[] {
                Focus(), FocusGreater(), FocusMythic(),
                Specialization(), SpecializationGreater(), SpecializationMythic(), MasteryParametrized(),
                ImprovedCritical(), ImprovedCriticalMythic(),
                TricksterImprovedImprovedCritical(), TricksterImprovedImprovedImprovedCritical(), TricksterImprovedImprovedImprovedImprovedCritical()
            });
        }

        private static IEnumerable<FeatSpec> SpellSchool(this IEnumerable<FeatSpec> feats, string name)
        {
            var displayName = name.AsDisplayable();
            const string paramKey = "SpellSchool";
            FeatSpec ExpandedArsenalSchool() => New("Expanded Arsenal School " + displayName, "f137089c48364014aa3ec3b92ccaf2e2", paramKey, name);
            FeatSpec SchoolMasteryMythic() => New("School Mastery Mythic " + displayName, "ac830015569352b458efcdfae00a948c", paramKey, name);
            FeatSpec FocusMythic() => New("Spell Focus Mythic " + displayName, "41fa2470ab50ff441b4cfbb2fc725109", paramKey, name);

            return feats.Concat(new[] {
                ExpandedArsenalSchool(),
                SchoolMasteryMythic(),
                FocusMythic()
            });
        }

        static WotrPredefinedFeats()
        {
            All = Define()
                .Weapon("Bardiche")
                .Weapon("BastardSword")
                .Weapon("Battleaxe")
                .Weapon("Bite")
                .Weapon("Claw")
                .Weapon("Club")
                .Weapon("Dagger")
                .Weapon("DoubleAxe")
                .Weapon("DoubleSword")
                .Weapon("DuelingSword")
                .Weapon("Urgrosh")
                .Weapon("DwarvenWaraxe")
                .Weapon("EarthBreaker")
                .Weapon("ElvenCurveBlade")
                .Weapon("Estoc")
                .Weapon("Falcata")
                .Weapon("Falchion")
                .Weapon("Fauchard")
                .Weapon("Flail")
                .Weapon("Glaive")
                .Weapon("Gore")
                .Weapon("Greataxe")
                .Weapon("Greatclub")
                .Weapon("Greatsword")
                .Weapon("Handaxe")
                .Weapon("HeavyFlail")
                .Weapon("HeavyMace")
                .Weapon("HeavyPick")
                .Weapon("Hoof")
                .Weapon("HookedHammer")
                .Weapon("IncorporealTouch")
                .Weapon("Kama")
                .Weapon("Kukri")
                .Weapon("LightHammer")
                .Weapon("LightMace")
                .Weapon("LightPick")
                .Weapon("LightShield")
                .Weapon("Longspear")
                .Weapon("Longsword")
                .Weapon("Nunchaku")
                .Weapon("PunchingDagger")
                .Weapon("Quarterstaff")
                .Weapon("Rapier")
                .Weapon("Sai")
                .Weapon("Scimitar")
                .Weapon("Scythe")
                .Weapon("Shortspear")
                .Weapon("Shortsword")
                .Weapon("Sickle")
                .Weapon("Slam")
                .Weapon("Spear")
                .Weapon("Spike")
                .Weapon("Starknife")
                .Weapon("Sting")
                .Weapon("Tail")
                .Weapon("Talon")
                .Weapon("Tentacle")
                .Weapon("Tongi")
                .Weapon("Trident")
                .Weapon("Warhammer")
                .Weapon("Unarmed")
                .Weapon("Wing")

                // Ranged
                .Weapon("Bomb")
                .Weapon("Dart")
                .Weapon("HeavyCrossbow")
                .Weapon("Javelin")
                .Weapon("LightCrossbow")
                .Weapon("Longbow")
                .Weapon("Ray")
                .Weapon("Shortbow")
                .Weapon("SlingStaff")
                .Weapon("ThrowingAxe")
                .Weapon("Touch")

                // Spell Schools
                .SpellSchool("Abjuration")
                .SpellSchool("Conjuration")
                .SpellSchool("Divination")
                .SpellSchool("Enchantment")
                .SpellSchool("Evocation")
                .SpellSchool("Illusion")
                .SpellSchool("Necromancy")
                .SpellSchool("Transmutation")

                .ToArray();

            Lookup = All.ToDictionary(x => x.Id);
        }
    }

}