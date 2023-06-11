using Arcemi.Pathfinder.Kingmaker;
using Newtonsoft.Json.Linq;
using System.Linq;
using Xunit;

namespace Arcemi.Pathfinder.Tests
{
    public class ItemTemplateTests
    {
        [Fact]
        public void AbrubtForce()
        {
            var res = new GameResources();
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(BlueprintTypes.ItemWeapon).Where(f => f.DisplayName == "Abrupt Force").First();
            
            var template = res.GetItemTemplate(feat.Id);
            Assert.NotNull(template);
            Assert.NotEmpty(template.Facts.Items);

            var export = template.Export();
            Assert.NotNull(export);

            var refs = new References(res);
            var raw = new JObject();
            WeaponItemModel.Prepare(refs, raw, feat);

            var accessor = new ModelDataAccessor(raw, refs, res);
            var item = ItemModel.Create(accessor);

            item.Import(template);
            Assert.Equal(3, item.Facts.Items.Count);

            var displayableFacts = item.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Equal(2, displayableFacts.Length);
        }

        [Fact]
        public void AncestralDwarwenShield()
        {
            var res = new GameResources();
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(BlueprintTypes.ItemShield).Where(f => f.DisplayName == "Ancestral Dwarwen Shield").First();

            var template = (ShieldItemModel)res.GetItemTemplate(feat.Id);
            Assert.NotNull(template);
            Assert.NotNull(template.ArmorComponent);

            var export = template.Export();
            Assert.NotNull(export);

            var refs = new References(res);
            var raw = new JObject();
            ShieldItemModel.Prepare(refs, raw, feat);

            var accessor = new ModelDataAccessor(raw, refs, res);
            var item = (ShieldItemModel)ItemModel.Create(accessor);

            item.Import(template);
            Assert.Equal(2, item.ArmorComponent.Facts.Items.Count);

            var displayableFacts = item.ArmorComponent.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Equal(1, displayableFacts.Length);
        }

        [Fact]
        public void SpikedShieldOfHolyThornShield()
        {
            var res = new GameResources();
            res.LoadGameFolder(@"C:\Games\Pathfinder Wrath of the Righteous");
            var feat = res.Blueprints.GetEntries(BlueprintTypes.ItemShield).Where(f => f.DisplayName == "Spiked Shield Of Holy Thorn Shield").First();

            var template = (ShieldItemModel)res.GetItemTemplate(feat.Id);
            Assert.NotNull(template);
            Assert.NotNull(template.WeaponComponent);
            Assert.NotNull(template.ArmorComponent);

            var export = template.Export();
            Assert.NotNull(export);

            var refs = new References(res);
            var raw = new JObject();
            ShieldItemModel.Prepare(refs, raw, feat);

            var accessor = new ModelDataAccessor(raw, refs, res);
            var item = (ShieldItemModel)ItemModel.Create(accessor);

            item.Import(template);
            Assert.Equal(2, item.WeaponComponent.Facts.Items.Count);

            item.Import(template);
            Assert.Equal(1, item.ArmorComponent.Facts.Items.Count);

            var displayableFacts = item.WeaponComponent.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Equal(1, displayableFacts.Length);

            displayableFacts = item.ArmorComponent.Facts.Items.OfType<EnchantmentFactItemModel>().Where(f => !f.IsLevel).ToArray();
            Assert.Equal(0, displayableFacts.Length);
        }
    }
}
