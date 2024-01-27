using System.IO;

namespace System
{
    /// <summary>
    /// Path 扩展方法
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// 替换文件后缀。支持文件名或路径。
        /// </summary>
        public static string ReplaceFileExtension(this string path, string newExtension)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"{nameof(path)} is null or white space.");
            }

            var extension = Path.GetExtension(path);
            if(extension?.Length == 0)
            {
                throw new Exception("No extension");
            }

            var newPath = $"{path[..^extension!.Length]}{newExtension}";
            return newPath;
        }
    }
}
