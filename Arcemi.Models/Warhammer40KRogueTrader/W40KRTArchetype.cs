using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTArchetypes
    {
        public W40KRTArchetypes(string name, string selectionId, params W40KRTArchetype[] types)
        {
            Name = name;
            SelectionId = selectionId;
            Types = types;
            SelectionOptions = types.Select(x => new BlueprintOption(x.SelectionId, x.Name)).ToArray();
            _Blueprints = new HashSet<string>(StringComparer.Ordinal);
            foreach (var type in types) {
                _Blueprints.Add(type.SelectionId);
                _Blueprints.Add(type.CareerPathId);
                _Blueprints.Add(type.KeystoneAbilityId);
            }
        }

        public string Name { get; }
        public string SelectionId { get; }
        public IReadOnlyList<W40KRTArchetype> Types { get; }

        public IReadOnlyList<BlueprintOption> SelectionOptions { get; }

        private readonly HashSet<string> _Blueprints;

        public static W40KRTArchetypes Tier1 { get; } = new W40KRTArchetypes("Tier 1", "f889a299ac7746cfb9b59438fc79018e", new[] {
            new W40KRTArchetype("Officer", "33725d84e95e4323ac46d8fbf899b250", "93d2b8c0a10d4cd0a20a897124cbf3e5", "a5a17fc75f8a45279a39a7b40c1f86f6"),
            new W40KRTArchetype("Soldier", "06f4f78a9c1a472b85cd79a9a142153d", "567936698f674003bba05fa02e7130ae", "7a76af1ef02c4699b62db147e2b9a172"),
            new W40KRTArchetype("Operative", "1529e5a0e7844bf3bb8d0cc0501264d4", "81c325968f774360a61536404a57c2ec", "6cfd9b9830ea403db07a816843a8e39a"),
            new W40KRTArchetype("Warrior", "974496d72fbe4329b438ee15cf004bd2", "3a9169d7ece94faf86b53298067a5b4c", "6a4310c9966946a3b82ed7acc94464da"),
        });

        public bool Has(string blueprint)
        {
            return _Blueprints.Contains(blueprint);
        }

        public W40KRTArchetype Get(string value)
        {
            return Types.FirstOrDefault(x => x.SelectionId.Eq(value) || x.CareerPathId.Eq(value));
        }
    }
    internal class W40KRTArchetype
    {
        public W40KRTArchetype(string name, string careerPathId, string selectionId, string keystoneAbilityId)
        {
            Name = name;
            CareerPathId = careerPathId;
            SelectionId = selectionId;
            KeystoneAbilityId = keystoneAbilityId;
        }

        public string Name { get; }
        public string CareerPathId { get; }
        public string SelectionId { get; }
        public string KeystoneAbilityId { get; }
    }
}