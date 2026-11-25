using System;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal sealed class W40KRTColonyConsumableModel : RefModel
    {
        // Rogue Trader measures colony cooldowns in eight-hour campaign segments.
        private const int HoursPerSegment = 8;

        public W40KRTColonyConsumableModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Item => A.Value<string>();
        public int SegmentsToRefill => A.Value<int>();
        public int MaxCount => A.Value<int>();
        public TimeSpan LastRefill { get => A.Value<TimeSpan>(); private set => A.Value(value); }

        public IGameDataObject CreateEditorData(PlayerModel player, InventoryModel loot, IGameResourcesProvider resources)
        {
            long? UnclaimedCount() => loot?.Items?.Where(item => item.Blueprint.Eq(Item)).Sum(item => (long)item.Count);

            bool CanMakeEligible() => !string.IsNullOrEmpty(Item)
                && UnclaimedCount() is long count && count < MaxCount
                && SegmentsToRefill > 0
                && player.GameTime.TotalHours >= (long)SegmentsToRefill * HoursPerSegment
                && LastRefill > ReadyTimestamp(player.GameTime);

            var name = string.IsNullOrEmpty(Item) ? "Unknown reward" : resources.GetItemName(Item).OrIfEmpty(Item);
            return GameDataModels.Object(name, new IGameData[] {
                GameDataModels.Text("Last refill (campaign time)", this, reward => reward.LastRefill.ToString("c"), size: GameDataSize.Medium),
                GameDataModels.Integer("Refill interval (segments)", this, reward => reward.SegmentsToRefill, minValue: 0),
                GameDataModels.Integer("Maximum", this, reward => reward.MaxCount, minValue: 0, size: GameDataSize.Small),
                GameDataModels.Text("Unclaimed", this, _ => UnclaimedCount()?.ToString() ?? "Unavailable", size: GameDataSize.Small),
                GameDataModels.Action("Make eligible to refill", () => {
                    if (CanMakeEligible()) LastRefill = ReadyTimestamp(player.GameTime);
                }, CanMakeEligible),
            }, this);
        }

        private TimeSpan ReadyTimestamp(TimeSpan campaignTime)
            => campaignTime - TimeSpan.FromHours((long)SegmentsToRefill * HoursPerSegment);
    }
}
