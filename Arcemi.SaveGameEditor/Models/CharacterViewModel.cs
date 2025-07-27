using Arcemi.Models;
using System;
using System.Collections.Generic;
namespace Arcemi.SaveGameEditor.Models
{
    public class CharacterViewModel
    {
        private readonly IEditFileSession _session;

        public CharacterViewModel(IEditFileSession session)
        {
            _session = session;
        }

        public bool CanEdit => _session.CanEdit;
        public IEnumerable<IGameUnitModel> Characters => _session.Game.Characters;
        public IGameAccessor Game => _session.Game;

        public IGameResourcesProvider Resources => _session.Game.Definition.Resources;

        public bool IsPlayerButNotMainCharacter(IGameUnitModel unit)
        {
            if (unit.Type != UnitEntityType.Player) return false;
            if (string.IsNullOrEmpty(unit.UniqueId)) return false;
            return !string.Equals(_session.Game.MainCharacterId, unit.UniqueId, StringComparison.Ordinal);
        }

        public IGameUnitModel GetOwnerOf(IGameUnitModel unit) => _session.Game.GetOwnerOf(unit);

        public void SetAsHero(IGameUnitModel unit)
        {
            _session.Game.SetMainCharacter(unit);
        }

        public void DeleteCharacter(IGameUnitModel unit)
        {
            _session.Game.Characters.Remove(unit);
        }
    }
}
