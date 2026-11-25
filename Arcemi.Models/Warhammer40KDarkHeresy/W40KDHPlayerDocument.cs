using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Arcemi.Models.Warhammer40KDarkHeresy
{
    internal sealed class W40KDHPlayerDocument
    {
        internal static readonly SaveEntry<byte[]> PlayerEntry = new("player.owl", SaveGameFile.ReadBytes);
    
        private readonly byte[] _player;
        private long? _savedMoney;

        public W40KDHPlayerDocument(SaveGameFile archive)
        {
            _player = archive.Read(PlayerEntry);
            Data = W40KDHOwlTraversal.ReadPlayer(_player);
            _savedMoney = Data.Money;
            MoneyText = _savedMoney?.ToString(CultureInfo.InvariantCulture);
            var writable = W40KDHOwlChecksums.CanUpdate(archive, PlayerEntry.Name, _player);
            CanEditMoney = _savedMoney.HasValue && Data.MoneyOffset.HasValue && writable;
            Inventory = new W40KDHInventoryModel("Shared stash", Data.Stash, GameDefinition.Warhammer40K_DarkHeresy.Resources, writable);
        }

        public W40KDHPlayerData Data { get; }
        public W40KDHInventoryModel Inventory { get; }
        public bool CanEdit => CanEditMoney || Inventory.CanEdit;
        public bool CanEditMoney { get; }
        public string MoneyText { get; set; }

        public void StageChanges(Dictionary<string, byte[]> changes)
        {
            long? money = CanEditMoney ? ReadMoneyValue() : null;
            if (!Inventory.HasChanges && (!money.HasValue || money == _savedMoney)) return;

            var player = (byte[])_player.Clone();
            Inventory.WriteTo(player);
            if (money.HasValue) {
                var field = player.AsSpan(Data.MoneyOffset.Value, sizeof(long));
                if (BinaryPrimitives.ReadInt64LittleEndian(field) != Data.Money.Value) {
                    throw new InvalidDataException("The OWL money field differs from the value originally read.");
                }
                BinaryPrimitives.WriteInt64LittleEndian(field, money.Value);
            }

            changes.Add(PlayerEntry.Name, player);
        }

        public void AcceptChanges()
        {
            Inventory.AcceptChanges();
            if (CanEditMoney) _savedMoney = ReadMoneyValue();
        }

        private long ReadMoneyValue()
        {
            if (!long.TryParse(MoneyText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var money) || money < 0) {
                throw new InvalidDataException("Money must be a nonnegative whole number within the 64-bit integer range.");
            }
            return money;
        }
    }
}
