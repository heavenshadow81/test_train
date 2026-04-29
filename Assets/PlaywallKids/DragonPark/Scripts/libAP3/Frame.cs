using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion
{
    [Serializable]
    /// <summary> 
    /// Frame is the class for motion control of one frame
    /// It contains some information for frame and motor controls.
    /// </summary> 
    public class Frame
    {
        // Field 
        public uint index;
        public ushort namelength;
        public string name;
        public ushort reserved;
        public List<Control> control;


        /// <summary> 
        /// Constructor for Frame without initialization
        /// </summary>
        /// <param name="motorcount">Used to define fixed motorcount of frame</param>
        /// <typeparam name="motorcount">int</typeparam>
        public Frame(int motorcount)
        {
            index = 0;
            namelength = 0;
            name = "";
            reserved = 0;
            control = new List<Control>();
            for (int i = 0; i < motorcount; i++)            // Modified by SJ
                control.Add(new Control());         
        }

        /// <summary> 
        /// Constructor for Frame with initialization
        /// </summary>
        /// <param name="buffer">Used to set initial values according to the given protocol</param>
        /// <typeparam name="buffer">byte[]</typeparam>
        /// <param name="motorcount">Used to define fixed motorcount of frame</param>
        /// <typeparam name="motorcount">int</typeparam>
        public Frame(byte[] buffer, int motorcount)
        {
            int position = 0;

            index = BitConverter.ToUInt32(buffer, 0);
            namelength = BitConverter.ToUInt16(buffer, 4);
            name = BitConverter.ToString(buffer, 6, namelength);
            position = 6 + namelength;

            // reserved
            reserved = BitConverter.ToUInt16(buffer, position);
            position += 2;

            control = new List<Control>(motorcount);
            // control 당 바이트 수 : 13
            for (int i = 0; i < motorcount; i++)
            {
                //control[i] = new Control(buffer.Skip(position).Take(Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY).ToArray());
                control.Add(new Control(buffer.Skip(position).Take(Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY).ToArray()));    // Modified by SJ
                position += Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY;
            }
        }

        /// <summary> 
        /// Method for returning the byte array for object
        /// </summary>
        public byte[] ToByteArray(int motorcount)
        {
            // the bytecount for frame is 8+namelength+13*motorcount
            int bytecount = Utils.SIZE_FRAME_DEFAULT_BYTE_ARRAY + namelength + Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY*motorcount;
            byte[] data = new byte[bytecount];

            int position = 0;
            Utils.ToByteArray(ref data, position, index, 4);
            position += 4;
            Utils.ToByteArray(ref data, position, namelength, 2);
            position += 2;
            Utils.ToByteArray(ref data, position, name, namelength);
            position += namelength;
            Utils.ToByteArray(ref data, position, reserved, 2);
            position += 2;

            for (int i = 0; i < motorcount; i++)
            {
                byte[] controlbytearray = control[i].ToByteArray();
                for (int j = 0; j < Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY; j++)
                {
                    data[position + j] = controlbytearray[j];
                }
                position += Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY;
            }

            return data;
        }

    }
}
