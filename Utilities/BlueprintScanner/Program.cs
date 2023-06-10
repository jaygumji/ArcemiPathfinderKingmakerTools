using Arcemi.Pathfinder.Kingmaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

if (args.Length < 1) {
    Console.WriteLine("Must supply atleast 1 argument");
    Console.WriteLine("1: Game directory");
    Console.WriteLine("E. g. BlueprintScanner.exe \"C:\\Games\\Pathfinder Wrath of the Righteous\"");
    return;
}
var gameDirectory = args[0];

Console.WriteLine($"Game directory: {gameDirectory}");

Console.WriteLine("Scanning files...");
var resources = new GameResources();
resources.LoadGameFolder(gameDirectory);

var factTemplates = new List<JObject>();
var references = new References(resources);
var features = resources.Blueprints.GetEntries(BlueprintTypes.Feature);

foreach (var feature in features) {
    Console.WriteLine(feature.DisplayName);

    var blueprintAccessor = resources.BlueprintsArchive.Load(feature);
    if (blueprintAccessor is null) {
        Console.WriteLine("Doesn't exist");
        continue;
    }

    var factTemplateRaw = new JObject();
    FeatureFactItemModel.Prepare(references, factTemplateRaw);
    var factTemplateAccessor = new ModelDataAccessor(factTemplateRaw, new References(resources), resources);
    var factTemplate = FactItemModel.Factory(factTemplateAccessor);
    factTemplate.Blueprint = feature.Id;
    factTemplate.Context = new FactContextModel(new ModelDataAccessor(new JObject(), references, resources));
    factTemplate.Context.AssociatedBlueprint = feature.Id;

    foreach (var component in blueprintAccessor.Data.Components) {
        if (factTemplate.Components.ContainsKey(component.Name)) {
            Console.WriteLine("Warning - Duplicate component: " + component.Name);
            continue;
        }

        // Not all components need to be added,
        // and some components need extra data
        // Luckily, the game corrects both these problems for us
        factTemplate.Components.AddNull(component.Name);
    }

    factTemplates.Add(factTemplateRaw);
}

File.WriteAllText("FeatTemplates.json", JsonConvert.SerializeObject(factTemplates));
