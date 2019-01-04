#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public delegate void ValueChangedHandler(ValueChangedArgs args);

    public class ValueChangedArgs
    {
        public Model Model { get; }
        public string Name { get; }

        public ValueChangedArgs(Model model, string name)
        {
            Model = model;
            Name = name;
        }
    }

    public class NotifyChangeTracker
    {
        private readonly Model _model;
        private readonly Func<PropertyChangedEventHandler> _propertyChangedGetter;

        private readonly Dictionary<string, List<ValueChangedHandler>> _on;
        private readonly HashSet<ChangeTrackerForwarding> _forwarders;
        private bool _supress;

        public NotifyChangeTracker(Model model, Func<PropertyChangedEventHandler> propertyChangedGetter)
        {
            _model = model;
            _propertyChangedGetter = propertyChangedGetter;
            _on = new Dictionary<string, List<ValueChangedHandler>>(StringComparer.Ordinal);
            _forwarders = new HashSet<ChangeTrackerForwarding>();
        }

        public void On(string listenOnName, ValueChangedHandler callback)
        {
            if (_on.TryGetValue(listenOnName, out var list)) {
                foreach (var reg in list) {
                    if (reg == callback) {
                        return;
                    }
                }
            }
            else  {
                list = new List<ValueChangedHandler>();
                _on.Add(listenOnName, list);
            }
            list.Add(callback);
        }

        public void Supress()
        {
            _supress = true;
        }

        public void Resume()
        {
            _supress = false;
        }

        public void On(string listenOnName, string notifyOn)
        {
            if (string.Equals(listenOnName, notifyOn, StringComparison.Ordinal)) {
                return;
            }

            var key = new ChangeTrackerForwarding(listenOnName, notifyOn);
            if (_forwarders.Contains(key)) {
                return;
            }
            _forwarders.Add(key);
            On(listenOnName, a => Notify(notifyOn));
        }

        public void On(Model model, string listenOnName, string notifyOn)
        {
            if (model == null) return;
            var admin = (IModelAdmin)model;

            admin.ChangeTracker.On(listenOnName, a => Notify(notifyOn));
        }

        public void Updated(string name, JToken value)
        {
            Notify(name);
        }

        public void Notify(string name)
        {
            if (_supress) return;
            _propertyChangedGetter()?.Invoke(_model, new PropertyChangedEventArgs(name));
            if (_on.TryGetValue(name, out var list)) {
                var args = new ValueChangedArgs(_model, name);
                foreach (var callback in list) {
                    callback.Invoke(args);
                }
            }
        }
    }
}