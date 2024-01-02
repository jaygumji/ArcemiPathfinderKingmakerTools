using System;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameManagementPlaceIntDataEntry : IGameDataInteger
    {
        public RefModel Ref { get; }
        public ModelDataAccessor A { get; }
        public string Label { get; }
        public int Value { get => A?.Value<int>("InitialValue") ?? 0; set => A?.Value(value, "InitialValue"); }
        public int MinValue => A?.Value<int>() ?? 0;
        public int MaxValue => A?.Value<int>() ?? 0;
        public int Modifiers => (int)Math.Round(A?.List<RefModel>("Modifiers")?.Sum(x => x.GetAccessor().Value<double>("Value")) ?? 0.0);
        public bool IsReadOnly => Ref is null;
        public GameDataSize Size => GameDataSize.Medium;

        public W40KRTGameManagementPlaceIntDataEntry(RefModel model, string label)
        {
            Ref = model;
            Label = label;
            A = model?.GetAccessor();
        }
    }
}