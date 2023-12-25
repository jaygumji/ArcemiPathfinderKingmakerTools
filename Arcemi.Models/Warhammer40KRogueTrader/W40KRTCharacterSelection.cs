using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTCharacterSelectionOption : BlueprintOption
    {
        public W40KRTCharacterSelectionOption(string id, string name, string subSelectionId = null)
            : base(id, name)
        {
            SubSelectionId = subSelectionId;
        }

        public void Select(IGameUnitSelectionProgressionEntry selectionEntry)
        {
            OnSelect(selectionEntry);
        }

        protected virtual void OnSelect(IGameUnitSelectionProgressionEntry selectionEntry)
        {
            var @ref = (W40KRTGameUnitSelectionProgressionEntry)selectionEntry;
            var owner = @ref.Owner;
            var selection = @ref.Ref;
            var oldFeature = selection.Feature;
            selection.Feature = Id;
            if (selection.Feature.HasValue()) {
                var count = owner.Progression.Selections.Count(x => x.Feature.Eq(Id));
                var feat = owner.Feats.FirstOrDefault(f => f.Blueprint.Eq(Id)) ?? owner.Feats.AddByBlueprint(Id);
                feat.Rank = count;
            }

            if (oldFeature.HasValue()) {
                var oldCount = owner.Progression.Selections.Count(x => x.Feature.Eq(oldFeature));
                var feat = owner.Feats.FirstOrDefault(f => f.Blueprint.Eq(oldFeature));
                if (feat is object) {
                    if (oldCount == 0)
                        owner.Feats.Remove(feat);
                    else
                        feat.Rank = oldCount;
                }

                if (W40KRTCharacterSelection.TryGet(selection.Selection, out var oldCharSel)) {
                    var option = oldCharSel.Options.FirstOrDefault(o => o.Id.Eq(oldFeature));
                    if (option is object && option.SubSelectionId.HasValue()) {
                        var subSelection = owner.Progression.Selections.FirstOrDefault(s => ((W40KRTGameUnitSelectionProgressionEntry)s).Ref.Selection.Eq(option.SubSelectionId));
                        if (subSelection is object) {
                            owner.Progression.Selections.Remove(subSelection);
                        }
                    }
                }
            }
            if (SubSelectionId.HasValue()) {
                var subSelection = owner.Progression.Selections.FirstOrDefault(s => ((W40KRTGameUnitSelectionProgressionEntry)s).Ref.Selection.Eq(SubSelectionId));
                if (subSelection is null) {
                    if (W40KRTCharacterSelection.TryGet(SubSelectionId, out var subCharSel)) {
                        var index = owner.Progression.Selections.IndexOf(selectionEntry);
                        subSelection = owner.Progression.Selections.InsertByBlueprint(index + 1, SubSelectionId, subCharSel.Options.First().Id);
                    }
                }
            }
        }

        public string SubSelectionId { get; }
        public static IReadOnlyList<W40KRTCharacterSelectionOption> AllAttributes { get; } = new[] {
            new W40KRTCharacterSelectionOption("3ea34afcda634bcebb7bf42882374418", "Weapon Skill"),
            new W40KRTCharacterSelectionOption("ffb8b3b1805e49ad8284a135193a4d1e", "Ballistic Skill"),
            new W40KRTCharacterSelectionOption("0adb0795441d41759effe618f130d6d9", "Strength"),
            new W40KRTCharacterSelectionOption("e46b9eb4289e42f7937b8720601a67b8", "Toughness"),
            new W40KRTCharacterSelectionOption("de083114d03f483ba7d854ac1bfb6009", "Agility"),
            new W40KRTCharacterSelectionOption("9301177cbed648188ae686c36759f37a", "Intelligence"),
            new W40KRTCharacterSelectionOption("22649b5ed41d4c07b8f037f5bbfb9cc5", "Perception"),
            new W40KRTCharacterSelectionOption("4a355bc2ac7040ae85174abac9a38bbf", "Willpower"),
            new W40KRTCharacterSelectionOption("e17f0bc7e5584678a3c6dfbfb7608f78", "Fellowship"),
        };
    }
    internal class W40KRTCharacterSelection
    {
        public W40KRTCharacterSelection(string name, int level, string selectionId, IReadOnlyList<W40KRTCharacterSelectionOption> options)
        {
            Name = name;
            Level = level;
            SelectionId = selectionId;
            Options = options;
        }

        public string Name { get; }
        public int Level { get; }
        public string SelectionId { get; }
        public IReadOnlyList<W40KRTCharacterSelectionOption> Options { get; }

        public static W40KRTCharacterSelection Homeworld { get; } = new W40KRTCharacterSelection("Homeworld", 0, "10fefb03369d430e88b65aabaf68deac", new[] {
            new W40KRTCharacterSelectionOption("6efb99601c094cbd94d4e6e29dab0970", "Void born"),
            new W40KRTCharacterSelectionOption("e21cd160c88649c39b6c693078c384ef", "Unknown"),
            new W40KRTCharacterSelectionOption("f41ee79802b04d448daec83dd5e8163f", "Leira", "c45cef90b495464591b2fab45f5dc527"),
            new W40KRTCharacterSelectionOption("d7953c4cbf47463090ee3025ef390063", "Commoragh"),
            //new W40KRTCharacterSelectionOption("def5bfa5b1a947f6a1764029f06b58f1", "Forge World", "38a36879cbe44b9d982a9014cf9e29ed"), // Invalid???
            new W40KRTCharacterSelectionOption("6ba65fc6da1e40cf929d922f5b021a52", "Forge World", "38a36879cbe44b9d982a9014cf9e29ed"),
            new W40KRTCharacterSelectionOption("a6e871aa095f4a1fa813fab77658ab78", "Craft World"),
            new W40KRTCharacterSelectionOption("5ab7897d0830402ba9809eca3df21d58", "Death World"),
            new W40KRTCharacterSelectionOption("2ef8b22a98c049a5b4f44864c1d1b642", "Fenris"),
            new W40KRTCharacterSelectionOption("b504a419af314de1ab85c86bc8b54907", "Fortress World"),
            new W40KRTCharacterSelectionOption("442ffe5d7db542b39f1f6f55e254c621", "Hive World"),
            new W40KRTCharacterSelectionOption("af0650bf35124dd2ae16c87f7d376bdc", "Imperial World Feudal", "c45cef90b495464591b2fab45f5dc527"),
            new W40KRTCharacterSelectionOption("927671f69b384423af7c5faa1fa1dbd5", "Imperial World", "c45cef90b495464591b2fab45f5dc527")
        });

        public static W40KRTCharacterSelection ForgeWorld { get; } = new W40KRTCharacterSelection("Forge World", 0, "38a36879cbe44b9d982a9014cf9e29ed", new[] {
                new W40KRTCharacterSelectionOption("96fd88d5d1fe4ba1a35746cde88aaf3c", "Analytics System"), // ForgeWorld_ForgedForPurpose_AnalyticsSystem_Feature
                new W40KRTCharacterSelectionOption("eda2be0dda7841ce9cce6186ef6e0ce1", "Locomotion System"), // ForgeWorld_ForgedForPurpose_LocomotionSystem_Feature
                new W40KRTCharacterSelectionOption("08d6f8a4ebe143fe995582bb5c02072b", "Subskin Armour"), // ForgeWorld_ForgedForPurpose_SubskinArmour_Feature
            });

        public static W40KRTCharacterSelection ImperialWorld { get; } = new W40KRTCharacterSelection("Imperial World", 0, "c45cef90b495464591b2fab45f5dc527", new[] {
            new W40KRTCharacterSelectionOption("c4d908b662184bc9b6d110ef5abba8ba", "Humanity's Finest - Strength"),
            new W40KRTCharacterSelectionOption("7fa71f4f1b954dfe8d76050b76b68b0c", "Humanity's Finest - Toughness"),
            new W40KRTCharacterSelectionOption("8f94867f55864f80a496102bd582b195", "Humanity's Finest - Agility"),
            new W40KRTCharacterSelectionOption("8385693f20824317a76ef4f1331f87bd", "Humanity's Finest - Intelligence"),
            new W40KRTCharacterSelectionOption("0237442f0ba3426aa08a714c0b607497", "Humanity's Finest - Perception"),
            new W40KRTCharacterSelectionOption("b1b7502accb64befa0edd130641a38ff", "Humanity's Finest - Willpower"),
            new W40KRTCharacterSelectionOption("00c996410c2842e88b2380e9982a047f", "Humanity's Finest - Fellowship"),
        });

        public static W40KRTCharacterSelection Occupation { get; } = new W40KRTCharacterSelection("Occupation", 0, "ff001e095e7240ac99b40ceb2bdadf0a", new[] {
            new W40KRTCharacterSelectionOption("1518d1434ed646039215da3fdda6b096", "Sanctioned Psyker", "912495ad4ffc4c4da72819d2602f7976"),
            new W40KRTCharacterSelectionOption("395a77ff6fd344f5b8b4a0cc0def06dc", "Unsanctioned Psyker"),
            new W40KRTCharacterSelectionOption("a69ab12837ae4bfea6bb56f834892d7f", "Space Marine"),
            new W40KRTCharacterSelectionOption("4b908491051a4f36b9703b95e048a5a3", "Astra Militarum"),
            new W40KRTCharacterSelectionOption("00b183680643424abe015263aac81c5b", "Commissar"),
            new W40KRTCharacterSelectionOption("8fab55c9130a4ae0a745f4fa1674c5df", "Criminal"),
            new W40KRTCharacterSelectionOption("d840a5dc947546e0b4ac939287191fd8", "Ministorum Crusader"),
            new W40KRTCharacterSelectionOption("962c310fd1664ae996c759e4d11a2d88", "Navy Officer"),
            new W40KRTCharacterSelectionOption("06180233245249eea90d222bb1c13f00", "Nobility"),
            new W40KRTCharacterSelectionOption("abe45adeb7d7415ca96df8fc6cd1acd2", "Kabalite Dracon"),
            new W40KRTCharacterSelectionOption("33497e0597e64570bb5cf78b19a95d96", "Ranger"),
            new W40KRTCharacterSelectionOption("8e0cfa654ec24dbbba9e80c27433cc8e", "Navigator"),
            new W40KRTCharacterSelectionOption("31d25d8b646c454a8fbc17bc8f775c2c", "Tech Priest"),
            new W40KRTCharacterSelectionOption("b53037d92c984cf3921df309241e48ca", "Abelard (Navy Officer)"),
            new W40KRTCharacterSelectionOption("b6962fcc54054af98961dd9a6c0f9e18", "Argenta (Adepta Sororitas)"),
            new W40KRTCharacterSelectionOption("777d9f9c570443b59120e78f2d9dd515", "Pasqual (Tech-Priest)"),
            new W40KRTCharacterSelectionOption("176a50ee98944e939d3959841e3eb269", "Heinrix (Sanctioned Psyker)"),
            new W40KRTCharacterSelectionOption("aa932c209cdd43c9bb749d5380fc126e", "Jae (Cold Trader)"),

            //new W40KRTCharacterSelectionOption("53970cc124ea433f93baa41028a8781a", "Comissar (Duplicate?)"),
            //new W40KRTCharacterSelectionOption("b23606b3443b42e490951c03203e0a10", "Criminal (Duplicate?)"),
            //new W40KRTCharacterSelectionOption("90e3b428e6ec45d49a429c3abc288b0b", "Fake"),
            //new W40KRTCharacterSelectionOption("b152728dc5a24a2ab6ca8b3ff20c0b71", "Navy Officer (Duplicate?)"),
            //new W40KRTCharacterSelectionOption("0ea28de90038465885d1d71a7e297ee4", "Empty"),
        });

        public static W40KRTCharacterSelection SanctionedPsyker { get; } = new W40KRTCharacterSelection("Sanctioned Psyker", 0, "912495ad4ffc4c4da72819d2602f7976", new[] {
            new W40KRTCharacterSelectionOption("89473c8809654dcfa003d1c1e6399c70", "Biomancer"),
            new W40KRTCharacterSelectionOption("c06c977585be4dea87cce167188ab68c", "Divinator"),
            new W40KRTCharacterSelectionOption("18647a9ed2ea4d98b950cfb8cae7c91a", "Pyromancer"),
            //new W40KRTCharacterSelectionOption("4c06ad653cbd47839a004008dcd7453d", "Pyromancy"), //???
            new W40KRTCharacterSelectionOption("8ecd0d7d642e4e73a40e02ad781bfafa", "Sanctic"),
            new W40KRTCharacterSelectionOption("3180fee0775146bc905322a318e93600", "Telepath"),
            //new W40KRTCharacterSelectionOption("3bc17e7685f343a39b6a800f7f95a623", "Telepathy"), //???
        });

        public static W40KRTCharacterSelection DarkestHour { get; } = new W40KRTCharacterSelection("Darkest Hour", 0, "a54affd2f8404dbcbffc3e0312061b17", new[] {
            new W40KRTCharacterSelectionOption("be767d8335c24f1788f543cef7fde0e6", "None (Companion)"),
            new W40KRTCharacterSelectionOption("04f4eca5cad54087a648a05168cbea96", "Dark Signs Commissar"),
            new W40KRTCharacterSelectionOption("6c68b0b4d1a64b2791473d4daaa50aec", "Dark Signs Criminal"),
            new W40KRTCharacterSelectionOption("7b7ece038d3343bfbf31646fecca3ea3", "Dark Signs Ecclesiarchy"),
            new W40KRTCharacterSelectionOption("47c83106aabf4e7f98597c6e12cc80c6", "Dark Signs Militarum"),
            new W40KRTCharacterSelectionOption("4385c080dade487fa7b743b5e2600662", "Dark Signs Navy"),
            new W40KRTCharacterSelectionOption("12c37b4a75944ce79edaa02fac6dd751", "Dark Signs Nobility"),
            new W40KRTCharacterSelectionOption("5b6e0dff115643559d0eba1f1328ace7", "Dark Signs Psyker"),
            new W40KRTCharacterSelectionOption("8a83889572ac45bd80a9941a9cd0e9b8", "Shame Commissar"),
            new W40KRTCharacterSelectionOption("368650db0ef647fd8e62b680386dc071", "Shame Criminal"),
            new W40KRTCharacterSelectionOption("a88fa4c3b3de42408a1030e864a28c18", "Shame Ecclesiarchy"),
            new W40KRTCharacterSelectionOption("bdb92a5b23374aa5be57e0e37f76e65f", "Shame Militarum"),
            new W40KRTCharacterSelectionOption("fd5566807b6f4a79ad0972fb97d3cb9b", "Shame Navy"),
            new W40KRTCharacterSelectionOption("139f390bf5c84cee95846301020baeaf", "Shame Nobility"),
            new W40KRTCharacterSelectionOption("a3c6bfc6917a49718a333f1e3ed4b2aa", "Shame Psyker"),
            new W40KRTCharacterSelectionOption("a609de0f250d499fbaffcfd37c59c5fd", "Tortures Commissar"),
            new W40KRTCharacterSelectionOption("9380ef2326334b2d95e1eeacbb785cef", "Tortures Criminal"),
            new W40KRTCharacterSelectionOption("7f05891ab82b48b2b9aef99722024591", "Tortures Ecclesiarchy"),
            new W40KRTCharacterSelectionOption("78797666bb9f4a4a81dea0a6e9c21d65", "Tortures Militarum"),
            new W40KRTCharacterSelectionOption("4a142f95698445c9a4ad22b921612d6d", "Tortures Navy"),
            new W40KRTCharacterSelectionOption("2e0e2530061f4d97abd7ba11f9e171e6", "Tortures Nobility"),
            new W40KRTCharacterSelectionOption("cc456183aedf4616aaa44040cb08bcb7", "Tortures Psyker"),
        });

        public static W40KRTCharacterSelection MomentOfTriumph { get; } = new W40KRTCharacterSelection("Moment Of Triumph", 0, "d97cf3c474034359851b1a8ffba73715", new[] {
            new W40KRTCharacterSelectionOption("1420e40e2a114a8b961a5587381359e0", "None (Companion)"),
            new W40KRTCharacterSelectionOption("7dbb4d8408c34bf0a840b277feafa021", "Feat Of Mind Commisar"),
            new W40KRTCharacterSelectionOption("12af6c0764aa4cd0a51719cb16dd56be", "Feat Of Mind Criminal"),
            new W40KRTCharacterSelectionOption("53befc9fdc6f4856ae8aaa14d67356ca", "Feat Of Mind Ecclesiarchy"),
            new W40KRTCharacterSelectionOption("eed7083c3cda430bb03a38063576fcf2", "Feat Of Mind Militarum"),
            new W40KRTCharacterSelectionOption("18de8919406c4d589a0b74a512f04608", "Feat Of Mind Navy"),
            new W40KRTCharacterSelectionOption("83b0ed54c3ed48129b91860e873c4952", "Feat Of Mind Nobility"),
            new W40KRTCharacterSelectionOption("fd2e95e7e1254088a4959bde4f219105", "Feat Of Mind Psyker"),
            new W40KRTCharacterSelectionOption("d92ccc0f17a14e8e8df0082edbd0427a", "Glory Commissar"),
            new W40KRTCharacterSelectionOption("ad3286f82b9044968f64264c9f431068", "Glory Criminal"),
            new W40KRTCharacterSelectionOption("5d6f71aa393340908f26b27474021fd6", "Glory Ecclesiarchy"),
            new W40KRTCharacterSelectionOption("dd15357977564606a598b5454f4fb37c", "Glory Militarum"),
            new W40KRTCharacterSelectionOption("c73eca5c4570412097a2399a98403baf", "Glory Navy"),
            new W40KRTCharacterSelectionOption("57956895f8a1469e8cb5c27a6a5fcd52", "Glory Nobility"),
            new W40KRTCharacterSelectionOption("66a1900bac4f4c2db6bfd1254b10126d", "Glory Psyker"),
            new W40KRTCharacterSelectionOption("36928a81856640ea9296904a7b9f2178", "Great Deed Commissar"),
            new W40KRTCharacterSelectionOption("9aa6694a186d4af6b64d0ead2a4c312c", "Great Deed Criminal"),
            new W40KRTCharacterSelectionOption("675cb7de11ec49559ca1a5ef314b8676", "Great Deed Ecclesiarchy"),
            new W40KRTCharacterSelectionOption("4307806003254495a4ab2d72cdc17afe", "Great Deed Militarum"),
            new W40KRTCharacterSelectionOption("cea0b1fe9e854f129e1aa37d2c8526f1", "Great Deed Navy"),
            new W40KRTCharacterSelectionOption("f244d8cd9fd74a8594e6ee6d81e6ed3f", "Great Deed Nobility"),
            new W40KRTCharacterSelectionOption("c160ccb31fc64d70bee6f9860aebc007", "Great Deed Psyker"),
        });
        public static W40KRTCharacterSelection Attribute1 { get; } = new W40KRTCharacterSelection("Attribute 1", 0, "6d7a24a3a9bc4dc88b362bcce25f4dda", W40KRTCharacterSelectionOption.AllAttributes);
        public static W40KRTCharacterSelection Attribute2 { get; } = new W40KRTCharacterSelection("Attribute 2", 0, "c8651fb533d64fd8a9bfa0121f309250", W40KRTCharacterSelectionOption.AllAttributes);
        public static W40KRTCharacterSelection Attribute3 { get; } = new W40KRTCharacterSelection("Attribute 3", 0, "ea79d49a3c8944d4a59b333956feca4e", W40KRTCharacterSelectionOption.AllAttributes);
        public static W40KRTCharacterSelection Attribute4 { get; } = new W40KRTCharacterSelection("Attribute 4", 0, "f0119fa971ca445ea27ed327f6c0603c", W40KRTCharacterSelectionOption.AllAttributes);
        public static W40KRTCharacterSelection Attribute5 { get; } = new W40KRTCharacterSelection("Attribute 5", 0, "e957c9f30faa466b8c58d9482592c011", W40KRTCharacterSelectionOption.AllAttributes);
        public static W40KRTCharacterSelection Attribute6 { get; } = new W40KRTCharacterSelection("Attribute 6", 0, "31bd1811b7024abba2bb29b8bc079ce0", W40KRTCharacterSelectionOption.AllAttributes);
        //public static W40KRTCharacterSelection abc { get; } = ;

        public static IReadOnlyList<W40KRTCharacterSelection> All { get; } = new[] {
            Homeworld,
            ForgeWorld,
            ImperialWorld,
            Occupation,
            SanctionedPsyker,
            DarkestHour,
            MomentOfTriumph,
            Attribute1,
            Attribute2,
            Attribute3,
            Attribute4,
            Attribute5,
            Attribute6,
        };

        private static readonly IReadOnlyDictionary<string, W40KRTCharacterSelection> Lookup = All.ToDictionary(x => x.SelectionId, StringComparer.Ordinal);

        internal static bool TryGet(string selectionId, out W40KRTCharacterSelection selection)
        {
            return Lookup.TryGetValue(selectionId, out selection);
        }
    }
}
