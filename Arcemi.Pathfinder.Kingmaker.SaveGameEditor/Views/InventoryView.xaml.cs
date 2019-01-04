#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Views
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();
        }

        private void OpenSelectItem_Click(object sender, RoutedEventArgs e)
        {
            DlgSelectItem.IsOpen = true;
            e.Handled = true;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            DlgSelectItem.IsOpen = false;
            e.Handled = true;

            var el = (FrameworkElement)sender;
            var item = (ItemViewModel)el.DataContext;

            ((InventoryViewModel)DataContext).Inventory.AddItem(item.RawData);
        }

        private void FixScrolling_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListViewScrollingFix.PreviewMouseWheel(sender, e);
        }
    }
}
