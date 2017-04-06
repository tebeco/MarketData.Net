using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MarketDataCommon.Infrastructure
{
    public class RandomSequenceGenerator
    {
        private readonly double _min;
        private readonly double _max;
        private readonly Random _random;
        private double _bias = 0.3;

        public RandomSequenceGenerator(double min, double max)
        {
            this._min = min;
            this._max = max;
            this._random = new Random();
        }

        public IObservable<Double> Create(TimeSpan interval)
        {
            double init = (_min + _max) / 2;
            return Observable
                .Interval(interval)
                .Aggregate(init, (previous, i) => ComputeNextNumber(previous));
        }

        public IObservable<int> CreateIntegerSequence(TimeSpan interval)
        {
            double range = _max - _min;
            return Observable
                .Interval(interval)
                .Select(i => (int)(_random.NextDouble() * range + _min));
        }

        public double ComputeNextNumber(double previous)
        {
            double range = (_max - _min) / 20;
            double scaled = (_random.NextDouble() - 0.5 + _bias) * range;
            double shifted = previous + scaled;
            if (shifted < _min || shifted > _max)
            {
                shifted = previous - scaled;
                _bias = -_bias;
            }

            shifted = (long)Math.Round(shifted * 10000) / 10000d;

            return shifted;
        }
    }
}
