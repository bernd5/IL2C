using System;
using System.Runtime.CompilerServices;

namespace IL2C.ILConverters
{
    [TestCase(12, "Int32_Int32", 288, 24)]
    [TestCase(12, "Int32_IntPtr", 288, 24)]
    [TestCase(12L, "Int64_Int64", 288L, 24L)]
    [TestCase(12, "IntPtr_Int32", 288, 24)]
    [TestCase(12, "IntPtr_IntPtr", 288, 24)]
    [TestCase(123.0f / 0.45f, "Single_Single", 123.0f, 0.45f)]
    [TestCase(123.0f / 0.45, "Single_Double", 123.0f, 0.45)]
    [TestCase(123.0 / 0.45f, "Double_Single", 123.0, 0.45f)]
    [TestCase(123.0 / 0.45, "Double_Double", 123.0, 0.45)]
    public sealed class Div
    {
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int Int32_Int32(int lhs, int rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern IntPtr Int32_IntPtr(int lhs, IntPtr rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern long Int64_Int64(long lhs, long rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern IntPtr IntPtr_Int32(IntPtr lhs, int rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern IntPtr IntPtr_IntPtr(IntPtr lhs, IntPtr rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern float Single_Single(float lhs, float rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern double Single_Double(float lhs, double rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern double Double_Single(double lhs, float rhs);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern double Double_Double(double lhs, double rhs);
    }
}
