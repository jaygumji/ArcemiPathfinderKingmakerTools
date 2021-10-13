using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IGameResourcesProvider
    {
        PathfinderAppData AppData { get; }
        BlueprintData Blueprints { get; }

        string GetPortraitId(string blueprint);
        string GetCharacterPotraitIdentifier(string blueprint);
        string GetLeaderPortraitUri(string blueprint);
        string GetPortraitsUri(string id);
        bool TryGetPortraitsUri(string characterBlueprint, out string uri);
        IReadOnlyList<Portrait> GetAvailableFor(string characterBlueprint);

        string GetCharacterName(string blueprint);
        string GetArmyUnitName(string blueprint);
        IEnumerable<IBlueprint> GetAvailableArmyUnits();
        bool TryGetLeader(string blueprint, out LeaderDataMapping leader);
        string GetLeaderName(string blueprint);

        string GetRaceName(string id);
        string GetClassTypeName(string id);
        string GetClassArchetypeName(IReadOnlyList<string> archetypes);
        bool IsMythicClass(string blueprint);
        string GetItemName(string blueprint);
    }
}
