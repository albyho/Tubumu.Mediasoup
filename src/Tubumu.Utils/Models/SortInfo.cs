using System.Collections.Generic;
using System.Linq;

namespace Tubumu.Utils.Models
{
    /// <summary>
    /// 排序信息
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// 排序方向
        /// </summary>
        public SortDirection SortDir { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string? Sort { get; set; }
    }

    /// <summary>
    /// 排序信息扩展
    /// </summary>
    public static class SortInfoExtensions
    {
        /// <summary>
        /// 验证排序信息合法性
        /// </summary>
        public static bool IsValid(this SortInfo sortInfo)
        {
            return sortInfo != null && !string.IsNullOrWhiteSpace(sortInfo.Sort);
        }

        /// <summary>
        /// 验证排序信息合法性
        /// </summary>
        public static bool IsValid(this IEnumerable<SortInfo> sortInfos)
        {
            return sortInfos?.All(m => m.IsValid()) == true;
        }
    }

    /// <summary>
    /// 排序方向
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// 正序
        /// </summary>
        ASC,

        /// <summary>
        /// 倒序
        /// </summary>
        DESC
    }
}
