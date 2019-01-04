using ReactiveUI;

namespace Arcemi.Pathfinder.Kingmaker.Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
        }

        private string _greeting = "Hello World";
        public string Greeting { get => _greeting; set => this.RaiseAndSetIfChanged(ref _greeting, value); }
    }
}
