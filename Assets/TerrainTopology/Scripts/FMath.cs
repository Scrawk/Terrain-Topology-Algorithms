using System;
using System.Runtime.CompilerServices;

namespace TerrainTopology
{
    public class FMath
    {
        public const float EPS = 1e-18f;

        public const float PI = (float)Math.PI;

        public const float SQRT2 = 1.414213562373095f;

        public const float Rad2Deg = 180.0f / PI;

        public const float Deg2Rad = PI / 180.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeAcos(float r)
        {
            return (float)Math.Acos(Math.Min(1.0f, Math.Max(-1.0f, r)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeAsin(float r)
        {
            return (float)Math.Asin(Math.Min(1.0f, Math.Max(-1.0f, r)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeSqrt(float v)
        {
            if (v <= 0.0f) return 0.0f;
            return (float)Math.Sqrt(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeLog(float v)
        {
            if (v <= 0.0f) return 0.0f;
            return (float)Math.Log(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeLog10(float v)
        {
            if (v <= 0.0) return 0.0f;
            return (float)Math.Log10(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeInvSqrt(float n, float d, float eps = EPS)
        {
            if (d <= 0.0f) return 0.0f;
            d = (float)Math.Sqrt(d);
            if (d < eps) return 0.0f;
            return n / d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeInv(float v, float eps = EPS)
        {
            if (Math.Abs(v) < eps) return 0.0f;
            return 1.0f / v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SafeDiv(float n, float d, float eps = EPS)
        {
            if (Math.Abs(d) < eps) return 0.0f;
            return n / d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignOrZero(float v)
        {
            if (v == 0) return 0;
            return Math.Sign(v);
        }

    }
}