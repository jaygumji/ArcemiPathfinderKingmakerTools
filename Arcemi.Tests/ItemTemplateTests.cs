using Arcemi.Models;
using Arcemi.Models.PathfinderWotr;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arcemi.Tests
{
    public class ItemTemplateTests
    {
        private string GetGameFolder()
        {
            const string folder = @"D:\Games\Pathfinder Wrath of the Righteous";
            if (!Directory.Exists(folder)) {
                return null;
            }
            return folder;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task EnchantmentsRemainAvailableWithMissingBlueprintData(bool armor)
        {
            var folder = Directory.CreateTempSubdirectory();
            try {
                using var resources = new GameResources(GameDefinition.Pathfinder_WrathOfTheRighteous, new WotrBlueprintProvider());
                var bundles = folder.CreateSubdirectory("Bundles");
                var enchantmentType = armor ? WotrBlueprintProvider.ArmorEnchantment : WotrBlueprintProvider.WeaponEnchantment;
                var predefined = armor ? Enchantments.Armor.All : Enchantments.Weapon.All;
                var entries = new[] {
                    new { Guid = "11111111111111111111111111111111", Name = "CustomEnchantment", Path = @"Blueprints\Custom.jbp" },
                    new { Guid = "22222222222222222222222222222222", Name = "MissingEntryEnchantment", Path = @"Blueprints\Missing.jbp" },
                    new { Guid = "33333333333333333333333333333333", Name = "MissingDataEnchantment", Path = @"Blueprints\MissingData.jbp" },
                    new { Guid = "44444444444444444444444444444444", Name = "MissingPathEnchantment", Path = (string)null },
                };
                await File.WriteAllTextAsync(Path.Combine(bundles.FullName, "cheatdata.json"), JsonConvert.SerializeObject(new {
                    Entries = entries.Select(entry => new { entry.Guid, entry.Name, entry.Path, TypeFullName = enchantmentType.FullName })
                }));
                var accessor = new ModelDataAccessor(new JObject(), new References());
                ItemModel item = armor ? new ArmorItemModel(accessor) : new WeaponItemModel(accessor);
                await resources.LoadGameFolderAsync(null, folder.FullName);
                Assert.Equal(predefined.Count, resources.GetEnchantments(item).Count);

                using (var archive = ZipFile.Open(Path.Combine(folder.FullName, "blueprints.zip"), ZipArchiveMode.Create)) {
                    using (var writer = new StreamWriter(archive.CreateEntry("Custom.jbp").Open())) {
                        writer.Write("""{"Data":{"Components":[{"name":"CustomComponent"}]}}""");
                    }
                    using (var writer = new StreamWriter(archive.CreateEntry("MissingData.jbp").Open())) {
                        writer.Write("{}");
                    }
                }
                await resources.LoadGameFolderAsync(null, folder.FullName);
                var enchantments = resources.GetEnchantments(item);
                Assert.Equal(predefined.Count + 1, enchantments.Count);
                var custom = Assert.Single(enchantments, enchantment => enchantment.Blueprint == entries[0].Guid);
                Assert.Equal("CustomComponent", Assert.Single(custom.Components).Id);
            }
            finally {
                folder.Delete(recursive: true);
            }
        }

        [Fact]
        public async Task AbrubtForce()
        {
            var gameFolder = GetGameFolder();
            if (string.IsNullOrWhiteSpace(gameFolder)) return;

            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            await res.LoadGameFolderAsync(null, gameFolder);
            var feat = res.Blueprints.GetEntries(WotrBlueprintProvider.ItemWeapon).Where(f => f.DisplayName == "Abrupt Force").First();
            
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
        public async Task AncestralDwarwenShield()
        {
            var gameFolder = GetGameFolder();
            if (string.IsNullOrWhiteSpace(gameFolder)) return;

            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            await res.LoadGameFolderAsync(null, gameFolder);
            var feat = res.Blueprints.GetEntries(WotrBlueprintProvider.ItemShield).Where(f => f.DisplayName == "Ancestral Dwarwen Shield").First();

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
        public async Task SpikedShieldOfHolyThornShield()
        {
            var gameFolder = GetGameFolder();
            if (string.IsNullOrWhiteSpace(gameFolder)) return;

            var res = GameDefinition.Pathfinder_WrathOfTheRighteous.Resources;
            await res.LoadGameFolderAsync(null, gameFolder);
            var feat = res.Blueprints.GetEntries(WotrBlueprintProvider.ItemShield).Where(f => f.DisplayName == "Spiked Shield Of Holy Thorn Shield").First();

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
