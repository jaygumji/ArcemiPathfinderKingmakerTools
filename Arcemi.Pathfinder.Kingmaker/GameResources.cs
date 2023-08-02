using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class GameResources : IGameResourcesProvider, IDisposable
    {
        private bool _isDevelopmentModeEnabled;

        public PathfinderAppData AppData { get; private set; }
        public BlueprintMetadata Blueprints { get; private set; }
        public List<FeatureFactItemModel> FeatTemplates { get; private set; }
        public GameBlueprintsArchive BlueprintsArchive { get; private set; }

        public GameResources()
        {
            Blueprints = BlueprintMetadata.Empty;
        }

        public void LoadGameFolder(string gameFolder)
        {
            Blueprints = BlueprintMetadata.Load(gameFolder);
            if (BlueprintsArchive is object) BlueprintsArchive.Dispose();
            BlueprintsArchive = new GameBlueprintsArchive(gameFolder, this);
        }

        public void SetStaticBlueprints(params BlueprintMetadataEntry[] entries)
        {
            Blueprints = new BlueprintMetadata(entries);
        }

        public void LoadFeatTemplates()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Defs", "FeatTemplates.json");
            var contents = File.ReadAllText(path);
            var jObjects = JsonConvert.DeserializeObject<List<JObject>>(contents);
            var templates = new List<FeatureFactItemModel>();
            foreach (var item in jObjects) {
                templates.Add(new FeatureFactItemModel(new ModelDataAccessor(item, new References(this), this)));
            }
            FeatTemplates = templates;
        }

        public void SetDevelopmentMode(bool isEnabled)
        {
            _isDevelopmentModeEnabled = isEnabled;
        }

        public void LoadAppDataWwwRoot(string appDataFolder)
        {
            var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
#if DEBUG
            if (!Directory.Exists(wwwRoot)) {
                // We're probably running in the debugger without dotnet publish
                wwwRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
#endif
            var resourceProvider = new WwwRootResourceProvider(wwwRoot, () => appDataFolder);
            AppData = new PathfinderAppData(resourceProvider);
        }

        public IReadOnlyDictionary<PortraitCategory, IReadOnlyList<Portrait>> GetAvailablePortraits()
        {
            var res = AppData.Portraits.Available
                .GroupBy(p => p.Category)
                .OrderBy(g => g.Key.Order)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<Portrait>)g.ToArray());

            var unknownUri = AppData.Portraits.GetUnknownUri();
            var unmappedPortraits = Blueprints.GetEntries(BlueprintTypes.Portrait)
                .Where(e => e.Name.Original.IndexOf("BCT_", StringComparison.OrdinalIgnoreCase) < 0)
                .Where(e => !AppData.Portraits.Available.Any(p => string.Equals(p.Key, e.Id, StringComparison.Ordinal)))
                .Select(e => new Portrait(e.Id, unknownUri, PortraitCategory.Unmapped, name: e.DisplayName))
                .ToArray();

            res.Add(PortraitCategory.Unmapped, unmappedPortraits);

            return res;
        }

        public string GetLeaderPortraitUri(string blueprint)
        {
            if (TryGetLeader(blueprint, out var leader)) {
                return GetPortraitsUri(leader.Portrait);
            }
            return GetPortraitsUri(blueprint);
        }

        public string GetPortraitsUri(string id)
        {
            return AppData.Portraits.GetPortraitsUri(id);
        }

        public bool TryGetPortraitsUri(string key, out string uri)
        {
            return AppData.Portraits.TryGetPortraitsUri(key, out uri);
        }

        public string GetPortraitId(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) {
                return null;
            }

            if (Mappings.Leaders.TryGetValue(blueprint, out var leader)) {
                return leader.Portrait.OrIfEmpty("_c_" + leader.Name);
            }
            if (Mappings.Characters.TryGetValue(blueprint, out var character)) {
                return "_c_" + character.Name;
            }
            return null;
        }

        public string GetCharacterPotraitIdentifier(string blueprint)
        {
            if (Mappings.Characters.TryGetValue(blueprint, out var character)) {
                if (!string.IsNullOrEmpty(character.Portrait)) {
                    return character.Portrait;
                }
            }
            return blueprint;
        }

        public string GetCharacterName(string blueprint)
        {
            return Mappings.Characters.TryGetValue(blueprint, out var character)
                ? character.Name
                : Blueprints.TryGetName(blueprint, out var name) ? name : "";
        }

        public string GetArmyUnitName(string blueprint)
        {
            return Blueprints.TryGetName(blueprint, out var name) ? name : blueprint;
        }

        public IEnumerable<IBlueprintMetadataEntry> GetAvailableArmyUnits()
        {
            var blueprints = Blueprints.GetEntries(BlueprintTypes.Unit);
            return blueprints.Where(b => b.Name.StartsWith("Army"));
        }

        public bool TryGetLeader(string blueprint, out LeaderDataMapping leader)
        {
            return Mappings.Leaders.TryGetValue(blueprint, out leader);
        }

        public string GetLeaderName(string blueprint)
        {
            if (string.IsNullOrEmpty(blueprint)) {
                return "";
            }

            if (Mappings.Leaders.TryGetValue(blueprint, out var leader)) {
                return leader.Name;
            }
            if (Mappings.Characters.TryGetValue(blueprint, out var character)) {
                return character.Name;
            }
            if (Blueprints.TryGetName(blueprint, out var name)) {
                return name;
            }
            return "";
        }

        public string GetRaceName(string id)
        {
            if (string.IsNullOrEmpty(id)) return "N/A";
            return Mappings.Races.TryGetValue(id, out var m) ? m.Name : Blueprints.TryGetName(id, out var name) ? name : id;
        }

        public string GetClassTypeName(string id)
        {
            return Mappings.Classes.TryGetValue(id, out var mapping)
                ? mapping.Name
                : Blueprints.TryGetName(id, out var name) ? name : id;
        }

        public string GetClassArchetypeName(IReadOnlyList<string> archetypes)
        {
            if (archetypes == null || archetypes.Count == 0) return null;

            var name = archetypes
                .Select(a => Mappings.Classes.TryGetValue(a, out var cls) ? cls.Name : Blueprints.TryGetName(a, out var n) ? n : null)
                .Where(a => a != null)
                .FirstOrDefault();

            if (name != null) return name;

            return archetypes.First();
        }

        public bool IsMythicClass(string blueprint)
        {
            return Mappings.Classes.TryGetValue(blueprint, out var cls) && (cls.Flags?.Contains("M") ?? false);
        }

        public string GetItemName(string blueprint)
        {
            return Blueprints.TryGetName(blueprint, out var name) ? name : null;
        }

        public ItemModel GetItemTemplate(string blueprint)
        {
            var metadata = Blueprints.Get(blueprint);
            return GetItemTemplate(metadata);
        }

        private void SetupFactItemModel(References refs, IBlueprintMetadataEntry metadata, Blueprint blueprintAccessor, FactItemModel template)
        {
            template.Blueprint = metadata.Id;
            template.Context = new FactContextModel(new ModelDataAccessor(new JObject(), refs, this));
            template.Context.AssociatedBlueprint = metadata.Id;

            if (template is FeatureFactItemModel feat) {
                if (blueprintAccessor.Data.Ranks > 1) {
                    feat.Rank = 1;
                    var rankToSource = feat.RankToSource.Insert(0);
                    rankToSource.Blueprint = feat.Blueprint;
                    rankToSource.Level = 1;
                }
            }

            foreach (var component in blueprintAccessor.Data.Components) {
                if (template.Components.ContainsKey(component.Name)) {
                    continue;
                }

                // Not all components need to be added,
                // and some components need extra data
                // Luckily, the game corrects both these problems for us
                template.Components.AddNull(component.Name);
            }
        }

        private void SetupEnchantments(References refs, Blueprint blueprintAccessor, FactsContainerModel facts)
        {
            if (blueprintAccessor.Data.Enchantments is null) return;
            if (blueprintAccessor.Data.Enchantments.Count <= 0) return;

            foreach (var enchantmentId in blueprintAccessor.Data.Enchantments) {
                if (enchantmentId == null || enchantmentId.Length < 5) continue;
                var encBlueprint = enchantmentId.Remove(0, 4); // Starts with "!bp_"
                var encMetadata = Blueprints.Get(encBlueprint);
                if (encMetadata is null) continue;

                var encBlueprintAccessor = BlueprintsArchive.Load(encMetadata);
                if (encBlueprintAccessor is null) continue;

                var enchantmentFact = facts.Items.Add(EnchantmentFactItemModel.Prepare);
                SetupFactItemModel(refs, encMetadata, blueprintAccessor, enchantmentFact);
            }
        }

        public ItemModel GetItemTemplate(IBlueprintMetadataEntry metadata)
        {
            var blueprintAccessor = BlueprintsArchive.Load(metadata);
            if (blueprintAccessor is object) {
                var templateRaw = new JObject();
                var refs = new References(this);
                ItemModel.Prepare(refs, templateRaw, metadata);
                var itemAccessor = new ModelDataAccessor(templateRaw, refs, this);
                var template = ItemModel.Create(itemAccessor);
                template.Blueprint = metadata.Id;

                SetupEnchantments(refs, blueprintAccessor, template.Facts);

                if (template is ShieldItemModel shield) {
                    if (blueprintAccessor.Data.WeaponComponent != null && blueprintAccessor.Data.WeaponComponent.Length > 4) {
                        var weapBlueprint = blueprintAccessor.Data.WeaponComponent.Remove(0, 4); // Starts with "!bp_"
                        var weapMetadata = Blueprints.Get(weapBlueprint);
                        if (weapMetadata is object) {
                            var weapBlueprintAccessor = BlueprintsArchive.Load(weapMetadata);
                            if (weapBlueprintAccessor is object) {
                                shield.EnsureWeaponComponent(refs);
                                SetupEnchantments(refs, weapBlueprintAccessor, shield.WeaponComponent.Facts);
                            }
                        }
                    }
                    if (blueprintAccessor.Data.ArmorComponent != null && blueprintAccessor.Data.ArmorComponent.Length > 4) {
                        var armorBlueprint = blueprintAccessor.Data.ArmorComponent.Remove(0, 4); // Starts with "!bp_"
                        var armorMetadata = Blueprints.Get(armorBlueprint);
                        if (armorMetadata is object) {
                            var armorBlueprintAccessor = BlueprintsArchive.Load(armorMetadata);
                            if (armorBlueprintAccessor is object) {
                                shield.EnsureArmorComponent(refs);
                                SetupEnchantments(refs, armorBlueprintAccessor, shield.ArmorComponent.Facts);
                            }
                        }
                    }
                }

                return template;
            }
            return null;
        }

        public FactItemModel GetFeatTemplate(string blueprint)
        {
            var metadata = Blueprints.Get(blueprint);
            return GetFeatTemplate(metadata);
        }
        public FactItemModel GetFeatTemplate(IBlueprintMetadataEntry metadata)
        {
            var blueprintAccessor = BlueprintsArchive.Load(metadata);
            if (blueprintAccessor is object) {
                var factTemplateRaw = new JObject();
                var refs = new References(this);
                FeatureFactItemModel.Prepare(refs, factTemplateRaw);
                var factTemplateAccessor = new ModelDataAccessor(factTemplateRaw, refs, this);
                var factTemplate = (FeatureFactItemModel)FactItemModel.Factory(factTemplateAccessor);
                SetupFactItemModel(refs, metadata, blueprintAccessor, factTemplate);
                return factTemplate;
            }

            return FeatTemplates?.FirstOrDefault(t => t.Blueprint == metadata.Id);
        }

        private IReadOnlyList<EnchantmentSpec> _weaponEnchantments;
        private IReadOnlyList<EnchantmentSpec> _armorEnchantments;
        private IReadOnlyList<EnchantmentSpec> _shieldEnchantments;

        private IReadOnlyList<EnchantmentSpec> LoadEnchantments(BlueprintType type, IEnumerable<EnchantmentSpec> predefined)
        {
            string DisplayName(IBlueprintMetadataEntry m)
            {
                var name = m.DisplayName;
                if (name.IEnd("Enchantment")) name = name.Remove(name.Length - 12);
                return name;
            }

            var blueprintEnchantments =
                from b in Blueprints.GetEntries(type)
                where !predefined.Any(e => e.Blueprint.Eq(b.Id))
                let bspec = BlueprintsArchive.Load(b)
                select new EnchantmentSpec(DisplayName(b), b.Id, bspec.Data.Components.Select(c => c.Name).ToArray());

            return blueprintEnchantments
                    .Concat(predefined)
                    .OrderBy(e => e.Name)
                    .ToArray();
        }

        private IReadOnlyList<EnchantmentSpec> WeaponEnchantments
        {
            get {
                if (_weaponEnchantments is object) return _weaponEnchantments;
                return _weaponEnchantments = LoadEnchantments(BlueprintTypes.WeaponEnchantment, Enchantments.Weapon.All);
            }
        }

        private IReadOnlyList<EnchantmentSpec> ArmorEnchantments
        {
            get {
                if (_armorEnchantments is object) return _armorEnchantments;
                return _armorEnchantments = LoadEnchantments(BlueprintTypes.ArmorEnchantment, Enchantments.Armor.All);
            }
        }

        public IReadOnlyList<EnchantmentSpec> GetEnchantments(ItemModel item)
        {
            if (item is WeaponItemModel) {
                return WeaponEnchantments;
            }
            else if (item is ArmorItemModel) {
                return ArmorEnchantments;
            }
            else if (item is ShieldItemModel) {
                return _shieldEnchantments ?? (_shieldEnchantments = Enchantments.Shield.All);
            }
            return Array.Empty<EnchantmentSpec>();
        }

        public void Dispose()
        {
            if (BlueprintsArchive is object) {
                BlueprintsArchive.Dispose();
            }
        }
    }
}
