using Arcemi.Models.Warhammer40KRogueTrader;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Arcemi.Tests.W40KRT
{
    public class W40KRTBlueprintProviderTests
    {
        [Fact]
        public async Task Test()
        {
            const string workingDirectory = @"C:\Temp\W40KRT\WorkingDirectory";
            const string gameDirectory = @"D:\Programs\Steam\steamapps\common\Warhammer 40,000 Rogue Trader";
            if (!Directory.Exists(gameDirectory)) return;

            if (Directory.Exists(workingDirectory)) {
                Directory.Delete(workingDirectory, true);
            }
            var target = new W40KRTBlueprintProvider();
            await target.SetupAsync(new Models.BlueprintProviderSetupArgs(Models.GameDefinition.Warhammer40K_RogueTrader,
                workingDirectory, gameDirectory));

            if (!target.TryGet("8382395e2da7471e925222eaaa4f2bf1", out var entry)) return;
            entry.ToString();
        }
    }
}
