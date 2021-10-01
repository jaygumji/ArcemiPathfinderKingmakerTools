using Arcemi.Pathfinder.Kingmaker;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public interface ISaveDataProvider
    {
        PlayerModel Player { get; }
        PartyModel Party { get; }
        UnitEntityModel PlayerEntity { get; set; }
        IEnumerable<UnitEntityModel> Characters { get; }
    }
}