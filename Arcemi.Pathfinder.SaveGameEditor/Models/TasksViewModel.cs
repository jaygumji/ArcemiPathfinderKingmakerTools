using Arcemi.Pathfinder.Kingmaker;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Pathfinder.SaveGameEditor.Models
{
    public class TasksViewModel
    {
        private readonly MainViewModel main;

        public TasksViewModel(MainViewModel main)
        {
            this.main = main;
        }

        public bool CanEdit => main.CanEdit;

        public IEnumerable<PlayerKingdomTaskModel> Tasks => main.Player?.Kingdom?.Leaders?.Where(l => l.AssignedTask != null).Select(l => l.AssignedTask);
        public bool HasTasks => Tasks?.Any() ?? false;

        public int CurrentTurn => main.Player?.Kingdom?.CurrentTurn ?? 0;
    }
}
