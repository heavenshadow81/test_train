using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion
{
    [Serializable]
    /// <summary> 
    /// Motor is the class for motor characteristics
    /// It contains some information for motor settings.
    /// </summary> 
    public class Motor
    {
        // Field 
        public byte id;
        public byte useyn;
        public byte role;
        public byte dialx;
        public byte dialy;
        public double initial;
        public double max;
        public double min;
        public byte tpgain;
        public byte tdgain;
        public byte tigain;
        public byte portmode;
        public uint reserved;
        public double zeropad;

        /// <summary> 
        /// Constructor for Motor with initialization
        /// </summary>


        
        public Motor(byte id, byte useyn, byte role, byte dialx, byte dialy, double initial, double max, double min, byte tpgain, byte tdgain, byte tigain, byte portmode, double zeropad)
        {
            this.id = id;
            this.useyn = useyn;
            this.role = role;
            this.dialx = dialx;
            this.dialy = dialy;
            this.initial = initial;
            this.max = max;
            this.min = min;
            this.tpgain = tpgain;
            this.tdgain = tdgain;
            this.tigain = tigain;
            this.portmode = portmode;
            this.reserved = 0;
            this.zeropad = zeropad;
        }

        /// <summary> 
        /// Constructor for Motor with initialization
        /// </summary>
        /// <param name="buffer">Used to set initial values according to the given protocol</param>
        /// <typeparam name="buffer">byte[]</typeparam>
        public Motor(byte[] buffer)
        {
            this.id = buffer[0];
            this.useyn = buffer[1];
            this.role = buffer[2];
            this.dialx = buffer[3];
            this.dialy = buffer[4];
            this.initial = BitConverter.ToDouble(buffer, 5);
            this.max = BitConverter.ToDouble(buffer, 13);
            this.min = BitConverter.ToDouble(buffer, 21);
            this.tpgain = buffer[29];
            this.tdgain = buffer[30];
            this.tigain = buffer[31];
            this.portmode = buffer[32];
            this.reserved = BitConverter.ToUInt32(buffer, 33);
            this.zeropad = BitConverter.ToUInt32(buffer, 37);
        }

        /// <summary> 
        /// Method for returning the byte array for object
        /// </summary>
        public byte[] ToByteArray()
        {
            int bytecount = Utils.SIZE_MOTOR_BYTE_ARRAY;
            byte[] data = new byte[bytecount];

            data[0] = id;
            data[1] = useyn;
            data[2] = role;
            data[3] = dialx;
            data[4] = dialy;
            Utils.ToByteArray(ref data, 5, initial, 8);
            Utils.ToByteArray(ref data, 13, max, 8);
            Utils.ToByteArray(ref data, 21, min, 8);
            data[29] = tpgain;
            data[30] = tdgain;
            data[31] = tigain;
            data[32] = portmode;
            Utils.ToByteArray(ref data, 33, reserved, 4);
            Utils.ToByteArray(ref data, 37, zeropad, 8);

            return data;
        }
        
        /// <summary> 
        /// Constructor for Motor without initialization
        /// </summary>
        public Motor()
        {
        }

    }
}
