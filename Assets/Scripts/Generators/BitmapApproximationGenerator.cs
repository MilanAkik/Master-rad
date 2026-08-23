using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generators
{
    [CreateAssetMenu(menuName = "CylinderGenerator/Bitmap Approximation")]
    public class BitmapApproximationGenerator : CylinderGenerator
    {
        private const int WorkingResolution = 64;
        private const float MinimumRadiusInPixels = 0.5f;

        [Tooltip("Binary silhouette to approximate. It is sampled as a 64 x 64 mask.")]
        public Texture2D bitmap;

        [Tooltip("Pixels at or above this brightness are treated as white.")]
        [Range(0f, 1f)]
        public float threshold = 0.5f;

        [Tooltip("When enabled, white pixels form the silhouette. Disable for a black-on-white bitmap.")]
        public bool whitePixelsAreForeground = true;

        public override Matrix4x4[] getCylinderMatrices(GeneratorParameters parameters)
        {
            if (bitmap == null || parameters == null || parameters.CylinderCount <= 0)
            {
                return Array.Empty<Matrix4x4>();
            }

            bool[] foreground = ReadForegroundMask();
            List<Pixel> foregroundPixels = GetForegroundPixels(foreground);
            if (foregroundPixels.Count == 0)
            {
                return Array.Empty<Matrix4x4>();
            }

            List<Pixel> backgroundPixels = GetBackgroundPixels(foreground);
            List<CircleCandidate> candidates = BuildCandidates(foregroundPixels, backgroundPixels);
            List<CircleCandidate> selected = SelectGreedyCover(candidates, foregroundPixels.Count, parameters.CylinderCount);
            var res = CreateMatrices(selected, parameters);
            return res;
        }

        private bool[] ReadForegroundMask()
        {
            Color32[] pixels = ReadPixelsAtWorkingResolution();
            var result = new bool[pixels.Length];
            float byteThreshold = threshold * 255f;

            for (int i = 0; i < pixels.Length; i++)
            {
                Color32 color = pixels[i];
                float brightness = (color.r + color.g + color.b) / 3f;
                bool isWhite = brightness >= byteThreshold;
                result[i] = isWhite == whitePixelsAreForeground;
            }

            return result;
        }

        private Color32[] ReadPixelsAtWorkingResolution()
        {
            RenderTexture previous = RenderTexture.active;
            FilterMode previousFilterMode = bitmap.filterMode;
            RenderTexture renderTexture = RenderTexture.GetTemporary( WorkingResolution, WorkingResolution, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            renderTexture.filterMode = FilterMode.Point;
            var readableTexture = new Texture2D(WorkingResolution, WorkingResolution, TextureFormat.RGBA32, false);

            try
            {
                bitmap.filterMode = FilterMode.Point;
                Graphics.Blit(bitmap, renderTexture);
                RenderTexture.active = renderTexture;
                readableTexture.ReadPixels(new Rect(0, 0, WorkingResolution, WorkingResolution), 0, 0);
                readableTexture.Apply();
                return readableTexture.GetPixels32();
            }
            finally
            {
                bitmap.filterMode = previousFilterMode;
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(renderTexture);
                if (Application.isPlaying)
                {
                    Destroy(readableTexture);
                }
                else
                {
                    DestroyImmediate(readableTexture);
                }
            }
        }

        private static List<Pixel> GetForegroundPixels(bool[] foreground)
        {
            var pixels = new List<Pixel>();
            for (int index = 0; index < foreground.Length; index++)
            {
                if (foreground[index])
                {
                    pixels.Add(new Pixel(index % WorkingResolution, index / WorkingResolution));
                }
            }

            return pixels;
        }

        private static List<Pixel> GetBackgroundPixels(bool[] foreground)
        {
            var pixels = new List<Pixel>();
            for (int index = 0; index < foreground.Length; index++)
            {
                if (!foreground[index])
                {
                    pixels.Add(new Pixel(index % WorkingResolution, index / WorkingResolution));
                }
            }

            return pixels;
        }

        private static List<CircleCandidate> BuildCandidates(List<Pixel> foregroundPixels, List<Pixel> backgroundPixels)
        {
            var candidates = new List<CircleCandidate>(foregroundPixels.Count);
            foreach (Pixel center in foregroundPixels)
            {
                float radius = GetInscribedRadius(center, backgroundPixels);
                float radiusSquared = radius * radius;
                var coveredPixels = new List<int>();

                for (int i = 0; i < foregroundPixels.Count; i++)
                {
                    Pixel pixel = foregroundPixels[i];
                    float dx = pixel.x - center.x;
                    float dz = pixel.z - center.z;
                    if (dx * dx + dz * dz <= radiusSquared + 0.0001f)
                    {
                        coveredPixels.Add(i);
                    }
                }

                candidates.Add(new CircleCandidate(center, radius, coveredPixels.ToArray()));
            }

            return candidates;
        }

        private static float GetInscribedRadius(Pixel center, List<Pixel> backgroundPixels)
        {
            // The edge of the image acts as background. Pixel centers are half a pixel from it.
            float radius = Mathf.Min(
                Mathf.Min(center.x + 0.5f, WorkingResolution - center.x - 0.5f),
                Mathf.Min(center.z + 0.5f, WorkingResolution - center.z - 0.5f));

            foreach (Pixel background in backgroundPixels)
            {
                float dx = background.x - center.x;
                float dz = background.z - center.z;
                float distanceToPixelEdge = Mathf.Sqrt(dx * dx + dz * dz) - 0.5f;
                if (distanceToPixelEdge < radius)
                {
                    radius = distanceToPixelEdge;
                }
            }

            return Mathf.Max(MinimumRadiusInPixels, radius);
        }

        private static List<CircleCandidate> SelectGreedyCover(
            List<CircleCandidate> candidates, int pixelCount, int maximumCircleCount)
        {
            var covered = new bool[pixelCount];
            var selected = new List<CircleCandidate>(Mathf.Min(maximumCircleCount, pixelCount));
            int coveredCount = 0;

            while (selected.Count < maximumCircleCount && coveredCount < pixelCount)
            {
                CircleCandidate best;
                int bestGain;

                // Cached gains are upper bounds because pixels only change from uncovered to
                // covered. Recalculate the current best until its bound is exact. This keeps
                // the greedy set-cover result while avoiding a full circle recount every pass.
                while (true)
                {
                    best = null;
                    foreach (CircleCandidate candidate in candidates)
                    {
                        if (!candidate.selected &&
                            (best == null || candidate.remainingGain > best.remainingGain ||
                             (candidate.remainingGain == best.remainingGain && candidate.radius > best.radius)))
                        {
                            best = candidate;
                        }
                    }

                    if (best == null || best.remainingGain == 0)
                    {
                        break;
                    }

                    bestGain = 0;
                    foreach (int pixelIndex in best.coveredPixelIndices)
                    {
                        if (!covered[pixelIndex])
                        {
                            bestGain++;
                        }
                    }

                    if (bestGain == best.remainingGain)
                    {
                        break;
                    }

                    best.remainingGain = bestGain;
                }

                if (best == null || best.remainingGain == 0)
                {
                    break;
                }

                best.selected = true;
                selected.Add(best);
                foreach (int pixelIndex in best.coveredPixelIndices)
                {
                    if (!covered[pixelIndex])
                    {
                        covered[pixelIndex] = true;
                        coveredCount++;
                    }
                }
            }

            return selected;
        }

        private static Matrix4x4[] CreateMatrices(List<CircleCandidate> circles, GeneratorParameters parameters)
        {
            var matrices = new Matrix4x4[circles.Count];
            Vector3 areaSize = parameters.AreaMax - parameters.AreaMin;
            float radiusAreaScale = Mathf.Min(Mathf.Abs(areaSize.x), Mathf.Abs(areaSize.z));
            float y = Mathf.Lerp(parameters.AreaMin.y, parameters.AreaMax.y, 0.5f);
            float height = parameters.HeightMultiplier;

            for (int i = 0; i < circles.Count; i++)
            {
                CircleCandidate circle = circles[i];
                float normalizedX = (circle.center.x + 0.5f) / WorkingResolution;
                float normalizedZ = (circle.center.z + 0.5f) / WorkingResolution;
                float normalizedRadius = circle.radius / WorkingResolution;
                float x = Mathf.Lerp(parameters.AreaMin.x, parameters.AreaMax.x, normalizedX);
                float z = Mathf.Lerp(parameters.AreaMin.z, parameters.AreaMax.z, normalizedZ);
                float radius = normalizedRadius * radiusAreaScale * parameters.RadiusMultiplier;

                matrices[i] = new Matrix4x4(
                    new Vector4(x, y, z, radius),
                    new Vector4(height, 0f, 0f, 0f),
                    Vector4.zero,
                    Vector4.zero);
            }

            return matrices;
        }

        private struct Pixel
        {
            public readonly int x;
            public readonly int z;

            public Pixel(int x, int z)
            {
                this.x = x;
                this.z = z;
            }
        }

        private sealed class CircleCandidate
        {
            public readonly Pixel center;
            public readonly float radius;
            public readonly int[] coveredPixelIndices;
            public bool selected;
            public int remainingGain;

            public CircleCandidate(Pixel center, float radius, int[] coveredPixelIndices)
            {
                this.center = center;
                this.radius = radius;
                this.coveredPixelIndices = coveredPixelIndices;
                remainingGain = coveredPixelIndices.Length;
            }
        }
    }
}
