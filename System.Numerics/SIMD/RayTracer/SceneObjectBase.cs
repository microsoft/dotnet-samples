using System.Numerics;

namespace RayTracer
{
    /// <summary>
    /// The base class for all scene objects, which must contain a world-space position in the scene.
    /// </summary>
    public abstract class SceneObjectBase
    {
        /// <summary>
        /// The world-space position of the scene object
        /// </summary>
        public Vector3f Position { get; set; }
        public SceneObjectBase(Vector3f position)
        {
            this.Position = position;
        }
    }
}
