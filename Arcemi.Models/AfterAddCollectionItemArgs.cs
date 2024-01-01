namespace Arcemi.Models
{
    public class AfterAddCollectionItemArgs<TGameModel, TModel>
    {
        public AfterAddCollectionItemArgs(TGameModel gameModel, TModel model, string blueprint, object data)
        {
            GameModel = gameModel;
            Model = model;
            Blueprint = blueprint;
            Data = data;
        }

        public TGameModel GameModel { get; }
        public TModel Model { get; }
        public string Blueprint { get; }
        public object Data { get; }
    }
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