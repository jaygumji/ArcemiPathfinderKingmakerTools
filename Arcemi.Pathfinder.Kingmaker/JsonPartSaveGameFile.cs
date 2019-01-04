#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{

    public class JsonPartSaveGameFile
    {
        private readonly string _path;
        private readonly JObject _json;

        private readonly References _refs;
        private readonly Dictionary<BlueprintIdentifier, List<Model>> _blueprintRefs;

        private Model _root;

        public JsonPartSaveGameFile(string path, JObject json)
        {
            _path = path;
            _json = json;
            _refs = new References();
            _blueprintRefs = new Dictionary<BlueprintIdentifier, List<Model>>();
            VisitTree(json);
        }

        public T GetRoot<T>(Func<ModelDataAccessor, T> factory = null)
            where T : Model
        {
            if (_root != null) {
                return (T)_root;
            }

            var accessor = new ModelDataAccessor(_json, _refs);
            var root = (factory ?? Mappings.GetFactory<T>()).Invoke(accessor);
            _root = root;
            return root;
        }

        public IReadOnlyList<T> GetAllOf<T>()
        {
            var blueprintIds = Mappings.GetBlueprintId<T>();
            return blueprintIds
                .SelectMany(id => _blueprintRefs.TryGetValue(id, out var models)
                    ? models
                    : (IEnumerable<Model>)new Model[0])
                .Cast<T>()
                .ToArray();
        }

        private void AddBlueprintRef(BlueprintIdentifier blueprintId, JObject obj)
        {
            if (!Mappings.TryGetFactory(blueprintId, out var factory)) {
                return;
            }

            if (!_blueprintRefs.TryGetValue(blueprintId, out var list)) {
                list = new List<Model>();
                _blueprintRefs.Add(blueprintId, list);
            }

            var model = (Model)_refs.CreateObject(obj, factory);
            list.Add(model);
        }

        private void VisitTree(JToken node)
        {
            if (node is JArray arr) {
                foreach (var value in arr) {
                    VisitTree(value);
                }
                return;
            }

            if (node is JObject obj) {
                foreach (var property in obj.Properties()) {
                    if (string.Equals(property.Name, "$id", StringComparison.Ordinal)) {
                        var id = property.Value.Value<string>();
                        var blueprintId = BlueprintIdentifier.From(obj);
                        AddBlueprintRef(blueprintId, obj);
                        _refs.Add(id, obj);
                    }

                    VisitTree(property.Value);
                }
            }
        }

        public void Save()
        {
            using (var stream = new FileStream(_path, FileMode.Create, FileAccess.Write)) {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8)) {
                    using (var writer = new JsonTextWriter(streamWriter)) {
                        _json.WriteTo(writer);
                    }
                }
            }
        }
    }
}