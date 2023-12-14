using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class BeforeAddCollectionItemArgs
    {
        public BeforeAddCollectionItemArgs(IReferences references, JObject obj, string blueprint, object data)
        {
            References = references;
            Obj = obj;
            Blueprint = blueprint;
            Data = data;
        }

        public IReferences References { get; }
        public JObject Obj { get; }
        public string Blueprint { get; }
        public object Data { get; }
    }
}