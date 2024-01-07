using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTGameAccessor : IGameAccessor
    {
        public static bool Detect(IGameEditFile file) => file.Player.GetAccessor().HasProperty("StarSystemsState");
        public GameDefinition Definition => GameDefinition.Warhammer40K_RogueTrader;

        public W40KRTGameAccessor(IGameEditFile file)
        {
            File = file;
            Party = new W40KRTGamePartyModel(file.Player, file.Header);
            SharedStash = new W40KRTCargoInventoryModel(file.Player.GetAccessor().Object<RefModel>("CargoState"));
            Characters = new GameModelCollection<IGameUnitModel, UnitEntityModel>(file.Party.UnitEntities, a => new W40KRTGameUnitModel(a));
            MainCharacter = Characters.FirstOrDefault(c => c.UniqueId.Eq(MainCharacterId));
            SharedInventory = new W40KRTGameSharedInventoryModel(((W40KRTGameUnitModel)MainCharacter)?.RefInventory);
            Management = new W40KRTGameManagementModel(file.Player);
            State = new W40KRTGameStateModel(file.Player);
        }

        public string MainCharacterId { get => File.Player.GetAccessor().Value<string>("MainCharacter"); set => File.Player.GetAccessor().Value(value, "MainCharacter"); }
        public IGameEditFile File { get; }
        public IGamePartyModel Party { get; }
        public IGameUnitModel MainCharacter { get; private set; }
        public IGameModelCollection<IGameUnitModel> Characters { get; }
        public IGameInventoryModel SharedInventory { get; }
        public IGameInventoryModel SharedStash { get; }
        public IGameManagementModel Management { get; }
        public IGameStateModel State { get; }

        public void BeforeSave()
        {
        }

        public IGameUnitModel GetOwnerOf(IGameUnitModel unit)
        {
            return null;
        }

        public void SetMainCharacter(IGameUnitModel unit)
        {
            MainCharacterId = unit.UniqueId;
            MainCharacter = unit;
        }
    }
}
