using System;

namespace OpenCog.Utility
{
    public class NumberUtil
    {
        /**
         * Keep a value between the range of [0, 1]
         */
        public static double zeroOneCut(double f)
        {
            if (f > 1.0)
            {
                return 1.0;
            }
            else if (f < 0.0)
            {
                return 0.0;
            }
            return f;
        }
    }
}
