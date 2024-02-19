using System;
using System.Collections;
using System.Collections.Generic;

namespace Arcemi.Models
{
    public interface IGameModelCollection
    {
        object AddByBlueprint(string blueprint, object data = null);
        bool Remove(object model);
        IReadOnlyList<IBlueprintMetadataEntry> AvailableEntries { get; }
    }
    public interface IGameModelCollection<T> : IGameModelCollection, IReadOnlyList<T>
    {
        T AddByCode(string code);
        new T AddByBlueprint(string blueprint, object data = null);
        T InsertByBlueprint(int index, string blueprint, object data = null);
        T Duplicate(T model);
        bool Remove(T model);
        void Clear();

        int IndexOf(T model);

        bool IsAddEnabled { get; }
        bool IsRemoveEnabled { get; }
    }
    public class GameModelCollectionProjection<TIn, TOut> : IGameModelCollection<TOut>
    {
        private readonly IGameModelCollection<TIn> inner;
        private readonly Func<TIn, TOut> project;

        public GameModelCollectionProjection(IGameModelCollection<TIn> inner, Func<TIn, TOut> project)
        {
            this.inner = inner;
            this.project = project;
        }

        public TOut this[int index] => project(inner[index]);
        public bool IsAddEnabled => false;
        public bool IsRemoveEnabled => false;
        public IReadOnlyList<IBlueprintMetadataEntry> AvailableEntries => inner.AvailableEntries;
        public int Count => inner.Count;

        public TOut AddByBlueprint(string blueprint, object data = null)
        {
            var tin = inner.AddByBlueprint(blueprint, data);
            return project(tin);
        }

        public TOut AddByCode(string code)
        {
            var tin = inner.AddByCode(code);
            return project(tin);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public TOut Duplicate(TOut model)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            foreach (var entry in inner) {
                yield return project(entry);
            }
        }

        public int IndexOf(TOut model)
        {
            return -1;
        }

        public TOut InsertByBlueprint(int index, string blueprint, object data = null)
        {
            var tin = inner.InsertByBlueprint(index, blueprint, data);
            return project(tin);
        }

        public bool Remove(TOut model)
        {
            return false;
        }

        bool IGameModelCollection.Remove(object model)
        {
            return false;
        }

        object IGameModelCollection.AddByBlueprint(string blueprint, object data)
        {
            return AddByBlueprint(blueprint, data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public abstract class GameModelCollectionWriter<TGameModel, TModel>
    {
        public static GameModelCollectionWriter<TGameModel, TModel> ReadOnly { get; } = new ReadOnlyImpl();
        private class ReadOnlyImpl : GameModelCollectionWriter<TGameModel, TModel>
        {
            public override void BeforeAdd(BeforeAddCollectionItemArgs args) { }
            public override bool IsAddEnabled => false;
            public override bool IsRemoveEnabled => false;
        }

        public virtual bool IsAddEnabled => true;
        public virtual bool IsRemoveEnabled => true;
        public abstract void BeforeAdd(BeforeAddCollectionItemArgs args);
        public virtual void AfterAdd(AfterAddCollectionItemArgs<TGameModel, TModel> args) { }
        public virtual void AfterRemove(AfterRemoveCollectionItemArgs<TGameModel, TModel> args) { }

        public virtual IReadOnlyList<IBlueprintMetadataEntry> GetAvailableEntries(IEnumerable<TGameModel> current)
        {
            return Array.Empty<IBlueprintMetadataEntry>();
        }
    }
    public static class GameModelCollection
    {
        public static IGameModelCollection<TOut> Project<TIn, TOut>(this IGameModelCollection<TIn> collection, Func<TIn, TOut> project)
        {
            return new GameModelCollectionProjection<TIn, TOut>(collection, project);
        }

    }
    public static class GameModelCollection<TGameModel>
    {
        public static IGameModelCollection<TGameModel> Empty { get; } = new GameModelCollection<TGameModel, Model>(null, x => default);
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

        public IReadOnlyList<IBlueprintMetadataEntry> AvailableEntries => writer?.GetAvailableEntries(_inner) ?? Array.Empty<IBlueprintMetadataEntry>();

        public IEnumerator<TGameModel> GetEnumerator() => _inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IGameModelCollection.Remove(object model) => Remove((TGameModel)model);
        public bool Remove(TGameModel model)
        {
            if (_inner.Remove(model) && _reverse.TryGetValue(model, out var accessorModel)) {
                _accessor.Remove(accessorModel);
                _reverse.Remove(model);
                writer?.AfterRemove(new AfterRemoveCollectionItemArgs<TGameModel, TModel>(model, accessorModel));
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

        public int IndexOf(TGameModel model)
        {
            return _inner.IndexOf(model);
        }

        public bool IsAddEnabled => _accessor is object && writer is object && writer.IsAddEnabled;
        public bool IsRemoveEnabled => _accessor is object && (writer is null || writer.IsRemoveEnabled);

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

        object IGameModelCollection.AddByBlueprint(string blueprint, object data) => AddByBlueprint(blueprint, data);
        public TGameModel AddByBlueprint(string blueprint, object data = null)
        {
            var model = _accessor.Add((r, o) => writer?.BeforeAdd(new BeforeAddCollectionItemArgs(r, o, blueprint, data)));
            var gameModel = _factory(model);
            writer?.AfterAdd(new AfterAddCollectionItemArgs<TGameModel, TModel>(gameModel, model, blueprint, data));
            
            _inner.Add(gameModel);
            _reverse.Add(gameModel, model);
            return gameModel;
        }

        public TGameModel InsertByBlueprint(int index, string blueprint, object data = null)
        {
            var model = _accessor.Insert(index, (r, o) => writer?.BeforeAdd(new BeforeAddCollectionItemArgs(r, o, blueprint, data)));
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
