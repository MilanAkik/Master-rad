using System.Drawing;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class TextureUtilities
    {
        public static RenderTexture CreateRenderTexture(int width, int height)
        {
            RenderTexture texture = new RenderTexture(width, height, 0) {
                enableRandomWrite = true,
                filterMode = FilterMode.Point
            };
            texture.Create();
            return texture;
        }

        public static RenderTexture CreateRenderTexture(int width, int height, int depth)
        {
            RenderTexture texture = new RenderTexture(width, height, 0)
            {
                enableRandomWrite = true,
                volumeDepth = depth,
                dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
                filterMode = FilterMode.Point
            };
            texture.Create();
            return texture;
        }
    }
}
