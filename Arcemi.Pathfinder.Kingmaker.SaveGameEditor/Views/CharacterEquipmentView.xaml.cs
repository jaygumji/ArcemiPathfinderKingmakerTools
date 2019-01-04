#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Views
{
    /// <summary>
    /// Interaction logic for CharacterEquipmentView.xaml
    /// </summary>
    public partial class CharacterEquipmentView : UserControl
    {
        public CharacterEquipmentView()
        {
            InitializeComponent();
        }

        private void FixScrolling_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListViewScrollingFix.PreviewMouseWheel(sender, e);
        }

    }
}
