using Arcemi.Models.Warhammer40KRogueTrader;
using System.Threading.Tasks;
using Xunit;

namespace Arcemi.Tests.W40KRT
{
    public class W40KRTBlueprintProviderTests
    {
        [Fact]
        public async Task Test()
        {
            var target = new W40KRTBlueprintProvider();
            await target.SetupAsync(new Models.BlueprintProviderSetupArgs(@"C:\Temp\W40KRT\WorkingDirectory", @"C:\Games\Steam\steamapps\common\Warhammer 40,000 Rogue Trader"));
            var entry = target.Get("8382395e2da7471e925222eaaa4f2bf1");
            entry.ToString();
        }
    }
}
