using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion
{
    [Serializable]
    /// <summary> 
    /// Motion is the class for motion data file.
    /// It contains some information for motion data file, the array of motor, and the array of frame.
    /// </summary>
    public class Motion
    {
        // Field 
        public byte version;
        public ushort snlength;
        public string sn;
        public string author;
        public string tel;
        public ushort namelength;
        public string name;
        public byte platform;
        public uint framecount;
        public ushort sampliing;
        public ushort motorcount;
        public List<Motor> motor;
        public List<Frame> frame;

        /// <summary> 
        /// Constructor for Motion without data
        /// </summary> 

        public Motion()
        {
            version = 0;
            snlength = 0;
            sn = "";
            author = "";
            tel = "";
            namelength = 0;
            name = "";
            platform = 0;
            framecount = 0;
            sampliing = 0;
            motorcount = 0;
            motor = new List<Motor>();
            frame = new List<Frame>();
        }

        /// <summary> 
        /// Constructor for Motion with motorcount and framecount 
        /// </summary> 
        public Motion(ushort motorcnt, uint framecnt)
        {
            version = 0;
            snlength = 0;
            sn = "";
            author = "";
            tel = "";
            namelength = 0;
            name = "";
            platform = 0;
            framecount = framecnt;
            sampliing = 0;
            motorcount = motorcnt;
            //motor = new List<Motor>(motorcnt);
            //frame = new List<Frame>((int)framecnt);
            motor = new List<Motor>();  // Modified by SJ
            frame = new List<Frame>();
            for(int i=0;i<motorcnt;i++)
                motor.Add(new Motor());
            for (int i = 0; i < (int)framecnt; i++)
                frame.Add(new Frame(motorcnt));
            
        }

        /// <summary> 
        /// Constructor for Motion with initialization
        /// </summary>
        /// <param name="buffer">Used to set initial values according to the given protocol</param>
        /// <typeparam name="buffer">byte[]</typeparam>
        public Motion(byte[] buffer)
        {
            int position = 0;

            version = buffer[position];
            position += 1;

            snlength = BitConverter.ToUInt16(buffer, position);
            position += 2;

            sn = BitConverter.ToString(buffer, position, snlength);
            position += snlength;

            author = BitConverter.ToString(buffer, position, 10);
            position += 10;

            tel = BitConverter.ToString(buffer, position, 20);
            position += 20;

            namelength = BitConverter.ToUInt16(buffer, position);
            position += 2;

            name = BitConverter.ToString(buffer, position, namelength);
            //position += snlength;   // <--------- namelength? 수정수정수정수정수정수정수정수정수정수정수정수정수정수정수정수정수정수정
            position += namelength;   // Modified by SJ

            // reserved
            position += 4;

            platform = buffer[position];
            position += 1;

            framecount = BitConverter.ToUInt32(buffer, position);
            position += 4;

            // reserved
            position += 2;

            sampliing = BitConverter.ToUInt16(buffer, position);
            position += 2;

            // reserved
            position += 2;

            motorcount = BitConverter.ToUInt16(buffer, position);
            position += 2;

            // reserved
            position += 4;

            motor = new List<Motor>(motorcount);
            frame = new List<Frame>((int)framecount);

            // motor 당 바이트 수 : 37
            int motordatalength = 37;
            for (int i = 0; i < motorcount; i++)
            {
                //motor[i] = new Motor(buffer.Skip(position).Take(motordatalength).ToArray());
                motor.Add(new Motor(buffer.Skip(position).Take(motordatalength).ToArray()));// Modified by SJ
                position += motordatalength;                
            }

            // frame 당 바이트 수 : 프레임 이름에 따라 달라짐, 21+alpha
            int framedatalength = 0;
            for (int i = 0; i < motorcount; i++)
            {
                int framenamelength = BitConverter.ToInt16(buffer, position+6);
                framedatalength = 21 + framenamelength;
                //frame[i] = new Frame(buffer.Skip(position).Take(framedatalength).ToArray(), motorcount);
                frame.Add(new Frame(buffer.Skip(position).Take(framedatalength).ToArray(), motorcount));// Modified by SJ
                position += framedatalength;
            }
        }

        /// <summary> 
        /// Method for returning the byte array for object
        /// </summary>
        public byte[] ToByteArray()
        {
            // the bytecount for frame is 56+snlength+namelength+37*motorcout, frame byte count
            int bytecount = Utils.SIZE_MOTION_DEFAULT_BYTE_ARRAY + snlength + namelength
                        + Utils.SIZE_MOTOR_BYTE_ARRAY * motorcount;/////Frame정보 계산안됨!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!수정수정수정수정수정수정수정수정수정수정수정수정
            
            for (int i = 0; i<framecount; i++)  // Added by SJ
            {
                bytecount += frame[i].namelength + 8;
                bytecount += Utils.SIZE_FRAME_CONTROL_BYTE_ARRAY * motorcount;
            }
            
            
            
            byte[] data = new byte[bytecount];

            int position = 0;
            data[position] = version;
            position += 1;
            Utils.ToByteArray(ref data, position, snlength, 2);
            position += 2;
            Utils.ToByteArray(ref data, position, sn, snlength);
            position += snlength;
            Utils.ToByteArray(ref data, position, author, 10);
            position += 10;
            Utils.ToByteArray(ref data, position, tel, 20);
            position += 20;
            Utils.ToByteArray(ref data, position, namelength, 2);
            position += 2;
            Utils.ToByteArray(ref data, position, name, namelength);
            position += namelength;
            position += 4;
            data[position] = platform;
            position += 1;
            Utils.ToByteArray(ref data, position, framecount, 4);
            position += 4; 
            position += 2;
            Utils.ToByteArray(ref data, position, sampliing, 2);
            position += 2;
            position += 2;
            Utils.ToByteArray(ref data, position, motorcount, 2);
            position += 2;
            position += 4;

            for (int i = 0; i < motorcount; i++)
            {
                byte[] motorbytearray = motor[i].ToByteArray();
                for (int j = 0; j < Utils.SIZE_MOTOR_BYTE_ARRAY; j++)
                {
                    data[position + j] = motorbytearray[j];
                }
                position += Utils.SIZE_MOTOR_BYTE_ARRAY;
            }

            for (int i = 0; i < framecount; i++)
            {
                byte[] framebytearray = frame[i].ToByteArray(motorcount);
                for (int j = 0; j < framebytearray.Length; j++)
                {
                    data[position + j] = framebytearray[j];
                }
                position += framebytearray.Length;
            }

            return data;
        }


    }
}
