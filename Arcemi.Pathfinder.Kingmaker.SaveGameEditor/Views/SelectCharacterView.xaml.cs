#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models;
using System.Windows;
using System.Windows.Controls;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Views
{
    /// <summary>
    /// Interaction logic for SelectCharacterView.xaml
    /// </summary>
    public partial class SelectCharacterView : UserControl
    {
        public SelectCharacterView()
        {
            InitializeComponent();
        }

        private void GotoCharacter_MouseDown(object sender, RoutedEventArgs e)
        {
            var model = (MainModel)DataContext;
            var el = (FrameworkElement)sender;
            var character = (CharacterModel)el.DataContext;
            model.Character = character;
        }

    }
}
