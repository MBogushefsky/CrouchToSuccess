﻿using System;

namespace Frugal.Handlers
{
    public class NullHandler
    {
        public static double GetZeroIfNull(double? number)
        {
            return number != null ? (double)number : 0.00;
        }

        public static int GetZeroIfNull(int? number)
        {
            return number != null ? (int)number : 0;
        }
    }
}