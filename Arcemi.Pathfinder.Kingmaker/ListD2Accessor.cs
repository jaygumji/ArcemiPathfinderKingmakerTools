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
    public class ListD2Accessor<T> : IList, IReadOnlyList<IReadOnlyList<T>>, INotifyCollectionChanged, IModelContainer
        where T : Model
    {
        private readonly JArray _array;
        private readonly IReferences _refs;
        private readonly Func<ModelDataAccessor, T> _factory;
        private readonly IGameResourcesProvider _res;
        private readonly List<List<T>> _items;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ListD2Accessor(JArray array, IReferences refs, IGameResourcesProvider res, Func<ModelDataAccessor, T> factory)
        {
            _array = array;
            _refs = refs;
            _factory = factory;
            _res = res;
            _items = new List<List<T>>();
            ((IModelContainer)this).Refresh();
        }

        void IModelContainer.Refresh()
        {
            if (_items.Count > 0) _items.Clear();
            foreach (var i1 in _array) {
                if (i1 == null || i1.Type == JTokenType.Null) {
                    _items.Add(null);
                    continue;
                }
                var jarr = (JArray)i1;
                var arr = new List<T>();
                _items.Add(arr);

                foreach (var i2 in jarr) {
                    var obj = _refs.GetReferred((JObject)i2);
                    var accessor = new ModelDataAccessor(obj, _refs, _res);
                    var instance = _factory.Invoke(accessor);
                    arr.Add(instance);
                }
            }
        }

        private T InitAndInsert(int index1 = -1, int index2 = -1, Action<IReferences, JObject> init = null)
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
            JArray jarr;
            List<T> arr;
            if (index1 < 0) {
                jarr = new JArray();
                _array.Add(jarr);
                arr = new List<T>();
                _items.Add(arr);
            }
            else {
                jarr = (JArray)_array[index1];
                arr = _items[index1];
            }

            if (index2 < 0) {
                jarr.Add(obj);
                arr.Add(item);
            }
            else {
                jarr.Insert(index2, obj);
                arr.Insert(index2, item);
            }
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            return item;
        }

        public void Touch()
        {
        }

        public T Insert(int index1, int index2, Action<IReferences, JObject> init = null)
        {
            return InitAndInsert(index1, index2, init);
        }

        public T Add(int index, Action<IReferences, JObject> init = null)
        {
            return InitAndInsert(index, -1, init);
        }

        public T Add(Action<IReferences, JObject> init = null)
        {
            return InitAndInsert(-1, -1, init);
        }

        public void AddRef<TRef>(int index, TRef item)
            where TRef : RefModel, T
        {
            _items[index].Add(item);
            ((JArray)_array[index]).Add(_refs.CreateReference(_array, item.Id));
        }

        public void AddRef<TRef>(TRef item)
            where TRef : RefModel, T
        {
            var jarr = new JArray { _refs.CreateReference(_array, item.Id) };
            var arr = new List<T> { item };

            _items.Add(arr);
            _array.Add(jarr);
        }

        public void SetRef<TRef>(int index1, int index2, TRef item)
            where TRef : RefModel, T
        {
            _items[index1][index2] = item;
            ((JArray)_array[index1])[index2] = _refs.CreateReference(_array, item.Id);
        }

        public int FirstEmptyIndex()
        {
            for (var i = 0; i < _items.Count; i++) {
                if (_items[i] == null) return i;
                if (_items[i] == null) return i;
            }
            return -1;
        }

        public IReadOnlyList<T> this[int index] => _items[index];
        public T this[int index1, int index2] => _items[index1][index2];

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

        public IEnumerator<IReadOnlyList<T>> GetEnumerator()
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

        public bool TryGetIndexOf(T item, out int index1, out int index2)
        {
            for (index1 = 0; index1 < _items.Count; index1++) {
                var list = _items[index1];
                if (list == null) continue;
                index2 = list.IndexOf(item);
                if (index2 >= 0) return true;
            }
            index1 = -1;
            index2 = -1;
            return false;
        }

        public bool Remove(T item)
        {
            if (!TryGetIndexOf(item, out var index1, out var index2)) return false;
            RemoveAt(index1, index2);
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

        public void RemoveAt(int index1, int index2)
        {
            var arr = _items[index1];
            var jarr = (JArray)_array[index1];

            var item = arr[index2];
            var token = jarr[index2];

            _refs.BubbleRemoval(token);

            jarr.RemoveAt(index2);
            arr.RemoveAt(index2);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}