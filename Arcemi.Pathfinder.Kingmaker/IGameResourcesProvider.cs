using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IGameResourcesProvider
    {
        PathfinderAppData AppData { get; }
        BlueprintMetadata Blueprints { get; }

        string GetPortraitId(string blueprint);
        string GetCharacterPotraitIdentifier(string blueprint);
        string GetLeaderPortraitUri(string blueprint);
        string GetPortraitsUri(string id);
        bool TryGetPortraitsUri(string characterBlueprint, out string uri);
        IReadOnlyDictionary<PortraitCategory, IReadOnlyList<Portrait>> GetAvailablePortraits();

        string GetCharacterName(string blueprint);
        string GetArmyUnitName(string blueprint);
        IEnumerable<IBlueprintMetadataEntry> GetAvailableArmyUnits();
        bool TryGetLeader(string blueprint, out LeaderDataMapping leader);
        string GetLeaderName(string blueprint);

        string GetRaceName(string id);
        string GetClassTypeName(string id);
        string GetClassArchetypeName(IReadOnlyList<string> archetypes);
        bool IsMythicClass(string blueprint);
        string GetItemName(string blueprint);

        ItemModel GetItemTemplate(string blueprint);
        ItemModel GetItemTemplate(IBlueprintMetadataEntry metadata);
        FactItemModel GetFeatTemplate(string blueprint);
        FactItemModel GetFeatTemplate(IBlueprintMetadataEntry metadata);

        IReadOnlyList<EnchantmentSpec> GetEnchantments(ItemModel item);
    }
}
