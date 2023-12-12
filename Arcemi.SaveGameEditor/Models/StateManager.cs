using System;
using System.Threading.Tasks;

namespace Arcemi.SaveGameEditor.Models
{
    public class StateManager
    {
        public bool IsBusy { get; private set; }

        public async Task ExecuteAsync(Func<Task> action)
        {
            IsBusy = true;
            await Task.Yield();
            try {
                await action();
            }
            catch (Exception) {
                throw;
            }
            finally {
                IsBusy = false;
            }
        }

        public async Task ExecuteAsync(Action action)
        {
            IsBusy = true;
            await Task.Yield();
            try {
                action();
            }
            catch (Exception) {
                throw;
            }
            finally {
                IsBusy = false;
            }
        }
    }
}
