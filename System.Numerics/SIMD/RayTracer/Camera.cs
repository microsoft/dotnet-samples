using RayTracer.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace RayTracer
{
    /// <summary>
    /// The scene camera, contains all relevant rendering methods and algorithms.
    /// </summary>
    public class Camera : SceneObjectBase
    {
        private Vector3f forward, up, right;
        private Vector3f screenPosition;
        private float fieldOfView;
        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                fieldOfView = value;
                RecalculateFieldOfView();
            }
        }
        private float yRatio;

        public int ReflectionDepth { get; set; }
        private Size renderSize;
        public Size RenderSize { get { return renderSize; } set { renderSize = value; OnRenderSizeChanged(); } }

        private void OnRenderSizeChanged()
        {
            this.yRatio = (float)renderSize.Height / (float)renderSize.Width;
        }

        public Camera() : this(Vector3f.Zero, Util.ForwardVector, Util.UpVector, 70f, new Size(640, 480)) { }

        public Camera(Vector3f position, Vector3f forward, Vector3f worldUp, float fieldOfView, Size renderSize)
            : base(position)
        {
            this.ReflectionDepth = 5;
            this.forward = forward.Normalized();
            this.right = Util.CrossProduct(worldUp, forward).Normalized();
            this.up = -Util.CrossProduct(right, forward).Normalized();
            this.fieldOfView = fieldOfView;
            this.RenderSize = renderSize;

            RecalculateFieldOfView();
        }

        private void RecalculateFieldOfView()
        {
            var screenDistance = 1f / (float)Math.Tan(Util.DegreesToRadians(fieldOfView) / 2f);

            this.screenPosition = this.Position + forward * new Vector3f(screenDistance);
        }

        private Ray GetRay(float viewPortX, float viewPortY)
        {
            var rayWorldPosition = screenPosition + ((new Vector3f(viewPortX) * right) + (new Vector3f(viewPortY) * up * new Vector3f(yRatio)));
            var direction = rayWorldPosition - this.Position;
            return new Ray(rayWorldPosition, direction);
        }

        private Ray GetReflectionRay(Vector3f origin, Vector3f normal, Vector3f impactDirection)
        {
            float c1 = VectorMath.DotProduct(-normal, impactDirection);
            Vector3f reflectionDirection = impactDirection + (normal * new Vector3f(2 * c1));
            return new Ray(origin + reflectionDirection * new Vector3f(.01f), reflectionDirection); // Ensures the ray starts "just off" the reflected surface
        }

        private Ray GetRefractionRay(Vector3f origin, Vector3f normal, Vector3f previousDirection, float refractivity)
        {
            float c1 = VectorMath.DotProduct(normal, previousDirection);
            float c2 = 1 - refractivity * refractivity * (1 - c1 * c1);
            if (c2 < 0)
                c2 = (float)Math.Sqrt(c2);
            Vector3f refractionDirection = (normal * new Vector3f((refractivity * c1 - c2)) - previousDirection * new Vector3f(refractivity)) * new Vector3f(-1);
            return new Ray(origin, refractionDirection.Normalized()); // no refraction
        }

        /// <summary>
        /// Renders the given scene to a bitmap, using one thread per line of pixels in the image.
        /// </summary>
        /// <param name="scene">The scene to render</param>
        /// <returns>A bitmap of the rendered scene.</returns>
        public async Task<Bitmap> RenderSceneToBitmapThreaded(Scene scene, int width = -1, int height = -1)
        {
            if (width == -1 || height == -1)
            {
                width = renderSize.Width;
                height = renderSize.Height;
            }
            else
            {
                renderSize = new Size(width, height);
            }

            var before = DateTime.UtcNow;
            Bitmap bitmap = new Bitmap(width, height);

            List<Task> tasks = new List<Task>();

            Color[,] colors = new Color[width, height];

            for (int xCounter = 0; xCounter < width; xCounter++)
            {
                var x = xCounter;
                var task = Task.Run(() =>
                    {
                        for (int yCounter = 0; yCounter < height; yCounter++)
                        {
                            var y = yCounter;
                            var viewPortX = ((2 * x) / (float)width) - 1;
                            var viewPortY = ((2 * y) / (float)height) - 1;
                            var color = TraceRayAgainstScene(GetRay(viewPortX, viewPortY), scene);
                            colors[x, y] = color;
                        }
                    });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            // Copy all pixel data into bitmap.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bitmap.SetPixel(x, height - y - 1, colors[x, y]);
                }
            }

            var after = DateTime.UtcNow;
            System.Diagnostics.Debug.WriteLine("Total render time: " + (after - before).TotalMilliseconds + " ms");
            return bitmap;
        }

        /// <summary>
        /// Renders the given scene in a background thread. Uses a single thread for rendering.
        /// </summary>
        /// <param name="scene">The scene to render</param>
        /// <returns>A bitmap of the rendered scene.</returns>
        public async Task<Bitmap> RenderSceneToBitmap(Scene scene, int width = -1, int height = -1)
        {
            if (width == -1 || height == -1)
            {
                width = renderSize.Width;
                height = renderSize.Height;
            }
            else
            {
                renderSize = new Size(width, height);
            }

            var before = DateTime.UtcNow;

            Bitmap bitmap = new Bitmap(width, height);

            var task = Task.Run(() =>
                {

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            var viewPortX = ((2 * x) / (float)width) - 1;
                            var viewPortY = ((2 * y) / (float)height) - 1;
                            var color = TraceRayAgainstScene(GetRay(viewPortX, viewPortY), scene);
                            bitmap.SetPixel(x, height - y - 1, color);
                        }
                    }
                });

            await task;

            var after = DateTime.UtcNow;
            System.Diagnostics.Debug.WriteLine("Total render time: " + (after - before).TotalMilliseconds + " ms");
            return bitmap;
        }

        /// <summary>
        /// Renders the given scene, providing a callback for each individual line rendered. Each line is rendered on a separate thread.
        /// </summary>
        /// <param name="width">The width of the rendered image</param>
        /// <param name="width">The height of the rendered image</param>
        /// <param name="scene">The scene to render</param>
        /// <param name="callback">The delegate invoked when a thread completes rendering a line</param>
        public void RenderSceneLinesThreaded(Scene scene, int width, int height, LineFinishedHandler callback)
        {
            if (width == -1 || height == -1)
            {
                width = renderSize.Width;
                height = renderSize.Height;
            }
            else
            {
                renderSize = new Size(width, height);
            }

            var before = DateTime.UtcNow;
            List<Task> tasks = new List<Task>();

            for (int yCounter = height - 1; yCounter >= 0; yCounter--)
            {
                var y = yCounter;
                Color[] colors = new Color[width];
                Task.Run(() =>
                {
                    for (int xCounter = 0; xCounter < width; xCounter++)
                    {
                        var x = xCounter;
                        var viewPortX = ((2 * x) / (float)width) - 1;
                        var viewPortY = ((2 * y) / (float)height) - 1;
                        var color = TraceRayAgainstScene(GetRay(viewPortX, viewPortY), scene);
                        colors[x] = color;
                    }
                    callback(y, colors);
                });
            }

            var after = DateTime.UtcNow;
            System.Diagnostics.Debug.WriteLine("Total render time: " + (after - before).TotalMilliseconds + " ms");
        }

        /// <summary>
        /// Renders the given scene, providing a callback for each individual line rendered. Uses a single background thread.
        /// </summary>
        /// <param name="width">The width of the rendered image</param>
        /// <param name="height">The height of the rendered image</param>
        /// <param name="scene">The scene to render</param>
        /// <param name="callback">The delegate invoked when a thread completes rendering a line</param>
        public void RenderSceneLines(Scene scene, int width, int height, LineFinishedHandler callback)
        {
            if (width == -1 || height == -1)
            {
                width = renderSize.Width;
                height = renderSize.Height;
            }
            else
            {
                renderSize = new Size(width, height);
            }

            var before = DateTime.UtcNow;

            Task.Run(() => // Even single-threaded method should run in the background to avoid freezing UI.
                {
                    for (int y = height - 1; y >= 0; y--)
                    {
                        var colors = new Color[width];
                        for (int x = 0; x < width; x++)
                        {
                            var viewPortX = ((2 * x) / (float)width) - 1;
                            var viewPortY = ((2 * y) / (float)height) - 1;
                            var color = TraceRayAgainstScene(GetRay(viewPortX, viewPortY), scene);
                            colors[x] = color;
                        }
                        callback(y, colors);
                    }
                });
            var after = DateTime.UtcNow;
            System.Diagnostics.Debug.WriteLine("Total render time: " + (after - before).TotalMilliseconds + " ms");
        }

        private Color TraceRayAgainstScene(Ray ray, Scene scene)
        {
            Intersection intersection;
            if (TryCalculateIntersection(ray, scene, null, out intersection))
            {
                return CalculateRecursiveColor(intersection, scene, 0);
            }
            else
            {
                return scene.BackgroundColor;
            }
        }

        /// <summary>
        /// Recursive algorithm base
        /// </summary>
        /// <param name="intersection">The intersection the recursive step started from</param>
        /// <param name="ray">The ray, starting from the intersection</param>
        /// <param name="scene">The scene to trace</param>
        private Color CalculateRecursiveColor(Intersection intersection, Scene scene, int depth)
        {
            // Ambient light:
            var color = Color.Lerp(Color.Black, intersection.Color * scene.AmbientLightColor, scene.AmbientLightIntensity);

            foreach (Light light in scene.Lights)
            {
                var lightContribution = new Color();
                var towardsLight = (light.Position - intersection.Point).Normalized();
                var lightDistance = Util.Distance(intersection.Point, light.Position);

                // Accumulate diffuse lighting:
                var lightEffectiveness = VectorMath.DotProduct(towardsLight, intersection.Normal);
                if (lightEffectiveness > 0.0f)
                {
                    lightContribution = lightContribution + (intersection.Color * light.GetIntensityAtDistance(lightDistance) * light.Color * lightEffectiveness);
                }

                // Render shadow
                var shadowRay = new Ray(intersection.Point, towardsLight);
                Intersection shadowIntersection;
                if (TryCalculateIntersection(shadowRay, scene, intersection.ObjectHit, out shadowIntersection) && shadowIntersection.Distance < lightDistance)
                {
                    var transparency = shadowIntersection.ObjectHit.Material.Transparency;
                    var lightPassThrough = Util.Lerp(.25f, 1.0f, transparency);
                    lightContribution = Color.Lerp(lightContribution, Color.Zero, 1 - lightPassThrough);
                }

                color += lightContribution;
            }

            if (depth < ReflectionDepth)
            {
                // Reflection ray
                var objectReflectivity = intersection.ObjectHit.Material.Reflectivity;
                if (objectReflectivity > 0.0f)
                {
                    var reflectionRay = GetReflectionRay(intersection.Point, intersection.Normal, intersection.ImpactDirection);
                    Intersection reflectionIntersection;
                    if (TryCalculateIntersection(reflectionRay, scene, intersection.ObjectHit, out reflectionIntersection))
                    {
                        color = Color.Lerp(color, CalculateRecursiveColor(reflectionIntersection, scene, depth + 1), objectReflectivity);
                    }
                }

                // Refraction ray
                var objectRefractivity = intersection.ObjectHit.Material.Refractivity;
                if (objectRefractivity > 0.0f)
                {
                    var refractionRay = GetRefractionRay(intersection.Point, intersection.Normal, intersection.ImpactDirection, objectRefractivity);
                    Intersection refractionIntersection;
                    if (TryCalculateIntersection(refractionRay, scene, intersection.ObjectHit, out refractionIntersection))
                    {
                        var refractedColor = CalculateRecursiveColor(refractionIntersection, scene, depth + 1);
                        color = Color.Lerp(color, refractedColor, 1 - (intersection.ObjectHit.Material.Opacity));
                    }
                }
            }

            color = color.Limited;
            return color;
        }

        /// <summary>
        /// Determines whether a given ray intersects with any scene objects (other than excludedObject)
        /// </summary>
        /// <param name="ray">The ray to test</param>
        /// <param name="scene">The scene to test</param>
        /// <param name="excludedObject">An object that is not tested for intersections</param>
        /// <param name="intersection">If the intersection test succeeds, contains the closest intersection</param>
        /// <returns>A value indicating whether or not any scene object intersected with the ray</returns>
        private bool TryCalculateIntersection(Ray ray, Scene scene, DrawableSceneObject excludedObject, out Intersection intersection)
        {
            var closestDistance = float.PositiveInfinity;
            var closestIntersection = new Intersection();
            foreach (var sceneObject in scene.DrawableObjects)
            {
                Intersection i;
                if (sceneObject != excludedObject && sceneObject.TryCalculateIntersection(ray, out i))
                {
                    if (i.Distance < closestDistance)
                    {

                        closestDistance = i.Distance;
                        closestIntersection = i;
                    }
                }
            }

            if (closestDistance == float.PositiveInfinity)
            {
                intersection = new Intersection();
                return false;
            }
            else
            {
                intersection = closestIntersection;
                return true;
            }
        }
    }

    public delegate void LineFinishedHandler(int rowNumber, Color[] lineColors);
}
