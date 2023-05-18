using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Google.Protobuf;
using Gymize.Protobuf;

namespace Gymize
{
    public enum CompressionType
    {
        UNSPECIFIED,
        NONE,
        PNG,
        JPG
    }

    public class Image : IInstance
    {
        byte[] m_Image;
        CompressionType m_CompressionType;
        string m_DType;
        List<int> m_Shape;
        List<int> m_TransposeAxes;

        public Image()
        {
            m_Image = null;
            m_CompressionType = CompressionType.UNSPECIFIED;
            m_DType = "|u1";
            m_Shape = null;
            m_TransposeAxes = null;
        }
        public Image(byte[] image, CompressionType compressionType)
        {
            m_Image = image;
            m_CompressionType = compressionType;
            m_DType = "|u1";
            m_Shape = null;
            m_TransposeAxes = null;
        }
        public Image(byte[] image, CompressionType compressionType, string dtype, List<int> shape, List<int> transposeAxes)
        {
            m_Image = image;
            m_CompressionType = compressionType;
            m_DType = dtype;
            m_Shape = shape;
            m_TransposeAxes = transposeAxes;
        }

        public InstanceProto ToProtobuf()
        {
            ImageProto image = new ImageProto();
            switch (m_CompressionType)
            {
                case CompressionType.NONE:
                    image.CompressionType = CompressionTypeProto.None;
                    throw new NotSupportedException();
                case CompressionType.PNG:
                    image.CompressionType = CompressionTypeProto.Png;
                    break;
                case CompressionType.JPG:
                    image.CompressionType = CompressionTypeProto.Jpg;
                    break;
                default:
                    image.CompressionType = CompressionTypeProto.Unspecified;
                    throw new NotSupportedException();
            }
            image.Data = ByteString.CopyFrom(m_Image);
            image.Dtype = (m_DType == "uint8") ? "|u1" : m_DType;
            if (m_Shape != null) image.Shape.AddRange(m_Shape);
            if (m_TransposeAxes != null) image.TransposeAxes.AddRange(m_TransposeAxes);
            
            InstanceProto instance = new InstanceProto
            {
                Type = InstanceTypeProto.Image,
                Image = image
            };
            return instance;
        }

        public static Texture2D ToTexture2D(RenderTexture rt, bool GrayScale = false, bool ReleaseRenderTexture = true)
        {
            // !!! Note: This function will release (delete) the original RenderTexture rt if ReleaseRenderTexture is set to true !!!

            var oldRT = RenderTexture.active;
            RenderTexture.active = rt;

            // TextureFormat.R8 is for grayscale, TextureFormat.ARGB32 is for RGB
            TextureFormat format = GrayScale? TextureFormat.R8 : TextureFormat.ARGB32;
            Texture2D tex = new Texture2D(rt.width, rt.height, format, false);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();

            // Restore the previous active RenderTexture
            RenderTexture.active = oldRT;

            // Release the unused render texture
            if (ReleaseRenderTexture)
            {
                if (RenderTexture.active == rt) RenderTexture.active = null;
                rt.Release();
            }

            return tex;
        }

        public static byte[] ToImage(Texture2D texture, CompressionType format = CompressionType.PNG, bool ReleaseTexture = true)
        {
            // !!! Note: This function will release (delete) the original Texture2D texture if ReleaseTexture is set to true !!!

            byte[] buffer;
            if (format == CompressionType.PNG)
            {
                buffer = texture.EncodeToPNG();
            }
            else if (format == CompressionType.JPG)
            {
                buffer = texture.EncodeToJPG();
            }
            else
            {
                throw new NotSupportedException();
            }

            if (ReleaseTexture) UnityEngine.Object.DestroyImmediate(texture, true);

            return buffer;
        }

        public static RenderTexture Scale(RenderTexture rt, float scaleX = 1f, float scaleY = 1f, int scaledWidth = -1, int scaledHeight = -1)
        {
            // Using the power of GPU

            if (scaledWidth == -1) scaledWidth = (int)Mathf.Floor(rt.width * scaleX);
            if (scaledHeight == -1) scaledHeight = (int)Mathf.Floor(rt.height * scaleY);
            
            if (scaledWidth == rt.width && scaledHeight == rt.height) return rt;

            // Create a temporary RenderTexture
            var temp = RenderTexture.GetTemporary(rt.descriptor);
            Graphics.Blit(rt, temp);

            rt.Release();
            rt.width = scaledWidth;
            rt.height = scaledHeight;

            Graphics.Blit(temp, rt);

            // Destroy the temporary RenderTexture
            RenderTexture.ReleaseTemporary(temp);
            return rt;
        }
        public static RenderTexture ToGrayScale(RenderTexture rt)
        {
            // Using the power of GPU

            // Create a temporary RenderTexture
            var temp = RenderTexture.GetTemporary(rt.descriptor);
            Graphics.Blit(rt, temp);
            // Create a Material with Shader
            Material material = new Material(Shader.Find("Custom/Grayscale"));
            // Set the input texture
            material.SetTexture("_MainTex", temp);
            // Render to original RenderTexture
            Graphics.Blit(null, rt, material);
            // Destroy the temporary RenderTexture
            RenderTexture.ReleaseTemporary(temp);
            return rt;
        }
        public static RenderTexture FlipVertical(RenderTexture rt)
        {
            // Using the power of GPU

            // Create a temporary RenderTexture
            var temp = RenderTexture.GetTemporary(rt.descriptor);
            // Flip Vertical, render to temporary RenderTexture
            Graphics.Blit(rt, temp, new Vector2(1, -1), new Vector2(0, 1));
            // Render to original RenderTexture
            Graphics.Blit(temp, rt);
            // Destroy the temporary RenderTexture
            RenderTexture.ReleaseTemporary(temp);
            return rt;
        }
        public static RenderTexture AutoFlipVertical(RenderTexture rt)
        {
            // This is useful when you want to capture the screen!

            // Check if the environment need to flip upside-down
            var flipY = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore ||
                        SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 ||
                        SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 ||
                        SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan ?
                        false: true;
            if (flipY) return FlipVertical(rt);
            else return rt;
        }
    }
}