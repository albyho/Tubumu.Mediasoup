﻿using System;

namespace Tubumu.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime SpecifyKindLocal(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        public static DateTime SpecifyKindUtc(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static DateTime SpecifyKindUnspecified(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }

        public static DateTime EnsureLocal(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime();
            }
            return dateTime;
        }
    }
}
