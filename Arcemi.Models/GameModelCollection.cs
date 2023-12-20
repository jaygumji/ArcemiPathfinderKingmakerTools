using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public interface IGameModelCollection<T> : IReadOnlyList<T>
    {
        T AddByCode(string code);
        T AddByBlueprint(string blueprint, object data = null);
        T Duplicate(T model);
        bool Remove(T model);
        void Clear();

        bool IsAddEnabled { get; }
        bool IsRemoveEnabled { get; }
        IReadOnlyList<IBlueprintMetadataEntry> AvailableEntries { get; }
    }
    public abstract class GameModelCollectionWriter<TGameModel, TModel>
    {
        public abstract void BeforeAdd(BeforeAddCollectionItemArgs args);
        public virtual void AfterAdd(AfterAddCollectionItemArgs<TGameModel, TModel> args)
        {
        }

        public virtual IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<TGameModel> current)
        {
            return Array.Empty<IBlueprintMetadataEntry>();
        }
    }
    public class GameModelCollection<TGameModel, TModel> : IGameModelCollection<TGameModel>
        where TModel : Model
    {
        private readonly List<TGameModel> _inner;
        private readonly ListAccessor<TModel> _accessor;
        private readonly Func<TModel, TGameModel> _factory;
        private readonly GameModelCollectionWriter<TGameModel, TModel> writer;
        private readonly Dictionary<TGameModel, TModel> _reverse;

        public GameModelCollection(ListAccessor<TModel> accessor, Func<TModel, TGameModel> factory, Func<TModel, bool> predicate = null, GameModelCollectionWriter<TGameModel, TModel> writer = null)
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
            this.writer = writer;
        }

        public string Name { get; }
        public TGameModel this[int index] => _inner[index];
        public int Count => _inner.Count;

        public IReadOnlyList<IBlueprintMetadataEntry> AvailableEntries => writer.GetAvailableEntries(_inner);

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

        public bool IsAddEnabled => _accessor is object && writer is object;
        public bool IsRemoveEnabled => _accessor is object;

        public TGameModel AddByCode(string code)
        {
            var model = _accessor.Add((r, o) => writer?.BeforeAdd(new BeforeAddCollectionItemArgs(r, o, null, null)));
            model.Import(code);
            var gameModel = _factory(model);
            writer?.AfterAdd(new AfterAddCollectionItemArgs<TGameModel, TModel>(gameModel, model, null, null));

            _inner.Add(gameModel);
            _reverse.Add(gameModel, model);
            return gameModel;
        }

        public TGameModel AddByBlueprint(string blueprint, object data = null)
        {
            var model = _accessor.Add((r, o) => writer?.BeforeAdd(new BeforeAddCollectionItemArgs(r, o, blueprint, data)));
            var gameModel = _factory(model);
            writer?.AfterAdd(new AfterAddCollectionItemArgs<TGameModel, TModel>(gameModel, model, blueprint, data));
            
            _inner.Add(gameModel);
            _reverse.Add(gameModel, model);
            return gameModel;
        }

        public TGameModel Duplicate(TGameModel model)
        {
            var data = _reverse[model];
            var blueprint = data.GetAccessor().Value<string>("Blueprint");
            return blueprint.HasValue() ? AddByBlueprint(blueprint) : AddByCode(data.Export());
        }
    }
}
