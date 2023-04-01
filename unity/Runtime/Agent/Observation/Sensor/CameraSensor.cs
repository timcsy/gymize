using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PAIA.Gymize
{
    public class CameraSensor : SensorComponent
    {
        public Camera Camera;
        public int Width = 1920;
        public int Height = 1080;
        public float Scale = 1;
        public COMPRESSION_TYPE CompressionType = COMPRESSION_TYPE.PNG;

        public override IData GetObservation(int cacheId = -1)
        {
            RenderTexture rt = new RenderTexture(Width, Height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            RenderTexture oldRT = Camera.targetTexture;
            Camera.targetTexture = rt;
            Camera.Render();
            Camera.targetTexture = oldRT;

            Texture2D tex;
            if ((int)Mathf.Floor(Scale) == 1)
            {
                tex = ToTexture2D(rt);
            }
            else
            {
                int outputWidth = (int)Mathf.Floor(Width * Scale);
                int outputHeight = (int)Mathf.Floor(Height * Scale);
                var outputTexture = new RenderTexture(outputWidth, outputHeight, 0);
                Graphics.Blit(rt, outputTexture);
                tex = ToTexture2D(outputTexture);
                DestroyImmediate(outputTexture, true);
            }
            byte[] image = ToImage(tex, CompressionType);

            DestroyImmediate(rt, true);
            DestroyImmediate(tex, true);

            return new Image(image, CompressionType);
        }

        private Texture2D ToTexture2D(RenderTexture rt)
        {
            // !!! Note: You have to delete the original RenderTexture rt !!!

            var oldRT = RenderTexture.active;

            var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            RenderTexture.active = oldRT;

            return tex;
        }
        private byte[] ToImage(Texture2D texture, COMPRESSION_TYPE format = COMPRESSION_TYPE.PNG)
        {
            // !!! Note: You have to delete the original Texture2D texture !!!

            byte[] buffer;
            if (format == COMPRESSION_TYPE.PNG)
            {
                buffer = texture.EncodeToPNG();
            }
            else if (format == COMPRESSION_TYPE.JPG)
            {
                buffer = texture.EncodeToJPG();
            }
            else
            {
                buffer = texture.GetRawTextureData();
            }
            
            return buffer;
        }
        
        private Texture2D FlipTexture(Texture2D tex)
        {
            // !!! Note: You have to delete the original Texture2D tex !!!

            // upside-down
            Texture2D snap = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
            Color[] pixels = tex.GetPixels();
            Color[] pixelsFlipped = new Color[pixels.Length];

            for (int i = 0; i < tex.height; i++)
            {
                Array.Copy(pixels, i * tex.width, pixelsFlipped, (tex.height - i - 1) * tex.width, tex.width);
            }

            snap.SetPixels(pixelsFlipped);
            snap.Apply();

            int identificador = GC.GetGeneration(pixels);
            pixels = null;
            GC.Collect(identificador, GCCollectionMode.Forced);
            identificador = GC.GetGeneration(pixelsFlipped);
            pixelsFlipped = null;
            GC.Collect(identificador, GCCollectionMode.Forced);

            return snap;
        }
        private Texture2D AutoFlipTexture(Texture2D texture)
        {
            // !!! Note: You have to delete the original Texture2D texture !!!

            // Check if the environment need to flip upside-down
            var flipY = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore ||
                        SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 ||
                        SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 ||
                        SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan ?
                        false: true;
            Texture2D tex;
            if (flipY) tex = FlipTexture(texture);
            else tex = texture;

            return tex;
        }
    }
}
