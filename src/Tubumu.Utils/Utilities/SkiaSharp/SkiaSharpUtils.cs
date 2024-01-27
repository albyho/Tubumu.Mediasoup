using System.Collections.Generic;
using SkiaSharp;

namespace System.IO
{
    /// <summary>
    /// SkiaSharp 扩展方法
    /// <see cref="https://github.com/dresdf/PicturesASP/blob/ecd168dace09e5185446107e95f51da08fcefb84/PicturesASP/Utils/ImageProcessor.cs"/>
    /// </summary>
    public static class SkiaSharpUtils
    {
        private const int Quality = 75;

        private const int WidthMax = 800;

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputWidth"></param>
        public static void SaveImage(Stream imageStream, string outputFilePath, int? outputWidth = null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);
            var format = GetFormat(outputFilePath);

            using var inputStream = new SKManagedStream(imageStream);
            using var codec = SKCodec.Create(inputStream);
            using var original = SKBitmap.Decode(codec);
            using var image = HandleOrientation(original, codec.EncodedOrigin);
            var width = outputWidth ?? WidthMax;
            width = width > image.Width ? image.Width : width;
            var height = (int)Math.Round(width * ((float)image.Height / image.Width));

            var info = new SKImageInfo(width, height);
            using var resized = image.Resize(info, SKFilterQuality.Medium);
            using var thumb = SKImage.FromBitmap(resized);
            using var fs = new FileStream(outputFilePath, FileMode.CreateNew, FileAccess.ReadWrite);
            thumb.Encode(format, Quality)
                 .SaveTo(fs);
        }

        /// <summary>
        /// 创建系列缩略图
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <param name="widths"></param>
        /// <returns></returns>
        public static string[] CreateThumbnails(Stream imageStream, string folder, string fileName, IEnumerable<int> widths)
        {
            Directory.CreateDirectory(folder);
            var displayName = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            var format = GetFormat(fileName);

            var result = new List<string>();
            using(var inputStream = new SKManagedStream(imageStream))
            using(var codec = SKCodec.Create(inputStream))
            using(var original = SKBitmap.Decode(codec))
            using(var image = HandleOrientation(original, codec.EncodedOrigin))
            {
                foreach(var width in widths)
                {
                    var height = (int)Math.Round(width * ((float)image.Height / image.Width));

                    var thumbnailPath = Path.Combine(folder, $"{displayName}-{width}x{height}{ext}");
                    result.Add(thumbnailPath);
                    var info = new SKImageInfo(width, height);

                    var sKBitmap = image.Resize(info, SKFilterQuality.Medium);
                    using var resized = sKBitmap;
                    using var thumb = SKImage.FromBitmap(resized);
                    using var fs = new FileStream(thumbnailPath, FileMode.CreateNew, FileAccess.ReadWrite);
                    thumb.Encode(format, Quality)
                         .SaveTo(fs);
                }
            }

            return result.ToArray();
        }

        private static SKBitmap HandleOrientation(SKBitmap bitmap, SKEncodedOrigin orientation)
        {
            SKBitmap rotated;
            switch(orientation)
            {
                case SKEncodedOrigin.BottomRight:

                    using(var surface = new SKCanvas(bitmap))
                    {
                        surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        surface.DrawBitmap(bitmap.Copy(), 0, 0);
                    }

                    return bitmap;

                case SKEncodedOrigin.RightTop:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                    using(var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(rotated.Width, 0);
                        surface.RotateDegrees(90);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }

                    return rotated;

                case SKEncodedOrigin.LeftBottom:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                    using(var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(0, rotated.Height);
                        surface.RotateDegrees(270);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }

                    return rotated;
                case SKEncodedOrigin.TopLeft:
                    break;
                case SKEncodedOrigin.TopRight:
                    break;
                case SKEncodedOrigin.BottomLeft:
                    break;
                case SKEncodedOrigin.LeftTop:
                    break;
                case SKEncodedOrigin.RightBottom:
                    break;
            }

            return bitmap;
        }

        private static SKEncodedImageFormat GetFormat(string fileName)
        {
            string ext = Path.GetExtension(fileName.ToLowerInvariant());

            switch(ext)
            {
                case ".gif":
                    return SKEncodedImageFormat.Gif;

                case ".png":
                    return SKEncodedImageFormat.Png;

                case ".webp":
                    return SKEncodedImageFormat.Webp;

                case ".bmp":
                    return SKEncodedImageFormat.Bmp;

                case ".jpeg":
                    return SKEncodedImageFormat.Jpeg;
                default:
                    break;
            }

            return SKEncodedImageFormat.Jpeg;
        }
    }
}
