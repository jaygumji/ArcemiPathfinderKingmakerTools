using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public abstract class FeatSpecCollection
    {
        private IReadOnlyDictionary<string, FeatSpec> DefinedLookup { get; }
        private IReadOnlyList<FeatSpec> DefinedAll { get; }

        protected FeatSpecCollection()
        {
            var args = new FeatSpecDefineArgs(this);
            OnDefine(args);
            DefinedAll = args.Finalize();
            DefinedLookup = DefinedAll.ToDictionary(x => x.Id);
        }

        protected abstract void OnDefine(FeatSpecDefineArgs args);

        public class FeatSpecDefineArgs
        {
            private readonly FeatSpecCollection collection;
            private IEnumerable<FeatSpec> feats;
            public FeatSpecDefineArgs(FeatSpecCollection collection)
            {
                feats = Array.Empty<FeatSpec>();
                this.collection = collection;
            }

            public FeatSpecDefineArgs Melee(string name)
            {
                feats = collection.Melee(feats, name);
                return this;
            }

            public FeatSpecDefineArgs Ranged(string name)
            {
                feats = collection.Ranged(feats, name);
                return this;
            }

            public FeatSpecDefineArgs SpellSchool(string name)
            {
                feats = collection.SpellSchool(feats, name);
                return this;
            }

            public IReadOnlyList<FeatSpec> Finalize() => feats.ToArray();
        }

        private IReadOnlyList<IBlueprintMetadataEntry> Source { get; set; }
        private IReadOnlyList<IBlueprintMetadataEntry> Combined { get; set; }

        public bool Is(string blueprint) => DefinedLookup.ContainsKey(blueprint);
        public bool TryGet(string blueprint, out FeatSpec spec) => DefinedLookup.TryGetValue(blueprint, out spec);

        public IReadOnlyList<IBlueprintMetadataEntry> Combine(IReadOnlyList<IBlueprintMetadataEntry> entries)
        {
            if (entries is null) entries = Array.Empty<IBlueprintMetadataEntry>();
            if (Source is object && ReferenceEquals(Source, entries)) {
                return Combined;
            }

            var hashset = new HashSet<string>(DefinedAll.Select(e => e.Name));
            Combined = DefinedAll.Concat(entries.Where(x => !hashset.Contains(x.DisplayName))).ToArray();
            Source = entries;
            return Combined;
        }

        protected FeatSpec New(string name, string blueprint, string paraName, string paraValue)
            => FeatSpec.New(name, blueprint, PathfinderWotr.WotrBlueprintProvider.Feature, paraName, paraValue);
        protected FeatSpec WeaponNew(string featName, string blueprint, string name) => New(string.Concat(featName, " - ", name.AsDisplayable()), blueprint, "WeaponCategory", name);
        protected FeatSpec Focus(string name) => WeaponNew("Weapon Focus", "1e1f627d26ad36f43bbd26cc2bf8ac7e", name);
        protected FeatSpec FocusGreater(string name) => WeaponNew("Weapon Focus Greater", "09c9e82965fb4334b984a1e9df3bd088", name);
        protected FeatSpec Specialization(string name) => WeaponNew("Weapon Specialization", "31470b17e8446ae4ea0dacd6c5817d86", name);
        protected FeatSpec SpecializationGreater(string name) => WeaponNew("Weapon Specialization Greater", "7cf5edc65e785a24f9cf93af987d66b3", name);
        protected FeatSpec MasteryParametrized(string name) => WeaponNew("Weapon Mastery Parametrized", "38ae5ac04463a8947b7c06a6c72dd6bb", name);
        protected FeatSpec ImprovedCritical(string name) => WeaponNew("Improved Critical", "f4201c85a991369408740c6888362e20", name);
        protected FeatSpec SlashingGrace(string name) => WeaponNew("Slashing Grace", "697d64669eb2c0543abb9c9b07998a38", name);
        protected FeatSpec FencingGrace(string name) => WeaponNew("Fencing Grace", "47b352ea0f73c354aba777945760b441", name);
        protected FeatSpec SwordSaintWeapon(string name) => WeaponNew("Sword Saint Weapon", "c0b4ec0175e3ff940a45fc21f318a39a", name);

        protected abstract IEnumerable<FeatSpec> Melee(IEnumerable<FeatSpec> feats, string name);

        protected FeatSpec PointBlankMaster(string name) => WeaponNew("Point Blank Master", "05a3b543b0a0a0346a5061e90f293f0b", name);

        protected abstract IEnumerable<FeatSpec> Ranged(IEnumerable<FeatSpec> feats, string name);

        protected FeatSpec SpellNew(string featName, string blueprint, string name) => New(string.Concat(featName, " - ", name.AsDisplayable()), blueprint, "SpellSchool", name);
        protected FeatSpec SpellFocus(string name) => SpellNew("Spell Focus", "16fa59cc9a72a6043b566b49184f53fe", name);
        protected FeatSpec SpellFocusGreater(string name) => SpellNew("Spell Focus Greater", "5b04b45b228461c43bad768eb0f7c7bf", name);

        protected abstract IEnumerable<FeatSpec> SpellSchool(IEnumerable<FeatSpec> feats, string name);
    }
    public class FeatSpec : IBlueprintMetadataEntry
    {
        private readonly BlueprintName _name;
        public FeatSpec(string name, string blueprint, BlueprintType type, IReadOnlyDictionary<string, string> parameters)
        {
            Id = CreateIdentifier(blueprint, parameters.Select(p => p.Value));
            Name = name;
            Blueprint = blueprint;
            Parameters = parameters;
            _name = new BlueprintName(type, name, name);
        }

        public static string CreateIdentifier(string blueprint, params string[] paraValues) => CreateIdentifier(blueprint, (IEnumerable<string>)paraValues);
        private static string CreateIdentifier(string blueprint, IEnumerable<string> paraValues)
        {
            var actParaValues = paraValues.Where(v => v.HasValue()).ToArray();
            return actParaValues.Length > 0 ? string.Concat(blueprint, '_', string.Join("_", actParaValues)) : blueprint;
        }

        public string Id { get; }
        public string Name { get; }
        public string Blueprint { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }

        BlueprintName IBlueprintMetadataEntry.Name => _name;
        BlueprintType IBlueprintMetadataEntry.Type => _name.Type;
        string IBlueprintMetadataEntry.DisplayName => Name;
        string IBlueprintMetadataEntry.Path => null;

        public void ApplyOn(JObject obj)
        {
            obj["Blueprint"] = Blueprint;
            var context = (JObject)obj.Property("m_Context")?.Value;
            if (context is null) {
                context = new JObject();
                obj.Add("m_Context", context);
            }
            context["AssociatedBlueprint"] = Blueprint;

            if (Parameters.Count > 0) {
                var param = (JObject)obj.Property("Param")?.Value;
                if (param is null) {
                    param = new JObject();
                    obj.Add("Param", param);
                }
                foreach (var p in Parameters) {
                    param.Add(p.Key, p.Value);
                }
            }
        }

        public static FeatSpec New(string name, string blueprint, BlueprintType type, string paraName, string paraValue)
        {
            return new FeatSpec(name, blueprint, type, new Dictionary<string, string> { { paraName, paraValue } });
        }
    }
}
