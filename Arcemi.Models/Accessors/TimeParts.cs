using System;

namespace Arcemi.Models
{
    public class TimeParts
    {
        private readonly Func<TimeSpan> getter;
        private readonly Action<TimeSpan> setter;

        public TimeParts(Func<TimeSpan> getter, Action<TimeSpan> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        private void Set(int days = -1, int hours = -1, int minutes = -1, int seconds = -1, int milliseconds = -1)
        {
            var v = getter();
            var newValue = new TimeSpan(days >= 0 ? days : v.Days,
                hours >= 0 ? hours : v.Hours,
                minutes >= 0 ? minutes : v.Minutes,
                seconds >= 0 ? seconds : v.Seconds,
                milliseconds >= 0 ? milliseconds : v.Milliseconds);
            setter(newValue);
        }

        public bool IsEmpty => getter() == TimeSpan.Zero;

        public int Days { get => getter().Days; set => Set(days: value); }
        public int Hours { get => getter().Hours; set => Set(hours: value); }
        public int Minutes { get => getter().Minutes; set => Set(minutes: value); }
        public int Seconds { get => getter().Seconds; set => Set(seconds: value); }
        public int Milliseconds { get => getter().Milliseconds; set => Set(milliseconds: value); }
    }
}