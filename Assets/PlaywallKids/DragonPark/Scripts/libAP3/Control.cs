using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion
{
    [Serializable]
    /// <summary> 
    /// Control is the class for each motor control
    /// It contains some information for motor control.
    /// </summary> 
    public class Control
    {
        public byte id;
        public double pos;
        public byte torque;
        public byte portdata;
        public ushort reserved;


        /// <summary> 
        /// Constructor for Frame without initialization
        /// </summary>
        /// <param name="motorcount">Used to define fixed motorcount of frame</param>
        /// <typeparam name="motorcount">int</typeparam>
        public Control()
        {
            id = 0;
            pos = 0;
            torque = 0;
            portdata = 0;
            reserved = 0;
        }

        /// <summary> 
        /// Constructor for Frame with initialization by each value
        /// </summary>
        /// <param name="motorcount">Used to define fixed motorcount of frame</param>
        /// <typeparam name="motorcount">int</typeparam>
        public Control(byte id, double pos, byte torque, byte portdata)
        {
            this.id = id;
            this.pos = pos;
            this.torque = torque;
            this.portdata = portdata;
            reserved = 0;
        }

        /// <summary> 
        /// Constructor for Frame with initialization by buffer array
        /// </summary>
        /// <param name="buffer">Used to set initial values according to the given protocol</param>
        /// <typeparam name="buffer">byte[]</typeparam>
        /// <param name="motorcount">Used to define fixed motorcount of frame</param>
        /// <typeparam name="motorcount">int</typeparam>
        public Control(byte[] buffer)
        {
            int position = 0;

            id = buffer[position];
            position += 1;

            pos = BitConverter.ToDouble(buffer, position);
            position += 8;

            torque = buffer[position];
            position += 1;

            portdata = buffer[position];
            position += 1;

            reserved = BitConverter.ToUInt16(buffer, position);
            position += 2;
        }


        /// <summary> 
        /// Method for returning the byte array for object
        /// </summary>
        public byte[] ToByteArray()
        {
            // the bytecount for frame is 8+namelength+13*motorcount
            int bytecount = Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY;
            byte[] data = new byte[bytecount];

            int position = 0;
            data[position] = id;
            position += 1;

            Utils.ToByteArray(ref data, position, pos, 8);
            position += 8;

            data[position] = torque;
            position += 1;

            data[position] = portdata;
            position += 1;

            // reserved
            Utils.ToByteArray(ref data, position, reserved, 2);
            position += 2;

            return data;
        }


    }
}
