using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class FeatSpec
    {
        public FeatSpec(string name, string blueprint, IReadOnlyDictionary<string, string> parameters)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name;
            Blueprint = blueprint;
            Parameters = parameters;
        }

        public string Id { get; }
        public string Name { get; }
        public string Blueprint { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }

        public void AddTo(ListAccessor<FactItemModel> feats)
        {
            var exists = feats.OfType<FeatureFactItemModel>().Any(f => {
                if (!f.Blueprint.Eq(Blueprint)) return false;
                if (Parameters.Count <= 0) return true;

                var a = f.Param?.GetAccessor();
                if (a is null) return false;
                foreach (var p in Parameters) {
                    var paramValue = a.Value<string>(p.Key);
                    if (!p.Value.Eq(paramValue)) {
                        return false;
                    }
                }
                return true;
            });
            if (exists) return;

            JObject param = null;
            var feat = (FeatureFactItemModel)feats.Add((r, o) => {
                FeatureFactItemModel.Prepare(r, o);
                o.Add("m_Context", new JObject());
                if (Parameters.Count > 0) {
                    param = new JObject();
                    o.Add("Param", param);
                }
            });
            feat.Blueprint = Blueprint;
            feat.Context.AssociatedBlueprint = Blueprint;
            foreach (var p in Parameters) {
                param.Add(p.Key, p.Value);
            }
        }

        public static FeatSpec New(string name, string blueprint, string paraName, string paraValue)
        {
            return new FeatSpec(name, blueprint, new Dictionary<string, string> { { paraName, paraValue } });
        }
    }
    public static class PredefinedFeats
    {
        private static IReadOnlyDictionary<string, FeatSpec> AllLookup { get; }
        public static IEnumerable<FeatSpec> All => AllLookup.Values;

        public static bool TryGet(string id, out FeatSpec spec) => AllLookup.TryGetValue(id, out spec);

        private static int MetadataHash;
        private static IEnumerable<SelectableFeat> Combined;
        public static IEnumerable<SelectableFeat> Combine(IEnumerable<IBlueprintMetadataEntry> entries)
        {
            var hash = entries.GetHashCode();
            if (Combined is object && hash == MetadataHash) return Combined;

            MetadataHash = hash;
            var entriesLookup = new HashSet<string>(entries.Select(e => e.DisplayName));
            Combined = All.Where(s => !entriesLookup.Contains(s.Name)).Select(s => new SelectableFeat(s))
                .Concat(entries.Select(e => new SelectableFeat(e)))
                .OrderBy(s => s.DisplayName)
                .ToArray();
            return Combined;
        }

        private static IEnumerable<FeatSpec> Define()
        {
            return Enumerable.Empty<FeatSpec>();
        }
        private static IEnumerable<FeatSpec> Weapon(this IEnumerable<FeatSpec> feats, string name)
        {
            var displayName = name.AsDisplayable();
            const string paramKey = "WeaponCategory";
            FeatSpec Focus() => FeatSpec.New("Weapon Focus " + displayName, "1e1f627d26ad36f43bbd26cc2bf8ac7e", paramKey, name);
            FeatSpec FocusGreater() => FeatSpec.New("Weapon Focus Greater " + displayName, "09c9e82965fb4334b984a1e9df3bd088", paramKey, name);
            FeatSpec FocusMythic() => FeatSpec.New("Weapon Focus Mythic " + displayName, "74eb201774bccb9428ba5ac8440bf990", paramKey, name);
            FeatSpec Specialization() => FeatSpec.New("Weapon Specialization " + displayName, "31470b17e8446ae4ea0dacd6c5817d86", paramKey, name);
            FeatSpec SpecializationGreater() => FeatSpec.New("Weapon Specialization Greater " + displayName, "7cf5edc65e785a24f9cf93af987d66b3", paramKey, name);
            FeatSpec SpecializationMythic() => FeatSpec.New("Weapon Specialization Mythic " + displayName, "d84ac5b1931bc504a98bfefaa419e34f", paramKey, name);
            FeatSpec MasteryParametrized() => FeatSpec.New("Weapon Mastery Parametrized " + displayName, "38ae5ac04463a8947b7c06a6c72dd6bb", paramKey, name);
            FeatSpec ImprovedCritical() => FeatSpec.New("Improved Critical " + displayName, "f4201c85a991369408740c6888362e20", paramKey, name);
            FeatSpec TricksterImprovedImprovedCritical() => FeatSpec.New("Trickster Improved Improved Critical " + displayName, "56f94badbba018b4b8277ce6e2e79e72", paramKey, name);
            FeatSpec TricksterImprovedImprovedImprovedCritical() => FeatSpec.New("Trickster Improved Improved Improved Critical " + displayName, "006a966007802a0478c9e21007207aac", paramKey, name);
            FeatSpec TricksterImprovedImprovedImprovedImprovedCritical() => FeatSpec.New("Trickster Improved Improved Improved Improved Critical " + displayName, "319c882ab3cc51544ad2f3f43633d5b1", paramKey, name);
            FeatSpec ImprovedCriticalMythic() => FeatSpec.New("Improved Critical Mythic " + displayName, "8bc0190a4ec04bd489eec290aeaa6d07", paramKey, name);

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
            FeatSpec ExpandedArsenalSchool() => FeatSpec.New("Expanded Arsenal School " + displayName, "f137089c48364014aa3ec3b92ccaf2e2", paramKey, name);
            FeatSpec SchoolMasteryMythic() => FeatSpec.New("School Mastery Mythic " + displayName, "ac830015569352b458efcdfae00a948c", paramKey, name);
            FeatSpec FocusMythic() => FeatSpec.New("Spell Focus Mythic " + displayName, "41fa2470ab50ff441b4cfbb2fc725109", paramKey, name);

            return feats.Concat(new[] {
                ExpandedArsenalSchool(),
                SchoolMasteryMythic(),
                FocusMythic()
            });
        }

        static PredefinedFeats()
        {
            AllLookup = Define()
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

                .ToDictionary(x => x.Id, StringComparer.Ordinal);
        }
    }
}
