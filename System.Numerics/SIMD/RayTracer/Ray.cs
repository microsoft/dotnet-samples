using System.Numerics;

namespace RayTracer
{
    /// <summary>
    /// Represents a ray primitive. Used as a basis for intersection calculations.
    /// </summary>
    public struct Ray
    {
        public readonly Vector3f Origin;
        public readonly Vector3f Direction;
        public readonly float Distance;
        public Ray(Vector3f start, Vector3f direction, float distance)
        {
            this.Origin = start;
            this.Direction = direction.Normalized();
            this.Distance = distance;
        }
        public Ray(Vector3f start, Vector3f direction) : this(start, direction, float.PositiveInfinity) { }
    }
}
