namespace Arcemi.Models
{
    public class AfterRemoveCollectionItemArgs<TGameModel, TModel>
    {
        public AfterRemoveCollectionItemArgs(TGameModel gameModel, TModel model)
        {
            GameModel = gameModel;
            Model = model;
        }

        public TGameModel GameModel { get; }
        public TModel Model { get; }
    }
}