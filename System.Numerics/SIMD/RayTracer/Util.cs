using System;
using System.Numerics;

namespace RayTracer
{
    /// <summary>
    /// Contains various mathematic helper methods for scalars and vectors
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Clamps the given value between min and max
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            return value > max ? max : value < min ? min : value;
        }

        /// <summary>
        /// Linearly interpolates between two values, based on t
        /// </summary>
        public static float Lerp(float from, float to, float t)
        {
            return (from * (1 - t)) + (to * t);
        }

        /// <summary>
        /// Returns the maximum of the given set of values
        /// </summary>
        public static float Max(params float[] values)
        {
            float max = values[0];
            for (int g = 1; g < values.Length; g++)
            {
                if (values[g] > max)
                {
                    max = values[g];
                }
            }
            return max;
        }

        /// <summary>
        /// Converts an angle from degrees to radians.
        /// </summary>
        internal static float DegreesToRadians(float angleInDegrees)
        {
            var radians = (float)((angleInDegrees / 360f) * 2 * Math.PI);
            return radians;
        }

        public static readonly Vector3f RightVector = new Vector3f(1, 0, 0);
        public static readonly Vector3f UpVector = new Vector3f(0, 1, 0);
        public static readonly Vector3f ForwardVector = new Vector3f(0, 0, 1);

        public static Vector3f CrossProduct(Vector3f left, Vector3f right)
        {
            return new Vector3f(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }

        public static float Magnitude(this Vector3f v)
        {
            return (float)Math.Abs(Math.Sqrt(VectorMath.DotProduct(v,v)));
        }

        public static Vector3f Normalized(this Vector3f v)
        {
            var mag = v.Magnitude();
            if (mag != 1)
            {
                return v / new Vector3f(mag);
            }
            else
            {
                return v;
            }
        }

        public static float Distance(Vector3f first, Vector3f second)
        {
            return (first - second).Magnitude();
        }

        public static Vector3f Projection(Vector3f projectedVector, Vector3f directionVector)
        {
            var mag = VectorMath.DotProduct(projectedVector, directionVector.Normalized());
            return directionVector * mag;
        }
    }
}
