using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    [SaveFileType("Kingmaker.EntitySystem.Entities.UnitEntityData, Assembly-CSharp")]
    public class UnitEntityModel : RefModel
    {
        public UnitEntityModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string UniqueId => A.Value<string>();

        public bool IsRevealed { get => A.Value<bool>("m_IsRevealed"); set => A.Value(value, "m_IsRevealed"); }
        public bool IsInGame { get => A.Value<bool>("m_IsInGame", defaultValue: true); set => A.Value(value, "m_IsInGame"); }

        public CharacterModel Descriptor => A.Object<CharacterModel>();
        public FactsContainerModel Facts => A.Object(factory: a => new FactsContainerModel(a));
        public PartsContainerModel Parts => A.Object(factory: a => new PartsContainerModel(a));

        private static readonly IList<string> IgnoreRankFeatures = new[] {
            "6948b379c0562714d9f6d58ccbfa8faa"
        };
        public IEnumerable<FeatureFactItemModel> FindRankedFeatures()
        {
            return Facts.Items.OfType<FeatureFactItemModel>().Where(f => f.Rank > 0 && !IgnoreRankFeatures.Contains(f.Blueprint)).ToArray();
        }

        public bool IsPet => Parts.Items.OfType<UnitPetPartItemModel>().Any();

        public bool IsPlayer => string.Equals(Descriptor?.Blueprint, "4391e8b9afbb0cf43aeba700c089f56d", StringComparison.Ordinal);
        public bool IsMercenary => string.Equals(Descriptor?.Blueprint, "baaff53a675a84f4983f1e2113b24552", StringComparison.Ordinal);
        public bool IsCompanion => !IsPlayer && !IsMercenary && !IsPet;
    }
}
