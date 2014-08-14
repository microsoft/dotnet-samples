using RayTracer.Materials;
using System.Numerics;

namespace RayTracer.Objects
{
    /// <summary>
    /// A two-dimensional circular plane object. Limited in radius rather than extending infinitely.
    /// </summary>
    public class Disc : Plane
    {
        private float radius;

        public Disc(Vector3f centerPosition, Material material, Vector3f normalDirection, float radius, float cellWidth)
            : base(centerPosition, material, normalDirection, cellWidth)
        {
            this.radius = radius;
        }

        public override bool TryCalculateIntersection(Ray ray, out Intersection intersection)
        {
            if (base.TryCalculateIntersection(ray, out intersection) && WithinArea(intersection.Point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool WithinArea(Vector3f location)
        {
            var distanceFromCenter = (this.Position - location).Magnitude();
            return distanceFromCenter <= radius;
        }
    }
}
