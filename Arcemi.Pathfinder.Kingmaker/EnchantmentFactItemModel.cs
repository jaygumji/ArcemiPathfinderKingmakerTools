using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class EnchantmentFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.Blueprints.Items.Ecnchantments.ItemEnchantment, Assembly-CSharp";
        public EnchantmentFactItemModel(ModelDataAccessor accessor) : base(accessor) { }

        public void ActivateCustomEnchantments()
        {
            if (ParentContext.AssociatedBlueprint == null) {
                // Needs to be set for the changes to take effect
                ParentContext.AssociatedBlueprint = Enchantments.TricksterKnowledgeArcanaTier3Feature;
            }
        }

        public bool IsLevel => Enchantments.IsLevel(Blueprint);
        public bool IsMapped => Enchantments.IsMapped(Blueprint);

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}