using System.Threading.Tasks;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class SettingsViewModel 
    {
        private readonly MainViewModel main;

        public SettingsViewModel(MainViewModel main)
        {
            this.main = main;
        }

        public bool HasUnsavedConfigChanges => main.HasUnsavedConfigChanges;
        public AppUserConfiguration Config => main.EditConfig;

        public async Task InitializeAsync()
        {
            await main.InitializeAsync();
        }

        public bool ValidateAppDataFolder()
        {
            return Config?.ValidateAppDataFolder() ?? false;
        }

        public bool ValidateGameFolder()
        {
            return Config?.ValidateGameFolder() ?? false;
        }

        public async Task SaveConfigAsync()
        {
            await main.SaveConfigAsync();
        }
    }
}
