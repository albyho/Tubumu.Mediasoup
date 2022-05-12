using Microsoft.International.Converters.PinYinConverter;

namespace System
{
    public static class PinYinExtensions
	{
        /// <summary>
        /// ConvertToPinYin
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertToPinYin(this string source)
        {
            var pinYin = "";
            foreach (var item in source)
            {
                if (ChineseChar.IsValidChar(item))
                {
                    var cc = new ChineseChar(item);
                    //PYstr += string.Join("", cc.Pinyins.ToArray());
                    pinYin += cc.Pinyins[0][0..^1].ToLowerInvariant();
                    //PYstr += cc.Pinyins[0].Substring(0, cc.Pinyins[0].Length - 1).Substring(0, 1).ToLower();
                }
                else
                {
                    pinYin += item.ToString();
                }
            }
            return pinYin;
        }

        /// <summary>
        /// ConvertToPinYinPY
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Tuple<string, string> ConvertToPinYinPY(this string source)
        {
            var pinYin = "";
            var py = "";
            foreach (char item in source)
            {
                if (ChineseChar.IsValidChar(item))
                {
                    var cc = new ChineseChar(item);
                    var pinYinString = cc.Pinyins[0][0..^1].ToLowerInvariant();
                    pinYin += pinYinString;
                    py += pinYinString[..1];
                }
                else
                {
                    var charString = item.ToString();
                    pinYin += charString;
                    py += charString;
                }
            }
            return new Tuple<string, string>(pinYin, py);
        }

        /// <summary>
        /// ConvertToPY
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertToPY(this string source)
        {
            string py = "";
            foreach (char item in source)
            {
                if (ChineseChar.IsValidChar(item))
                {
                    var cc = new ChineseChar(item);
                    py += cc.Pinyins[0][..1].ToLowerInvariant();
                }
                else
                {
                    var charString = item.ToString();
                    py += charString;
                }
            }
            return py;
        }
    }
}

