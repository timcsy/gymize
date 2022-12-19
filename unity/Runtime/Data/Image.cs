using System.Collections;
using System.Collections.Generic;
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
        public static Protobuf.CompressionType ToProtobuf(this COMPRESSION_TYPE value)
        {
            switch (value)
            {
                case COMPRESSION_TYPE.NONE:
                    return Protobuf.CompressionType.None;
                case COMPRESSION_TYPE.PNG:
                    return Protobuf.CompressionType.Png;
                case COMPRESSION_TYPE.JPG:
                    return Protobuf.CompressionType.Jpg;
                default:
                    return Protobuf.CompressionType.Unspecified;
            }
        }

        public static COMPRESSION_TYPE FromProtobuf(this Protobuf.CompressionType value)
        {
            switch (value)
            {
                case Protobuf.CompressionType.None:
                    return COMPRESSION_TYPE.NONE;
                case Protobuf.CompressionType.Png:
                    return COMPRESSION_TYPE.PNG;
                case Protobuf.CompressionType.Jpg:
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

        public Protobuf.Data ToProtobuf()
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

        // TODO: sth about Box
    }
}