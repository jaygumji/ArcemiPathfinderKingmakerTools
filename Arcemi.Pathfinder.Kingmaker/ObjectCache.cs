#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ObjectCache
    {
        private readonly Dictionary<JObject, Dictionary<string, object>> _local;
        private readonly Dictionary<JArray, Dictionary<int, object>> _localArrs;
        private readonly Dictionary<string, object> _global;

        public ObjectCache()
        {
            _local = new Dictionary<JObject, Dictionary<string, object>>();
            _global = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public bool TryGetGlobal<T>(JObject item, out T obj)
        {
            if (item.TryGetRefId(out var id) && _global.TryGetValue(id, out var globalObj)) {
                if (globalObj is T tobj) {
                    obj = tobj;
                    return true;
                }
            }
            obj = default;
            return false;
        }

        public bool TryGet<T>(JObject parent, string name, out T obj)
        {
            var property = parent.Property(name);
            if (property != null && property.Value.TryGetRefId(out var id)) {
                if (_global.TryGetValue(id, out var globalObj)) {
                    obj = (T)globalObj;
                    return true;
                }
            }
            else if (_local.TryGetValue(parent, out var cache) && cache.TryGetValue(name, out var untypedObj)) {
                obj = (T)untypedObj;
                return true;
            }
            obj = default;
            return false;
        }

        public void Add<T>(JObject parent, string name, T obj)
        {
            var property = parent.Property(name);
            if (property != null && property.Value.TryGetRefId(out var id)) {
                _global.Add(id, obj);
                return;
            }
            if (!_local.TryGetValue(parent, out var cache)) {
                cache = new Dictionary<string, object>(StringComparer.Ordinal);
                _local.Add(parent, cache);
            }
            cache.Add(name, obj);
        }

        public void AddGlobal(string id, object obj)
        {
            if (obj is ITypedModel typed && typed.Type.IsEmpty()) return;
            _global.Add(id, obj);
        }

        public void AddGlobal(JObject item, object obj)
        {
            if (item.TryGetRefId(out var id)) {
                AddGlobal(id, obj);
            }
        }

        public bool Remove(JObject parent, string name)
        {
            var property = parent.Property(name);
            if (property != null && property.Value.TryGetRefId(out var id)) {
                return _global.Remove(id);
            }
            if (!_local.TryGetValue(parent, out var cache)) {
                return false;
            }
            return cache.Remove(name);
        }

        public void Refresh(JObject parent)
        {
            if (!_local.TryGetValue(parent, out var cache)) return;

            foreach (var modelContainer in cache.Values.OfType<IModelContainer>()) {
                modelContainer.Refresh();
            }
        }
    }
}