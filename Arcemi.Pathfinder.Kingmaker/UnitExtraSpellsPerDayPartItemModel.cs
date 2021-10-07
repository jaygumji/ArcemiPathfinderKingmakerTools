using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class UnitExtraSpellsPerDayPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.UnitPartExtraSpellsPerDay, Assembly-CSharp";

        public static UnitExtraSpellsPerDayPartItemModel AddTo(PartsContainerModel parts)
        {
            return (UnitExtraSpellsPerDayPartItemModel)parts.Items.Add((refs, obj) => {
                obj.Add("$type", TypeRef);
                obj.Add("BonusSpells", new Newtonsoft.Json.Linq.JArray { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            });
        }

        public UnitExtraSpellsPerDayPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public ListValueAccessor<int> BonusSpells => A.ListValue<int>(createIfNull: true);

        public IEnumerable<IndexAccessor> BonusSpellsAccessors => BonusSpells.Select((x, i) => new IndexAccessor(i, BonusSpells));

        public class IndexAccessor
        {
            private readonly ListValueAccessor<int> _list;

            public int Index { get; }
            public int Level { get; }
            public int Value { get => _list[Index]; set => _list[Index] = value; }
            public IndexAccessor(int index, ListValueAccessor<int> list)
            {
                _list = list;
                Index = index;
                Level = index;
            }
        }
    }
}