using System;
using System.Diagnostics;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace NPOI
{
    /// <summary>
    /// NPOI 扩展方法
    /// </summary>
    public static class NPOIExetnsion
    {
        /// <summary>
        /// 从某个单元格中复制数字
        /// </summary>
        public static void NumericValueCopy(this IRow row, string cellName, IRow sourceRow, string sourceCellName)
        {
            var numericValue = sourceRow.GetCellByName(sourceCellName).NumericCellValue;
            row.GetCellByName(cellName).SetCellValue(numericValue);
        }

        /// <summary>
        /// 从某个单元格中复制字符串
        /// </summary>
        public static void StringValueCopy(this IRow row, string cellName, IRow sourceRow, string sourceCellName)
        {
            var stringValue = sourceRow.GetCellByName(sourceCellName).StringCellValue;
            row.GetCellByName(cellName).SetCellValue(stringValue);
        }

        /// <summary>
        /// 从某个单元格中复制值
        /// </summary>
        public static void ValueCopy(this IRow row, string cellName, IRow sourceRow, string sourceCellName)
        {
            // 注意：1、只支持基于数字的公式 2、公式计算出数字值 3、空白当做数字读取
            var sourceCell = sourceRow.GetCellByName(sourceCellName);
            var targetCell = row.GetCellByName(cellName);

            switch(sourceCell.CellType)
            {
                case CellType.Numeric:
                    targetCell.SetCellValue(sourceCell.NumericCellValue);
                    break;

                case CellType.String:
                    targetCell.SetCellValue(sourceCell.StringCellValue);
                    break;

                case CellType.Formula:
                    targetCell.SetCellValue(sourceCell.NumericCellValue);
                    break;

                case CellType.Blank:
                    targetCell.SetCellValue(sourceCell.NumericCellValue);
                    break;

                default:
                    throw new NotSupportedException($"{sourceCellName}{sourceRow.RowNum + 1} 不是数字、字符串或基于数字的公式");
            }
        }

        /// <summary>
        /// 通过列名获取单元格的值
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="name">列名</param>
        public static ICell GetCellByName(this IRow row, string name)
        {
            return row.GetCell(FromNumberSystem26(name) - 1);
        }

        /// <summary>
        /// 将指定的自然数转换为26进制表示。映射关系：[1-26] ->[A-Z]。
        /// </summary>
        /// <param name="n">自然数（如果无效，则返回空字符串）。</param>
        /// <returns>26进制表示。</returns>
        public static string ToNumberSystem26(int n)
        {
            var s = string.Empty;
            while(n > 0)
            {
                int m = n % 26;
                if(m == 0)
                {
                    m = 26;
                }

                s = (char)(m + 64) + s;
                n = (n - m) / 26;
            }

            return s;
        }

        /// <summary>
        /// 将指定的26进制表示转换为自然数。映射关系：[A-Z] ->[1-26]。
        /// </summary>
        /// <param name="s">26进制表示（如果无效，则返回0）。</param>
        /// <returns>自然数。</returns>
        public static int FromNumberSystem26(string s)
        {
            if(string.IsNullOrEmpty(s))
            {
                return 0;
            }

            var n = 0;
            for(int i = s.Length - 1, j = 1; i >= 0; i--, j *= 26)
            {
                var c = char.ToUpper(s[i]);
                if(c is < 'A' or > 'Z')
                {
                    return 0;
                }

                n += (c - 64) * j;
            }

            return n;
        }

        /// <summary>
        /// 获取严格的单元格范围
        /// </summary>
        public static CellRangeAddress? GetStrictCellRangeAddress(this ISheet sheet, int rowNum, int colNum)
        {
            var regionsCount = sheet.NumMergedRegions;
            for(var i = 0; i < regionsCount; i++)
            {
                CellRangeAddress range = sheet.GetMergedRegion(i);
                //sheet.IsMergedRegion(range);
                if(range.FirstRow == rowNum && range.FirstColumn == colNum)
                {
                    //int rowSpan = range.LastRow - range.FirstRow + 1;
                    //int colSpan = range.LastColumn - range.FirstColumn + 1;
                    return range;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取单元格范围
        /// </summary>
        public static CellRangeAddress? GetCellRangeAddress(this ISheet sheet, int rowNum, int colNum)
        {
            var regionsCount = sheet.NumMergedRegions;
            for(var i = 0; i < regionsCount; i++)
            {
                CellRangeAddress range = sheet.GetMergedRegion(i);
                if(range.FirstRow >= rowNum && range.FirstColumn <= colNum)
                {
                    return range;
                }
            }

            return null;
        }

        /// <summary>
        /// 单位文件
        /// </summary>
        public static IWorkbook OpenExcelFile(string path)
        {
            IWorkbook book;
            try
            {
                book = new XSSFWorkbook(path);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"OpenExecelFile failure | {ex.Message}");
                var file = new FileStream(path, FileMode.Open, FileAccess.Read);
                book = new HSSFWorkbook(file);
            }

            return book;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        public static void SaveExcelFile(this IWorkbook wb, string outPath)
        {
            using var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            wb.Write(fs);
        }
    }
}
