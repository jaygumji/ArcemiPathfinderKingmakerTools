namespace Arcemi.Pathfinder.Kingmaker
{
    public class Spells
    {
        public static bool IsSpecial(IBlueprint x)
        {
            if (x.Name.Original.ILike("Cutscene")) {
                return true;
            }
            if (x.Name.Original.ILike("Cutcene")) {
                return true;
            }
            if (x.Name.Original.ILike("Backgrounds")) {
                return true;
            }
            if (x.Name.Original.ILike("Test")) {
                return true;
            }
            if (x.Name.Original.ILike("Ability")) {
                return true;
            }
            if (x.Name.Original.ILike("Quest")) {
                return true;
            }
            if (x.Name.Original.ILike("Staff")) {
                return true;
            }
            if (x.Name.Original.ILike("Puzzle")) {
                return true;
            }
            if (x.Name.Original.ILike("RegQ")) {
                return true;
            }
            if (x.Name.Original.IStart("Army")) {
                return true;
            }
            if (x.Name.Original.IStart("CastleOfKnives")) {
                return true;
            }
            return false;
        }
    }
}
