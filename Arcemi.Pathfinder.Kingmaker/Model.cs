#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class Model : IModelAdmin, INotifyPropertyChanged
    {
        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        protected ModelDataAccessor A { get; }
        protected NotifyChangeTracker N { get; }

        public Model(ModelDataAccessor accessor)
        {
            A = accessor;
            N = new NotifyChangeTracker(this, () => _propertyChanged);
            A.SetChangeTracker(N);
        }

        ModelDataAccessor IModelAdmin.Accessor => A;
        NotifyChangeTracker IModelAdmin.ChangeTracker => N;

        public ModelDataAccessor GetAccessor() => A;
    }
}