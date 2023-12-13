using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Models
{
    public class QuestFactItemModel : FactItemModel
    {
        public const string TypeRef = "Kingmaker.AreaLogic.QuestSystem.Quest, Assembly-CSharp";
        public const string TypeRefCode = "Kingmaker.AreaLogic.QuestSystem.Quest, Code";

        public QuestFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int NextObjectiveOrder { get => A.Value<int>("m_NextObjectiveOrder"); set => A.Value(value, "m_NextObjectiveOrder"); }
        public string State { get => A.Value<string>("m_State"); set => A.Value(value, "m_State"); }
        public bool IsInUiSelected { get => A.Value<bool>("m_IsInUiSelected"); set => A.Value(value, "m_IsInUiSelected"); }

        public bool IsCompleted => State == "Completed";
        public bool IsExpanded { get; set; }
        public void ToggleExpansion()
        {
            IsExpanded = !IsExpanded;
        }

        public ListAccessor<PersistentObjectiveModel> PersistentObjectives => A.List(factory: a => new PersistentObjectiveModel(a));

        public static new void Prepare(IReferences refs, JObject obj)
        {
            obj.Add("$type", TypeRef);
            FactItemModel.Prepare(refs, obj);
        }
    }
}