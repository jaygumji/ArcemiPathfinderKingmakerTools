using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
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

        public IEnumerable<SpellIndexAccessor> BonusSpellsAccessors => BonusSpells.Select((x, i) => new SpellIndexAccessor(i, BonusSpells));

    }
}