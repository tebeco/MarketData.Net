using System;
using System.Linq;
using System.Reactive.Linq;

namespace MarketData.Common.Infrastructure
{
    public class RandomSequenceGenerator
    {
        private readonly double _min;
        private readonly double _max;
        private readonly Random _random;
        private double _bias = 0.3;

        public RandomSequenceGenerator(double min, double max)
        {
            _min = min;
            _max = max;
            _random = new Random();
        }

        public IObservable<Double> Create(TimeSpan interval)
        {
            double init = (_min + _max) / 2;
            return Observable
                .Interval(interval)
                .Scan(init, (previous, i) => ComputeNextNumber(previous));
        }

        public IObservable<int> CreateIntegerSequence(TimeSpan interval)
        {
            return Observable
                .Interval(interval)
                .Select(i => _random.Next((int)_min, (int)_max));
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
