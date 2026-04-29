using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Motion;

namespace LibAP3
{

    public class AP2
    {
        public Motion.Motion MotionData;
        public AP2()
        {
            MotionData = new Motion.Motion();
        }

        public uint getHeaderBytes()
        {
            uint output = 0;
            uint temp = 0;
            output += 1; //버전정보
            output += 2; //SN길이
            output += System.Convert.ToUInt32(MotionData.snlength); //SN코드
            output += 10; //만든이
            output += 20; //연락처
            output += 2; //모션이름길이
            output += System.Convert.ToUInt32(MotionData.namelength); //모션이름
            output += 4; //Reserved
            output += 1; //플랫폼
            output += 4; //프레임수
            output += 2; //Reserved
            output += 2; //샘플링 주파수
            output += 2; //Reserved
            output += 2; //wCK(SAM) 수
            output += 4; //Reserved

            // wCK 반복
            temp += 1; //wCK(SAM)의 ID
            temp += 1; //사용 여부
            temp += 1; //장착 위치
            temp += 1; //다이얼 위치X
            temp += 1; //다이얼 위치Y
            temp += 8; //기본 자세값
            temp += 8; //위치 상한값
            temp += 8; //위치 하한값
            temp += 1; //TPGAIN
            temp += 1; //TDGAIN
            temp += 1; //TIGAIN
            temp += 1; //PORT MODE
            temp += 4; //Reserved
            //temp += 4; //Zero Pad

            output += temp * System.Convert.ToUInt32(MotionData.motorcount);

            return output;
        }
        public uint getFrameBytes(int frameidx)
        {
            uint output = 0;
            output += 4; // 프레임 인덱스
            output += 2; // 프레임 이름 길이
            output += System.Convert.ToUInt32(MotionData.frame[frameidx].namelength); //프레임 이름
            output += 2; // Reserved

            // Frame, wCK 마다 반복
            for (int j = 0; j < MotionData.motorcount; j++)
            {
                output += 1; // wCK(SAM)의 ID
                output += 8; // POS
                output += 1; // TORQUE
                output += 1; // PORT DATA
                output += 2; // Reserved
            }
            return output;
        }
        public uint getBodyBytes()
        {
            uint output=0;
          
             // Frame 반복
            for (int i = 0; i < MotionData.framecount; i++)
            {
                output += getFrameBytes(i);

            }
            
            return output;
        }
        public uint getTotalBytes()
        {
            return getHeaderBytes() + getBodyBytes();
        }


        public void loadAP2Header(System.IO.BinaryReader fr)
        {
            byte reserved_1 = System.Convert.ToByte(0);
            UInt16 reserved_2 = System.Convert.ToUInt16(0);
            UInt32 reserved_4 = System.Convert.ToUInt32(0);

            // 버전정보 (1 Byte)
            read_1byte(fr, ref MotionData.version);

            // SN길이 (2 Bytes)
            read_2byte(fr, ref MotionData.snlength);

            // SN코드 (SN길이 Bytes)
            read_string(fr, ref MotionData.sn, MotionData.snlength);

            // 만든이 (10 Bytes)
            read_string(fr, ref MotionData.author, 10);

            // 연락처 (20 Bytes)
            read_string(fr, ref MotionData.tel, 20);

            // 모션이름길이 (2 Bytes)
            read_2byte(fr, ref MotionData.namelength);

            // 모션이름 (모션이름길이 Bytes)
            read_string(fr, ref MotionData.name, MotionData.namelength);

            // Reserved(4 Bytes)
            read_4byte(fr, ref reserved_4);

            // 플랫폼 (1 Bytes)
            read_1byte(fr, ref MotionData.platform);

            // 프레임수(4 Bytes)
            read_4byte(fr, ref MotionData.framecount);

            // Reserved(2 Bytes)
            read_2byte(fr, ref reserved_2);
            
            // 샘플링 주파수 (2 Bytes)
            read_2byte(fr, ref MotionData.sampliing);
            //MotionData.sampliing = 80;

            // Reserved(2 Bytes)
            read_2byte(fr, ref reserved_2);

            // wCK(SAM) 수 (2 Bytes)
            read_2byte(fr, ref MotionData.motorcount);

            // Reserved (4 Bytes)
            read_4byte(fr, ref reserved_4);

            
            ////// malloc /////
            
            
            for (int i = 0; i < MotionData.motorcount; i++)
            {
                MotionData.motor.Add(new Motor());
            }
                        
            /////// wck(SAM) 수만큼 반복  ////////
            for (int i = 0; i < MotionData.motorcount; i++)
            {
                // wCK(SAM)의 ID (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].id);
                // 사용여부 (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].useyn);
                // 장착위치 (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].role);
                // 다이얼 위치X (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].dialx);
                // 다이얼 위치Y (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].dialy);
                // 기본 자세값 (8 Bytes)
                read_double(fr, ref MotionData.motor[i].initial);
                // 위치 상한값 (8 Bytes)
                read_double(fr, ref MotionData.motor[i].max);
                // 위치 하한값 (8 Bytes)
                read_double(fr, ref MotionData.motor[i].min);
                // TPGAIN (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].tpgain);
                // TDGAIN (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].tdgain);
                // TIGAIN (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].tigain);
                // PORT MODE (1 Byte)
                read_1byte(fr, ref MotionData.motor[i].portmode);
                // Reserved (4 Byte)
                read_4byte(fr, ref reserved_4);
                // Zero pad (4 Byte)
                //read_double(fr, ref MotionData.motor[i].zeropad);

            }
        }

        public void saveAP2Header(System.IO.BinaryWriter fw)
        {
            byte reserved_1 = System.Convert.ToByte(0);
            UInt16 reserved_2 = System.Convert.ToUInt16(0);
            UInt32 reserved_4 = System.Convert.ToUInt32(0);

            // 버전정보 (1 Byte)
            write_1byte(fw, MotionData.version);

            // SN길이 (2 Bytes)
            write_2byte(fw, MotionData.snlength);

            // SN코드 (SN길이 Bytes)
            write_string(fw, MotionData.sn, MotionData.snlength);

            // 만든이 (10 Bytes)
            write_string(fw, MotionData.author, 10);

            // 연락처 (20 Bytes)
            write_string(fw, MotionData.tel, 20);

            // 모션이름길이 (2 Bytes)
            write_2byte(fw, MotionData.namelength);

            // 모션이름 (모션이름길이 Bytes)
            write_string(fw, MotionData.name, MotionData.namelength);

            // Reserved(4 Bytes)
            write_4byte(fw, reserved_4);

            // 플랫폼 (1 Bytes)
            write_1byte(fw, MotionData.platform);

            // 프레임수(4 Bytes)
            write_4byte(fw, MotionData.framecount);

            // Reserved(2 Bytes)
            write_2byte(fw, reserved_2);

            // 샘플링 주파수 (2 Bytes)
            write_2byte(fw, MotionData.sampliing);

            // Reserved(2 Bytes)
            write_2byte(fw, reserved_2);

            // wCK(SAM) 수 (2 Bytes)
            write_2byte(fw, MotionData.motorcount);

            // Reserved (4 Bytes)
            write_4byte(fw, reserved_4);

            /////// wck(SAM) 수만큼 반복  ////////
            for (int i = 0; i < MotionData.motorcount; i++)
            {
                // wCK(SAM)의 ID (1 Byte)
                write_1byte(fw, MotionData.motor[i].id);
                // 사용여부 (1 Byte)
                write_1byte(fw, MotionData.motor[i].useyn);
                // 장착위치 (1 Byte)
                write_1byte(fw, MotionData.motor[i].role);
                // 다이얼 위치X (1 Byte)
                write_1byte(fw, MotionData.motor[i].dialx);
                // 다이얼 위치Y (1 Byte)
                write_1byte(fw, MotionData.motor[i].dialy);
                // 기본 자세값 (8 Bytes)
                write_double(fw, MotionData.motor[i].initial);
                // 위치 상한값 (8 Bytes)
                write_double(fw, MotionData.motor[i].max);
                // 위치 하한값 (8 Bytes)
                write_double(fw, MotionData.motor[i].min);
                // TPGAIN (1 Byte)
                write_1byte(fw, MotionData.motor[i].tpgain);
                // TDGAIN (1 Byte)
                write_1byte(fw, MotionData.motor[i].tdgain);
                // TIGAIN (1 Byte)
                write_1byte(fw, MotionData.motor[i].tigain);
                // PORT MODE (1 Byte)
                write_1byte(fw, MotionData.motor[i].portmode);
                // Reserved (4 Byte)
                write_4byte(fw, reserved_4);
                // Zero pad (4 Byte)
               // write_double(fw, MotionData.motor[i].zeropad);
                
            }
        }

        public void saveAP2File(string fn)
        {
            System.IO.FileStream fStream = new System.IO.FileStream(fn, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fStream);
            saveAP2Header(bw);
            saveAP2Body(bw);
            bw.Close();
            fStream.Close();
        }

        public void saveAP2Frame(System.IO.BinaryWriter fw, int frameidx)
        {
            byte reserved_1 = System.Convert.ToByte(0);
            UInt16 reserved_2 = System.Convert.ToUInt16(0);
            UInt32 reserved_4 = System.Convert.ToUInt32(0);

            // 프레임 인덱스 (4 Bytes)
            write_4byte(fw, MotionData.frame[frameidx].index);

            // 프레임 이름 길이 (2 Bytes)
            write_2byte(fw, MotionData.frame[frameidx].namelength);

            // 프레임 이름 (프레임 이름 길이 Bytes)
            write_string(fw, MotionData.frame[frameidx].name, MotionData.frame[frameidx].namelength);

            // Reserved (2 Bytes)
            write_2byte(fw, reserved_2);

            // wCK 마다 반복
            for (int j = 0; j < MotionData.motorcount; j++)
            {
                // wCK(SAM)의 ID(1 Byte)
                write_1byte(fw, MotionData.frame[frameidx].control[j].id);

                // POS (8 Bytes)
                write_double(fw, MotionData.frame[frameidx].control[j].pos);

                // TORQUE (1 Byte)
                write_1byte(fw, MotionData.frame[frameidx].control[j].torque);

                // PORT DATA (1 Byte)
                if (MotionData.motor[j].portmode == 1)
                write_1byte(fw, MotionData.frame[frameidx].control[j].portdata);

                // Reserved (2 Bytes)
                write_2byte(fw, reserved_2);

            }
        }

        public void saveAP2NullFrame(System.IO.BinaryWriter fw)
        {

            // 프레임 인덱스 (4 Bytes)
            write_4byte(fw, 0);

            // 프레임 이름 길이 (2 Bytes)
            write_2byte(fw, 1);

            // 프레임 이름 (프레임 이름 길이 Bytes)
            write_string(fw, " ", 1);

            // Reserved (2 Bytes)
            write_2byte(fw, 0);

            // wCK 마다 반복
            for (int j = 0; j < MotionData.motorcount; j++)
            {
                // wCK(SAM)의 ID(1 Byte)
                write_1byte(fw, 0);

                // POS (8 Bytes)
                write_double(fw, 0);

                // TORQUE (1 Byte)
                write_1byte(fw, 0);

                // PORT DATA (1 Byte)
                if (MotionData.motor[j].portmode == 1)
                    write_1byte(fw, 0);

                // Reserved (2 Bytes)
                write_2byte(fw, 0);

            }
        }

        public void loadAP2NullFrame(System.IO.BinaryReader fr)
        {
            byte data_1 = System.Convert.ToByte(0);
            UInt16 data_2 = System.Convert.ToUInt16(0);
            UInt32 data_4 = System.Convert.ToUInt32(0);
            Double data_8 = System.Convert.ToDouble(0);

            string temp = " ";

            // 프레임 인덱스 (4 Bytes)
            read_4byte(fr, ref data_4);

            // 프레임 이름 길이 (2 Bytes)
            read_2byte(fr, ref data_2);

            // 프레임 이름 (프레임 이름 길이 Bytes)
            read_string(fr, ref temp, 1);

            // Reserved (2 Bytes)
            read_2byte(fr, ref data_2);

            // wCK 마다 반복
            for (int j = 0; j < MotionData.motorcount; j++)
            {
                // wCK(SAM)의 ID(1 Byte)
                read_1byte(fr, ref data_1);

                // POS (8 Bytes)
                read_double(fr, ref data_8);

                // TORQUE (1 Byte)
                read_1byte(fr, ref data_1);

                // PORT DATA (1 Byte)
                if (MotionData.motor[j].portmode == 1)
                    read_1byte(fr, ref data_1);

                // Reserved (2 Bytes)
                read_2byte(fr, ref data_2);

            }
        }

        public void saveAP2Body(System.IO.BinaryWriter fw)
        {

            // Frame 마다 반복
            for (int i = 0; i < MotionData.framecount; i++)
            {
                saveAP2Frame(fw,i);
            }


        }

        public void loadAP2Frame(System.IO.BinaryReader fr, int frameidx)
        {
            byte reserved_1 = System.Convert.ToByte(0);
            UInt16 reserved_2 = System.Convert.ToUInt16(0);
            UInt32 reserved_4 = System.Convert.ToUInt32(0);

            // 프레임 인덱스 (4 Bytes)
            read_4byte(fr, ref MotionData.frame[frameidx].index);

            // 프레임 이름 길이 (2 Bytes)
            read_2byte(fr, ref MotionData.frame[frameidx].namelength);

            // 프레임 이름 (프레임 이름 길이 Bytes)
            read_string(fr, ref MotionData.frame[frameidx].name, MotionData.frame[frameidx].namelength);

            // Reserved (2 Bytes)
            read_2byte(fr, ref reserved_2);

            // wCK 마다 반복
            for (int j = 0; j < MotionData.motorcount; j++)
            {
                // wCK(SAM)의 ID(1 Byte)
                read_1byte(fr, ref MotionData.frame[frameidx].control[j].id);

                // POS (8 Bytes)
                read_double(fr, ref MotionData.frame[frameidx].control[j].pos);

                // TORQUE (1 Byte)
                read_1byte(fr, ref MotionData.frame[frameidx].control[j].torque);

                // PORT DATA (1 Byte)
                if (MotionData.motor[j].portmode == 1)
                read_1byte(fr, ref MotionData.frame[frameidx].control[j].portdata);

                // Reserved (2 Bytes)
                read_2byte(fr, ref reserved_2);

            }
        }
        public void openMotion(Motion.Motion motionInfo)
        {
            MotionData = motionInfo;
        }

        public void openFile(string fn)
        {
            System.IO.FileStream fStream;
            System.IO.BinaryReader bReader;
            fStream = new System.IO.FileStream(fn, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            bReader = new System.IO.BinaryReader(fStream);

            loadAP2Header(bReader);
            loadAP2Body(bReader);

            bReader.Close();
            fStream.Close();
        }
        public void saveFile(string fn)
        {
            System.IO.FileStream fStream = new System.IO.FileStream(fn, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fStream);
            saveAP2Header(bw);
            saveAP2Body(bw);
            bw.Close();
            fStream.Close();
        }

        public void loadAP2Body(System.IO.BinaryReader fr)
        {

            //MotionData.frame = new List<Frame>((int)MotionData.framecount);
            
            
            
            // Frame 마다 반복
            for (int i = 0; i < MotionData.framecount; i++)
            {
                MotionData.frame.Add(new Frame(MotionData.motorcount));
                loadAP2Frame(fr, i);
            }
        }

        private void write_string(System.IO.BinaryWriter fw, string data, int length)
        {
            int temp;

            byte[] buffer = null;
            string data_string = null;
            data_string = data;
            temp = data_string.Length;
            if (temp < length)
            {
                for (int i = 0; i < length - temp; i++)
                    data_string += " ";
            }
            buffer = Encoding.UTF8.GetBytes(data_string);

            fw.Write(buffer, 0, length);

        }
        private void write_1byte(System.IO.BinaryWriter fw, byte data)
        {
            fw.Write(data);

        }
        private void write_2byte(System.IO.BinaryWriter fw, UInt16 data)
        {
            fw.Write(data);

        }
        private void write_4byte(System.IO.BinaryWriter fw, UInt32 data)
        {
            fw.Write(data);

        }

        private void write_double(System.IO.BinaryWriter fw, double data)
        {
            fw.Write(data);
        }


        private void read_1byte(System.IO.BinaryReader fr, ref byte data)
        {
            data = fr.ReadByte();

        }
        private void read_2byte(System.IO.BinaryReader fr, ref UInt16 data)
        {
            data = fr.ReadUInt16();

        }
        private void read_4byte(System.IO.BinaryReader fr, ref UInt32 data)
        {
            data = fr.ReadUInt32();

        }
        private void read_double(System.IO.BinaryReader fr, ref double data)
        {
            data = fr.ReadDouble();
        }
        private void read_string(System.IO.BinaryReader fr, ref string data, int length)
        {
            //data = fr.ReadChars(length).ToString();

            byte[] temp = new byte[256];

            fr.Read(temp, 0, length);
            data = Encoding.UTF8.GetString(temp, 0, length);
            
        }
    }
}
