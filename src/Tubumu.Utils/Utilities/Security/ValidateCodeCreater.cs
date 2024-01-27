using System;
using System.IO;
using SkiaSharp;

namespace Tubumu.Utils.Utilities.Security
{
    /// <summary>
    /// 生成验证码的类
    /// </summary>
    public class ValidationCodeCreater
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="codeLength"></param>
        /// <param name="validationCode"></param>
        public ValidationCodeCreater(int codeLength, out string? validationCode)
        {
            if(codeLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(codeLength));
            }

            validationCode = CreateValidationCode(codeLength);
        }

        /// <summary>
        /// CreateValidationCode
        /// </summary>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        public static string? CreateValidationCode(int codeLength)
        {
            const string chars = "1234567890qwertyuipasdfghjklzxcvbnm";
            var rand = new Random(Guid.NewGuid().GetHashCode());

            string? result = null;
            for(int i = 0; i < codeLength; i++)
            {
                var r = rand.Next(chars.Length);

                result = string.Concat(result, chars[r]);
            }

            return result;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        public static byte[] CreateValidationCodeGraphic(string validationCode)
        {
            if(validationCode.IsNullOrWhiteSpace())
            {
                throw new Exception($"验证码参数 {nameof(validationCode)} 为空。");
            }

            var rand = new Random(Guid.NewGuid().GetHashCode());

            const int randAngle = 40;
            var mapWidth = validationCode.Length * 18;
            const int mapHeight = 28;

            using var bitmap = new SKBitmap(mapWidth, mapHeight);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.AliceBlue);

            var paint = new SKPaint() { Color = SKColors.LightGray, };
            for(int i = 0; i < 50; i++)
            {
                int x = rand.Next(0, bitmap.Width);
                int y = rand.Next(0, bitmap.Height);

                canvas.DrawRect(new SKRect(x, y, x + 1, y + 1), paint);
            }

            var chars = validationCode.ToCharArray();
            var colors = new[] { SKColors.Black, SKColors.Red, SKColors.DarkBlue, SKColors.Green, SKColors.Orange, SKColors.Brown, SKColors.DarkCyan, SKColors.Purple };
            var fonts = new[]
            {
                        SKTypeface.FromFamilyName("Verdana"),
                        SKTypeface.FromFamilyName("Microsoft Sans Serif"),
                        SKTypeface.FromFamilyName("Comic Sans MS"),
                        SKTypeface.FromFamilyName("Arial")
                    };

            canvas.Translate(-4, 0);

            for(var i = 0; i < chars.Length; i++)
            {
                var colorIndex = rand.Next(colors.Length);
                var fontIndex = rand.Next(fonts.Length);

                var fontColor = colors[colorIndex];
                var foneSize = rand.Next(18, 25);
                float angle = rand.Next(-randAngle, randAngle);

                var point = new SKPoint(16, (28 / 2) + 4);

                canvas.Translate(point);
                canvas.RotateDegrees(angle);

                var textPaint = new SKPaint()
                {
                    TextAlign = SKTextAlign.Center,
                    Color = fontColor,
                    TextSize = foneSize,
                    Typeface = fonts[fontIndex],

                    //IsAntialias = rand.Next(1) == 1 ? true : false,
                    //FakeBoldText = true,
                    //FilterQuality = SKFilterQuality.High,
                    //HintingLevel = SKPaintHinting.Full,

                    //IsEmbeddedBitmapText = true,
                    //LcdRenderText = true,
                    //Style = SKPaintStyle.StrokeAndFill,
                    //TextEncoding = SKTextEncoding.Utf8,
                };

                canvas.DrawText(chars[i].ToString(), new SKPoint(0, 0), textPaint);
                canvas.RotateDegrees(-angle);
                canvas.Translate(0, -point.Y);
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var ms = new MemoryStream();
            image.Encode(SKEncodedImageFormat.Png, 90).SaveTo(ms);
            return ms.ToArray();
        }
    }
}
