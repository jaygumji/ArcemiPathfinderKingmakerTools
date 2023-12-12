namespace Arcemi.Models
{
    public class SingleReferenceModel : Model
    {
        public SingleReferenceModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Ref => A.Value<string>("m_Ref");
    }
}