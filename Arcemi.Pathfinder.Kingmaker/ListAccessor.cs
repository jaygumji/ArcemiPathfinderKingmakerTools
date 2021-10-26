#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ListAccessor<T> : IList, IReadOnlyList<T>, INotifyCollectionChanged, IModelContainer
        where T : Model
    {
        private readonly JArray _array;
        private readonly IReferences _refs;
        private readonly Func<ModelDataAccessor, T> _factory;
        private readonly IGameResourcesProvider _res;
        private readonly List<T> _items;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ListAccessor(JArray array, IReferences refs, IGameResourcesProvider res, Func<ModelDataAccessor, T> factory)
        {
            _array = array;
            _refs = refs;
            _factory = factory;
            _res = res;
            _items = new List<T>();
            ((IModelContainer)this).Refresh();
        }

        void IModelContainer.Refresh()
        {
            if (_items.Count > 0) _items.Clear();
            foreach (var t in _array) {
                if (t == null || t.Type == JTokenType.Null) {
                    _items.Add(null);
                    continue;
                }
                var obj = _refs.GetReferred((JObject)t);
                var accessor = new ModelDataAccessor(obj, _refs, _res);
                var instance = _factory.Invoke(accessor);
                _items.Add(instance);
            }
        }

        private T InitAndInsert(int index = -1, Action<IReferences, JObject> init = null)
        {
            JObject obj;
            if (typeof(T).IsSubclassOf(typeof(RefModel))) {
                obj = _refs.Create();
            }
            else {
                obj = new JObject();
            }
            init?.Invoke(_refs, obj);
            var accessor = new ModelDataAccessor(obj, _refs, _res);
            var item = _factory.Invoke(accessor);
            if (index < 0) {
                _array.Add(obj);
                _items.Add(item);
            }
            else {
                _array.Insert(index, obj);
                _items.Insert(index, item);
            }
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            return item;
        }

        public void Touch()
        {
        }

        public T Insert(int index, Action<IReferences, JObject> init = null)
        {
            return InitAndInsert(index, init);
        }

        public T Add(Action<IReferences, JObject> init = null)
        {
            return InitAndInsert(-1, init);
        }

        public void AddRef<TRef>(TRef item)
            where TRef : RefModel, T
        {
            _items.Add(item);
            _array.Add(_refs.CreateReference(item.Id));
        }

        public void SetRef<TRef>(int index, TRef item)
            where TRef : RefModel, T
        {
            _items[index] = item;
            _array[index] = _refs.CreateReference(item.Id);
        }

        public int FirstEmptyIndex()
        {
            for (var i = 0; i < _items.Count; i++) {
                if (_items[i] == null) return i;
                if (_items[i] == null) return i;
            }
            return -1;
        }

        public T this[int index] => _items[index];

        object IList.this[int index] { get => _items[index]; set => throw new NotImplementedException(); }

        public int Count => _items.Count;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((IList)_items).SyncRoot;

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            foreach (var item in _array) {
                _refs.BubbleRemoval(item);
            }

            _array.Clear();
            _items.Clear();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        bool IList.Contains(object value)
        {
            return ((IList)_items).Contains(value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_items).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_items).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        public bool Remove(T item)
        {
            var idx = _items.IndexOf(item);
            if (idx < 0) return false;
            RemoveAt(idx);
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = _items[index];
            var token = _array[index];

            _refs.BubbleRemoval(token);

            _array.RemoveAt(index);
            _items.RemoveAt(index);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}