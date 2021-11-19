using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class EtudesSystemModel : RefModel
    {
        public EtudesSystemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }
        public ListAccessor<KeyValuePairModel<string>> EtudesData => A.List("m_EtudesData", a => new KeyValuePairModel<string>(a));
        public PartsContainerModel Parts => A.Object(factory: a => new PartsContainerModel(a));
        public FactsContainerModel Facts => A.Object(factory: a => new FactsContainerModel(a));
    }
    public static class EtudeStates
    {
        public static IReadOnlyList<string> All { get; } = new[] {
            Unknown,
            Started,
            Completed,
            PreStarted,
            PreCompleted
        };

        public const string Unknown = nameof(Unknown);
        public const string Started = nameof(Started);
        public const string Completed = nameof(Completed);
        public const string PreStarted = nameof(PreStarted);
        public const string PreCompleted = nameof(PreCompleted);
    }
}