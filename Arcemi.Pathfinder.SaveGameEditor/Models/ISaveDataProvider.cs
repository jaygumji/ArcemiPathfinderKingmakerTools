using Arcemi.Pathfinder.Kingmaker;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public interface ISaveDataProvider
    {
        PlayerModel Player { get; }
        UnitEntityModel PlayerEntity { get; }
        IEnumerable<UnitEntityModel> Characters { get; }
    }
}