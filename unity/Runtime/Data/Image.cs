using System.Collections;
using System.Collections.Generic;
using PAIA.Marenv.Protobuf;
using Google.Protobuf;

namespace PAIA.Marenv
{
    public enum COMPRESSION_TYPE
    {
        UNSPECIFIED,
        NONE,
        PNG,
        JPG
    }

    public static class CompressionTypeExtensions
    {
        public static CompressionType ToProtobuf(this COMPRESSION_TYPE value)
        {
            switch (value)
            {
                case COMPRESSION_TYPE.NONE:
                    return CompressionType.None;
                case COMPRESSION_TYPE.PNG:
                    return CompressionType.Png;
                case COMPRESSION_TYPE.JPG:
                    return CompressionType.Jpg;
                default:
                    return CompressionType.Unspecified;
            }
        }

        public static COMPRESSION_TYPE FromProtobuf(this CompressionType value)
        {
            switch (value)
            {
                case CompressionType.None:
                    return COMPRESSION_TYPE.NONE;
                case CompressionType.Png:
                    return COMPRESSION_TYPE.PNG;
                case CompressionType.Jpg:
                    return COMPRESSION_TYPE.JPG;
                default:
                    return COMPRESSION_TYPE.UNSPECIFIED;
            }
        }
    }

    public class Image : IData
    {
        byte[] m_Image;
        COMPRESSION_TYPE m_CompressionType;

        public Image(byte[] image, COMPRESSION_TYPE compressionType)
        {
            m_Image = image;
            m_CompressionType = compressionType;
        }

        public Data ToProtobuf()
        {
            Protobuf.Image image = new Protobuf.Image();
            image.CompressionType = m_CompressionType.ToProtobuf();
            image.Data = ByteString.CopyFrom(m_Image);
            // TODO: shape and dimension_mapping
            Protobuf.Data data = new Protobuf.Data
            {
                SpaceType = Protobuf.SpaceType.Image,
                Image = image
            };
            return data;
        }
        
        public void FromProtobuf(Protobuf.Data data)
        {
            m_Image = data.Image.Data.ToByteArray();
            m_CompressionType = data.Image.CompressionType.FromProtobuf();
            // TODO: shape and dimension_mapping
        }

        public IData Merge(IData original, Mapping mapping)
        {
            // TODO
            return this;
        }

        // TODO: sth about Box
    }
}