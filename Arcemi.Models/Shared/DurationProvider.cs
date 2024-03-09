using System;

namespace Arcemi.Models
{
    public class DurationProvider
    {
        private readonly Func<TimeSpan> endTimeGetter;
        private readonly Action<TimeSpan> endTimeSetter;
        private readonly IGameTimeProvider gameTimeProvider;

        public DurationProvider(Func<TimeSpan> endTimeGetter, Action<TimeSpan> endTimeSetter, IGameTimeProvider gameTimeProvider)
        {
            this.endTimeGetter = endTimeGetter;
            this.endTimeSetter = endTimeSetter;
            this.gameTimeProvider = gameTimeProvider;
        }

        public TimeSpan Duration
        {
            get {
                var EndTime = endTimeGetter.Invoke();
                var gameTime = gameTimeProvider.Get();
                if (EndTime == TimeSpan.Zero || gameTime == TimeSpan.Zero) return TimeSpan.Zero;
                var duration = EndTime.Subtract(gameTime);
                if (duration < TimeSpan.Zero) return TimeSpan.Zero;
                return duration;
            }
            set {
                endTimeSetter.Invoke(gameTimeProvider.Get() + value);
            }
        }

        private TimeParts _durationParts;
        public TimeParts DurationParts => _durationParts ?? (_durationParts = new TimeParts(() => Duration, v => Duration = v));

    }
}