#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;

namespace Arcemi.Models
{
    public class ChangeTrackerForwarding
    {
        public string ListenOnName { get; }
        public string NotifyOn { get; }

        public ChangeTrackerForwarding(string listenOnName, string notifyOn)
        {
            ListenOnName = listenOnName;
            NotifyOn = notifyOn;
        }

        public override int GetHashCode()
        {
            return ListenOnName.GetHashCode() ^ NotifyOn.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ChangeTrackerForwarding other) {
                return string.Equals(ListenOnName, other.ListenOnName, StringComparison.Ordinal)
                    && string.Equals(NotifyOn, other.NotifyOn, StringComparison.Ordinal);
            }
            return false;
        }
    }
}