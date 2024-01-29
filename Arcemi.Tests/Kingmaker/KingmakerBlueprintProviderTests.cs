using Arcemi.Models.Kingmaker;
using System.Threading.Tasks;
using Xunit;

namespace Arcemi.Tests.Kingmaker
{
    public class KingmakerBlueprintProviderTests
    {
        [Fact]
        public async Task TestAsync()
        {
            var target = new KingmakerBlueprintProvider();
            await target.SetupAsync(new Models.BlueprintProviderSetupArgs(Models.GameDefinition.Pathfinder_Kingmaker,
                @"C:\Temp\Kingmaker\WorkingDirectory", null));
            var entry = target.Get("127be665d87f8634a85af05d394ee14f");
            entry.ToString();
        }
    }
}
