#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Views
{
    /// <summary>
    /// Interaction logic for PlayerStashView.xaml
    /// </summary>
    public partial class PlayerStashView : UserControl
    {
        public PlayerStashView()
        {
            InitializeComponent();
        }

        private void FixScrolling_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListViewScrollingFix.PreviewMouseWheel(sender, e);
        }
    }
}
