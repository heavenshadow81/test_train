using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    /*
     * 패킷 구조
     * 
     * TEMPLATE{packet-size}{user-id}{template-name}{template-image-count}{template-image-i-size}{template-image-i-bytes}....{template-image-n-bytes}

     */
    public class SeaStoryTemplatePacket
    {
        public struct ImageInfo
        {
            public int width;
            public int height;
            public byte[] bytes;

            public Color32[] colors
            {
                get
                {
                    int count = width * height;
                    int offset = 0;

                    Color32[] value = new Color32[count];
                    for (int i = 0; i < count; i++)
                    {
                        Color32 color = new Color32();
                        color.r = bytes[offset++];
                        color.g = bytes[offset++];
                        color.b = bytes[offset++];
                        color.a = bytes[offset++];
                        value[i] = color;
                    }
                    return value;
                }
                set
                {
                    bytes = new byte[value.Length * 4];
                    for (int i = 0, offset = 0; i < value.Length; i++)
                    {
                        bytes[offset++] = value[i].r;
                        bytes[offset++] = value[i].g;
                        bytes[offset++] = value[i].b;
                        bytes[offset++] = value[i].a;
                    }
                }
            }
        }

        private int _userId = 0;
        public int userId
        {
            get
            {
                return _userId;
            }
        }

        private string _templateName = "";
        public string templateName
        {
            get
            {
                return _templateName;
            }
        }

        private List<ImageInfo> _templateImages = new List<ImageInfo>();
        public List<ImageInfo> templateImages
        {
            get
            {
                return _templateImages;
            }
        }

        private static byte[] _cachedIntegerBytes = new byte[4];
        private static byte[] _cachedTemplateNameBytes = new byte[16];

        public static SeaStoryTemplatePacket Parse(List<byte> buffer)
        {
            SeaStoryTemplatePacket packet = null;

            if (buffer == null) return null;
            if (buffer.Count < 12) return null;

            int offset = 8;

            // 0~6 : TEMPLATE
            // 7~10 : Packet Size
            // 11~14 : User ID
            // 15-30 : Template Name
            // 31-34 : Template Image Count
            // 35-38 : Template Image i width
            // 39-42 : Template Image i height
            // 39~? : Image bytes (r, g, b, a)

            // check packet size
            for (int i = 0; i < _cachedIntegerBytes.Length; i++)
            {
                _cachedIntegerBytes[i] = buffer[offset++];
            }

            int packetSize = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);
            if (buffer.Count < packetSize) return null;

            // user id
            for (int i = 0; i < _cachedIntegerBytes.Length; i++)
            {
                _cachedIntegerBytes[i] = buffer[offset++];
            }

            int userId = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

            // template name
            int nameLength = 0;
            for (int i = 0; i < _cachedTemplateNameBytes.Length; i++)
            {
                byte b = buffer[offset++];
                _cachedTemplateNameBytes[i] = b;
                if (b != 0)
                {
                    nameLength++;
                }
            }

            string templateName = System.Text.Encoding.UTF8.GetString(_cachedTemplateNameBytes, 0, nameLength);

            // template image count
            for (int i = 0; i < _cachedIntegerBytes.Length; i++)
            {
                _cachedIntegerBytes[i] = buffer[offset++];
            }

            int templateImageCount = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);
            List<ImageInfo> templateImages = new List<ImageInfo>();

            // loop
            for (int t = 0; t < templateImageCount; t++)
            {
                // image width
                for (int i = 0; i < _cachedIntegerBytes.Length; i++)
                {
                    _cachedIntegerBytes[i] = buffer[offset++];
                }

                int width = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                // image height
                for (int i = 0; i < _cachedIntegerBytes.Length; i++)
                {
                    _cachedIntegerBytes[i] = buffer[offset++];
                }

                int height = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                // get color
                Color32[] colors = new Color32[width * height];

                for (int i = 0; i < colors.Length; i++)
                {
                    Color32 color = new Color32();
                    color.r = buffer[offset++];
                    color.g = buffer[offset++];
                    color.b = buffer[offset++];
                    color.a = buffer[offset++];
                    colors[i] = color;
                }

                ImageInfo image = new ImageInfo();
                image.width = width;
                image.height = height;
                image.colors = colors;

                templateImages.Add(image);
            }

            packet = new SeaStoryTemplatePacket(userId, templateName, templateImages);

            return packet;
        }

        public byte[] ToByteArray()
        {
            // contents
            List<byte> bytes = new List<byte>();
            bytes.AddRange(System.BitConverter.GetBytes(userId));

            byte[] dummyBytes = new byte[16];
            byte[] templateNameBytes = System.Text.Encoding.UTF8.GetBytes(templateName);
            for (int i = 0; i < 16; i++)
            {
                if (i < templateNameBytes.Length)
                {
                    dummyBytes[i] = templateNameBytes[i];
                }
                else
                {
                    dummyBytes[i] = 0;
                }
            }
            bytes.AddRange(dummyBytes);
            bytes.AddRange(System.BitConverter.GetBytes(templateImages.Count));
            for (int i = 0; i < templateImages.Count; i++)
            {
                ImageInfo image = templateImages[i];
                bytes.AddRange(System.BitConverter.GetBytes(image.width));
                bytes.AddRange(System.BitConverter.GetBytes(image.height));
                bytes.AddRange(image.bytes);
            }

            // result
            List<byte> result = new List<byte>();
            result.Capacity = bytes.Count + 11;

            result.AddRange(System.Text.Encoding.UTF8.GetBytes("TEMPLATE"));
            result.AddRange(System.BitConverter.GetBytes(bytes.Count));
            result.AddRange(bytes);

            return result.ToArray();
        }

        public SeaStoryTemplatePacket(int userId, string templateName, List<ImageInfo> templateImages)
        {
            _userId = userId;
            _templateName = templateName;
            _templateImages.Clear();
            _templateImages.AddRange(templateImages);
        }
    }
}