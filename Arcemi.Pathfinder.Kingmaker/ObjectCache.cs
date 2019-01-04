#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ObjectCache
    {
        private readonly Dictionary<JObject, Dictionary<string, object>> _local;
        private readonly Dictionary<string, object> _global;

        public ObjectCache()
        {
            _local = new Dictionary<JObject, Dictionary<string, object>>();
            _global = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        private bool TryGetId(JObject parent, string name, out string id)
        {
            id = null;
            var property = parent.Property(name);
            if (property == null) return false;
            if (property.Value is null) return false;
            if (property.Value is JObject obj) {
                return TryGetId(obj, out id);
            }
            return false;
        }

        private bool TryGetId(JObject parent, out string id)
        {
            id = null;
            var idProperty = parent.Property("$id");
            if (idProperty == null) return false;
            if (idProperty.Value is null) return false;

            id = idProperty.Value.Value<string>();
            return !string.IsNullOrEmpty(id);
        }

        public bool TryGet<T>(JObject parent, string name, out T obj)
        {
            if (TryGetId(parent, name, out var id)) {
                if (_global.TryGetValue(id, out var globalObj)) {
                    obj = (T)globalObj;
                    return true;
                }
            }
            else if (_local.TryGetValue(parent, out var cache) && cache.TryGetValue(name, out var untypedObj)) {
                obj = (T)untypedObj;
                return true;
            }
            obj = default(T);
            return false;
        }

        public void Add<T>(JObject parent, string name, T obj)
        {
            if (TryGetId(parent, name, out var id)) {
                _global.Add(id, obj);
                return;
            }
            if (!_local.TryGetValue(parent, out var cache)) {
                cache = new Dictionary<string, object>(StringComparer.Ordinal);
                _local.Add(parent, cache);
            }
            cache.Add(name, obj);
        }

        public void Add(string id, object obj)
        {
            _global.Add(id, obj);
        }

        public bool Remove(JObject parent, string name)
        {
            if (TryGetId(parent, name, out var id)) {
                return _global.Remove(id);
            }
            if (!_local.TryGetValue(parent, out var cache)) {
                return false;
            }
            return cache.Remove(name);
        }
    }
}