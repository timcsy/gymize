using System.Collections.Generic;
using UnityEngine;

namespace Gymize
{
    public class CameraSensor : SensorComponent
    {
        public Camera Camera;
        public int Width = 1920;
        public int Height = 1080;
        public float Scale = 1;
        public CompressionType CompressionType = CompressionType.PNG;
        public string Dtype = "uint8";
        public bool GrayScale = false;
        public int HeightAxis = 0;
        public int WidthAxis = 1;
        public int ChannelAxis = 2;

        public override IInstance GetObservation()
        {
            RenderTexture rt = new RenderTexture(Width, Height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            RenderTexture oldRT = Camera.targetTexture;
            Camera.targetTexture = rt;
            Camera.Render();
            Camera.targetTexture = oldRT;
            
            rt = Image.Scale(rt, Scale, Scale);
            if (GrayScale) rt = Image.ToGrayScale(rt);

            Texture2D tex = Image.ToTexture2D(rt, GrayScale);

            List<int> shape;
            List<int> transposeAxes;
            if (GrayScale) shape = new List<int>{ tex.height, tex.width };
            else shape = new List<int>{ tex.height, tex.width, 3 };
            if (GrayScale) transposeAxes = new List<int>{ HeightAxis, WidthAxis };
            else transposeAxes = new List<int>{ HeightAxis, WidthAxis, ChannelAxis };

            byte[] image = Image.ToImage(tex, CompressionType);

            return new Image(image, CompressionType, Dtype, shape, transposeAxes);
        }
    }
}
