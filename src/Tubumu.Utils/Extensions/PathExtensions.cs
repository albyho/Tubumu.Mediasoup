using System;
using System.IO;

namespace Kavana.Utils.Extensions
{
    public static class PathExtensions
    {
        public static string ReplaceFileExtension(this string path, string newExtension)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"{nameof(path)} is null or white space.");
            }

            var extension = Path.GetExtension(path);
            if (extension == string.Empty)
            {
                throw new Exception("No extension");
            }

            var newPath = $"{path[..^extension.Length]}{newExtension}";
            return newPath;
        }
    }
}

