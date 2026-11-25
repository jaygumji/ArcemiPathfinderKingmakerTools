using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    internal static class W40KRTForgeWorldAugments
    {
        public const string Homeworld = "6ba65fc6da1e40cf929d922f5b021a52";
        public const string Selection = "38a36879cbe44b9d982a9014cf9e29ed";
        public const string Slot = "1ec46a6594cf4377a1eaef6525df7b42";
        private const string Equipped = "77f907f993554283a083f2241cb2d9ad";
        private const string Metallicization = "9ab5246290b8436fb74cf272b2efc8c7";

        private static readonly (string Grant, string Item, string Effect, string Name)[] Options = {
            ("06a6304a4bb64df39f03319ca284de40", "9574a2028a5640b181202068c88759f8", "96fd88d5d1fe4ba1a35746cde88aaf3c", "Analytics System"),
            ("49572d6dd2064c718231d60b31abf3f4", "b986d0399c6e4165a43596b8101abf8c", "eda2be0dda7841ce9cce6186ef6e0ce1", "Locomotion System"),
            ("84c50690d9b84dadb77adba474a90ead", "ada7f3aa826a411d97fe3715e9c33cf5", "08d6f8a4ebe143fe995582bb5c02072b", "Subdermal Armour")
        };

        public static W40KRTCharacterSelectionOption[] CreateOptions() => Options
            .Select(o => new W40KRTCharacterSelectionOption(o.Grant, o.Name,
                upgrade: a => Equip((W40KRTGameUnitModel)a.Owner, o.Item, o.Effect))).ToArray();

        public static string CanonicalChoice(string id) => Options.FirstOrDefault(o => o.Effect.Eq(id)).Grant ?? id;

        private static ListAccessor<RefModel> GetSlots(W40KRTGameUnitModel unit, bool create = false)
        {
            return ((W40KRTGameUnitBodyModel)unit.Body).Ref?.GetAccessor()
                .Object<RefModel>("Augments", createIfNull: create)?.GetAccessor()
                .List<RefModel>("m_Slots", createIfNull: create);
        }

        private static RefModel GetSlot(ListAccessor<RefModel> slots) =>
            slots?.FirstOrDefault(s => s.GetAccessor().Value<string>("Key").Eq(Slot));

        public static void Remove(W40KRTGameUnitModel unit)
        {
            var slots = GetSlots(unit);
            var entry = GetSlot(slots);
            if (entry is null) return;
            var slot = entry.GetAccessor().Object<RefModel>("Value");
            RemoveInstalled(unit, slot);
            slots.Remove(entry);
        }

        private static void RemoveInstalled(W40KRTGameUnitModel unit, RefModel slot)
        {
            var a = slot.GetAccessor();
            var id = a.Value<string>("m_ItemRef");
            if (!id.HasValue()) return;
            W40KRTProgressionFacts.RemoveItemGrants(unit, id);
            unit.Mediator.SharedInventory?.RemoveEquipped(id);
            a.Value(JValue.CreateNull(), "m_ItemRef");
        }

        private static void Equip(W40KRTGameUnitModel unit, string blueprint, string effect)
        {
            var inventory = unit.Mediator.SharedInventory;
            if (inventory?.IsSupported != true || !unit.Body.IsSupported) return;
            var slots = GetSlots(unit, create: true);
            var entry = GetSlot(slots) ?? slots.Add((refs, obj) => {
                obj.Add("Key", Slot);
                var value = refs.Create();
                value.Add("m_Active", true);
                obj.Add("Value", value);
            });
            var slot = entry.GetAccessor().Object<RefModel>("Value");
            var currentId = slot.GetAccessor().Value<string>("m_ItemRef");
            if (inventory.FindEquipped(currentId)?.Blueprint.Eq(blueprint) == true) return;
            RemoveInstalled(unit, slot);

            var item = inventory.AddEquippedAugment(blueprint, unit.UniqueId);
            var itemId = item.GetAccessor().Value<string>("UniqueId");
            slot.GetAccessor().Value(itemId, "m_ItemRef");
            var applied = item.GetAccessor().List<FactItemModel>("m_FactsAppliedToWielder", FactItemModel.Factory, createIfNull: true);
            var feats = (GameModelCollection<IGameUnitFeatEntry, FactItemModel>)unit.Feats;
            var factsComeFirst = JTokenDocumentOrder.Instance.Compare(unit.Ref.GetAccessor().UnderlyingObject["Facts"], item.GetAccessor().UnderlyingObject) < 0;
            foreach (var id in new[] { Equipped, Metallicization, effect }) {
                // Older editor builds wrote the selected effect as a standalone feature.
                foreach (var legacy in feats.OfType<W40KRTGameUnitFeatEntry>()
                    .Where(f => f.Blueprint.Eq(id) && !W40KRTProgressionFacts.HasSources(f.Model)).ToArray()) {
                    feats.Remove(legacy);
                }
                if (factsComeFirst) {
                    var feat = (W40KRTGameUnitFeatEntry)feats.AddByBlueprint(id);
                    AddItemSource(feat.Model.GetAccessor().UnderlyingObject, itemId);
                    applied.AddRef(feat.Model);
                }
                else {
                    var model = applied.Add((_, obj) => {
                        W40KRTGameModelFeatCollectionWriter.PrepareFeature(obj, id, unit.UniqueId);
                        AddItemSource(obj, itemId);
                    });
                    feats.AddReference(model);
                }
            }
        }

        private static void AddItemSource(JObject fact, string itemId)
        {
            ((JArray)fact["m_Sources"]).Add(new JObject { { "m_EntityRef", itemId } });
        }

    }
}
