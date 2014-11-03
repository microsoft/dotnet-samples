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

        public static readonly Vector3 RightVector = new Vector3(1, 0, 0);
        public static readonly Vector3 UpVector = new Vector3(0, 1, 0);
        public static readonly Vector3 ForwardVector = new Vector3(0, 0, 1);

        public static Vector3 CrossProduct(Vector3 left, Vector3 right)
        {
            return new Vector3(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }

        public static float Magnitude(this Vector3 v)
        {
            return (float)Math.Abs(Math.Sqrt(Vector3.Dot(v,v)));
        }

        public static Vector3 Normalized(this Vector3 v)
        {
            var mag = v.Magnitude();
            if (mag != 1)
            {
                return v / new Vector3(mag);
            }
            else
            {
                return v;
            }
        }

        public static float Distance(Vector3 first, Vector3 second)
        {
            return (first - second).Magnitude();
        }

        public static Vector3 Projection(Vector3 projectedVector, Vector3 directionVector)
        {
            var mag = Vector3.Dot(projectedVector, directionVector.Normalized());
            return directionVector * mag;
        }
    }
}
