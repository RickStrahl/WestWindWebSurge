using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSurge
{
    public static class PercentileExtensions
    {
        public static decimal Percentile(this IEnumerable<decimal> sequence, decimal percentile)
        {
            var orderedSequence = sequence.OrderBy(x => x).ToList();
            var count = orderedSequence.Count;
            var rank = (count - 1) * percentile + 1;

            if (rank == 1)
            {
                return orderedSequence[0];
            }

            if (rank == count)
            {
                return orderedSequence[count - 1];
            }

            var integerPortion = (int)rank;
            var fractionalPortion = rank - integerPortion;
            return orderedSequence[integerPortion - 1] + fractionalPortion * (orderedSequence[integerPortion] - orderedSequence[integerPortion - 1]);
        }

        public static decimal Percentile<TSource>(this IEnumerable<TSource> sequence, Func<TSource, decimal> selector, decimal percentile)
        {
            return sequence.Select(selector).Percentile(percentile);
        }
    }
}