using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Extension
{
    public static class DateTimeEx
    {
        public static bool IsDateBetween(this DateTime dt, DateTime startDate, DateTime endDate)
        {
            return dt >= startDate && dt <= endDate;
        }

        public static DateTime KszbDateFrom(this DateTime date)
        {
            return date.AddDays(-374);
        }
        public static DateTime ClientDateFrom(this DateTime date)
        {
            return date.AddDays(-364);
        }
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static int MonthDiff(this DateTime startDate, DateTime endDate)
        {
            var sDate = FirstDayOfMonth(startDate);
            var eDate = LastDayOfMonth(endDate).AddDays(1);
            var diff = (eDate.Month - sDate.Month) + (12 * (eDate.Year - sDate.Year));
            return diff;
        }
    }
}
