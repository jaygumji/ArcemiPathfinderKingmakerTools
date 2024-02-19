using System.Linq;
using System;

namespace Arcemi.Models.Kingmaker
{
    public class KingmakerGameAccessor : IGameAccessor
    {
        public static bool Detect(IGameEditFile file)
        {
            return file.Party.UnitEntities.Any(x => x.Descriptor is object);
        }

        public GameDefinition Definition { get; } = GameDefinition.Pathfinder_Kingmaker;

        public KingmakerGameAccessor(IGameEditFile file)
        {
            File = file;
            Characters = new GameModelCollection<IGameUnitModel, UnitEntityModel>(file.Party.UnitEntities, a => new KingmakerGameUnitModel(a), a => a.Descriptor is object);
            MainCharacter = Characters.FirstOrDefault(c => c.UniqueId.Eq(MainCharacterId));
            Party = new KingmakerGamePartyModel(file.Player, Characters);
            SharedStash = new KingmakerGameInventoryModel(file.Player.SharedStash, file.Player.GameTime, "Shared Stash");
            SharedInventory = new KingmakerGameInventoryModel(MainCharacter.Ref.Descriptor.Inventory, file.Player.GameTime, "Party");
            Management = new KingmakerGameManagementModel(file.Player, Characters);
            State = new KingmakerGameStateModel(file.Player);
        }

        public string MainCharacterId {
            get => File.Player.GetAccessor().Object<Model>("m_MainCharacter").GetAccessor().Value<string>("m_UniqueId");
            set => File.Player.GetAccessor().Object<Model>("m_MainCharacter").GetAccessor().Value(value, "m_UniqueId");
        }
        public IGameEditFile File { get; }
        public IGamePartyModel Party { get; }
        public IGameUnitModel MainCharacter { get; private set; }
        public IGameModelCollection<IGameUnitModel> Characters { get; }
        public IGameInventoryModel SharedInventory { get; private set; }
        public IGameInventoryModel SharedStash { get; }
        public IGameManagementModel Management { get; }
        public IGameStateModel State { get; }

        public void BeforeSave()
        {
            foreach (var character in Characters) {
                if (character.Alignment.IsSupported && character.Alignment.History is object) {
                    var history = (KingmakerGameUnitAlignmentHistoryEntryModel)character.Alignment.History.LastOrDefault();
                    if (history is object && history.X == character.Alignment.X && history.Y == character.Alignment.Y) { continue; }

                    var kingmakerAlignment = (KingmakerGameUnitAlignmentModel)character.Alignment;

                    history = (KingmakerGameUnitAlignmentHistoryEntryModel) character.Alignment.History.AddByBlueprint(null);
                    history.Set(kingmakerAlignment.Vector.X, kingmakerAlignment.Vector.Y);
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
            SharedInventory = new KingmakerGameInventoryModel(unit.Ref.Descriptor.Inventory, File.Player.GameTime, "Party");
        }
    }
}
