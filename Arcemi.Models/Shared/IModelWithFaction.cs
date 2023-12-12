namespace Arcemi.Models
{
    public interface IModelWithFaction
    {
        string Faction { get; }
    }

    public static class ModelWithFactionExtensions
    {
        public static bool IsFactionCrusaders(this IModelWithFaction model)
        {
            return string.Equals(model?.Faction, "Crusaders", System.StringComparison.Ordinal);
        }

        public static bool IsFactionDemons(this IModelWithFaction model)
        {
            return string.Equals(model?.Faction, "Demons", System.StringComparison.Ordinal);
        }
    }
}