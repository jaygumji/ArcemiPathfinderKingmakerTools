using Arcemi.Pathfinder.Kingmaker;
using Arcemi.Pathfinder.SaveGameEditor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var extractedBlueprintsDirectory = args[0];
var gameDirectory = args[1];

var viewModel = new MainViewModel();
await viewModel.InitializeAsync();

var blueprintData = BlueprintMetadata.Load(gameDirectory);
var factTemplates = new List<JObject>();
var references = new References(viewModel.Resources);
foreach (var featureBlueprintReference in blueprintData.GetEntries(BlueprintTypes.Feature))
{
    Console.WriteLine(featureBlueprintReference.DisplayName);

    var blueprintPath = Path.Combine(extractedBlueprintsDirectory, featureBlueprintReference.Path);
    if (!File.Exists(blueprintPath))
    {
        Console.WriteLine("Doesn't exist");
        continue;
    }

    var blueprintJson = JObject.Parse(File.ReadAllText(blueprintPath));
    var blueprintAccessor = new Blueprint(new ModelDataAccessor(blueprintJson, references, viewModel.Resources));

    var factTemplateRaw = new JObject();
    FeatureFactItemModel.Prepare(references, factTemplateRaw);
    var factTemplateAccessor = new ModelDataAccessor(factTemplateRaw, new References(viewModel.Resources), viewModel.Resources);
    var factTemplate = FactItemModel.Factory(factTemplateAccessor);
    factTemplate.Blueprint = featureBlueprintReference.Id;
    factTemplate.Context = new FactContextModel(new ModelDataAccessor(new JObject(), references, viewModel.Resources));
    factTemplate.Context.AssociatedBlueprint = featureBlueprintReference.Id;
        
    foreach (var component in blueprintAccessor.Data.Components)
    {
        if (factTemplate.Components.ContainsKey(component.Name))
        {
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
