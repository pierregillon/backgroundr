using System;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class DefaultClock : IClock
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}