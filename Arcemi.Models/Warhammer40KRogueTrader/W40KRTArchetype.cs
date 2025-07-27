using Newtonsoft.Json.Linq;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTArchetypes
    {
        public W40KRTArchetypes(string name, int level, int masteryLevel, string selectionId, IReadOnlyList<W40KRTArchetype> types, IReadOnlyList<W40KRTArchetypeAutomaticFeat> automaticFeats = null)
        {
            Name = name;
            Level = level;
            MasteryLevel = masteryLevel;
            SelectionId = selectionId;
            Types = types;
            AutomaticFeats = automaticFeats ?? Array.Empty<W40KRTArchetypeAutomaticFeat>();
            SelectionOptions = types.Select(x => new BlueprintOption(x.SelectionId, x.Name)).ToArray();
            _Blueprints = new HashSet<string>(StringComparer.Ordinal);
            foreach (var type in types) {
                _Blueprints.Add(type.SelectionId);
                _Blueprints.Add(type.CareerPathId);
                foreach (var keystoneAbilityId in type.KeystoneAbilityIds) {
                    _Blueprints.Add(keystoneAbilityId);
                }
            }
            foreach (var feat in AutomaticFeats) {
                _Blueprints.Add(feat.Id);
            }
        }

        public string Name { get; }
        public int Level { get; }
        public int MasteryLevel { get; }
        public string SelectionId { get; }
        public IReadOnlyList<W40KRTArchetype> Types { get; }
        public IReadOnlyList<W40KRTArchetypeAutomaticFeat> AutomaticFeats { get; }
        public IReadOnlyList<BlueprintOption> SelectionOptions { get; }

        private readonly HashSet<string> _Blueprints;
        public IReadOnlyCollection<string> Blueprints => _Blueprints;

        public static W40KRTArchetypes Tier1 { get; } = new W40KRTArchetypes("Tier 1", 1, 15, "f889a299ac7746cfb9b59438fc79018e", new[] {
            new W40KRTArchetype("Officer", "33725d84e95e4323ac46d8fbf899b250", "93d2b8c0a10d4cd0a20a897124cbf3e5", "a5a17fc75f8a45279a39a7b40c1f86f6", new [] {
                new W40KRTArchetypeAutomaticFeat("2c6018eaf7c2413cbeac6f946fc5ad2d", 1), // Ascension
                new W40KRTArchetypeAutomaticFeat("f3bbf338354744a399d0fe19e0902112", 3), // Bring it down
                new W40KRTArchetypeAutomaticFeat("0ba4fa6cdd634bd0ac85888ba9ab78a2", 4), // Ultimate
            }),
            new W40KRTArchetype("Soldier", "06f4f78a9c1a472b85cd79a9a142153d", "567936698f674003bba05fa02e7130ae", "7a76af1ef02c4699b62db147e2b9a172", new [] {
                new W40KRTArchetypeAutomaticFeat("7354c0ae9bf84c49adf82f95c05d4d5c", 1), // Ascension
                new W40KRTArchetypeAutomaticFeat("8638221f9caf46649cefcbd7574804d1", 3), // Revel in slaughter
                new W40KRTArchetypeAutomaticFeat("618bd712c61d4b2d89e55ecf5f3a6348", 4), // Ultimate
            }),
            new W40KRTArchetype("Operative", "1529e5a0e7844bf3bb8d0cc0501264d4", "81c325968f774360a61536404a57c2ec", "6cfd9b9830ea403db07a816843a8e39a", new [] {
                new W40KRTArchetypeAutomaticFeat("5dc9e4fdc7284b6e8de76f421dd28f5e", 1), // Ascension
                new W40KRTArchetypeAutomaticFeat("fbdba5fe2a264c25bada29aecd64049a", 3), // Expose Armor
                new W40KRTArchetypeAutomaticFeat("264e8fc120fc4cdda336d31ddc0c15f7", 4), // Ultimate
            }),
            new W40KRTArchetype("Warrior", "974496d72fbe4329b438ee15cf004bd2", "3a9169d7ece94faf86b53298067a5b4c", "6a4310c9966946a3b82ed7acc94464da", new [] {
                new W40KRTArchetypeAutomaticFeat("a2ff69ee11a348b59f754bab649fc6a3", 1), // Ascension
                new W40KRTArchetypeAutomaticFeat("92193f4629c84812825841ae1b01d1da", 3), // Endure
                new W40KRTArchetypeAutomaticFeat("2ee6f62308474fd7b765a08467eb5ae6", 4), // Ultimate
            }),
            new W40KRTArchetype("Bladedancer", "dd6948ee596346a69733d0bb107c2f42", "1c12ab2cda9e46e49253a425e29733a5", "de2fa09b36314b2dafe09c3e8354d214", new[] {
                new W40KRTArchetypeAutomaticFeat("1cf0a934f1364670ba2679bf42fc2afd", 1), // Ascension
                new W40KRTArchetypeAutomaticFeat("c3d6e29e74e440a79cb243e4e93e0501", 3), // Death from above
                new W40KRTArchetypeAutomaticFeat("916fcf40f9cc410f9b0a810516089207", 4), // Ultimate
            })
        });

        public static W40KRTArchetypes Tier2 { get; } = new W40KRTArchetypes("Tier 2", 16, 35, "", new[] {
            new W40KRTArchetype("Assassin", "7b90955673a54136be9c11743943fdfe", "581aa8140e924692be2f0bbd8a932807", "74a1e39f3d0c4cab828d49d4acd54851", new [] {
                new W40KRTArchetypeAutomaticFeat("2f902b540fb74633a0b733eb8f114159", 4), // Ultimate
            }),
            new W40KRTArchetype("Vanguard", "fec9cd09f11b4615b7a17f441350d2d4", "710efd77331d4c5493905c71171f13a0", "fc83a8ac57e54db59c3bb70083dc385c", new [] {
                new W40KRTArchetypeAutomaticFeat("b08271b1055b4c30b9e0c463616ced8d", 4), // Ultimate
            }),
            new W40KRTArchetype("Bounty Hunter", "6f276e8a8e2c4a548504ae39d2a7f22a", "2f83a8e27f4c4e1ea730688eb044973f", "802d21391d2f4573bfff126023df39bb", new [] {
                new W40KRTArchetypeAutomaticFeat("57780ad000bb4283a4eb8796b357b963", 4), // Ultimate
            }),
            new W40KRTArchetype("Master Tactician", "604fa184d7d944c8ae5965f9700782b5", "68de11ce354f46058be9b048d2da2103", new [] {"7989c6e3109841329aee9e4546138395", "b669fe2c03a240fca1b3b2429ca87414"}, new [] {
                new W40KRTArchetypeAutomaticFeat("874c1ff6e51945a0ae4d4e33f3dc1b49", 4), // Ultimate
            }),
            new W40KRTArchetype("Grand Strategist", "a31b390cabe7464fbfd0e1ba53c4112f", "ab66d7e093a84a0daaec8bee98a12ddf", "e4e45f74936c4740b7bc03013f6991ba", new [] {
                new W40KRTArchetypeAutomaticFeat("afece68fac504e339ffefed3da8e5dc6", 4), // Ultimate
            }),
            new W40KRTArchetype("Arch-Militant", "651684417def4c258c72ba91f481b817", "63125df1390b45ac9cc6b2e0e777e9c2", "7f7d109653f24774939f758d094b8812", new [] {
                new W40KRTArchetypeAutomaticFeat("ad31784e7ced47a394736d4aeb4ec745", 4), // Ultimate
            }),
            new W40KRTArchetype("Executioner", "d6c0498a227040c891e4e2703eb55c13", "3e5f2be4dce140ab91343cee8ded6d2e", new [] {
                "4f04773685be45c29927c6740ccf64d8", // Executioner_Keystone_Passive1_Feature, Forced Repentance
                "47f5b3809fba409b9d27611ef0a77cd3", // Executioner_Keystone_Passive2_Feature, Scourging Strikes
                "5c06542194b74ecc922b92f3543703d2" // Executioner_Keystone_Active_Feature, Where It Hurts
                }, new [] {
                    new W40KRTArchetypeAutomaticFeat("e5874ecd259b4878bc71239c281c17e4", 4), // Ultimate
                }
            ),
            new W40KRTArchetype("Overseer", "21b0fc8cfbe940ecbef0114d5d27b44a", "bf826b7565cf4363bcca587a03b3ac4b", new [] {
                "79d7b9273feb4554b09c82f69c0b62d6", // Mastiff
                "9afe9cd1f46748fd8ffbc960b61744da", // Eagle
                "29b296e0153241f3aca55b7c4d50cec0", // Raven
                "dc7ce6fdeb964544b613e749c2610623", // Servoskull
            }, new [] {
                new W40KRTArchetypeAutomaticFeat("0cded06c29174502823f0f52154d8162", 1), // Reactivate
                new W40KRTArchetypeAutomaticFeat("616f42e2f9a842f191fdeca28d1bbf0d", 1), // Mastiff: Battle mode: Standby
                new W40KRTArchetypeAutomaticFeat("5955723361094eada17c84c6c9f84476", 1), // Mastiff: Protect
                new W40KRTArchetypeAutomaticFeat("078bea05041540fa87a53b93d639f243", 1), // Mastiff: Apprehend
                new W40KRTArchetypeAutomaticFeat("469ab777f2284aa18e1d242db2f9cc64", 1), // Mastiff: The Null Directive
                new W40KRTArchetypeAutomaticFeat("bf9f7a1559664c17b3ac5565c4771f06", 1), // Raven: Purification Discharge
                new W40KRTArchetypeAutomaticFeat("781021a32fd04104a523a33d986f1bee", 1), // Raven: Relocate
                new W40KRTArchetypeAutomaticFeat("0345824c4a104cca9438f871bcfff0c7", 1), // Raven: Warp Relay
                new W40KRTArchetypeAutomaticFeat("90622e48744a474e91770f71359dae9b", 1), // Raven: The Null Directive
                new W40KRTArchetypeAutomaticFeat("b2779133cc324256b3663dcbc5d9c377", 1), // Servoskull: Extrapolation
                new W40KRTArchetypeAutomaticFeat("57e2562dac5c40268e27f87317f008d0", 1), // Servoskull: Relocation
                new W40KRTArchetypeAutomaticFeat("a62ff7d3719640d0b5f23976cbb9544f", 1), // Servoskull: The Null Directive
                new W40KRTArchetypeAutomaticFeat("98362e16cc3a4e7f9ce63039186c2b23", 1), // Eagle: Obstruct Vision
                new W40KRTArchetypeAutomaticFeat("71bb3db5c0bd4fed938e8cfc01968f7b", 1), // Eagle: Soar!
                new W40KRTArchetypeAutomaticFeat("9c71981e5f6846ac994916c480331f82", 1), // Eagle: Winged Aegis
                new W40KRTArchetypeAutomaticFeat("0bff20e811334914a23cc29d3682da94", 1), // Eagle: The Null Directive
                //new W40KRTArchetypeAutomaticFeat("", 1), //
                new W40KRTArchetypeAutomaticFeat("67bed913aca646fca8bef97d7cc0648f", 4), // Ultimate
            }, downgrade: W40KRTArchetypeOverseer.Downgrade),
            new W40KRTArchetype("Overseer (Solomorne)", "f95e3d9a049345ec918926f092ec67e2", "bf826b7565cf4363bcca587a03b3ac4b", "b83f88696cd848f894dc01e5ad0d0d1b", new [] {
                new W40KRTArchetypeAutomaticFeat("0cded06c29174502823f0f52154d8162", 1), // Reactivate
                new W40KRTArchetypeAutomaticFeat("469ab777f2284aa18e1d242db2f9cc64", 1), // The Null Directive
                new W40KRTArchetypeAutomaticFeat("616f42e2f9a842f191fdeca28d1bbf0d", 1), // Mastiff: Battle mode: Standby
                new W40KRTArchetypeAutomaticFeat("5955723361094eada17c84c6c9f84476", 1), // Mastiff: Protect
                new W40KRTArchetypeAutomaticFeat("078bea05041540fa87a53b93d639f243", 1), // Mastiff: Apprehend
                new W40KRTArchetypeAutomaticFeat("67bed913aca646fca8bef97d7cc0648f", 4), // Ultimate
            }, downgrade: W40KRTArchetypeOverseer.Downgrade),
        }, new[] {
            new W40KRTArchetypeAutomaticFeat("064ce50cc4fa448a9e4c71c874301309", 5), // APIncreaseLevel20
        });

        public static W40KRTArchetypes Tier3 { get; } = new W40KRTArchetypes("Tier 3", 36, int.MaxValue, "", new[] {
            new W40KRTArchetype("Exemplar", "bcefe9c41c7841c9a99b1dbac1793025", "", ""),
        }, new[] {
            new W40KRTArchetypeAutomaticFeat("cda4f66b6766449392e152543e23c4b1", 10), // APIncreaseLevel10Ascension
            new W40KRTArchetypeAutomaticFeat("3eec38c890834d1ba7e396c832230099", 20), // APIncreaseLevel20Ascension
        });

        public static IReadOnlyList<W40KRTArchetypes> All { get; } = new[] {
            Tier1,
            Tier2,
            Tier3
        };

        public bool Has(string blueprint)
        {
            return _Blueprints.Contains(blueprint);
        }

        public W40KRTArchetype Get(string value)
        {
            return Types.FirstOrDefault(x => x.SelectionId.Eq(value) || x.CareerPathId.Eq(value));
        }

        public static IEnumerable<string> GetBlueprintsHigherThan(int value)
        {
            return All.Where(x => x.Level > value).SelectMany(x => x.Blueprints)
                .Concat(All.Where(x => x.MasteryLevel > value).SelectMany(x => x.Types.Select(t => t.CareerPathId)))
                .Concat(Tier1.AutomaticFeats.Where(x => x.Level > value).Select(x => x.Id))
                .Concat(Tier1.Types.SelectMany(t => t.AutomaticFeats.Where(x => x.Level > value).Select(x => x.Id)))
                .Concat(Tier2.AutomaticFeats.Where(x => Tier2.Level + x.Level - 1 > value).Select(x => x.Id))
                .Concat(Tier2.Types.SelectMany(t => t.AutomaticFeats.Where(x => Tier2.Level + x.Level - 1 > value).Select(x => x.Id)))
                .Concat(Tier3.AutomaticFeats.Where(x => Tier3.Level + x.Level - 1 > value).Select(x => x.Id))
                .Concat(Tier3.Types.SelectMany(t => t.AutomaticFeats.Where(x => Tier3.Level + x.Level - 1 > value).Select(x => x.Id)));
        }

        public static int ActualLevel(IGameUnitSelectionProgressionEntry sel)
        {
            var act = (W40KRTGameUnitSelectionProgressionEntry)sel;
            return ActualLevel(act.Ref.Path, act.Ref.Level);
        }
        public static int ActualLevel(string path, int level)
        {
            if (Tier1.Has(path)) return Tier1.Level + level - 1;
            if (Tier2.Has(path)) return Tier2.Level + level - 1;
            if (Tier3.Has(path)) return Tier3.Level + level - 1;
            return level;
        }
    }

    public class W40KRTArchetypeAutomaticFeat
    {
        public W40KRTArchetypeAutomaticFeat(string id, int level)
        {
            Id = id;
            Level = level;
        }

        public string Id { get; }
        public int Level { get; }
    }

    public class W40KRTArchetype
    {
        public W40KRTArchetype(string name, string careerPathId, string selectionId, string[] keystoneAbilityIds, IReadOnlyList<W40KRTArchetypeAutomaticFeat> automaticFeats = null, Action<W40KRTArchetypeDowngradeArguments> downgrade = null)
        {
            Name = name;
            CareerPathId = careerPathId;
            SelectionId = selectionId;
            KeystoneAbilityIds = keystoneAbilityIds;
            AutomaticFeats = automaticFeats ?? Array.Empty<W40KRTArchetypeAutomaticFeat>();
            Downgrade = downgrade;
        }
        public W40KRTArchetype(string name, string careerPathId, string selectionId, string keystoneAbilityId, IReadOnlyList<W40KRTArchetypeAutomaticFeat> automaticFeats = null, Action<W40KRTArchetypeDowngradeArguments> downgrade = null)
            : this(name, careerPathId, selectionId, new[] { keystoneAbilityId }, automaticFeats, downgrade)
        {
        }

        public string Name { get; }
        public string CareerPathId { get; }
        public string SelectionId { get; }
        public IReadOnlyList<string> KeystoneAbilityIds { get; }
        public IReadOnlyList<W40KRTArchetypeAutomaticFeat> AutomaticFeats { get; }
        public Action<W40KRTArchetypeDowngradeArguments> Downgrade { get; }
    }

    public class W40KRTArchetypeDowngradeArguments
    {
        public W40KRTArchetypeDowngradeArguments(IGameUnitModel owner, W40KRTUnitMediator mediator, W40KRTArchetypes tier, W40KRTArchetype archetype, int downgradeToLevel)
        {
            Owner = owner;
            Mediator = mediator;
            Tier = tier;
            Archetype = archetype;
            DowngradeToLevel = downgradeToLevel;
        }

        public IGameUnitModel Owner { get; }
        public W40KRTUnitMediator Mediator { get; }
        public W40KRTArchetypes Tier { get; }
        public W40KRTArchetype Archetype { get; }
        public int DowngradeToLevel { get; }
        public bool IsLastLevel => Tier.Level == DowngradeToLevel + 1;
    }
}