using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Motion
{
    /// <summary> 
    /// Utils is the class for util variable and methods related with namespace Motion
    /// </summary> 
    public static class Utils
    {
        public static int SIZE_MOTION_DEFAULT_BYTE_ARRAY = 56;
        public static int SIZE_MOTOR_BYTE_ARRAY = 45;
        public static int SIZE_FRAME_DEFAULT_BYTE_ARRAY = 8;
        public static int SIZE_FRAME_CONTROL_BYTE_ARRAY = 13;

        /// <summary> 
        /// Method for converting serializable object to byte array
        /// </summary> 
        public static byte[] ToByteArray(object source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }

        public static void ToByteArray(ref byte[] data, int position, String value, int bytesize)
        {
            byte[] temp = new byte[bytesize];
            temp = System.Text.Encoding.Default.GetBytes(value);
            for (int i = 0; i < bytesize; i++)
            {
                data[position + i] = temp[i];
            }
        }

        public static void ToByteArray(ref byte[] data, int position, ushort value, int bytesize)
        {
            byte[] temp = new byte[bytesize];
            temp = BitConverter.GetBytes(value);
            for (int i = 0; i < bytesize; i++)
            {
                data[position + i] = temp[i];
            }
        }

        public static void ToByteArray(ref byte[] data, int position, uint value, int bytesize)
        {
            byte[] temp = new byte[bytesize];
            temp = BitConverter.GetBytes(value);
            for (int i = 0; i < bytesize; i++)
            {
                data[position + i] = temp[i];
            }
        }

        public static void ToByteArray(ref byte[] data, int position, double value, int bytesize)
        {
            byte[] temp = new byte[bytesize];
            temp = BitConverter.GetBytes(value);
            for (int i = 0; i < bytesize; i++)
            {
                data[position + i] = temp[i];
            }
        }
    }
}
