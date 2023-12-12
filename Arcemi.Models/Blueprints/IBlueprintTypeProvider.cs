namespace Arcemi.Models
{
    public interface IBlueprintTypeProvider
    {
        BlueprintType Get(BlueprintTypeId id);
        BlueprintType Get(string fullName);
    }
}
