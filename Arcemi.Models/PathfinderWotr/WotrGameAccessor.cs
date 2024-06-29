using System.Linq;
using System;
using Arcemi.Models.Shared;

namespace Arcemi.Models.PathfinderWotr
{
    public class WotrGameAccessor : IGameAccessor
    {
        public static bool Detect(IGameEditFile file)
        {
            return file.Player.Corruption is object && file.Party.UnitEntities.Any(x => x.Descriptor is object);
        }

        public GameDefinition Definition { get; } = GameDefinition.Pathfinder_WrathOfTheRighteous;

        public WotrGameAccessor(IGameEditFile file)
        {
            File = file;
            GameTimeProvider = file.Player.GetGameTimeProvider();
            Characters = new GameModelCollection<IGameUnitModel, UnitEntityModel>(file.Party.UnitEntities, a => new WotrGameUnitModel(a, GameTimeProvider), a => a.Descriptor is object);
            Party = new WotrGamePartyModel(file.Player, Characters);
            SharedStash = new WotrGameInventoryModel(file.Player.SharedStash, "Shared Stash");
            MainCharacter = Characters.FirstOrDefault(c => c.UniqueId.Eq(MainCharacterId));
            SharedInventory = new WotrGameInventoryModel(MainCharacter.Ref.Descriptor.Inventory, "Party");
            PathfinderInventoryPatch.RemoveNaturalUnequippedWeapons(SharedInventory);
            Management = new WotrGameManagementModel(file.Player);
            State = new WotrGameStateModel(file.Player);
        }

        public string MainCharacterId { get => File.Player.GetAccessor().Value<string>("m_MainCharacter"); set => File.Player.GetAccessor().Value(value, "m_MainCharacter"); }
        public IGameEditFile File { get; }
        public IGamePartyModel Party { get; }
        public IGameUnitModel MainCharacter { get; private set; }
        public IGameModelCollection<IGameUnitModel> Characters { get; }
        public IGameInventoryModel SharedInventory { get; private set; }
        public IGameInventoryModel SharedStash { get; }
        public IGameManagementModel Management { get; }
        public IGameStateModel State { get; }
        public IGameTimeProvider GameTimeProvider { get; }

        public void BeforeSave()
        {
            foreach (WotrGameUnitModel character in Characters) {
                if (character.Ref.Descriptor?.Alignment?.History != null) {
                    var vector = character.Ref.Descriptor.Alignment.Vector;
                    var history = character.Ref.Descriptor.Alignment.History.LastOrDefault();
                    if (!string.Equals(vector.Value, history?.Position, StringComparison.Ordinal)) {
                        history = character.Ref.Descriptor.Alignment.History.Add();
                        history.Vector.X = vector.X;
                        history.Vector.Y = vector.Y;
                        history.Direction = vector.DirectionText;
                        history.Provider = null;
                    }
                }
            }
        }

        public IGameUnitModel GetOwnerOf(IGameUnitModel unit)
        {
            var petPart = unit.Ref.Parts.Items.OfType<UnitPetPartItemModel>().FirstOrDefault();
            if (petPart == null || string.IsNullOrEmpty(petPart.MasterRef?.Ref)) return null;
            var owner = Characters.FirstOrDefault(c => c.UniqueId.Eq(petPart.MasterRef.Ref));
            return owner;
        }

        public void SetMainCharacter(IGameUnitModel unit)
        {
            MainCharacterId = unit.UniqueId;
            MainCharacter = unit;
            SharedInventory = new WotrGameInventoryModel(unit.Ref.Descriptor.Inventory, "Party");
        }
    }
}
