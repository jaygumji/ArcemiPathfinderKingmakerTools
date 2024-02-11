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

            if (KingmakerPredefinedFeats.Instance.TryGet(args.Blueprint, out var spec)) {
                spec.ApplyOn(args.Obj);
            }
        }

        public override void AfterAdd(AfterAddCollectionItemArgs<IGameUnitFeatEntry, FactItemModel> args)
        {
            if (KingmakerPredefinedFeats.Instance.Is(args.Blueprint)) return;

            //var template = Res.GetFeatTemplate(args.Blueprint);
            //if (template is object) {
            //    args.Model.Import(template);
            //}
        }

        public override IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<IGameUnitFeatEntry> current)
        {
            var hashset = new HashSet<string>(current.Select(x => {
                var wfeat = (KingmakerGameUnitFeatEntry)x;
                return FeatSpec.CreateIdentifier(wfeat.Blueprint, wfeat.Model.Param?.SpellSchool, wfeat.Model.Param?.WeaponCategory);
            }), StringComparer.Ordinal);

            var entries = KingmakerPredefinedFeats.Instance.Combine(Res.Blueprints.GetEntries(BlueprintTypeId.Feature));
            return entries.Where(x => !hashset.Contains(x.Id)).ToArray();
        }
    }

    public class KingmakerPredefinedFeats : FeatSpecCollection
    {
        public static KingmakerPredefinedFeats Instance { get; } = new KingmakerPredefinedFeats();
        private KingmakerPredefinedFeats() { }

        protected override IEnumerable<FeatSpec> Melee(IEnumerable<FeatSpec> feats, string name)
        {
            return feats.Concat(new[] {
                Focus(name), FocusGreater(name),
                Specialization(name), SpecializationGreater(name), MasteryParametrized(name),
                ImprovedCritical(name),
                SlashingGrace(name), FencingGrace(name)
            });
        }

        protected override IEnumerable<FeatSpec> Ranged(IEnumerable<FeatSpec> feats, string name)
        {
            return feats.Concat(new[] {
                Focus(name), FocusGreater(name),
                Specialization(name), SpecializationGreater(name), MasteryParametrized(name),
                ImprovedCritical(name),
                PointBlankMaster(name)
            });
        }

        protected override IEnumerable<FeatSpec> SpellSchool(IEnumerable<FeatSpec> feats, string name)
        {
            return feats.Concat(new[] {
                SpellFocus(name),
                SpellFocusGreater(name),
            });
        }

        protected override void OnDefine(FeatSpecDefineArgs args)
        {
            args
                .Melee("Bardiche")
                .Melee("BastardSword")
                .Melee("Battleaxe")
                .Melee("Bite")
                .Melee("Claw")
                .Melee("Club")
                .Melee("Dagger")
                .Melee("DoubleAxe")
                .Melee("DoubleSword")
                .Melee("DuelingSword")
                .Melee("Urgrosh")
                .Melee("DwarvenWaraxe")
                .Melee("EarthBreaker")
                .Melee("ElvenCurveBlade")
                .Melee("Estoc")
                .Melee("Falcata")
                .Melee("Falchion")
                .Melee("Fauchard")
                .Melee("Flail")
                .Melee("Glaive")
                .Melee("Gore")
                .Melee("Greataxe")
                .Melee("Greatclub")
                .Melee("Greatsword")
                .Melee("Handaxe")
                .Melee("HeavyFlail")
                .Melee("HeavyMace")
                .Melee("HeavyPick")
                .Melee("Hoof")
                .Melee("HookedHammer")
                .Melee("IncorporealTouch")
                .Melee("Kama")
                .Melee("Kukri")
                .Melee("LightHammer")
                .Melee("LightMace")
                .Melee("LightPick")
                .Melee("LightShield")
                .Melee("Longspear")
                .Melee("Longsword")
                .Melee("Nunchaku")
                .Melee("PunchingDagger")
                .Melee("Quarterstaff")
                .Melee("Rapier")
                .Melee("Sai")
                .Melee("Scimitar")
                .Melee("Scythe")
                .Melee("Shortspear")
                .Melee("Shortsword")
                .Melee("Sickle")
                .Melee("Slam")
                .Melee("Spear")
                .Melee("Spike")
                .Melee("Starknife")
                .Melee("Sting")
                .Melee("Tail")
                .Melee("Talon")
                .Melee("Tentacle")
                .Melee("Tongi")
                .Melee("Trident")
                .Melee("Warhammer")
                .Melee("Unarmed")
                .Melee("Wing")

                // Ranged
                .Ranged("Bomb")
                .Ranged("Dart")
                .Ranged("HeavyCrossbow")
                .Ranged("Javelin")
                .Ranged("LightCrossbow")
                .Ranged("Longbow")
                .Ranged("Ray")
                .Ranged("Shortbow")
                .Ranged("SlingStaff")
                .Ranged("ThrowingAxe")
                .Ranged("Touch")

                // Spell Schools
                .SpellSchool("Abjuration")
                .SpellSchool("Conjuration")
                .SpellSchool("Divination")
                .SpellSchool("Enchantment")
                .SpellSchool("Evocation")
                .SpellSchool("Illusion")
                .SpellSchool("Necromancy")
                .SpellSchool("Transmutation");
        }
    }
}