using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameUnitSelectionProgressionEntry : IGameUnitSelectionProgressionEntry
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Warhammer40K_RogueTrader.Resources;
        public W40KRTGameUnitSelectionProgressionEntry(IGameUnitModel owner, UnitProgressionSelectionOfPartModel selection)
        {
            Owner = owner;
            Ref = selection;

            if (Ref.Selection.Eq("10fefb03369d430e88b65aabaf68deac")) { // Homeworld
                Options = new List<BlueprintOption> {
                    new BlueprintOption("6efb99601c094cbd94d4e6e29dab0970", "Void born"),
                    new BlueprintOption("e21cd160c88649c39b6c693078c384ef", "Unknown"),
                    new BlueprintOption("f41ee79802b04d448daec83dd5e8163f", "Leira"),
                    new BlueprintOption("d7953c4cbf47463090ee3025ef390063", "Commoragh"),
                    new BlueprintOption("def5bfa5b1a947f6a1764029f06b58f1", "Forge World"),
                    new BlueprintOption("a6e871aa095f4a1fa813fab77658ab78", "Craft World"),
                    new BlueprintOption("5ab7897d0830402ba9809eca3df21d58", "Death World"),
                    new BlueprintOption("2ef8b22a98c049a5b4f44864c1d1b642", "Fenris"),
                    new BlueprintOption("b504a419af314de1ab85c86bc8b54907", "Fortress World"),
                    new BlueprintOption("442ffe5d7db542b39f1f6f55e254c621", "Hive World"),
                    new BlueprintOption("af0650bf35124dd2ae16c87f7d376bdc", "Imperial World Feudal"),
                    new BlueprintOption("927671f69b384423af7c5faa1fa1dbd5", "Imperial World"),
                };
            }
            else if (Ref.Selection.Eq("ff001e095e7240ac99b40ceb2bdadf0a")) { // Occupation
                Options = new List<BlueprintOption> {
                    new BlueprintOption("1518d1434ed646039215da3fdda6b096", "Sanctioned Psyker"),
                    new BlueprintOption("395a77ff6fd344f5b8b4a0cc0def06dc", "Unsanctioned Psyker"),
                    new BlueprintOption("a69ab12837ae4bfea6bb56f834892d7f", "Space Marine"),
                    new BlueprintOption("4b908491051a4f36b9703b95e048a5a3", "Astra Militarum"),
                    new BlueprintOption("00b183680643424abe015263aac81c5b", "Commissar"),
                    new BlueprintOption("8fab55c9130a4ae0a745f4fa1674c5df", "Criminal"),
                    new BlueprintOption("d840a5dc947546e0b4ac939287191fd8", "Ministorum Crusader"),
                    new BlueprintOption("962c310fd1664ae996c759e4d11a2d88", "Navy Officer"),
                    new BlueprintOption("06180233245249eea90d222bb1c13f00", "Nobility"),
                    new BlueprintOption("abe45adeb7d7415ca96df8fc6cd1acd2", "Kabalite Dracon"),
                    new BlueprintOption("33497e0597e64570bb5cf78b19a95d96", "Ranger"),
                    new BlueprintOption("8e0cfa654ec24dbbba9e80c27433cc8e", "Navigator"),
                    new BlueprintOption("31d25d8b646c454a8fbc17bc8f775c2c", "Tech Priest"),
                    new BlueprintOption("b53037d92c984cf3921df309241e48ca", "Abelard (Navy Officer)"),
                    new BlueprintOption("b6962fcc54054af98961dd9a6c0f9e18", "Argenta (Adepta Sororitas)"),
                    new BlueprintOption("777d9f9c570443b59120e78f2d9dd515", "Pasqual (Tech-Priest)"),
                    new BlueprintOption("176a50ee98944e939d3959841e3eb269", "Heinrix (Sanctioned Psyker)"),
                    new BlueprintOption("aa932c209cdd43c9bb749d5380fc126e", "Jae (Cold Trader)"),

                    //new BlueprintOption("53970cc124ea433f93baa41028a8781a", "Comissar (Duplicate?)"),
                    //new BlueprintOption("b23606b3443b42e490951c03203e0a10", "Criminal (Duplicate?)"),
                    //new BlueprintOption("90e3b428e6ec45d49a429c3abc288b0b", "Fake"),
                    //new BlueprintOption("b152728dc5a24a2ab6ca8b3ff20c0b71", "Navy Officer (Duplicate?)"),
                    //new BlueprintOption("0ea28de90038465885d1d71a7e297ee4", "Empty"),
                };
            }
            else if (Ref.Selection.Eq("a54affd2f8404dbcbffc3e0312061b17")) { // Darkest Hour
                Options = new List<BlueprintOption> {
                    new BlueprintOption("04f4eca5cad54087a648a05168cbea96", "Dark Signs Commissar"),
                    new BlueprintOption("6c68b0b4d1a64b2791473d4daaa50aec", "Dark Signs Criminal"),
                    new BlueprintOption("7b7ece038d3343bfbf31646fecca3ea3", "Dark Signs Ecclesiarchy"),
                    new BlueprintOption("47c83106aabf4e7f98597c6e12cc80c6", "Dark Signs Militarum"),
                    new BlueprintOption("4385c080dade487fa7b743b5e2600662", "Dark Signs Navy"),
                    new BlueprintOption("12c37b4a75944ce79edaa02fac6dd751", "Dark Signs Nobility"),
                    new BlueprintOption("5b6e0dff115643559d0eba1f1328ace7", "Dark Signs Psyker"),
                    new BlueprintOption("8a83889572ac45bd80a9941a9cd0e9b8", "Shame Commissar"),
                    new BlueprintOption("368650db0ef647fd8e62b680386dc071", "Shame Criminal"),
                    new BlueprintOption("a88fa4c3b3de42408a1030e864a28c18", "Shame Ecclesiarchy"),
                    new BlueprintOption("bdb92a5b23374aa5be57e0e37f76e65f", "Shame Militarum"),
                    new BlueprintOption("fd5566807b6f4a79ad0972fb97d3cb9b", "Shame Navy"),
                    new BlueprintOption("139f390bf5c84cee95846301020baeaf", "Shame Nobility"),
                    new BlueprintOption("a3c6bfc6917a49718a333f1e3ed4b2aa", "Shame Psyker"),
                    new BlueprintOption("a609de0f250d499fbaffcfd37c59c5fd", "Tortures Commissar"),
                    new BlueprintOption("9380ef2326334b2d95e1eeacbb785cef", "Tortures Criminal"),
                    new BlueprintOption("7f05891ab82b48b2b9aef99722024591", "Tortures Ecclesiarchy"),
                    new BlueprintOption("78797666bb9f4a4a81dea0a6e9c21d65", "Tortures Militarum"),
                    new BlueprintOption("4a142f95698445c9a4ad22b921612d6d", "Tortures Navy"),
                    new BlueprintOption("2e0e2530061f4d97abd7ba11f9e171e6", "Tortures Nobility"),
                    new BlueprintOption("cc456183aedf4616aaa44040cb08bcb7", "Tortures Psyker"),
                };
            }
            else if (Ref.Selection.Eq("d97cf3c474034359851b1a8ffba73715")) { // Moment Of Triumph
                Options = new List<BlueprintOption> {
                    new BlueprintOption("7dbb4d8408c34bf0a840b277feafa021", "Feat Of Mind Commisar"),
                    new BlueprintOption("12af6c0764aa4cd0a51719cb16dd56be", "Feat Of Mind Criminal"),
                    new BlueprintOption("53befc9fdc6f4856ae8aaa14d67356ca", "Feat Of Mind Ecclesiarchy"),
                    new BlueprintOption("eed7083c3cda430bb03a38063576fcf2", "Feat Of Mind Militarum"),
                    new BlueprintOption("18de8919406c4d589a0b74a512f04608", "Feat Of Mind Navy"),
                    new BlueprintOption("83b0ed54c3ed48129b91860e873c4952", "Feat Of Mind Nobility"),
                    new BlueprintOption("fd2e95e7e1254088a4959bde4f219105", "Feat Of Mind Psyker"),
                    new BlueprintOption("d92ccc0f17a14e8e8df0082edbd0427a", "Glory Commissar"),
                    new BlueprintOption("ad3286f82b9044968f64264c9f431068", "Glory Criminal"),
                    new BlueprintOption("5d6f71aa393340908f26b27474021fd6", "Glory Ecclesiarchy"),
                    new BlueprintOption("dd15357977564606a598b5454f4fb37c", "Glory Militarum"),
                    new BlueprintOption("c73eca5c4570412097a2399a98403baf", "Glory Navy"),
                    new BlueprintOption("57956895f8a1469e8cb5c27a6a5fcd52", "Glory Nobility"),
                    new BlueprintOption("66a1900bac4f4c2db6bfd1254b10126d", "Glory Psyker"),
                    new BlueprintOption("36928a81856640ea9296904a7b9f2178", "Great Deed Commissar"),
                    new BlueprintOption("9aa6694a186d4af6b64d0ead2a4c312c", "Great Deed Criminal"),
                    new BlueprintOption("675cb7de11ec49559ca1a5ef314b8676", "Great Deed Ecclesiarchy"),
                    new BlueprintOption("4307806003254495a4ab2d72cdc17afe", "Great Deed Militarum"),
                    new BlueprintOption("cea0b1fe9e854f129e1aa37d2c8526f1", "Great Deed Navy"),
                    new BlueprintOption("f244d8cd9fd74a8594e6ee6d81e6ed3f", "Great Deed Nobility"),
                    new BlueprintOption("c160ccb31fc64d70bee6f9860aebc007", "Great Deed Psyker"),
                };
            }
            else if (Ref.Selection.Eq(W40KRTArchetypes.Tier1.SelectionId)) { // Career Path
                Options = new List<BlueprintOption>(W40KRTArchetypes.Tier1.SelectionOptions);
            }
            else {
                Options = new List<BlueprintOption>();
            }
            if (!Options.Any(o => o.Id.Eq(Feature))) {
                ((List<BlueprintOption>)Options).Insert(0, new BlueprintOption(Feature, Res.Blueprints.TryGet(Feature, out var entry) ? NameWithout(entry.DisplayName, Name) : Feature + " (Unknown)"));
                IsReadOnly = true;
            }
            ((List<BlueprintOption>)Options).Sort((l, r) => StringComparer.CurrentCultureIgnoreCase.Compare(l.Name, r.Name));
        }

        public string Name => NameWithout(Res.Blueprints.GetNameOrBlueprint(Ref.Selection), "Selection");

        public string Feature
        {
            get => Ref.Feature;
            set {
                var oldValue = Ref.Feature;
                Ref.Feature = value;
                if (Ref.Selection.Eq(W40KRTArchetypes.Tier1.SelectionId)) {
                    var oldType = W40KRTArchetypes.Tier1.Get(oldValue);
                    if (oldType is null) return;

                    var type = W40KRTArchetypes.Tier1.Get(value);
                    if (type is null) return;

                    var feats = Owner.Feats.Where(f => W40KRTArchetypes.Tier1.Has(f.Blueprint)).ToArray();
                    foreach (var feat in feats) {
                        Owner.Feats.Remove(feat);
                    }
                    Owner.Feats.AddByBlueprint(type.SelectionId);
                    Owner.Feats.AddByBlueprint(type.CareerPathId);
                    foreach (var keystoneAbilityId in type.KeystoneAbilityIds) {
                        Owner.Feats.AddByBlueprint(keystoneAbilityId);
                    }

                    var part = ((W40KRTGameUnitProgressionModel)Owner.Progression).Model;
                    var oldSelections = part.Selections.Where(x => x.Path.Eq(Ref.Path)).ToArray();
                    foreach (var selection in oldSelections) {
                        part.Selections.Remove(selection);
                    }
                }
                else {
                    var old = Options.FirstOrDefault(o => o.Id.Eq(oldValue));
                    if (old is null) return;

                    var @new = Options.FirstOrDefault(o => o.Id.Eq(value));
                    if (@new is null) return;

                    var feats = Owner.Feats.Where(f => f.Blueprint.Eq(old.Id)).ToArray();
                    foreach (var feat in feats) {
                        Owner.Feats.Remove(feat);
                    }
                    Owner.Feats.AddByBlueprint(@new.Id);
                }
            }
        }
        public IReadOnlyList<BlueprintOption> Options { get; }
        public bool IsReadOnly { get; }

        public IGameUnitModel Owner { get; }
        public UnitProgressionSelectionOfPartModel Ref { get; }

        private string NameWithout(string name, string suffix)
        {
            if (name == null) return name;

            suffix = " " + suffix;
            if (name.IEnd(suffix)) return name.Remove(name.Length - suffix.Length);

            return name;
        }
    }
}