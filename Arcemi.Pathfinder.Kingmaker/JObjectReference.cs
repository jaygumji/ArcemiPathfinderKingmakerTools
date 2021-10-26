#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class JObjectReferenceBy
    {
        public JObjectReferenceBy(JToken parent, JObject reference)
        {
            Parent = parent;
            Reference = reference;
        }

        public JToken Parent { get; }
        public JObject Reference { get; }

        public void SetDirectReference(JObject obj)
        {
            if (Parent is JArray arr) {
                for (var i = 0; i < arr.Count; i++) {
                    if (ReferenceEquals(arr[i], Reference)) {
                        arr[i] = obj;
                        return;
                    }
                }
            }
            else if (Parent is JObject parentObj) {
                foreach (var property in parentObj.Properties()) {
                    if (ReferenceEquals(property.Value, Reference)) {
                        property.Value = obj;
                        return;
                    }
                }
            }
        }
    }
    public class JObjectReference
    {
        private readonly List<JObjectReferenceBy> _referencedBy;
        public JObjectReference(JObject obj)
        {
            Obj = obj;
            _referencedBy = new List<JObjectReferenceBy>();
        }

        public JObject Obj { get; }
        public int RefCount => ReferencedBy.Count;
        public IReadOnlyList<JObjectReferenceBy> ReferencedBy => _referencedBy;

        public void AddReference(JToken parent, JObject reference)
        {
            _referencedBy.Add(new JObjectReferenceBy(parent, reference));
        }

        public void EnsureAnotherOwner()
        {
            if (_referencedBy.Count == 0) return;
            var newOwner = _referencedBy[0];
            _referencedBy.RemoveAt(0);
            newOwner.SetDirectReference(Obj);
        }

        public void RemoveReference(JObject obj)
        {
            var refBy = _referencedBy.FirstOrDefault(x => x.Reference == obj);
            if (refBy != null) _referencedBy.Remove(refBy);
        }
    }
}