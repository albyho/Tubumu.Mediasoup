namespace Tubumu.Mediasoup
{
    /// <summary>
    /// Class for matching bit patterns such as "x1xx0000" where 'x' is allowed to be
    /// either 0 or 1.
    /// </summary>
    public class BitPattern
    {
        private readonly int _mask;
        private readonly int _maskedValue;

        public BitPattern(string str)
        {
            _mask = ~ByteMaskString('x', str);
            _maskedValue = ByteMaskString('1', str);
        }

        public bool IsMatch(int value)
        {
            return _maskedValue == (value & _mask);
        }

        private int ByteMaskString(char c, string str)
        {
            return (
                ((str[0] == c) ? 1 << 7 : 0) |
                ((str[1] == c) ? 1 << 6 : 0) |
                ((str[2] == c) ? 1 << 5 : 0) |
                ((str[3] == c) ? 1 << 4 : 0) |
                ((str[4] == c) ? 1 << 3 : 0) |
                ((str[5] == c) ? 1 << 2 : 0) |
                ((str[6] == c) ? 1 << 1 : 0) |
                ((str[7] == c) ? 1 << 0 : 0)
            );
        }
    }
}
