using Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Views
{
    /// <summary>
    /// Interaction logic for PlayerKingdomLeaders.xaml
    /// </summary>
    public partial class PlayerKingdomLeadersView : UserControl
    {
        public PlayerKingdomLeadersView()
        {
            InitializeComponent();
        }

        private void GotoLeader_MouseDown(object sender, RoutedEventArgs e)
        {
            var model = (MainModel)DataContext;
            var el = (FrameworkElement)sender;
            var leader = (PlayerKingdomLeaderModel)el.DataContext;

            Keyboard.FocusedElement?.RaiseEvent(new RoutedEventArgs(LostFocusEvent));

            model.Leader = leader;
        }

        private void FixScrolling_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListViewScrollingFix.PreviewMouseWheel(sender, e);
        }

    }
}
