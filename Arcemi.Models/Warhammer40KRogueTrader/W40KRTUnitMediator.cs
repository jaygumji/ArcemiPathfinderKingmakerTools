using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public class W40KRTUnitMediator
    {
        private readonly W40KRTGameAccessor _accessor;
        private readonly GameOperationEvents _events;

        public W40KRTUnitMediator(W40KRTGameAccessor accessor, GameOperationEvents events)
        {
            _accessor = accessor;
            _events = events;
        }

        public IGameUnitModel ParentOf(IGameUnitModel child)
        {
            var parentId = child.Ref.GetAccessor().Value<string>("MasterRef");
            if (string.IsNullOrEmpty(parentId)) return null;

            return _accessor.Characters.FirstOrDefault(c => c.UniqueId.Eq(parentId));
        }

        public IGameUnitModel ChildOf(IGameUnitModel parent)
        {
            return _accessor.Characters.FirstOrDefault(c => {
                var parentId = c.Ref.GetAccessor().Value<string>("MasterRef");
                return parentId.HasValue() && parent.UniqueId.Eq(parentId);
            });
        }

        public void RemoveChildOf(IGameUnitModel parent)
        {
            var child = ChildOf(parent);
            if (child is null) {
                Logger.Current.Information($"Failed to find child of {parent.Name}");
                return;
            }

            if (_accessor.Characters.Remove(child)) {
                void RemoveChildPart(string type)
                {
                    var typeFullName = $"Kingmaker.UnitLogic.Parts.{type}, Code";
                    var childPart = parent.Ref.Parts.Container.FirstOrDefault(c => c.Type.Eq(typeFullName));
                    if (childPart is null || parent.Ref.Parts.Container.Remove(childPart)) {
                        Logger.Current.Warning($"When removing child unit, could not remove the {type} part");
                    }
                }
                RemoveChildPart("UnitPartPetOwner");
                RemoveChildPart("UnitPartNotMoveTrigger");
                RemoveChildPart("UnitPartFollowedByUnits");

                void RemoveChildUnitFact(string name, params string[] blueprints)
                {
                    var childFact = parent.Ref.Facts.Items.FirstOrDefault(f => f.Type.Eq("Kingmaker.UnitLogic.UnitFact, Code") && blueprints.Contains(f.Blueprint));
                    if (childFact is null || !parent.Ref.Facts.Items.Remove(childFact)) {
                        Logger.Current.Warning($"When removing child unit, could not remove the unit fact {name}");
                    }
                }
                RemoveChildUnitFact("PetOwner",
                    "91469656b6fb406f9d9abd036b502b05", // Master_ArbitesCyberMastifGlaito_PetOwner
                    "3f59ef1fcd024364b0e2bbdd3b29d4fe", // Master_ArbitesCyberMastiff_PetOwner
                    "8112234fb07a40db998b3621f3bb5c9f", // Master_CyberEagle_PetOwner
                    "ef6aa17f2c044943831909bb619a59dd", // Master_ExperimentalServitor_PetOwner
                    "cd7617e9f2034aeaad3426a9a6b12a81", // Master_PsykerPsyberRaven_PetOwner
                    "caf58f47004f4268a2eacc55e089f3e9" // Master_ServoskullSwarm_PetOwner
                );

                var unitDesc = parent.Ref.Parts.Container.FirstOrDefault(c => c.Type.Eq("Kingmaker.UnitLogic.Parts.PartUnitDescription, Code"));
                if (unitDesc is object) {
                    unitDesc.GetAccessor().RemoveProperty("CustomPetName");
                }
                Logger.Current.Information($"Removed child {child.Name} for parent {parent.Name}");
                _events.Raise("CharacterRemoved", child);
            }
            else {
                Logger.Current.Information($"Failed to remove child {child.Name} for parent {parent.Name} due to collection failure");
            }
        }
    }
}