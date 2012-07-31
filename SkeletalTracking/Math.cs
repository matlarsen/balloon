using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balloon.Engine {
    public static class _Math {

        /// <summary>
        /// Return the median value from an array. 
        /// Taken from http://stackoverflow.com/questions/4140719/i-need-c-sharp-function-that-will-calculate-median
        /// </summary>
        /// <param name="sourceNumbers"></param>
        /// <returns></returns>
        public static double GetMedian(double[] sourceNumbers) {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                return 0D;

            //make sure the list is sorted, but use a new array
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            sourceNumbers.CopyTo(sortedPNumbers, 0);
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }
    }
}
