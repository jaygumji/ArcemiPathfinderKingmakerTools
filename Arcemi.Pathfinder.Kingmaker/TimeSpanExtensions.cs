#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public static class TimeSpanExtensions
    {
        public static string AsDisplayableDayHourMinute(this TimeSpan value)
        {
            return $"{value.Days}d {value.Hours}h {value.Minutes}min";
        }
    }
}
