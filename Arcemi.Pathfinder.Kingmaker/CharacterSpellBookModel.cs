namespace Arcemi.Pathfinder.Kingmaker
{
    public class CharacterSpellBookModel : RefModel
    {
        public CharacterSpellBookModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public int BaseLevelInternal { get => A.Value<int>("m_BaseLevelInternal"); set => A.Value(value, "m_BaseLevelInternal"); }
        public int MythicLevelInternal { get => A.Value<int>("m_MythicLevelInternal"); set => A.Value(value, "m_MythicLevelInternal"); }
        public string Type { get => A.Value<string>("m_Type"); set => A.Value(value, "m_Type"); }
        public ListValueAccessor<int> SpontaneousSlots => A.ListValue<int>("m_SpontaneousSlots");
        public string Blueprint { get => A.Value<string>(); set => A.Value(value); }
        public string OppositionDescriptors { get => A.Value<string>(); set => A.Value(value); }
        public ListValueAccessor<int> BonusSpellSlots => A.ListValue<int>();
    }
}