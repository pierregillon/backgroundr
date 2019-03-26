using System;

namespace backgroundr.domain
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Divide(this TimeSpan duration, int number)
        {
            if (number == 0) {
                throw new DivideByZeroException();
            }

            return TimeSpan.FromMilliseconds(duration.TotalMilliseconds / number);
        }
    }
}
