using System.Collections;
using UnityEngine;

namespace Gymize
{
    public class GymRenderRecorder : MonoBehaviour
    {
        [Tooltip("The name of the view")]
        public string Name;
        [Tooltip("You have to put this component with AudioListener if RecordAudio is true")]
        public bool RecordAudio = true;
        [Tooltip("true means you don't use the Render Texture")]
        public bool CaptureScreen = true;
        [Tooltip("negative means using the original size with scaling")]
        public int VideoWidth = -1;
        [Tooltip("negative means using the original size with scaling")]
        public int VideoHeight = -1;
        public bool UseCamera = false;
        public Camera Camera;
        public bool UseRenderTexture = false;
        public RenderTexture RenderTexture;
        public CompressionType CompressionType = CompressionType.JPG;

        private int m_Width
        {
            get { return (VideoWidth < 0)? (int)Mathf.Floor(-Screen.width * VideoWidth): VideoWidth; }
        }
        private int m_Height
        {
            get { return (VideoHeight < 0)? (int)Mathf.Floor(-Screen.height * VideoHeight): VideoHeight; }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!RecordAudio) return;
            if (GymEnv.Render.IsActive(Name))
            {
                GymEnv.Render.AddAudioFrame(Name, data, channels);
            }
        }

        void Start()
        {
            GymEnv.Render.SamplingRate = AudioSettings.outputSampleRate;
        }

        void Update()
        {
            if (GymEnv.Render.IsActive(Name))
            {
                if (CaptureScreen) StartCoroutine(CaptureUI());
                if (UseCamera) CaptureCamera();
                if (UseRenderTexture) AddRenderTexture(RenderTexture);
            }
        }

        private void ResizeScreen()
        {
            int screenWidth = -1;
            if (GymEnv.Render.ScreenWidths.ContainsKey(Name))
            {
                screenWidth = GymEnv.Render.ScreenWidths[Name];
            }
            int screenHeight = -1;
            if (GymEnv.Render.ScreenHeights.ContainsKey(Name))
            {
                screenHeight = GymEnv.Render.ScreenHeights[Name];
            }
            bool fullscreen = false;
            if (GymEnv.Render.IsFullscreen.ContainsKey(Name))
            {
                fullscreen = GymEnv.Render.IsFullscreen[Name];
            }
            if (screenWidth >= 0 && screenHeight >= 0)
            {
                Screen.SetResolution(screenWidth, screenHeight, fullscreen);
            }
        }

        private IEnumerator CaptureUI()
        {
            yield return new WaitForEndOfFrame();

            ResizeScreen();

            RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 0);
            ScreenCapture.CaptureScreenshotIntoRenderTexture(screenTexture);
            screenTexture = Image.AutoFlipVertical(screenTexture);

            AddRenderTexture(screenTexture);
        }

        private void CaptureCamera()
        {
            RenderTexture rt = new RenderTexture(m_Width, m_Height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            RenderTexture oldRT = Camera.targetTexture;
            Camera.targetTexture = rt;
            Camera.Render();
            Camera.targetTexture = oldRT;

            AddRenderTexture(rt);
        }

        private void AddRenderTexture(RenderTexture rt)
        {
            rt = Image.Scale(rt, scaledWidth: m_Width, scaledHeight: m_Height);
            Texture2D tex = Image.ToTexture2D(rt);
            byte[] image_buffer = Image.ToImage(tex, CompressionType);
            Image image = new Image(image_buffer, CompressionType);

            GymEnv.Render.AddImageFrame(Name, image);
        }
    }
}