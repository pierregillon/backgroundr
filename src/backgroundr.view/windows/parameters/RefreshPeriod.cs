using System;

namespace backgroundr.view.windows.parameters
{
    public class RefreshPeriod
    {
        private readonly TimeSpan _timespan;

        public TimeSpan Value => _timespan;

        public RefreshPeriod(TimeSpan timespan)
        {
            _timespan = timespan;
        }

        public override string ToString()
        {
            if ((int) _timespan.TotalMinutes == 0) {
                return $"Every {(int) _timespan.TotalSeconds} seconds";
            }
            if ((int) _timespan.TotalHours == 0) {
                if ((int) _timespan.TotalMinutes == 1) {
                    return "Every minute";
                }
                return $"Every {(int) _timespan.TotalMinutes} minutes";
            }
            if ((int) _timespan.TotalDays == 0) {
                if ((int) _timespan.TotalHours == 1) {
                    return $"Every hour";
                }
                return $"Every {(int) _timespan.TotalHours} hours";
            }
            if ((int) _timespan.TotalDays == 1) {
                return "Every day";
            }
            return $"Every {(int) _timespan.TotalDays} days";
        }
    }
}