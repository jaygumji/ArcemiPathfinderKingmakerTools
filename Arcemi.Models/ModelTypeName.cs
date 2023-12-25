namespace Arcemi.Models
{
    public class ModelTypeName
    {
        public ModelTypeName(string plural, string singular)
        {
            Plural = plural;
            Singular = singular;
        }

        public string Plural { get; }
        public string Singular { get; }
    }
}