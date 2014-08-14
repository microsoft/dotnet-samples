using RayTracer.Materials;
using System;
using System.Numerics;

namespace RayTracer.Objects
{
    /// <summary>
    /// A plane is, conceptually, a sheet that extends infinitely in all directions.
    /// </summary>
    public class Plane : DrawableSceneObject
    {
        private Vector3f normalDirection;
        protected Vector3f uDirection;
        protected Vector3f vDirection;
        private float cellWidth;

        /// <summary>
        /// Constructs a plane with the properties provided
        /// </summary>
        /// <param name="position">The position of the plane's center</param>
        /// <param name="material">The plane's material</param>
        /// <param name="normalDirection">The normal direction of the plane</param>
        /// <param name="cellWidth">The width of a cell in the plane, used for texture coordinate mapping.</param>
        public Plane(Vector3f position, Material material, Vector3f normalDirection, float cellWidth)
            : base(position, material)
        {
            this.normalDirection = normalDirection.Normalized();
            if (normalDirection == Util.ForwardVector)
            {
                this.uDirection = -Util.RightVector;
            }
            else if (normalDirection == -Util.ForwardVector)
            {
                this.uDirection = Util.RightVector;
            }
            else
            {
                this.uDirection = Util.CrossProduct(normalDirection, Util.ForwardVector).Normalized();
            }

            this.vDirection = -Util.CrossProduct(normalDirection, uDirection).Normalized();
            this.cellWidth = cellWidth;
        }

        public override bool TryCalculateIntersection(Ray ray, out Intersection intersection)
        {
            intersection = new Intersection();

            Vector3f vecDirection = ray.Direction;
            Vector3f rayToPlaneDirection = ray.Origin - this.Position;

            float D = VectorMath.DotProduct(this.normalDirection, vecDirection);
            float N = -VectorMath.DotProduct(this.normalDirection, rayToPlaneDirection);

            if (Math.Abs(D) <= .0005f)
            {
                return false;
            }

            float sI = N / D;
            if (sI < 0 || sI > ray.Distance) // Behind or out of range
            {
                return false;
            }

            var intersectionPoint = ray.Origin + (new Vector3f(sI) * vecDirection);
            var uv = this.GetUVCoordinate(intersectionPoint);

            var color = Material.GetDiffuseColorAtCoordinates(uv);

            intersection = new Intersection(intersectionPoint, this.normalDirection, ray.Direction, this, color, (ray.Origin - intersectionPoint).Magnitude());
            return true;
        }

        public override UVCoordinate GetUVCoordinate(Vector3f position)
        {
            var uvPosition = this.Position + position;

            var uMag = VectorMath.DotProduct(uvPosition, uDirection);
            var u = (new Vector3f(uMag) * uDirection).Magnitude();
            if (uMag < 0)
            {
                u += cellWidth / 2f;
            }
            u = (u % cellWidth) / cellWidth;

            var vMag = VectorMath.DotProduct(uvPosition, vDirection);
            var v = (new Vector3f(vMag) * vDirection).Magnitude();
            if (vMag < 0)
            {
                v += cellWidth / 2f;
            }
            v = (v % cellWidth) / cellWidth;

            return new UVCoordinate(u, v);
        }
    }
}
