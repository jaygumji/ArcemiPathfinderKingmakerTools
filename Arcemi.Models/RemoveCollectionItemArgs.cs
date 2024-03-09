namespace Arcemi.Models
{
    public class RemoveCollectionItemArgs<TGameModel>
    {
        public RemoveCollectionItemArgs(TGameModel gameModel)
        {
            GameModel = gameModel;
        }

        public TGameModel GameModel { get; }
    }
}