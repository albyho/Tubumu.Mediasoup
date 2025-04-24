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
        Year,
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
        ///
        public static long DateDiff(this DateTime endDate, DateInterval interval, DateTime startDate)
        {
            var timeSpan = new TimeSpan(endDate.Ticks - startDate.Ticks);

            return interval switch
            {
                DateInterval.Second => (long)timeSpan.TotalSeconds,
                DateInterval.Minute => (long)timeSpan.TotalMinutes,
                DateInterval.Hour => (long)timeSpan.TotalHours,
                DateInterval.Day => (long)timeSpan.Days,
                DateInterval.Week => (long)(timeSpan.Days / 7),
                DateInterval.Month => (long)(timeSpan.Days / 30),
                DateInterval.Quarter => (long)(timeSpan.Days / 30 / 3),
                DateInterval.Year => (long)(timeSpan.Days / 365),
                _ => throw new NotImplementedException(interval.ToString()),
            };
        }

        /// <summary>
        /// 计算结束时间与开始时间的间隔
        /// </summary>
        /// <remarks>该方法主要是为了与 SQL 中计算时间间隔的 DateDiff 方法提供一致的调用</remarks>
        /// <param name="interval">时间间隔类型</param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">结束时间</param>
        ///
        public static long DateDiff(DateInterval interval, DateTime startDate, DateTime endDate)
        {
            return endDate.DateDiff(interval, startDate);
        }
    }
}
