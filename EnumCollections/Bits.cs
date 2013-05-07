
namespace EnumCollections
{
    internal static class Bits
    {
        public static int Count(ulong v)
        {
            unchecked
            {
                v = v - (v >> 1 & 0x5555555555555555UL);
                v = (v & 0x3333333333333333UL) + (v >> 2 & 0x3333333333333333UL);
                return (int)((v + (v >> 4) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL >> 56);
            }
        }

        public static int TrailingZeroes(ulong x)
        {
            unchecked
            {
                var n = 1;
                var y = (int)x;
                if (y == 0)
                {
                    n += 32;
                    y = (int)(x >> 32);
                }
                if ((y & 0x0000FFFF) == 0)
                {
                    n += 16;
                    y >>= 16;
                }
                if ((y & 0x000000FF) == 0)
                {
                    n += 8;
                    y >>= 8;
                }
                if ((y & 0x0000000F) == 0)
                {
                    n += 4;
                    y >>= 4;
                }
                if ((y & 0x00000003) == 0)
                {
                    n += 2;
                    y >>= 2;
                }
                return n - (y & 1);
            }
        }
    }
}
