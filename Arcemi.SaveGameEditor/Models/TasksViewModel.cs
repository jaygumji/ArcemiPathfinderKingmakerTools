using Arcemi.Models;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.SaveGameEditor.Models
{
    public class TasksViewModel
    {
        private readonly IEditFileSession _session;

        public TasksViewModel(IEditFileSession session)
        {
            this._session = session;
        }

        public bool CanEdit => _session.CanEdit;

        public IEnumerable<PlayerKingdomTaskModel> Tasks => _session.Player?.Kingdom?.Leaders?.Where(l => l.AssignedTask != null).Select(l => l.AssignedTask);
        public bool HasTasks => Tasks?.Any() ?? false;

        public int CurrentTurn => _session.Player?.Kingdom?.CurrentTurn ?? 0;
    }
}
