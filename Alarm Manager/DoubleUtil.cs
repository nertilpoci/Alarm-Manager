using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CustomRenderingSample
{
    /// <summary>
    /// Utility to provide double calculations.
    /// </summary>
    public static class DoubleUtil
    {
        // Const values come from sdk\inc\crt\float.h
        private const double DoubleEpsilon = 2.2204460492503131e-016; /* smallest such that 1.0+DoubleEpsilon != 1.0 */

        /// <summary> 
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication. 
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be 
        /// used for optimizations *only*.
        /// </summary> 
        /// <returns> 
        /// bool - the result of the AreClose comparision.
        /// </returns> 
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (value1 == value2) return true;
            // ReSharper restore CompareOfFloatsByEqualityOperator
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon 
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DoubleEpsilon;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary> 
        /// Compares two Size instances for fuzzy equality.  This function 
        /// helps compensate for the fact that double values can
        /// acquire error when operated upon 
        /// </summary>
        /// <param name='size1'>The first size to compare</param>
        /// <param name='size2'>The second size to compare</param>
        /// <returns>Whether or not the two Size instances are equal</returns> 
        public static bool AreClose(Size size1, Size size2)
        {
            return AreClose(size1.Width, size2.Width) &&
                   AreClose(size1.Height, size2.Height);
        }

        // The Point, Size, Rect and Matrix class have moved to WinCorLib.  However, we provide 
        // internal AreClose methods for our own use here.

        /// <summary>
        /// Compares two points for fuzzy equality.  This function
        /// helps compensate for the fact that double values can
        /// acquire error when operated upon 
        /// </summary>
        /// <param name='point1'>The first point to compare</param> 
        /// <param name='point2'>The second point to compare</param> 
        /// <returns>Whether or not the two points are equal</returns>
        public static bool AreClose(Point point1, Point point2)
        {
            return AreClose(point1.X, point2.X)
                && AreClose(point1.Y, point2.Y);
        }

        /// <summary>
        /// Compares two Vector instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary> 
        /// <param name='vector1'>The first Vector to compare</param> 
        /// <param name='vector2'>The second Vector to compare</param>
        /// <returns>Whether or not the two Vector instances are equal</returns> 
        public static bool AreClose(Vector vector1, Vector vector2)
        {
            return AreClose(vector1.X, vector2.X)
                && AreClose(vector1.Y, vector2.Y);
        }

        /// <summary> 
        /// LessThan - Returns whether or not the first double is less than the second double.
        /// That is, whether or not the first is strictly less than *and* not within epsilon of 
        /// the other number.  Note that this epsilon is proportional to the numbers themselves 
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which 
        /// are theoretically identical, so no code calling this should fail to work if this
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*. 
        /// </summary>
        /// <returns> 
        /// bool - the result of the LessThan comparision. 
        /// </returns>
        /// <param name="value1"> The first double to compare. </param> 
        /// <param name="value2"> The second double to compare. </param>
        public static bool LessThan(double value1, double value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }

        /// <summary>
        /// GreaterThan - Returns whether or not the first double is greater than the second double. 
        /// That is, whether or not the first is strictly greater than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which 
        /// are theoretically identical, so no code calling this should fail to work if this
        /// returns false.  This is important enough to repeat: 
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be 
        /// used for optimizations *only*.
        /// </summary> 
        /// <returns>
        /// bool - the result of the GreaterThan comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param> 
        /// <param name="value2"> The second double to compare. </param>
        public static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }

        /// <summary>
        /// LessThanOrClose - Returns whether or not the first double is less than or close to
        /// the second double.  That is, whether or not the first is strictly less than or within 
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers
        /// themselves to that AreClose survives scalar multiplication.  Note, 
        /// There are plenty of ways for this to return false even for numbers which 
        /// are theoretically identical, so no code calling this should fail to work if this
        /// returns false.  This is important enough to repeat: 
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns> 
        /// bool - the result of the LessThanOrClose comparision.
        /// </returns> 
        /// <param name="value1"> The first double to compare. </param> 
        /// <param name="value2"> The second double to compare. </param>
        public static bool LessThanOrClose(double value1, double value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// GreaterThanOrClose - Returns whether or not the first double is greater than or close to 
        /// the second double.  That is, whether or not the first is strictly greater than or within 
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers
        /// themselves to that AreClose survives scalar multiplication.  Note, 
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be 
        /// used for optimizations *only*.
        /// </summary> 
        /// <returns> 
        /// bool - the result of the GreaterThanOrClose comparision.
        /// </returns> 
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1), 
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision. 
        /// </returns>
        /// <param name="value"> The double to compare to 1. </param> 
        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * DoubleEpsilon;
        }

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0), 
        /// but this is faster.
        /// </summary> 
        /// <returns> 
        /// bool - the result of the AreClose comparision.
        /// </returns> 
        /// <param name="value"> The double to compare to 0. </param>
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DoubleEpsilon;
        }

        /// <summary>
        /// Test to see if a double is a finite number (is not NaN or Infinity).
        /// </summary>
        /// <param name='value'>
        /// The value to test.
        /// </param> 
        /// <returns>
        /// Whether or not the value is a finite number.
        /// </returns>
        public static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// Test to see if a double a valid size value (is finite and > 0). 
        /// </summary>
        /// <param name='value'>
        /// The value to test.
        /// </param> 
        /// <returns>
        /// Whether or not the value is a valid size value.
        /// </returns> 
        public static bool IsValidSize(double value)
        {
            return IsFinite(value) && GreaterThanOrClose(value, 0);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal readonly UInt64 UintValue;
        }

        /// <summary>
        /// Checks whether the double value is not a valid number or not. The standard CLR double.IsNaN() 
        /// function is approximately 100 times slower than this, so please make sure to use DoubleUtil.IsNaN()
        /// in performance sensitive code.
        /// </summary>
        /// <param name="value">
        /// The double value to check for.
        /// </param>
        /// <returns>
        /// True if <paramref name="value"/> is not a number. Otherwise true.
        /// </returns>
        public static bool IsNaN(double value)
        {
            NanUnion t = new NanUnion
            {
                DoubleValue = value
            };

            UInt64 exp = t.UintValue & 0xfff0000000000000;
            UInt64 man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }
    }
}
