using System;

namespace backgroundr.view.windows.parameters
{
    public class RefreshPeriod
    {
        public TimeSpan Value { get; }

        public RefreshPeriod(TimeSpan timespan)
        {
            Value = timespan;
        }

        public override string ToString()
        {
            if ((int) Value.TotalMinutes == 0) {
                return $"Every {(int) Value.TotalSeconds} seconds";
            }
            if ((int) Value.TotalHours == 0) {
                if ((int) Value.TotalMinutes == 1) {
                    return "Every minute";
                }
                return $"Every {(int) Value.TotalMinutes} minutes";
            }
            if ((int) Value.TotalDays == 0) {
                if ((int) Value.TotalHours == 1) {
                    return $"Every hour";
                }
                return $"Every {(int) Value.TotalHours} hours";
            }
            if ((int) Value.TotalDays == 1) {
                return "Every day";
            }
            return $"Every {(int) Value.TotalDays} days";
        }
    }
}