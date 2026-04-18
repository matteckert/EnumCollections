namespace EnumCollections;

internal static class EnumCollectionsExtensions
{
    public static ulong CountSetBits(this ulong v)
    {
        v = v - (v >> 1 & 0x5555555555555555UL);
        v = (v & 0x3333333333333333UL) + (v >> 2 & 0x3333333333333333UL);
        return (v + (v >> 4) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL >> 56;
    }
}