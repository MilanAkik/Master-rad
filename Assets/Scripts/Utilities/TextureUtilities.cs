using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class TextureUtilities
    {
        public static RenderTexture CreateRenderTexture(int width, int height)
        {
            RenderTexture texture = new RenderTexture(width, height, 0) { enableRandomWrite = true, filterMode = FilterMode.Point };
            texture.Create();
            return texture;
        }
    }
}
