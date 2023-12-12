#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System.Collections.Generic;

namespace Arcemi.Models
{
    public class Portraits
    {
        private readonly ResourceData _resources;

        public Portraits(IResourceProvider resourceProvider)
        {
            _resources = resourceProvider.GetResources();
        }

        public IReadOnlyList<Portrait> Available => _resources.AvailablePortraits;

        public string GetUnknownUri()
        {
            return _resources.AllPortraits["_s_unknown"].Uri;
        }

        public string GetPortraitsUri(string key)
        {
            return _resources.AllPortraits.TryGetValue(key, out var portrait) ? portrait.Uri : GetUnknownUri();
        }

        public bool TryGetPortraitsUri(string key, out string uri)
        {
            if (_resources.AllPortraits.TryGetValue(key, out var portrait)) {
                uri = portrait.Uri;
                return true;
            }
            uri = null;
            return false;
        }

        public bool TryGetPortrait(string key, out Portrait portrait)
        {
            return _resources.AllPortraits.TryGetValue(key, out portrait);
        }
    }
}