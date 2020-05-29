using System;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public static class Mathf
    {
        #region Lerp
        /// <summary>
        /// Linearly interpolates between two points according to a clamped value. 
        /// <para>
        /// Interpolates between the points <paramref name="a"/> and <paramref name="b"/> by the between 0 and 1 clamped interpolant <paramref name="t"/>. <br></br>
        /// This is used to find a interpolated value partway along a line between two values.
        /// </para>
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="t"> Interpolation value. </param>
        /// <returns> Point between <paramref name="a"/> and <paramref name="b"/> determined by interpolant <paramref name="t"/></returns>
        public static float ClampedLerp(float a, float b, float t)
        {
            t = Clamp(t, 0, 1);
            return (1.0f - t) * a + b * t;
        }

        /// <summary>
        /// Linearly interpolates between two points. 
        /// <para>
        /// Interpolates between the points <paramref name="a"/> and <paramref name="b"/> by the interpolant <paramref name="t"/>. <br></br>
        /// This is used to find a interpolated value partway along a line between two values.
        /// </para>
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="t"> Interpolation value </param>
        /// <returns> Point between <paramref name="a"/> and <paramref name="b"/> determined by interpolant <paramref name="t"/></returns>
        public static float Lerp(float a, float b, float t)
        {
            return ( 1.0f - t ) * a + b * t;
        }

        /// <summary>
        /// Linearly interpolates between two points. 
        /// <para>
        /// Interpolates between the points <paramref name="a"/> and <paramref name="b"/> by the interpolant <paramref name="t"/>. <br></br>
        /// This is used to find a interpolated value partway along a line between two values.
        /// </para>
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="t"> Interpolation value </param>
        /// <returns> Point between <paramref name="a"/> and <paramref name="b"/> determined by interpolant <paramref name="t"/></returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            var vector = Vector2.Zero();
            vector.X = Convert.ToInt32(( 1.0f - t ) * a.X + b.X * t);
            vector.Y = Convert.ToInt32(( 1.0f - t ) * a.Y + b.Y * t);
            return vector;
        }

        /// <summary>
        /// Linearly interpolates between two points. 
        /// <para>
        /// Interpolates between the points <paramref name="a"/> and <paramref name="b"/> by the interpolant <paramref name="t"/>. <br></br>
        /// This is used to find a interpolated value partway along a line between two values.
        /// </para>
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="t"> Interpolation value </param>
        /// <returns> Point between <paramref name="a"/> and <paramref name="b"/> determined by interpolant <paramref name="t"/></returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            var vector = Vector3.Zero();
            vector.X = Convert.ToInt32(( 1.0f - t ) * a.X + b.X * t);
            vector.Y = Convert.ToInt32(( 1.0f - t ) * a.Y + b.Y * t);
            vector.Z = Convert.ToInt32(( 1.0f - t ) * a.Z + b.Z * t);
            return vector;
        }

        /// <summary>
        /// Linearly interpolates between two points. 
        /// <para>
        /// Interpolates between the points <paramref name="a"/> and <paramref name="b"/> by the interpolant <paramref name="t"/>. <br></br>
        /// This is used to find a interpolated value partway along a line between two values.
        /// </para>
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="t"> Interpolation value </param>
        /// <returns> Point between <paramref name="a"/> and <paramref name="b"/> determined by interpolant <paramref name="t"/></returns>
        public static int Lerp(int a, int b, float t)
        {
            return Convert.ToInt32(( 1.0f - t ) * a + b * t);
        }

        /// <summary>
        /// Linearly interpolates between two points. 
        /// <para>
        /// Interpolates between the points <paramref name="a"/> and <paramref name="b"/> by the interpolant <paramref name="t"/>. <br></br>
        /// This is used to find a interpolated value partway along a line between two values.
        /// </para>
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="t"> Interpolation value </param>
        /// <returns> Point between <paramref name="a"/> and <paramref name="b"/> determined by interpolant <paramref name="t"/></returns>
        public static short Lerp(short a, short b, float t)
        {
            return Convert.ToInt16(( 1.0f - t ) * a + b * t);
        }
        #endregion

        #region Inverse Lerp
        /// <summary>
        /// Calculates the linear parameter t that produces the interpolant value <paramref name="v"/> within the range <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="v"> Interpolated value. </param>
        /// <returns> <see cref="float"/> Percentage of value between start and end. </returns>
        public static float InvLerp(float a, float b, float v)
        {
            return (v - a) / (b / a);
        }

        /// <summary>
        /// Calculates the linear parameter t that produces the interpolant value <paramref name="v"/> within the range <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="v"> Interpolated value. </param>
        /// <returns> <see cref="float"/> Percentage of value between start and end. </returns>
        public static float InvLerp(int a, int b, float v)
        {
            return (v - a) / (b / a);
        }

        /// <summary>
        /// Calculates the linear parameter t that produces the interpolant value <paramref name="v"/> within the range <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"> First Endpoint. </param>
        /// <param name="b"> Second Endpoint. </param>
        /// <param name="v"> Interpolated value. </param>
        /// <returns> <see cref="float"/> Percentage of value between start and end. </returns>
        public static float InvLerp(short a, short b, float v)
        {
            return (v - a) / (b / a);
        }
        #endregion

        #region Remap
        /// <summary>
        /// Remaps the first set of values (<paramref name="iMin"/>,<paramref name="iMax"/>) 
        /// to the second set of values (<paramref name="oMin"/>,<paramref name="oMax"/>) by inverse lerping the first set using <paramref name="v"/>  <br></br>
        /// and using outcome <paramref name="t"/> to calculate the second set.
        /// <para>( e.g. iMin( 1 ) iMax( 2 ) v( 1.5 ) = t( 0.5f ) => Lerp( oMin( 15 ), oMax( 30 ), t( 0.5f ) = 22.5f )</para>
        /// </summary>
        /// <param name="iMin"> First Endpoint of the first value set. </param>
        /// <param name="iMax"> Second Endpoint of the first value set. </param>
        /// <param name="oMin"> First Endpoint of the second value set. </param>
        /// <param name="oMax"> Second Endpoint of the second value set. </param>
        /// <param name="v"> Interpolated value. </param>
        /// <returns> Interpolated value from second value set interpolated by value according to first value set. </returns>
        public static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
        {
            var t = InvLerp( iMin, iMax, v );
            return Lerp( oMin, oMax, t );
        }
        #endregion

        #region util
        /// <summary>
        /// Clamps <paramref name="value"/> within a <paramref name="min"/> and <paramref name="max"/> value.
        /// </summary>
        /// <param name="value"> Value to be clamped. </param>
        /// <param name="min"> Clamp floor. </param>
        /// <param name="max"> Clamp Ceiling. </param>
        /// <returns> Clamped value. </returns>
        private static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
        #endregion
    }
}
