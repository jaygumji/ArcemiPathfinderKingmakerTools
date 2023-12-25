using System;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal class W40KRTGameManagementPlaceIntDataEntry : IGameManagementPlaceModelDataGroupingIntegerEntry
    {
        public RefModel Ref { get; }
        public ModelDataAccessor A { get; }
        public string Label { get; }
        public int Value { get => A.Value<int>("InitialValue"); set => A.Value(value, "InitialValue"); }
        public int MinValue => A.Value<int>();
        public int MaxValue => int.MaxValue; // A.Value<int>();
        public int Modifiers => (int)Math.Round(A.List<RefModel>("Modifiers")?.Sum(x => x.GetAccessor().Value<double>("Value")) ?? 0.0);

        public W40KRTGameManagementPlaceIntDataEntry(RefModel model, string label)
        {
            Ref = model;
            A = model.GetAccessor();
            Label = label;
        }
    }
}