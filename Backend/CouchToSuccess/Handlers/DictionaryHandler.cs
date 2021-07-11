using System;

namespace CouchToSuccess.Handlers
{
    public class DateTimeHandler
    {
        public static double GetEpochOfDateTime(DateTime dateTime)
        {
            TimeSpan timeSpan = (dateTime - new DateTime(1970, 1, 1));
            return (double) timeSpan.TotalSeconds;
        }

        public static DateTime GetDateTimeFromEpoch(double epoch)
        {
            DateTime resultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            resultEpoch.AddSeconds(epoch);
            return resultEpoch;
        }

        public static int GetZeroIfNull(int? number)
        {
            return number != null ? (int)number : 0;
        }
    }
}