using Newtonsoft.Json.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameUnitAlignmentHistoryCollectionWriter : GameModelCollectionWriter<IGameUnitAlignmentHistoryEntryModel, RefModel>
    {
        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            args.Obj.Add("Position", new JObject {
                { "x", 0.0 },
                { "y", 0.0 }
            });
            args.Obj.Add("Direction", null);
            args.Obj.Add("Provider", args.Blueprint);
        }
    }
}