using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTArchetypes
    {
        public W40KRTArchetypes(string name, int level, string selectionId, params W40KRTArchetype[] types)
        {
            Name = name;
            Level = level;
            SelectionId = selectionId;
            Types = types;
            SelectionOptions = types.Select(x => new BlueprintOption(x.SelectionId, x.Name)).ToArray();
            _Blueprints = new HashSet<string>(StringComparer.Ordinal);
            foreach (var type in types) {
                _Blueprints.Add(type.SelectionId);
                _Blueprints.Add(type.CareerPathId);
                foreach (var keystoneAbilityId in type.KeystoneAbilityIds) {
                    _Blueprints.Add(keystoneAbilityId);
                }
            }
        }

        public string Name { get; }
        public int Level { get; }
        public string SelectionId { get; }
        public IReadOnlyList<W40KRTArchetype> Types { get; }

        public IReadOnlyList<BlueprintOption> SelectionOptions { get; }

        private readonly HashSet<string> _Blueprints;

        public static W40KRTArchetypes Tier1 { get; } = new W40KRTArchetypes("Tier 1", 1, "f889a299ac7746cfb9b59438fc79018e", new[] {
            new W40KRTArchetype("Officer", "33725d84e95e4323ac46d8fbf899b250", "93d2b8c0a10d4cd0a20a897124cbf3e5", "a5a17fc75f8a45279a39a7b40c1f86f6"),
            new W40KRTArchetype("Soldier", "06f4f78a9c1a472b85cd79a9a142153d", "567936698f674003bba05fa02e7130ae", "7a76af1ef02c4699b62db147e2b9a172"),
            new W40KRTArchetype("Operative", "1529e5a0e7844bf3bb8d0cc0501264d4", "81c325968f774360a61536404a57c2ec", "6cfd9b9830ea403db07a816843a8e39a"),
            new W40KRTArchetype("Warrior", "974496d72fbe4329b438ee15cf004bd2", "3a9169d7ece94faf86b53298067a5b4c", "6a4310c9966946a3b82ed7acc94464da"),
        });

        public static W40KRTArchetypes Tier2 { get; } = new W40KRTArchetypes("Tier 2", 16, "", new[] {
            new W40KRTArchetype("Asssassin", "7b90955673a54136be9c11743943fdfe", "581aa8140e924692be2f0bbd8a932807", "74a1e39f3d0c4cab828d49d4acd54851"),
            new W40KRTArchetype("Vanguard", "fec9cd09f11b4615b7a17f441350d2d4", "710efd77331d4c5493905c71171f13a0", "fc83a8ac57e54db59c3bb70083dc385c"),
            new W40KRTArchetype("Bounty Hunter", "6f276e8a8e2c4a548504ae39d2a7f22a", "2f83a8e27f4c4e1ea730688eb044973f", "802d21391d2f4573bfff126023df39bb"),
            new W40KRTArchetype("Master Tactician", "604fa184d7d944c8ae5965f9700782b5", "68de11ce354f46058be9b048d2da2103", new [] {"7989c6e3109841329aee9e4546138395", "b669fe2c03a240fca1b3b2429ca87414"}),
            new W40KRTArchetype("Grand Strategist", "a31b390cabe7464fbfd0e1ba53c4112f", "ab66d7e093a84a0daaec8bee98a12ddf", "e4e45f74936c4740b7bc03013f6991ba"),
            new W40KRTArchetype("Arch-Militant", "651684417def4c258c72ba91f481b817", "63125df1390b45ac9cc6b2e0e777e9c2", "7f7d109653f24774939f758d094b8812"),
        });

        public static W40KRTArchetypes Tier3 { get; } = new W40KRTArchetypes("Tier 2", 36, "", new[] {
            new W40KRTArchetype("Exemplar", "bcefe9c41c7841c9a99b1dbac1793025", "", ""),
        });

        public bool Has(string blueprint)
        {
            return _Blueprints.Contains(blueprint);
        }

        public W40KRTArchetype Get(string value)
        {
            return Types.FirstOrDefault(x => x.SelectionId.Eq(value) || x.CareerPathId.Eq(value));
        }

        public static int ActualLevel(string path, int level)
        {
            if (Tier1.Has(path)) return Tier1.Level + level - 1;
            if (Tier2.Has(path)) return Tier2.Level + level - 1;
            return Tier3.Level + level - 1;
        }
    }
    internal class W40KRTArchetype
    {
        public W40KRTArchetype(string name, string careerPathId, string selectionId, string[] keystoneAbilityIds)
        {
            Name = name;
            CareerPathId = careerPathId;
            SelectionId = selectionId;
            KeystoneAbilityIds = keystoneAbilityIds;
        }
        public W40KRTArchetype(string name, string careerPathId, string selectionId, string keystoneAbilityId)
            : this(name, careerPathId, selectionId, new[] { keystoneAbilityId })
        {
        }

        public string Name { get; }
        public string CareerPathId { get; }
        public string SelectionId { get; }
        public IReadOnlyList<string> KeystoneAbilityIds { get; }
    }
}