#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    public class References : IReferences
    {
        private readonly Dictionary<string, JObjectReference> _lookup;
        private readonly Dictionary<JObject, JObjectReference> _refLookup;
        private int _maxId;
        private readonly ObjectCache _objects;
        private readonly IReferences _refs;

        public References()
        {
            _lookup = new Dictionary<string, JObjectReference>(StringComparer.Ordinal);
            _refLookup = new Dictionary<JObject, JObjectReference>();
            _objects = new ObjectCache();
            _refs = this;
        }

        public void VisitTree(JToken parent, JToken node)
        {
            if (node is JArray arr) {
                for (var i = 0; i < arr.Count; i++) {
                    VisitTree(arr, arr[i]);
                }
                return;
            }

            if (node is JObject obj) {
                foreach (var property in obj.Properties()) {
                    if (string.Equals(property.Name, "$id", StringComparison.Ordinal)) {
                        var id = property.Value.Value<string>();
                        Add(id, obj);
                    }
                    else if (string.Equals(property.Name, "$ref", StringComparison.Ordinal)) {
                        var id = property.Value.Value<string>();
                        ReferTo(parent, obj, id);
                    }

                    VisitTree(obj, property.Value);
                }
            }
        }

        public void Add(string id, JObject obj)
        {
            if (int.TryParse(id, out var intId)) {
                if (_maxId < intId) {
                    _maxId = intId;
                }
            }

            _lookup.Add(id, new JObjectReference(obj));
        }

        public object CreateObject(JObject jObj, Func<ModelDataAccessor, object> factory)
        {
            var accessor = new ModelDataAccessor(jObj, this);
            var obj = factory.Invoke(accessor);
            var id = jObj.Property("$id").Value.Value<string>();
            _objects.AddGlobal(id, obj);
            return obj;
        }

        JObject IReferences.Create()
        {
            var id = (++_maxId).ToString();
            var obj = new JObject {
                { "$id", id }
            };

            Add(id, obj);
            return obj;
        }

        JObject IReferences.CreateReference(JToken parent, string refId)
        {
            var obj = new JObject {
                {"$ref", refId}
            };
            ReferTo(parent, obj, refId);
            return obj;
        }

        JObject IReferences.CreateReference(JToken parent, JObject refObj)
        {
            var refId = refObj.Property("$id").Value.Value<string>();
            return ((IReferences)this).CreateReference(parent, refId);
        }

        bool IReferences.TryGetReferred(string refId, out JObject refObj)
        {
            if (_lookup.TryGetValue(refId, out var jRef)) {
                refObj = jRef.Obj;
                return true;
            }
            refObj = null;
            return false;
        }

        JObject IReferences.GetReferred(JObject obj)
        {
            var refProperty = obj.Property("$ref");
            if (refProperty != null && !(refProperty.Value is null)) {
                var refId = refProperty.Value.Value<string>();
                if (_refs.TryGetReferred(refId, out var refObj)) {
                    return refObj;
                }
                else {
                    throw new InvalidOperationException($"The reference to '{refId}' does not exist.");
                }
            }
            return obj;
        }

        JArray IReferences.NewArray(int size, JToken initialValue)
        {
            var arr = new JArray();
            for (var i = 0; i < size; i++) { arr.Add(initialValue); }
            return arr;
        }

        JArray IReferences.NewArray(int size, Func<JToken> initialValue)
        {
            var arr = new JArray();
            for (var i = 0; i < size; i++) { arr.Add(initialValue()); }
            return arr;
        }

        private ModelDataAccessor Get(JObject parent, string name)
        {
            var property = parent.Property(name);
            if (property == null || property.Value is null || property.Value.Type == JTokenType.Null) {
                return null;
            }
            if (!(property.Value is JObject obj)) {
                throw new ArgumentException($"Parameter {name} does not reference a valid reference object.");
            }

            obj = _refs.GetReferred(obj);
            return new ModelDataAccessor(obj, this);
        }

        private ModelDataAccessor Get(JToken item)
        {
            if (item == null || item.Type == JTokenType.Null) {
                return null;
            }
            if (!(item is JObject obj)) {
                throw new ArgumentException("Parameter item does not reference a valid reference object.");
            }

            obj = _refs.GetReferred(obj);
            return new ModelDataAccessor(obj, this);
        }

        T IReferences.GetOrCreateObject<T>(JObject item, Func<ModelDataAccessor, T> factory)
        {
            if (_objects.TryGetGlobal(item, out T obj)) {
                return obj;
            }

            var accessor = Get(item);
            if (accessor == null) {
                return default;
            }
            obj = (factory ?? ModelFactory.Get<T>()).Invoke(accessor);
            _objects.AddGlobal(item, obj);
            return obj;
        }

        T IReferences.GetOrCreateObject<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory, bool createIfNull)
        {
            if (_objects.TryGet(parent, name, out T obj)) {
                return obj;
            }

            var property = Get(parent, name);
            if (property == null) {
                if (createIfNull) {
                    var jObj = _refs.Create();
                    if (parent.Property(name) != null) {
                        parent[name] = jObj;
                    }
                    else {
                        parent.Add(name, jObj);
                    }
                    property = new ModelDataAccessor(jObj, this);
                }
                else {
                    return default;
                }
            }
            obj = (factory ?? ModelFactory.Get<T>()).Invoke(property);
            _objects.Add(parent, name, obj);
            return obj;
        }

        public void ReferTo(JToken parent, JObject reference, string id)
        {
            var jRef = _lookup[id];
            jRef.AddReference(parent, reference);
            _refLookup.Add(reference, jRef);
        }

        internal bool PreserveNestedReferencesOnRemoval { get; set; }

        public void BubbleRemoval(JToken token)
        {
            if (PreserveNestedReferencesOnRemoval) {
                RemoveReferenceTree(token, token);
                return;
            }
            if (token is JObject obj) {
                var refId = (string)obj["$ref"];
                if (refId.HasValue()) {
                    _refLookup[obj].RemoveReference(obj);
                    _refLookup.Remove(obj);
                    return;
                }
                var id = (string)obj["$id"];
                if (id.HasValue() && _lookup[id].RefCount > 0) {
                    _lookup[id].EnsureAnotherOwner();
                    return;
                }
                foreach (var property in obj.Properties()) BubbleRemoval(property.Value);
            }
            else if (token is JArray arr) {
                foreach (var item in arr) BubbleRemoval(item);
            }
        }

        private void RemoveReferenceTree(JToken token, JToken removedTree)
        {
            if (token is JObject obj) {
                var refId = (string)obj["$ref"];
                if (refId.HasValue()) {
                    if (_refLookup.Remove(obj, out var target)) target.RemoveReference(obj);
                    return;
                }

                var id = (string)obj["$id"];
                if (id.HasValue() && _lookup.TryGetValue(id, out var reference)) {
                    var newOwner = reference.ReferencedBy
                        .Where(r => r.Reference.Parent != null
                            && !r.Reference.AncestorsAndSelf().Contains(removedTree))
                        .OrderBy(r => r.Reference, JTokenDocumentOrder.Instance)
                        .FirstOrDefault();
                    if (newOwner is not null) {
                        _refLookup.Remove(newOwner.Reference);
                        reference.RemoveReference(newOwner.Reference);
                        // Keep the same JObject for existing model accessors. Assigning it while
                        // it still has a parent would cause Json.NET to clone it.
                        if (obj.Parent is not null) obj.Replace(JValue.CreateNull());
                        newOwner.Reference.Replace(obj);
                        RestoreDefinitionOrder(obj);
                        return;
                    }
                }

                foreach (var property in obj.Properties()) {
                    RemoveReferenceTree(property.Value, removedTree);
                }
            }
            else if (token is JArray arr) {
                for (var i = 0; i < arr.Count; i++) {
                    RemoveReferenceTree(arr[i], removedTree);
                }
            }
        }

        private void RestoreDefinitionOrder(JObject moved)
        {
            foreach (var obj in moved.DescendantsAndSelf().OfType<JObject>().ToArray()) {
                if ((string)obj["$id"] is { } id) {
                    MoveDefinitionBeforeReferences(_lookup[id]);
                }
                else if ((string)obj["$ref"] is { } refId && _lookup.TryGetValue(refId, out var definition)) {
                    MoveDefinitionBeforeReferences(definition);
                }
            }
        }

        private void MoveDefinitionBeforeReferences(JObjectReference definition)
        {
            var obj = definition.Obj;
            var first = definition.ReferencedBy
                .Select(r => r.Reference)
                .Where(r => r.Parent != null && ReferenceEquals(r.Root, obj.Root)
                    && JTokenDocumentOrder.Instance.Compare(r, obj) < 0)
                .OrderBy(r => r, JTokenDocumentOrder.Instance)
                .FirstOrDefault();
            if (first is null) return;

            _refLookup.Remove(first);
            definition.RemoveReference(first);
            obj.Replace(_refs.CreateReference(obj.Parent, obj));
            first.Replace(obj);
            RestoreDefinitionOrder(obj);
        }

        ListD2Accessor<T> IReferences.GetOrCreateListD2<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory, bool createIfNotDefined)
        {
            if (_objects.TryGet(parent, name, out ListD2Accessor<T> list)) {
                return list;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNotDefined) return null;
                parent.Add(name, new JArray());
                property = parent.Property(name);
            }
            if (property.Value is null || property.Value.Type == JTokenType.Null) {
                return null;
            }

            if (!(property.Value is JArray arr)) {
                throw new ArgumentException($"Parameter {name} does not reference a valid array.");
            }

            var listAccessor = new ListD2Accessor<T>(arr, _refs, factory ?? ModelFactory.Get<T>());
            _objects.Add(parent, name, listAccessor);
            return listAccessor;
        }

        ListAccessor<T> IReferences.GetOrCreateList<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory, bool createIfNotDefined)
        {
            if (_objects.TryGet(parent, name, out ListAccessor<T> list)) {
                return list;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNotDefined) return null;
                parent.Add(name, new JArray());
                property = parent.Property(name);
            }
            if (property.Value is null || property.Value.Type == JTokenType.Null) {
                return null;
            }

            if (!(property.Value is JArray arr)) {
                throw new ArgumentException($"Parameter {name} does not reference a valid array.");
            }

            var listAccessor = new ListAccessor<T>(arr, _refs, factory ?? ModelFactory.Get<T>());
            _objects.Add(parent, name, listAccessor);
            return listAccessor;
        }

        ListValueAccessor<T> IReferences.GetOrCreateListValue<T>(JObject parent, string name, bool createIfNull)
        {
            if (_objects.TryGet(parent, name, out ListValueAccessor<T> list)) {
                return list;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNull) return null;
                throw new ArgumentException($"Parameter {name} does not reference a valid array.");
            }

            if (!(property.Value is JArray arr)) {
                if (property.Value is null) {
                    throw new ArgumentException($"Parameter {name} does not reference a valid array.");
                }
                arr = new JArray();
                property.Value = arr;
            }

            var listAccessor = new ListValueAccessor<T>(arr);
            _objects.Add(parent, name, listAccessor);
            return listAccessor;
        }

        public DictionaryOfValueAccessor<TValue> GetOrCreateDictionaryOfValue<TValue>(JObject parent, string name, bool createIfNull = false)
        {
            if (_objects.TryGet(parent, name, out DictionaryOfValueAccessor<TValue> dict)) {
                return dict;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNull) return null;
                throw new ArgumentException($"Parameter {name} does not reference a valid object.");
            }

            if (!(property.Value is JObject dictObj)) {
                if (property.Value is null) {
                    throw new ArgumentException($"Parameter {name} does not reference a valid object.");
                }
                dictObj = new JObject();
                property.Value = dictObj;
            }

            dict = new DictionaryOfValueAccessor<TValue>(dictObj);
            _objects.Add(parent, name, dict);
            return dict;
        }

        public DictionaryAccessor<T> GetOrCreateDictionary<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false)
            where T : Model
        {
            if (_objects.TryGet(parent, name, out DictionaryAccessor<T> dict)) {
                return dict;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNull) return null;
                throw new ArgumentException($"Parameter {name} does not reference a valid object.");
            }

            if (!(property.Value is JObject dictObj)) {
                if (property.Value is null) {
                    throw new ArgumentException($"Parameter {name} does not reference a valid object.");
                }
                dictObj = new JObject();
                property.Value = dictObj;
            }

            dict = new DictionaryAccessor<T>(dictObj, _refs, factory ?? ModelFactory.Get<T>());
            _objects.Add(parent, name, dict);
            return dict;
        }

        public DictionaryOfValueListAccessor<TValue> GetOrCreateDictionaryOfValueList<TValue>(JObject parent, string name, bool createIfNull = false)
        {
            if (_objects.TryGet(parent, name, out DictionaryOfValueListAccessor<TValue> dict)) {
                return dict;
            }

            var property = parent.Property(name);
            if (property == null) {
                if (!createIfNull) return null;
                throw new ArgumentException($"Parameter {name} does not reference a valid object.");
            }

            if (!(property.Value is JObject dictObj)) {
                if (property.Value is null) {
                    throw new ArgumentException($"Parameter {name} does not reference a valid object.");
                }
                dictObj = new JObject();
                property.Value = dictObj;
            }

            dict = new DictionaryOfValueListAccessor<TValue>(dictObj);
            _objects.Add(parent, name, dict);
            return dict;
        }

        bool IReferences.RemoveObject(JObject parent, string name)
        {
            return _objects.Remove(parent, name);
        }

        bool IReferences.RemoveList(JObject parent, string name)
        {
            return _objects.Remove(parent, name);
        }

        bool IReferences.RemoveListValue(JObject parent, string name)
        {
            return _objects.Remove(parent, name);
        }

        void IReferences.Refresh(JObject parent)
        {
            _objects.Refresh(parent);
        }
    }
}