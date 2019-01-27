#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models;
using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for Party.xaml
    /// </summary>
    public partial class CharactersView : UserControl
    {
        public CharactersView()
        {
            InitializeComponent();
        }

        private void UpdatePortrait_MouseDown(object sender, RoutedEventArgs e)
        {
            DlgSelectPortrait.IsOpen = false;
            e.Handled = true;

            var model = (MainModel)DataContext;
            var ui = model.Character.UISettings;

            var el = (FrameworkElement)sender;
            var portrait = (Portrait)el.DataContext;
            ui.SetPortrait(portrait);
        }

        private void SelectPortrait_MouseDown(object sender, RoutedEventArgs e)
        {
            var model = (MainModel)DataContext;

            DlgSelectPortrait.IsOpen = true;
            e.Handled = true;
        }

        private void AlignmentEllipse_MouseDown(object sender, RoutedEventArgs e)
        {
            var el = (FrameworkElement)sender;
            var character = (CharacterModel)el.DataContext;

            var p = Mouse.GetPosition(el);
            var x = (p.X - 100) / 100;
            var y = (p.Y - 100) / 100 * -1;

            character.Alignment.Vector.X = x;
            character.Alignment.Vector.Y = y;
        }

        private void FixScrolling_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListViewScrollingFix.PreviewMouseWheel(sender, e);
        }
    }
}
