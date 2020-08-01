using System;

namespace Frugal.Handlers
{
    public class NullHandler
    {
        public static double GetZeroIfNull(double? number)
        {
            return number != null ? (double)number : 0.00;
        }
    }
}