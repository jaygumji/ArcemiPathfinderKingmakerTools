using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Kingmaker
{
    internal class KingmakerGameModelCollectionFeatWriter : GameModelCollectionWriter<IGameUnitFeatEntry, FactItemModel>
    {
        private readonly IGameResourcesProvider Res = GameDefinition.Pathfinder_Kingmaker.Resources;
        private readonly UnitEntityModel unit;

        public KingmakerGameModelCollectionFeatWriter(UnitEntityModel unit)
        {
            this.unit = unit;
        }

        public override void BeforeAdd(BeforeAddCollectionItemArgs args)
        {
            args.Obj.Add("$type", FeatureFactItemModel.TypeRef);
            var context = new JObject {
                { "m_Ranks", new JArray { 0, 0, 0, 0, 0, 0, 0 } },
                { "m_SharedValues", new JArray { 0, 0, 0, 0, 0, 0, 0 } },
                //{ "m_Params", null },
                { "AssociatedBlueprint", args.Blueprint },
                //{ "ParentContext", null },
                //{ "m_MainTarget", null },
                { "Params", new JObject() },
                //{ "SpellDescriptor", "None" },
                //{ "SpellSchool", "None" },
                //{ "SpellLevel", 0 },
                { "Direction", new JObject {
                    { "$type", "UnityEngine.Vector3, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" },
                    { "x", 0.0 },
                    { "y", 0.0 },
                    { "z", 0.0 }
                } }
            };
            context.Add("m_OwnerDescriptor", args.References.CreateReference(context, unit.Descriptor.Id));
            context.Add("m_CasterReference", new JObject { { "m_UniqueId", unit.UniqueId } });
            args.Obj.Add("m_Context", context);
            args.Obj.Add("Blueprint", args.Blueprint);
            args.Obj.Add("m_ComponentsData", new JArray());
            args.Obj.Add("Rank", 1);
            //args.Obj.Add("Source", null);
            args.Obj.Add("Param", new JObject());
            args.Obj.Add("IgnorePrerequisites", true);
            args.Obj.Add("Owner", args.References.CreateReference(args.Obj, unit.Descriptor.Id));
            args.Obj.Add("Initialized", true);
            args.Obj.Add("Active", true);
            //args.Obj.Add("SourceItem", null);
            //args.Obj.Add("SourceCutscene", null);
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitFeatEntry> current)
        {
            var hashset = new HashSet<string>(current.Select(x => x.Blueprint), StringComparer.Ordinal);
            return Res.Blueprints.GetEntries(BlueprintTypeId.Feature).Where(x => !hashset.Contains(x.Id)).ToArray();
        }
    }
}