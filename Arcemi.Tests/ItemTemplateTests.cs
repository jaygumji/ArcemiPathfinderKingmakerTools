using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using Newtonsoft.Json.Linq;
using System.Linq;
using Xunit;

namespace Arcemi.Tests
{
    public class ItemTemplateTests
    {
        [Fact]
        public void AbrubtForce()
        {
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(WotrBlueprintTypeProvider.ItemWeapon).Where(f => f.DisplayName == "Abrupt Force").First();
            
            var template = res.GetItemTemplate(feat.Id);
            Assert.NotNull(template);
            Assert.NotEmpty(template.Facts.Items);

            var export = template.Export();
            Assert.NotNull(export);

            var refs = new References();
            var raw = new JObject();
            WeaponItemModel.Prepare(refs, raw, feat);

            var accessor = new ModelDataAccessor(raw, refs);
            var item = ItemModel.Create(accessor);

            item.Import(template);
            Assert.Equal(3, item.Facts.Items.Count);

            var displayableFacts = item.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Equal(2, displayableFacts.Length);
        }

        [Fact]
        public void AncestralDwarwenShield()
        {
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(WotrBlueprintTypeProvider.ItemShield).Where(f => f.DisplayName == "Ancestral Dwarwen Shield").First();

            var template = (ShieldItemModel)res.GetItemTemplate(feat.Id);
            Assert.NotNull(template);
            Assert.NotNull(template.ArmorComponent);

            var export = template.Export();
            Assert.NotNull(export);

            var refs = new References();
            var raw = new JObject();
            ShieldItemModel.Prepare(refs, raw, feat);

            var accessor = new ModelDataAccessor(raw, refs);
            var item = (ShieldItemModel)ItemModel.Create(accessor);

            item.Import(template);
            Assert.Equal(2, item.ArmorComponent.Facts.Items.Count);

            var displayableFacts = item.ArmorComponent.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Single(displayableFacts);
        }

        [Fact]
        public void SpikedShieldOfHolyThornShield()
        {
            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(WotrBlueprintTypeProvider.ItemShield).Where(f => f.DisplayName == "Spiked Shield Of Holy Thorn Shield").First();

            var template = (ShieldItemModel)res.GetItemTemplate(feat.Id);
            Assert.NotNull(template);
            Assert.NotNull(template.WeaponComponent);
            Assert.NotNull(template.ArmorComponent);

            var export = template.Export();
            Assert.NotNull(export);

            var refs = new References();
            var raw = new JObject();
            ShieldItemModel.Prepare(refs, raw, feat);

            var accessor = new ModelDataAccessor(raw, refs);
            var item = (ShieldItemModel)ItemModel.Create(accessor);

            item.Import(template);
            Assert.Equal(2, item.WeaponComponent.Facts.Items.Count);

            item.Import(template);
            Assert.Single(item.ArmorComponent.Facts.Items);

            var displayableFacts = item.WeaponComponent.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Single(displayableFacts);

            displayableFacts = item.ArmorComponent.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Empty(displayableFacts);
        }
    }
}
