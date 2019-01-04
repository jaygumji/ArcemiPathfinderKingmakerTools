#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class RefModel : Model
    {
        public RefModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string Id => A.Value<string>("$id");

    }
}