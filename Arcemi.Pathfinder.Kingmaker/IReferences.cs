#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public interface IReferences
    {
        bool TryGetReferred(string refId, out JObject refObj);
        JObject GetReferred(JObject obj);
        JObject Create();
        JObject CreateReference(string refId);
        JObject CreateReference(JObject refObj);

        T GetOrCreateObject<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false);
        ListAccessor<T> GetOrCreateList<T>(JObject parent, string name, Func<ModelDataAccessor, T> factory = null, bool createIfNull = false);
        ListValueAccessor<T> GetOrCreateListValue<T>(JObject parent, string name, bool createIfNull = false);
        DictionaryOfValueListAccessor<TValue> GetOrCreateDictionaryOfValueList<TValue>(JObject parent, string name, bool createIfNull = false);

        bool RemoveObject(JObject parent, string name);
        bool RemoveList(JObject parent, string name);
        bool RemoveListValue(JObject parent, string name);
    }
}