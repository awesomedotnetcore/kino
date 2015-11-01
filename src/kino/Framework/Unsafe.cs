﻿namespace kino.Framework
{
    public static class Unsafe
    {
        public static int ComputeHash(this byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                var hash = (int) 2166136261;
                var length = data.Length;

                for (var i = 0; i < length; i++)
                {
                    hash = (hash ^ data[i]) * p;
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        public static unsafe bool Equals(byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length)
            {
                return false;
            }
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                var l = a1.Length;
                for (var i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                {
                    if (*((long*) x1) != *((long*) x2))
                    {
                        return false;
                    }
                }
                if ((l & 4) != 0)
                {
                    if (*((int*) x1) != *((int*) x2))
                    {
                        return false;
                    }
                    x1 += 4;
                    x2 += 4;
                }
                if ((l & 2) != 0)
                {
                    if (*((short*) x1) != *((short*) x2))
                    {
                        return false;
                    }
                    x1 += 2;
                    x2 += 2;
                }
                if ((l & 1) != 0)
                {
                    if (*((byte*) x1) != *((byte*) x2))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}