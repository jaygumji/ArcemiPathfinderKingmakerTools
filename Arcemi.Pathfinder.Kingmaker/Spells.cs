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
            if (x.Name.DisplayName.IStart("Test ")) {
                return true;
            }
            if (x.Name.DisplayName.IEnd(" Test")) {
                return true;
            }
            if (x.Name.DisplayName.IWord("Ability")) {
                return true;
            }
            if (x.Name.DisplayName.IStart("Quest ")) {
                return true;
            }
            if (x.Name.DisplayName.IEnd("Puzzle Trap")) {
                return true;
            }
            if (x.Name.Original.IEnd("RegQ2")) {
                return true;
            }
            if (x.Name.DisplayName.IStart("Army ")) {
                return true;
            }
            if (x.Name.Original.IStart("CastleOfKnives")) {
                return true;
            }
            return false;
        }
    }
}
