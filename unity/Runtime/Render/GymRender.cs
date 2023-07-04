using System;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Gymize.Protobuf;

namespace Gymize
{
    public struct AudioFrame
    {
        public float[] data;
        public int channels;
        public AudioFrame(float[] data, int channels)
        {
            this.data = data;
            this.channels = channels;
        }
    }

    public class GymRender
    {
        public Dictionary<string, bool> IsRecording;
        public Dictionary<string, bool> IsPaused;
        public Dictionary<string, bool> IsSingleFrame;
        public Dictionary<string, int> ScreenWidths;
        public Dictionary<string, int> ScreenHeights;
        public Dictionary<string, bool> IsFullscreen;
        public int SamplingRate = 44100;
        
        private Dictionary<string, List<FrameProto>> m_ImageFrames;
        private Dictionary<string, List<AudioFrame>> m_AudioFrames;
        private Dictionary<string, DateTime> m_LastTime;

        // This can be cross scene
        public GymRender()
        {
            IsRecording = new Dictionary<string, bool>();
            IsPaused = new Dictionary<string, bool>();
            IsSingleFrame = new Dictionary<string, bool>();
            ScreenWidths = new Dictionary<string, int>();
            ScreenHeights = new Dictionary<string, int>();
            IsFullscreen = new Dictionary<string, bool>();

            m_ImageFrames = new Dictionary<string, List<FrameProto>>();
            m_AudioFrames = new Dictionary<string, List<AudioFrame>>();
            m_LastTime = new Dictionary<string, DateTime>();
        }

        public void Begin(string name)
        {
            m_ImageFrames[name] = new List<FrameProto>();
            m_AudioFrames[name] = new List<AudioFrame>();

            IsRecording[name] = true;
            IsPaused[name] = false;
            m_LastTime[name] = DateTime.Now;
        }

        public void End(string name)
        {
            IsRecording[name] = false;
            IsPaused[name] = true;
        }

        public void Pause(string name)
        {
            IsPaused[name] = true;
        }

        public void Resume(string name)
        {
            IsPaused[name] = false;
            m_LastTime[name] = DateTime.Now;
        }

        public bool IsActive(string name)
        {
            if (!IsRecording.ContainsKey(name)) return false;
            if (!IsPaused.ContainsKey(name)) return false;
            return IsRecording[name] && !IsPaused[name];
        }

        public void AddImageFrame(string name, Image image)
        {
            if (IsActive(name))
            {
                DateTime startTime = m_LastTime[name];
                m_LastTime[name] = DateTime.Now;
                TimeSpan diff = m_LastTime[name] - startTime;
                FrameProto videoFrame = new FrameProto();
                videoFrame.Image = image.ToProtobuf();
                videoFrame.Duration = (float)diff.TotalSeconds;
                if (IsSingleFrame[name]) m_ImageFrames[name].Clear();
                m_ImageFrames[name].Add(videoFrame);
            }
        }

        public void AddAudioFrame(string name, float[] data, int channels)
        {
            if (IsActive(name))
            {
                float[] buffer = new float[data.Length];
                data.CopyTo(buffer, 0);
                AudioFrame audioFrame = new AudioFrame(buffer, channels);
                m_AudioFrames[name].Add(audioFrame);
            }
        }

        public List<FrameProto> GetImageFrames(string name)
        {
            if (!m_ImageFrames.ContainsKey(name)) return new List<FrameProto>();
            List<FrameProto> frames = m_ImageFrames[name];
            m_ImageFrames.Remove(name);
            return frames;
        }

        public byte[] GetAudio(string name) // .wav file format
        {
            int numFrames = m_AudioFrames[name].Count;
            if (!m_AudioFrames.ContainsKey(name)) numFrames = 0; // use empty audio
            int channels = 0;
            for (int i = 0; i < numFrames; i++)
            {
                if (m_AudioFrames[name][i].channels > channels) channels = m_AudioFrames[name][i].channels;
            }
            int numSamples = 0;
            for (int i = 0; i < numFrames; i++)
            {
                numSamples += m_AudioFrames[name][i].data.Length / m_AudioFrames[name][i].channels;
            }
            
            float[] buffer;
            if (channels * numSamples == 0)
            {
                // empty audio
                channels = 2;
                numSamples = 1; 
                buffer = new float[2];
            }
            else
            {
                buffer = new float[channels * numSamples];
                int offset = 0;
                for (int n = 0; n < numFrames; n++)
                {
                    for (int i = 0; i < m_AudioFrames[name][n].data.Length; i++)
                    {
                        int s = i / m_AudioFrames[name][n].channels;
                        int c = i % m_AudioFrames[name][n].channels;
                        buffer[(offset + s) * channels + c] = m_AudioFrames[name][n].data[i];
                    }
                    offset += m_AudioFrames[name][n].data.Length / m_AudioFrames[name][n].channels;
                }
            }
            AudioClip clip = AudioClip.Create("Audio", numSamples, channels, SamplingRate, false);
            clip.SetData(buffer, 0);
            byte[] wav = SavWav.GetWav(clip, out var length);

            m_AudioFrames[name].Clear();
            return wav;
        }

        public VideoProto GetVideo(string name)
        {
            VideoProto video = new VideoProto();
            video.Name = name;
            video.Frames.AddRange(GetImageFrames(name));
            video.Audio = ByteString.CopyFrom(GetAudio(name));
            return video;
        }

        public List<VideoProto> GetRendering(List<string> names)
        {
            List<VideoProto> rendering = new List<VideoProto>();
            foreach (string name in names)
            {
                rendering.Add(GetVideo(name));
            }
            return rendering;
        }
    }
}