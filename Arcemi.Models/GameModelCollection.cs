using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public interface IGameModelCollection<T> : IReadOnlyList<T>
    {
        T AddCode(string code);
        bool Remove(T model);
        void Clear();
    }
    public class GameModelCollection<TGameModel, TModel> : IGameModelCollection<TGameModel>
        where TModel : Model
    {
        private readonly List<TGameModel> _inner;
        private readonly ListAccessor<TModel> _accessor;
        private readonly Func<TModel, TGameModel> _factory;
        private readonly Action<IReferences, JObject> _prepare;
        private readonly Dictionary<TGameModel, TModel> _reverse;

        public GameModelCollection(ListAccessor<TModel> accessor, Func<TModel, TGameModel> factory, Func<TModel, bool> predicate = null, Action<IReferences, JObject> prepare = null)
        {
            _inner = new List<TGameModel>();
            _reverse = new Dictionary<TGameModel, TModel>();
            if (accessor is object) {
                foreach (var model in accessor) {
                    if (predicate != null && !predicate(model)) continue;
                    var gameModel = factory(model);
                    _inner.Add(gameModel);
                    _reverse.Add(gameModel, model);
                }
            }
            _accessor = accessor;
            _factory = factory;
            _prepare = prepare;
        }

        public TGameModel this[int index] => _inner[index];
        public int Count => _inner.Count;
        public IEnumerator<TGameModel> GetEnumerator() => _inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Remove(TGameModel model)
        {
            if (_inner.Remove(model) && _reverse.TryGetValue(model, out var accessorModel)) {
                _accessor.Remove(accessorModel);
                _reverse.Remove(model);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _inner.Clear();
            _reverse.Clear();
            _accessor.Clear();
        }

        public TGameModel AddCode(string code)
        {
            var model = _accessor.Add(_prepare);
            model.Import(code);
            var gameModel = _factory(model);
            _inner.Add(gameModel);
            _reverse.Add(gameModel, model);
            return gameModel;
        }
    }
}