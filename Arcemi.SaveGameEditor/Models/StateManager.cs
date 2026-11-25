using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor.Models
{
    public class StateManager
    {
        private readonly EditorErrorReports _reports;

        public StateManager(EditorErrorReports reports)
        {
            _reports = reports;
        }

        public bool IsBusy { get; private set; }

        public async Task ExecuteAsync(Func<Task> action, [CallerMemberName] string operation = null)
        {
            IsBusy = true;
            await Task.Yield();
            try {
                await action();
            }
            catch (Exception exception) {
                _reports.Report(operation, exception);
            }
            finally {
                IsBusy = false;
            }
        }

        public Task ExecuteAsync(Action action, [CallerMemberName] string operation = null)
            => ExecuteAsync(() => {
                action();
                return Task.CompletedTask;
            }, operation);
    }
}
