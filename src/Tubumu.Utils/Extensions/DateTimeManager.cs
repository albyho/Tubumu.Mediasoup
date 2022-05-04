namespace System
{
    /// <summary>
    /// 时间间隔类型
    /// </summary>
    public enum DateInterval
    {
        /// <summary>
        /// Second
        /// </summary>
        Second,

        /// <summary>
        /// Minute
        /// </summary>
        Minute,

        /// <summary>
        /// Hour
        /// </summary>
        Hour,

        /// <summary>
        /// Day
        /// </summary>
        Day,

        /// <summary>
        /// Week
        /// </summary>
        Week,

        /// <summary>
        /// Month
        /// </summary>
        Month,

        /// <summary>
        /// Quarter
        /// </summary>
        Quarter,

        /// <summary>
        /// Year
        /// </summary>
        Year
    }

    /// <summary>
    /// DateTimeManger
    /// </summary>
    public static class DateTimeManger
    {
        /// <summary>
        /// 计算结束时间与开始时间的间隔
        /// </summary>
        /// <param name="endDate">结束时间</param>
        /// <param name="interval">时间间隔类型</param>
        /// <param name="startDate">起始时间</param>
        /// <returns></returns>
        public static long DateDiff(this DateTime endDate, DateInterval interval, DateTime startDate)
        {
            long lngDateDiffValue;
            var timeSpan = new TimeSpan(endDate.Ticks - startDate.Ticks);
            switch (interval)
            {
                case DateInterval.Second:
                    lngDateDiffValue = (long)timeSpan.TotalSeconds;
                    break;

                case DateInterval.Minute:
                    lngDateDiffValue = (long)timeSpan.TotalMinutes;
                    break;

                case DateInterval.Hour:
                    lngDateDiffValue = (long)timeSpan.TotalHours;
                    break;

                case DateInterval.Day:
                    lngDateDiffValue = (long)timeSpan.Days;
                    break;

                case DateInterval.Week:
                    lngDateDiffValue = (long)(timeSpan.Days / 7);
                    break;

                case DateInterval.Month:
                    lngDateDiffValue = (long)(timeSpan.Days / 30);
                    break;

                case DateInterval.Quarter:
                    lngDateDiffValue = (long)(timeSpan.Days / 30 / 3);
                    break;

                case DateInterval.Year:
                    lngDateDiffValue = (long)(timeSpan.Days / 365);
                    break;

                default:
                    throw new NotImplementedException(interval.ToString());
            }
            return lngDateDiffValue;
        }

        /// <summary>
        /// 计算结束时间与开始时间的间隔
        /// </summary>
        /// <remarks>该方法主要是为了与 SQL 中计算时间间隔的 DateDiff 方法提供一致的调用</remarks>
        /// <param name="interval">时间间隔类型</param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public static long DateDiff(DateInterval interval, DateTime startDate, DateTime endDate)
        {
            return endDate.DateDiff(interval, startDate);
        }
    }
}
