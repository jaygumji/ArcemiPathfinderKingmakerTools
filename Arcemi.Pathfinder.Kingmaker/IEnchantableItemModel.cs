using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IEnchantableItemModel
    {
        FactsContainerModel Facts { get; }
        int EnchantmentLevel { get; set; }
        int MaxEnchantmentLevel { get; }
    }

    public static class EnchantableItemModelExtensions
    {
        public static bool HasEnchantment<T>(this T item, EnchantmentSpec enchantment)
            where T : IEnchantableItemModel
        {
            return item.Facts.Items.OfType<EnchantmentFactItemModel>().Any(f => f.Blueprint.Eq(enchantment.Blueprint));
        }

        public static EnchantmentFactItemModel AddEnchantmentFact<T>(this T item, EnchantmentSpec enchantment)
            where T : IEnchantableItemModel
        {
            var fact = (EnchantmentFactItemModel)item.Facts.Items.Add(EnchantmentFactItemModel.Prepare);
            fact.Blueprint = enchantment.Blueprint;
            fact.ActivateCustomEnchantments();
            foreach (var component in enchantment.Components) {
                component.AddTo(fact.Components);
            }
            return fact;
        }
    }
}