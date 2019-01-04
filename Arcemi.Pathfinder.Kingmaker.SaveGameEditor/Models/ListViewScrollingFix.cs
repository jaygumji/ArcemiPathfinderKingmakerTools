#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models
{
    public static class ListViewScrollingFix
    {
        public static void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled) return;

            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            var parent = (UIElement)((Control)sender).Parent;
            parent.RaiseEvent(eventArg);
        }
    }
}
