using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FluentAssertions;
using MarketDataCommon.Infrastructure;
using Xunit;

namespace MarketDataCommon.Tests.Infrastructure
{
    public class RandomSequenceGeneratorTest
    {
        [Fact]
        public void Should_generate_a_bounded_number()
        {
            //Given
            double min = 3.14;
            double max = 5.42;
            RandomSequenceGenerator generator = new RandomSequenceGenerator(min, max);
            
            //When
            double next = generator.ComputeNextNumber(4);

            //Then
            next.Should().BeGreaterOrEqualTo(min).And.BeLessOrEqualTo(max);
        }

        [Fact]
        public void Should_generate_a_bounded_number_close_to_previous()
        {
            //Given
            double min = 3;
            double max = 5;
            RandomSequenceGenerator generator = new RandomSequenceGenerator(min, max);
            
            //When
            int previous = 4;
            double next = generator.ComputeNextNumber(previous);
            
            //Then
            next.Should().BeGreaterOrEqualTo(previous - (max - min) / 10).And.BeLessOrEqualTo(previous + (max - min) / 10);
        }

        [Fact]
        public void Should_generate_a_number_with_a_limited_number_of_digits()
        {
            //Given
            double min = 3;
            double max = 5;
            RandomSequenceGenerator generator = new RandomSequenceGenerator(min, max);
            
            //When
            int previous = 4;
            Double next = generator.ComputeNextNumber(previous);
            
            //Then
            next.ToString(CultureInfo.InvariantCulture).Length.Should().BeLessThan(7);
        }
    }
}
