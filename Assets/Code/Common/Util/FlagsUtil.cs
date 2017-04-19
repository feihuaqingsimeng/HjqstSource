using System;
namespace Common.Util
{
    public class FlagsUtil
    {
        public static bool IsFlag(int flags, int value)
        {
            return (flags & value) == value;
        }
        public static bool IsFlag(long flags, long value)
        {
            return (flags & value) == value;
        }
        public static bool IsFlag(Enum flags, Enum value)
        {
            return IsFlag(Convert.ToInt64(flags), Convert.ToInt64(value));
        }
    }
}