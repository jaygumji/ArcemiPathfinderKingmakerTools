using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed record W40KDHAdvancementDefinition(string Name, int PointsPerRank, int MaximumRank, string ComponentName);

    internal static class W40KDHAdvancementCatalog
    {
        // Advancement blueprint identities and rank bounds from the OWL v3 closed beta, Steam build 23665898.
        private static readonly Dictionary<string, W40KDHAdvancementDefinition> Entries = new(StringComparer.OrdinalIgnoreCase) {
            { "23921e40f1954693ae36538629767030", new("Agility", 5, 4, "$StatAdvancement$10f7a1fc-1990-4565-8792-9b2497e83ee7") },
            { "ef35544ab8a040bd9461189652b179d4", new("Ballistic Skill", 5, 4, "$StatAdvancement$aa3a4017-b914-49b4-a37a-458961ea3d80") },
            { "b4ced0141f374f3d811177fc5fb08ae3", new("Fellowship", 5, 4, "$StatAdvancement$fa963c3a-5db0-43f3-9a7c-ed355a1d3328") },
            { "f60d1a2d62234c018e3e0882563a058c", new("Intelligence", 5, 4, "$StatAdvancement$c2878d7e-023a-4f1f-8baf-8bb17cfe38cd") },
            { "01c01d1bae2a4ce0baefd44cc3c1a641", new("Perception", 5, 4, "$StatAdvancement$cef4498f-3fb7-4622-98bc-30ca557d47f5") },
            { "d86a5d75aa32469289bf0e46835d27d5", new("Strength", 5, 4, "$StatAdvancement$c024c03c-4971-487f-becd-fdd1a4e9afd8") },
            { "1ea6d519b60c4d4993a386dab0f94253", new("Toughness", 5, 4, "$StatAdvancement$ed081b6d-f992-4704-9dfd-37a2bb4d339e") },
            { "9fe1b142bcfa4203b2949ca121fb4b19", new("Weapon Skill", 5, 4, "$StatAdvancement$9aaa470d-cfd4-4c76-8498-bd1804274622") },
            { "fbf0a0ed1b754f569512494e24e054b1", new("Willpower", 5, 4, "$StatAdvancement$0e60bab8-2ba9-4e43-ac8b-cbaf4cdb5cc7") },
            { "41cd7c199e734921a3e3b5a1fcc362f6", new("Willpower", 5, 1, "$StatAdvancement$2a4845ed-e309-411a-84fa-97ec0a3c2cfb") },
            { "f53f3695352f4a25968ee22fc00eaed8", new("Athletics", 10, 4, "$StatAdvancement$b6d63345-3554-4e35-bfdb-2e23994884f7") },
            { "963d7ad946374a9c9033d853091ed55e", new("Awareness", 10, 4, "$StatAdvancement$c54429ea-f23d-490e-974a-8f837eaeaa4d") },
            { "28000dbc252d4994996faa4b5f7a9bb4", new("Demolition", 10, 4, "$StatAdvancement$af28ca9f-9d6d-465a-acfc-edc28c24990d") },
            { "4c77d5c9e5f2479ea918ecfa55462113", new("Diplomacy", 10, 4, "$StatAdvancement$d16f81f5-171c-4a9c-9aaa-190079204dec") },
            { "359a62f25c204be797ec4c280be466b1", new("Interrogation", 10, 4, "$StatAdvancement$30e756dd-c668-40e1-b221-6461b92434c2") },
            { "7321c1acd38c4a3f8984313c223f47e9", new("Intimidation", 10, 4, "$StatAdvancement$a601d8fa-0fea-4a70-8ccd-d820e2c16be3") },
            { "4580edfdbca44ea2ace9e04c433841d6", new("Lore Heresy", 10, 4, "$StatAdvancement$aa2b3a3a-0b19-42d9-84ed-cbece0823c9e") },
            { "a00e0a152e964bb8bd6774f754c2860d", new("Lore Warp", 10, 4, "$StatAdvancement$5ac3d961-aeb8-4d41-a191-ebc0c8f77d71") },
            { "1fb90cb8a7f74acfa1ecadfa4390eefd", new("Lore Xenos", 10, 4, "$StatAdvancement$1eae97d6-74d0-4ff5-87ea-51dacc31a4a7") },
            { "a60de9ca50844f9aa07a9499e017b707", new("Medicae", 10, 4, "$StatAdvancement$dfafda4d-12df-4088-b23d-b50215f9e982") },
            { "9954ba4665664665b180f842893594f2", new("Mettle", 10, 4, "$StatAdvancement$7cbe320d-9d71-434f-a386-51e67ce5ea43") },
            { "1fdaf03d04124a559941ac77248b1ebc", new("Mobility", 10, 4, "$StatAdvancement$fbdcdd7c-5649-414d-a9e7-8bc33dc91f2d") },
            { "aefa5ca4b2fe495daf95ccfc10380dec", new("Reflexes", 10, 4, "$StatAdvancement$b3b05bf0-0840-40ab-a357-65898897b493") },
            { "aaf536a5947847ba9d9a8c67843a9a7f", new("Resistance", 10, 4, "$StatAdvancement$55e07f26-d8a0-4e50-b3f8-e8470a5bce39") },
            { "92421ec45ea34033b74fd1fb6f730851", new("Sleight of Hand", 10, 4, "$StatAdvancement$c358c669-25eb-4964-a95a-e9f33411989b") },
            { "83fed3e54f9446a19138f40630ac1443", new("Tech Use", 10, 4, "$StatAdvancement$872e2347-7d6c-4613-a446-697a9c472181") },
            { "5c3fde6ac014419ba2ecb8777f41c804", new("Tenacity", 10, 4, "$StatAdvancement$6cdfe718-6b59-4cd0-bf64-f74b2af26f6d") },
            { "91f3f090b17f4cccb413cfb950379999", new("Wits", 10, 4, "$StatAdvancement$3fd29da3-fd0d-481d-a0b8-5fd6f79e77d2") },
            { "7ebcd4987ba9414bb191e5639a464bf7", new("Lore Imperium", 10, 1, "$StatAdvancement$fb04e651-0809-40f5-b781-e78de605e16b") },
            { "28bf8c1a5e904b4ebc3ce8a11689b888", new("Lore Xenos", 10, 1, "$StatAdvancement$d81f0435-0c44-42a1-b7fc-9b5e917ef7a7") },
        };

        public static IEnumerable<KeyValuePair<string, W40KDHAdvancementDefinition>> Grants { get; } =
            Entries.GroupBy(entry => entry.Value.Name).Select(group => group.MaxBy(entry => entry.Value.MaximumRank)).ToArray();

        public static bool AvailableInGame(string blueprint, W40KDHAdvancementDefinition definition)
        {
            var metadata = GameDefinition.Warhammer40K_DarkHeresy.Resources.Blueprints;
            var type = definition.PointsPerRank == 5 ? W40KDHOwlFeatureWriter.AttributeType : W40KDHOwlFeatureWriter.SkillType;
            return metadata.IsEmpty || (metadata.TryGetType(blueprint, out var runtimeType)
                && string.Equals(runtimeType.Split(',')[0], type, StringComparison.Ordinal));
        }

        public static W40KDHAdvancementDefinition Find(string blueprint) =>
            blueprint is not null && Entries.TryGetValue(blueprint, out var definition) ? definition : null;
    }
}
