namespace Arcemi.Models
{
    public class CompanionPartItemModel : PartItemModel
    {
        public const string TypeRef = "Kingmaker.UnitLogic.Parts.UnitPartCompanion, Assembly-CSharp";
        public CompanionPartItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Spawner { get => A.Value<string>("m_Spawner"); set => A.Value(value, "m_Spawner"); }
        public bool HealOnExit { get => A.Value<bool>("m_HealOnExit"); set => A.Value(value, "m_HealOnExit"); }
        public string State { get => A.Value<string>(); set => A.Value(value); }
        public string LastCampingRole { get => A.Value<string>(); set => A.Value(value); }
    }
}