#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models;
using System.Windows;
using System.Windows.Controls;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var model = new MainModel();
            DataContext = model;

            InitializeComponent();

            Closing += (s, e) => {
                model.HandleClose();
            };
        }
    }
}
