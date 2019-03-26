using System;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class PseudoRandom : IRandom
    {
        private readonly Random _random = new Random((int) DateTime.Now.Ticks);

        public int RandomInteger(int maxValue)
        {
            return _random.Next(maxValue);
        }
    }
}